﻿using Microsoft.Xna.Framework;
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
using StereoGame.Hitbox;

using System.Collections.ObjectModel;
using StereoGame.Entities;

namespace StereoGame
{
    public class Scene : Entity
	{
		private const int DebugHitboxesThickness = 2;


		private List<Entity> regularEntitiesList;
		private List<CollisionEntity> collisionEntitiesList;

		public ReadOnlyCollection<Entity> RegularEntitiesList { get; private set; }
		public ReadOnlyCollection<CollisionEntity> CollisionEntitiesList { get; private set; }


		public bool DebugModeEnabled { get; set; } = true;
		public bool ShowHitboxes { get; set; } = true;

		public float TimeElapsed { get; private set; } = 0f;





		///<summary>
		///The constructor, only creates the basic fields, does NOT load the entities
		///</summary>
		public Scene()
		{
			regularEntitiesList = new List<Entity>();
			collisionEntitiesList = new List<CollisionEntity>();

			RegularEntitiesList = new ReadOnlyCollection<Entity>(regularEntitiesList);
			CollisionEntitiesList = new ReadOnlyCollection<CollisionEntity>(collisionEntitiesList);
		}


		/// <summary>
		/// The function for creating all of the entities of the scene
		/// </summary>
		public virtual void Load()
		{
			//set some hardcoded mappings
			InputHandler.CurrentHandler.AddAction("DebugToggle", new() { Keys.F });
		}

		/// <summary>
		/// Adds a NON COLLISION entity to its proper list to be updated
		/// </summary>
		/// <param name="entity"></param>
		public void AddEntity(Entity entity)
		{
			if (entity is not null)
			{
				entity.EntityEventCaller += HandleEntityEvent;
				regularEntitiesList.Add(entity);
			}
		}

		public void AddCollisionEntity(CollisionEntity entity)
		{
			entity.EntityEventCaller += HandleEntityEvent;
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


		public float GetTimeElapsed()
		{
			return TimeElapsed;
		}

		public new void RestartScene()
		{
			regularEntitiesList.Clear();
			collisionEntitiesList.Clear();

			Load();
		}



		// For now, this is only used to handle entities being able to destroy themselves
		public void HandleEntityEvent(Entity sender, EntityEvent eventType){
			switch (eventType)
			{
				case (EntityEvent.SELF_DESTRUCT):
					RemoveEntity(sender);
					break;

				case (EntityEvent.RESTART_SCENE):
					RestartScene();
					break;

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
			if (DebugModeEnabled && InputHandler.CurrentHandler.IsActionJustDown("DebugToggle"))
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
			//update timer 
			TimeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

			//updating scene entities
			UpdateAllEntities(gameTime);


			//detecting then correcting collisions
			for (int i = 0; i < collisionEntitiesList.Count-1; i++)
			{

				CollisionEntity e1 = collisionEntitiesList[i];

				for (int j = i+1; j< collisionEntitiesList.Count; j++)
				{
					CollisionEntity e2 = collisionEntitiesList[j];

					//start by checking for grid coordinate collision (cheapest check)
					if (((e1.GridXPos & e2.GridXPos) | (e1.GridYPos & e2.GridYPos)) == 0u)
					{
						continue;
					}
					
					//next, check for bounding box collision
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
								SpritedEntity.DepthLayer(100));
						}
						else if (curHitbox is CircleHitbox circHitbox)
						{
							SpritedEntity.CircleDraw(spriteBatch, new Vector2(circHitbox.X, circHitbox.Y),
													 circHitbox.Radius, DebugHitboxesThickness, Color.LawnGreen, 
													 SpritedEntity.DepthLayer(100));
						}
						else if (curHitbox is ConvexPHitbox polygon)
						{
							int numVertices = polygon.Vertices.Count;


							for (int i = 0; i<numVertices; i++)
							{
								SpritedEntity.LineDraw(spriteBatch, polygon.Vertices[i], polygon.Vertices[(i + 1) % numVertices],
									DebugHitboxesThickness, Color.LawnGreen, SpritedEntity.DepthLayer(100));
							}
						}
					}
				}
			}
		}
	}
}
