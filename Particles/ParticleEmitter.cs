using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using StereoGame.Entities;

namespace StereoGame.Particles
{
    public class ParticleEmitter : Entity
	{

		private List<Particle> particles;
		private ParticleShape emissionShape;
		private ParticleShape receptionShape;

		private List<Particle> deadParticles;
		public ParticleTemplate ParticleTemp { get; set; }
		public float EmissionDelay { get; set; }
		public int EmissionCount { get; set; }
		private float emissionTimer = 0;

		/// <summary>
		/// Creates a particle generator.
		/// </summary>
		/// <param name="_emissionShape">The shape where the particles will originate</param>
		/// <param name="_receptionshape">The shape that the particles will move towards</param>
		/// <param name="particleTemplate">The values used to generate particles</param>
		/// <param name="emissionDelay">The interval (s) between emissions</param>
		/// <param name="emissionCount">The number of particles created per emission</param>
		public ParticleEmitter(ParticleShape _emissionShape, ParticleShape _receptionshape,
							   ParticleTemplate particleTemplate, float emissionDelay, int emissionCount)
		{
			//keep as few fields/properties as possible
			particles = new();
			emissionShape = _emissionShape;
			receptionShape = _receptionshape;

			deadParticles = new();

			ParticleTemp = particleTemplate;
			EmissionDelay = emissionDelay;
			EmissionCount = emissionCount;
		}

		protected override void CustomUpdate(GameTime gameTime)
		{
			emissionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

			//check for timer event
			if (emissionTimer >= EmissionDelay)
			{
				//reset timer
				emissionTimer = 0;

				//generate the appropriate amount of particles
				for (int i = 0; i< EmissionCount; i++)
				{
					//create the particle from template
					Particle newP = Particle.FromTemplate(ParticleTemp);

					//add position and direction information
					Vector2 source, destination;
					source = emissionShape.NextPosition();
					destination = receptionShape.NextPosition();

					newP.Position = source;
					newP.MoveDirection = (destination - source).NormalizedCopy();


					//add particle to list
					particles.Add(newP);
				}
			}

			//update all particles + remove dead ones
			deadParticles.Clear();
			foreach(Particle p in particles)
			{
				p.Update(gameTime);

				if (p.IsDead)
					deadParticles.Add(p);
			}

			foreach(Particle dp in deadParticles)
			{
				particles.Remove(dp);
			}

		
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			foreach(Particle p in particles)
			{
				p.Draw(spriteBatch);
			}
		}

	}
}
