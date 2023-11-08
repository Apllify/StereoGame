using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
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

		public Color pColor; //must be field since struct
		public Color PColorGradient { get; set; } = Color.Black;



		/// <summary>
		/// Creates a particles with given properties.
		/// See property documentation for more info.
		/// </summary>
		public Particle(Vector2 pos, Vector2 velocity, Vector2 acceleration, 
						float lifespan, float growthRate, Color color)
		{
			(Position, Velocity, Acceleration, Lifespan, GrowthRate, pColor) = 
				(pos, velocity, acceleration, lifespan, growthRate, color);
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

			//TODO : finish me 

			
		}


	}
}
