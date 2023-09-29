﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnfinishedBusinessman.StereoGame.Hitbox;

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
					

					if (e1.GetHitbox().Intersects(e2.GetHitbox()))
					{

						//don't care about collisions between two static objects
						if (e1.CollisionWeight.Equals(Double.PositiveInfinity) && e2.CollisionWeight.Equals(Double.PositiveInfinity))
						{
							return;
						}



						//call the respective events
						e1.OnCollision(e2);
						e2.OnCollision(e1);


						//compute which percentage of the displacement each entity will do
						float w1 = e1.CollisionWeight;
						float w2 = e2.CollisionWeight;

						float normalConstant = (1 / w1) + (1 / w2);
						float e1MoveIntensity = (1 / w1) / normalConstant;
						float e2MoveIntensity = (1 / w2) / normalConstant;
						Vector2 penetrationVector;


						//get them out of collision state
						penetrationVector = e1.GetHitbox().SolveCollision(e2.GetHitbox());


						//TODOOOO SHIFT HERE THE TWO ENTITIES
						e1.ShiftPosition(penetrationVector * e1MoveIntensity);
						e2.ShiftPosition(-penetrationVector * e2MoveIntensity);

					}
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
						if ((curHitbox = curHitbox as RectangleHitbox) is not null){
							SpritedEntity.HRectangleDraw(spriteBatch, curHitbox.GetBoundingBox(), 3, Color.LawnGreen, 
								collisionEntity.LayerDepth - SpritedEntity.DepthStep);
						}
					}
				}
			}
		}
	}
}
