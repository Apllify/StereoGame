using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StereoGame
{
	public class Entity
	{
		public String Tag { get; protected set; }
		public float TimeScale { get; private set; } = 1;
		public bool IsVisible { get; set; } = true;


		public enum EntityEvent
		{
			SELF_DESTRUCT,	
			RESTART_SCENE
		}
		public delegate void EntityEventHandler(Entity entity, EntityEvent entityEvent);
		public event EntityEventHandler entityEventCaller;



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
			entityEventCaller?.Invoke(this, EntityEvent.SELF_DESTRUCT);
		}

		protected void RestartScene()
		{
			entityEventCaller?.Invoke(this, EntityEvent.RESTART_SCENE);
		}

	}
}
