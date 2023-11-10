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

		private Action<Particle> destroyer;

		public Vector2 Position { get; set; }
		public Vector2 MoveDirection { get; set; }
		public float Velocity { get; set; } = 0;
		public float Acceleration { get; set; } = 0;

		public float Lifespan { get; set; }
		public float Size { get; set; }
		public float GrowthRate { get; set; }

		private ColorF pColor; //must be field since struct
		public ColorF PColorGradient { get; set; } = new ColorF(0, 0, 0);



		/// <summary>
		/// Creates a particles with given properties.
		/// See property documentation for more info.
		/// </summary>
		public Particle(Action<Particle> _destroyer, Vector2 pos, Vector2 movDir, float velocity, float acceleration, 
						float lifespan, float size, float growthRate, ColorF color)
		{
			destroyer = _destroyer;

			Position = pos;
			MoveDirection = (movDir == Vector2.Zero) ? Vector2.Zero : movDir.NormalizedCopy();
			
			(Velocity, Acceleration) = (velocity, acceleration);
			(Size, GrowthRate) = (size, growthRate);

			Lifespan = lifespan;
			pColor = color;
		}


		/// <summary>
		/// Creates a particle using the information from a ParticleTemplate (see the 
		/// latter for more information).
		/// </summary>
		public static Particle FromTemplate(Action<Particle> destroyer, ParticleTemplate template)
		{
			Particle p;

			//assigns every single field accordingly
			//only missing fields are position, and movement dir (not part of template)
			float velocity = RNG.NextFloat(template.MinVelocity, template.MaxVelocity);
			float acceleration = template.Acceleration;

			float lifeSpan = RNG.NextFloat(template.MinLifeSpan, template.MaxLifeSpan);

			float size = template.Size;
			float growthRate = template.GrowthRate;

			ColorF pcolor = template.PColor;
			ColorF pcolorGradient = template.PColorGradient;


			//set the fields that aren't part of the constructor separately
			p = new Particle(destroyer, new(), new(), velocity, acceleration, lifeSpan, size, growthRate, pcolor);
			p.PColorGradient = pcolorGradient;
			
			return p;

		}

		public void Update(GameTime gameTime)
		{
			float deltaT = gameTime.ElapsedGameTime.Seconds;

			Lifespan -= deltaT;

			if (Lifespan <= 0f)
			{
				destroyer(this);
			}

			Velocity += Acceleration * deltaT;
			Position += MoveDirection * Velocity * deltaT;

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
