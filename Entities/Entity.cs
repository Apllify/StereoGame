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

        }



        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public void SetTimeScale(float newTimeScale)
        {
            TimeScale = newTimeScale;
        }

        protected void SelfDestruct()
        {
            EntityEventCaller?.Invoke(this, EntityEvent.SELF_DESTRUCT);
            DeathEvent?.Invoke();
        }

        protected void RestartScene()
        {
            EntityEventCaller?.Invoke(this, EntityEvent.RESTART_SCENE);
        }

    }
}
