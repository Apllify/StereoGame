using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StereoGame.Entities
{
    public class Entity
    {

        //object properties
        public string Tag { get; protected set; }
        public float TimeScale { get; private set; } = 1;
        public bool IsVisible { get; set; } = true;


        public enum EntityEvent
        {
            SELF_DESTRUCT,
            RESTART_SCENE
        }
        public delegate void EntityEventHandler(Entity entity, EntityEvent entityEvent);
        public event EntityEventHandler EntityEventCaller;

        public event Action DeathEvent;

        //object fields
        private bool IsDestroyQueued = false;
        private bool IsDestroyFrame = false;

        public void Update(GameTime gameTime)
        {
            PreUpdate(gameTime);
            CustomUpdate(gameTime);
            PostUpdate(gameTime);
        }

        protected virtual void PreUpdate(GameTime gameTime)
        {

        }

        protected virtual void CustomUpdate(GameTime gameTime)
        {

        }

        protected virtual void PostUpdate(GameTime gameTime)
        {
            //check if a self destruct is queued
            if (IsDestroyQueued)
            {
                if (IsDestroyFrame)
                {
                    DestroyInstant();
                }
                else
                {
                    IsDestroyFrame = true;
                }
            }
        }



        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public void SetTimeScale(float newTimeScale)
        {
            TimeScale = newTimeScale;
        }

        /// <summary>
        /// Instantly destroys the entity 
        /// (WARNING : unsafe method, only use if you know what you're doing)
        /// </summary>
        public void DestroyInstant()
        {
			EntityEventCaller?.Invoke(this, EntityEvent.SELF_DESTRUCT);
			DeathEvent?.Invoke();
		}

        /// <summary>
        /// Queues the entity to be destroyed in the next frame 
        /// </summary>
        public void Destroy()
        {
            IsDestroyQueued = true;
        }

        /// <summary>
        /// Restarts the ongoing scene
        /// </summary>
        protected void RestartScene()
        {
            EntityEventCaller?.Invoke(this, EntityEvent.RESTART_SCENE);
        }

    }
}
