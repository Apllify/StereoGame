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


		//variables for collision handling
		private CollisionEntity e1;
		private CollisionEntity e2;

		private double e1MoveIntensity;
		private double e2MoveIntensity;
		private double normalConstant;

		private Vector2 collisionVector;
		private Vector2 e1MoveAway;
		private Vector2 e2MoveAway;

		private float newCollisionAngle;
		private float e1HalfDiag;
		private float e2HalfDiag;
		private float safeDistance;
		




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


		/// <summary>
		/// After custom update : update all of the entities, correct and notify entities of collisions 
		/// </summary>
		/// <param name="gameTime">You know what GameTime is already</param>
		protected sealed override void PostUpdate(GameTime gameTime)
		{

			//updating all entities
			foreach(Entity entity in regularEntitiesList)
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

			foreach(Entity collisionEntity in collisionEntitiesList)
			{
				collisionEntity.Update(gameTime);
			}


			//detecting then correcting collisions
			//TODO : find a more optimized detection algorithm
			for (int i = 0; i < collisionEntitiesList.Count-1; i++)
			{

				e1 = collisionEntitiesList[i];

				for (int j = i+1; j< collisionEntitiesList.Count; j++)
				{
					e2 = collisionEntitiesList[j];
					

					if (e1.GetHitbox().Intersects(e2.GetHitbox()))
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


			//TODO : optimization once i find a better algorithm



			//IF YOU'RE READING THIS, the way to approach this i think should be : 
			// - slide both objects along the component of their displacement vector that is orthogonal
			// to the vector connecting the two objects 
			// - if that vector is 0, then just resort to the vector connecting the two.


			//computing the straightest vector to leave the collision point
			collisionVector = e1.GetPosition() - e2.GetPosition();
			collisionVector.Normalize();

			//TODO : fix this
			//e1MoveAway = e1.GetLastFrameMovement().ProjectOnto(collisionVector);
			//if (e1MoveAway.Equals(Vector2.Zero))
			//{
			//	e1MoveAway = collisionVector;
			//}
			//else
			//{
			//	e1MoveAway.Normalize();
			//}

			//e2MoveAway = -e2.GetLastFrameMovement().ProjectOnto(collisionVector);
			//if (e2MoveAway.Equals(Vector2.Zero))
			//{
			//	e2MoveAway = -collisionVector;
			//}
			//else
			//{
			//	e2MoveAway.Normalize();
			//}

			e1MoveAway = collisionVector;
			e2MoveAway = -collisionVector;




			//compute the distance at which we're guaranteed not to have a collision
			e1HalfDiag = e1.GetHitbox().GetDimensions().Length() / 2;
			e2HalfDiag = e2.GetHitbox().GetDimensions().Length() / 2;
			safeDistance = e1HalfDiag + e2HalfDiag;




			//compute which percentage of the displacement each entity will do
			normalConstant = (1 / e1.CollisionWeight) + (1 / e2.CollisionWeight);
			e1MoveIntensity = (1 / e1.CollisionWeight) * normalConstant;
			e2MoveIntensity = (1 / e2.CollisionWeight) * normalConstant;

			for (int counter = 0; counter < safeDistance; counter += CollisionEntity.CollisionPixelPrecision)
			{
				//shift both entities accordingly
				e1.ShiftPosition(e1MoveAway * (float)e1MoveIntensity);
				e2.ShiftPosition(e2MoveAway * (float)e2MoveIntensity);


				//leave if collision is finally gone
				if (!e1.GetHitbox().Intersects(e2.GetHitbox()))
				{
					return;
				}
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

			foreach (Entity collisionEntity in collisionEntitiesList)
			{

				collisionEntity.Draw(spriteBatch);
			}
		}
	}
}
