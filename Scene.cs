using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StereoGame;
using StereoGame.Hitbox;

using act = StereoGame.InputHandler.Action;
using System.Collections.ObjectModel;

namespace StereoGame
{
    public class Scene : Entity
	{
		private List<Entity> regularEntitiesList;
		private List<CollisionEntity> collisionEntitiesList;


		/// <summary>
		/// WARNING : performance may be bad.
		/// </summary>
		public ReadOnlyCollection<Entity> RegularEntitiesList
		{
			get => regularEntitiesList.AsReadOnly();
		}

		/// <summary>
		/// Read only version of the list, should NOT be 
		/// used on every frame because performance.
		/// </summary>
		public ReadOnlyCollection<CollisionEntity> CollisionEntitiesList
		{
			get => collisionEntitiesList.AsReadOnly();
		}


		public bool DebugModeEnabled = true;
		public bool ShowHitboxes { get; set; } = true;
		private const int DebugHitboxesThickness = 2;







		///<summary>
		///The constructor, only creates the basic fields, does NOT load the entities
		///</summary>
		public Scene()
		{
			regularEntitiesList = new List<Entity>();
			collisionEntitiesList = new List<CollisionEntity>();
		}


		/// <summary>
		/// The function for creating all of the entities of the scene
		/// </summary>
		public virtual void Load()
		{
			
		}

		/// <summary>
		/// Adds a NON COLLISION entity to its proper list to be updated
		/// </summary>
		/// <param name="entity"></param>
		public void AddEntity(Entity entity)
		{
			if (entity is not null)
			{
				entity.entityEventCaller += HandleEntityEvent;
				regularEntitiesList.Add(entity);
			}
		}

		public void AddCollisionEntity(CollisionEntity entity)
		{
			entity.entityEventCaller += HandleEntityEvent;
			collisionEntitiesList.Add(entity);
		}

		public void RemoveEntity(Entity entity)
		{
			if (entity is CollisionEntity)
			{
				collisionEntitiesList.Remove(entity as CollisionEntity);
			}
			else
			{
				regularEntitiesList.Remove(entity);
			}
		}



		public new void RestartScene()
		{
			regularEntitiesList.Clear();
			collisionEntitiesList.Clear();

			Load();
		}



		// For now, this is only used to handle entities being able to destroy themselves
		public void HandleEntityEvent(Entity sender, EntityEvent eventType){
			if (eventType == EntityEvent.SELF_DESTRUCT)
			{
				RemoveEntity(sender);
			}
			else if (eventType == EntityEvent.RESTART_SCENE)
			{
				RestartScene();
			}
		}


		/// <summary>
		/// Before custom update : logic common to all scenes that can't be gotten rid of 
		/// </summary>
		/// <param name="gameTime"></param>
		protected sealed override void PreUpdate(GameTime gameTime)
		{
			//update the input handler
			InputHandler.CurrentHandler.Update();


			//if debug enabled, allow the user to use debug keys
			if (DebugModeEnabled && InputHandler.CurrentHandler.IsActionJustDown(act.DebugToggle))
			{
				ShowHitboxes ^= true;
			}
		}


		private void UpdateAllEntities(GameTime gameTime)
		{
			//updating all entities
			foreach (Entity entity in regularEntitiesList)
			{
				//adapt the time scale for each entity ?
				//(probably figure out something better here imo)
				if (entity.TimeScale == 1f)
				{
					entity.Update(gameTime);
				}
				else
				{
					entity.Update(new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime * entity.TimeScale));
				}
			}

			foreach (Entity collisionEntity in collisionEntitiesList)
			{
				if (collisionEntity.TimeScale == 1f)
				{
					collisionEntity.Update(gameTime);
				}
				else
				{
					collisionEntity.Update(new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime * collisionEntity.TimeScale));
				}
			}
		}


		/// <summary>
		/// After custom update : update all of the entities, correct and notify entities of collisions 
		/// </summary>
		/// <param name="gameTime">You know what GameTime is already</param>
		protected sealed override void PostUpdate(GameTime gameTime)
		{

			//updating scene entities
			UpdateAllEntities(gameTime);


			//detecting then correcting collisions
			//TODO : find a more optimized detection algorithm
			for (int i = 0; i < collisionEntitiesList.Count-1; i++)
			{

				CollisionEntity e1 = collisionEntitiesList[i];

				for (int j = i+1; j< collisionEntitiesList.Count; j++)
				{
					CollisionEntity e2 = collisionEntitiesList[j];
					
					//check for bounding box collision first
					if (e1.GetHitbox().GetBoundingBox().Intersects(e2.GetHitbox().GetBoundingBox()))
					{

						//don't care about collisions between two static objects
						if (e1.CollisionWeight.Equals(Double.PositiveInfinity) && e2.CollisionWeight.Equals(Double.PositiveInfinity))
						{
							return;
						}


						//perform the proper collision check routine
						Vector2 penetrationVector = e1.GetHitbox().SolveCollision(e2.GetHitbox());

						if (penetrationVector != Vector2.Zero)
						{

							//call the respective events
							e1.OnCollision(e2);
							e2.OnCollision(e1);


							//compute which percentage of the displacement each entity will do
							float w1 = e1.CollisionWeight;
							float w2 = e2.CollisionWeight;

							float normalConstant = (1 / w1) + (1 / w2);
							float e1MoveIntensity = (1 / w1) / normalConstant;
							float e2MoveIntensity = (1 / w2) / normalConstant;


							e1.ShiftPosition(penetrationVector * e1MoveIntensity);
							e2.ShiftPosition(-penetrationVector * e2MoveIntensity);

						}

					}
				}
			}
		}

		public virtual void CustomDraw(SpriteBatch spriteBatch)
		{

		}


		/// <summary>
		/// Draw all entities + handle debug display
		/// </summary>
		/// <param name="spriteBatch"></param>
		public sealed override void Draw(SpriteBatch spriteBatch)
		{
			//drawing all held entities
			foreach (Entity entity in regularEntitiesList)
			{
				if (entity.IsVisible)
				{
					entity.Draw(spriteBatch);
				}
			}


			//call the scene's custom draw method if applicable
			CustomDraw(spriteBatch);



			//draw collision entities + hitboxes if needed
			foreach (CollisionEntity collisionEntity in collisionEntitiesList)
			{
				collisionEntity.Draw(spriteBatch);


				if (ShowHitboxes)
				{
					//draw the hitbox one depth step over the entity
					IHitbox curHitbox = collisionEntity.GetHitbox();

					if (curHitbox is not null)
					{
						if (curHitbox is RectangleHitbox){
							SpritedEntity.HRectangleDraw(spriteBatch, curHitbox.GetBoundingBox(), 
								DebugHitboxesThickness, Color.LawnGreen, 
								collisionEntity.LayerDepth - SpritedEntity.DepthStep);
						}
						else if (curHitbox is CircleHitbox)
						{
							CircleHitbox circHitbox = curHitbox as CircleHitbox;

							SpritedEntity.CircleDraw(spriteBatch, new Vector2(circHitbox.X, circHitbox.Y),
													 circHitbox.Radius, DebugHitboxesThickness, Color.LawnGreen);
						}
						else if (curHitbox is ConvexPHitbox)
						{
							ConvexPHitbox polygon = curHitbox as ConvexPHitbox;
							int numVertices = polygon.Vertices.Count;


							for (int i = 0; i<numVertices; i++)
							{
								SpritedEntity.LineDraw(spriteBatch, polygon.Vertices[i], polygon.Vertices[(i + 1) % numVertices],
									DebugHitboxesThickness, Color.LawnGreen, SpritedEntity.DepthLayer(10));
							}
						}
					}
				}
			}
		}
	}
}
