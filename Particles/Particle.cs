using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using StereoGame.Extras;
using StereoGame.Extensions;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;

namespace StereoGame.Particles
{
	public class Particle 
	{

		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public Vector2 Acceleration { get; set; } = Vector2.Zero;

		public bool IsAlive { get; set; } = true;

		public float Lifespan { get; set; }
		public float Size { get; set; }
		public float GrowthRate { get; set; }

		private ColorF pColor; //must be field since struct
		public ColorF PColorGradient { get; set; } = new ColorF(0, 0, 0);



		/// <summary>
		/// Creates a particles with given properties.
		/// See property documentation for more info.
		/// </summary>
		public Particle(Vector2 pos, Vector2 velocity, Vector2 acceleration, 
						float lifespan, float size, float growthRate, Color color)
		{
			(Position, Velocity, Acceleration, Size, Lifespan, GrowthRate, pColor) = 
				(pos, velocity, acceleration, size, lifespan, growthRate, color.ToColorF());
		}


		/// <summary>
		/// Creates a particle using the information from a ParticleTemplate (see the 
		/// latter for more information).
		/// </summary>
		public static Particle FromTemplate(ParticleTemplate template)
		{
			Particle p;
			float size = template.Size;
			float lifeSpan = RNG.NextFloat(template.MinLifeSpan, template.MaxLifeSpan);
		}

		public void Update(GameTime gameTime)
		{
			float deltaT = gameTime.ElapsedGameTime.Seconds;

			Lifespan -= deltaT;

			if (Lifespan <= 0f)
			{
				IsAlive = false;
			}

			Velocity += Acceleration * deltaT;
			Position += Velocity * deltaT;

			Size += GrowthRate * deltaT;

			//use incremental color
			pColor += PColorGradient;

		}


		/// <summary>
		/// For now, all particles are just squares
		/// Might consider optimizing in the future.
		/// </summary>
		public void Draw(SpriteBatch spriteBatch)
		{
			//for now, all particles are just squares
			//might want to optimize this to improve max particle count
			SpritedEntity.RectangleDraw(spriteBatch, new RectangleF(Position, new Size2(Size, Size)), pColor.ToColor());
		}

	}
}
