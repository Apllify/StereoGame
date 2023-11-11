using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using StereoGame.Extensions;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using System.Diagnostics;
using UnfinishedBusinessman.StereoGame.Extensions;

namespace StereoGame.Particles
{
    public class Particle 
	{


		public Vector2 Position { get; set; }
		public Vector2 MoveDirection { get; set; }
		public float Velocity { get; set; } = 0;
		public float Acceleration { get; set; } = 0;

		public bool IsDead { get; set; } = false;

		public float Lifespan { get; set; }
		public float Size { get; set; }
		public float GrowthRate { get; set; }

		private ColorF pColor  = Color.White.ToColorF(); //starting particle color
		private Vector3 colorMove; //evolution of pcolor over time 

		private float fadeout; //induce a fade out effect over time



		/// <summary>
		/// Creates a particles with given properties.
		/// See property documentation for more info.
		/// </summary>
		public Particle(Vector2 pos, Vector2 movDir, float velocity, float acceleration, 
						float lifespan, float size, float growthRate, ColorF startColor, ColorF? endColor)
		{

			Position = pos;
			MoveDirection = (movDir == Vector2.Zero) ? Vector2.Zero : movDir.NormalizedCopy();
			
			(Velocity, Acceleration) = (velocity, acceleration);
			(Size, GrowthRate) = (size, growthRate);

			Lifespan = lifespan;
			
			//color related things
			pColor = startColor;
			ColorF endPColor = (endColor.HasValue) ? endColor.Value : pColor;

			colorMove = new Vector3(endPColor.R - pColor.R,
									endPColor.G - pColor.G,
									endPColor.B - pColor.B);
			colorMove /= lifespan;
			fadeout = pColor.A / lifespan;
		}


		/// <summary>
		/// Creates a particle using the information from a ParticleTemplate (see the 
		/// latter for more information).
		/// </summary>
		public static Particle FromTemplate(ParticleTemplate template)
		{
			Particle p;

			//assigns every single field accordingly
			//only missing fields are position, and movement dir (not part of template)
			float velocity = RNG.NextFloat(template.MinVelocity, template.MaxVelocity);
			float acceleration = template.Acceleration;

			float lifeSpan = RNG.NextFloat(template.MinLifeSpan, template.MaxLifeSpan);

			float size = template.Size;
			float growthRate = template.GrowthRate;

			//end color guaranteed to have value 
			ColorF startC = template.StartPColor;
			ColorF? endC = template.EndPColor;


			
			p = new Particle(new(), new(), velocity, acceleration, 
							 lifeSpan, size, growthRate, startC, endC);
			
			return p;

		}

		public void Update(GameTime gameTime)
		{
			float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;

			Lifespan -= deltaT;

			if (Lifespan <= 0f)
			{
				IsDead = true;
			}

			Velocity += Acceleration * deltaT;
			Position += MoveDirection * Velocity * deltaT;

			Size += GrowthRate * deltaT;

			//use incremental color
			pColor += colorMove * deltaT;
			pColor.Alpha -= fadeout * deltaT;

		}


		/// <summary>
		/// For now, all particles are just squares
		/// Might consider optimizing in the future.
		/// </summary>
		public void Draw(SpriteBatch spriteBatch)
		{
			//for now, all particles are just squares
			//might want to optimize this to improve max particle count
			SpritedEntity.RectangleDraw(spriteBatch, new RectangleF(Position, new Size2(Size, Size)), 
										pColor.ToColor(), SpritedEntity.DepthLayer(20));
		}

	}
}
