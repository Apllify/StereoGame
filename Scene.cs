using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StereoGame
{
	public class Scene : Entity
	{
		private List<Entity> regularEntitiesList;
		private List<CollisionEntity> collisionEntitiesList;

		public bool ShowHitboxes { get; set; } = true;



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
			entity.entityEventCaller += HandleEntityEvent;
			regularEntitiesList.Add(entity);
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
					

					if (e1.GetHitbox().VisuallyIntersects(e2.GetHitbox()))
					{

						//call the respective events
						e1.OnCollision(e2);
						e2.OnCollision(e1);

						//get them out of collision state
						SolveCollision(e1, e2);

					}
				}
			}





		}

		private void SolveCollision(CollisionEntity e1, CollisionEntity e2)
		{
			//don't care about collisions between two static objects
			if (e1.CollisionWeight.Equals(Double.PositiveInfinity) && e2.CollisionWeight.Equals(Double.PositiveInfinity))
			{
				return;
			}

			//for now, don't care when two elements are perfectly superposed 
			if (e1.GetPosition().EqualsWithTolerence(e2.GetPosition(), 1E-02f))
			{
				return;
			}


			RectangleEntity e1Hitbox = e1.GetHitbox();
			RectangleEntity e2Hitbox = e2.GetHitbox();

			Rectangle collisionRectangle = e1Hitbox.VisualIntersectionRectangle(e2Hitbox);

			//compute which percentage of the displacement each entity will do
			float normalConstant = (1 / e1.CollisionWeight) + (1 / e2.CollisionWeight);
			float e1MoveIntensity = (1 / e1.CollisionWeight) / normalConstant;
			float e2MoveIntensity = (1 / e2.CollisionWeight) / normalConstant;





			//displace the entities based on which of the dimensions of intersection is smaller
			if (collisionRectangle.Width >= collisionRectangle.Height)
			{
				//displace vertically 
				int e1Dir = (e1Hitbox.GetTopLeftPosition().Y > e2Hitbox.GetTopLeftPosition().Y) ? 1 : -1;

				e1.ShiftPosition(0, e1Dir * (float)e1MoveIntensity * collisionRectangle.Height);
				e2.ShiftPosition(0, -e1Dir * (float)e2MoveIntensity * collisionRectangle.Height);
			}
			else
			{
				//displace horizontally 
				int e1Dir = (e1Hitbox.GetTopLeftPosition().X > e2Hitbox.GetTopLeftPosition().X) ? 1 : -1;

				e1.ShiftPosition(e1Dir * (float)e1MoveIntensity * collisionRectangle.Width, 0);
				e2.ShiftPosition(-e1Dir * (float)e2MoveIntensity * collisionRectangle.Width, 0);
			}



		}


		/// <summary>
		/// Just draw all of the entities
		/// </summary>
		/// <param name="spriteBatch"></param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			//drawing all held entities
			foreach (Entity entity in regularEntitiesList)
			{
				entity.Draw(spriteBatch);
			}


			//draw collision entities + hitboxes if needed
			foreach (Entity collisionEntity in collisionEntitiesList)
			{
				collisionEntity.Draw(spriteBatch);


				if (ShowHitboxes)
				{
					//do stuff here
				}
			}
		}
	}
}
