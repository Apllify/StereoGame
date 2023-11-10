using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StereoGame.Particles
{
	public class ParticleEmitter : Entity
	{

		private List<Particle> particles;
		private ParticleShape emissionShape;
		private ParticleShape receptionShape;

		public float MinV { get; set; }
		public float MaxV { get; set; }
		public float Acceleration { get; set; }

		public float MinSpan { get; set; }
		public float MaxSpan { get; set; }
		public Color PColor { get; set; }

		public float Size { get; set; }
		public float GrowthRate { get; set; }


		public ParticleEmitter(ParticleShape _emissionShape, ParticleShape _receptionshape,
							   float minV, float maxV, float acceleration,
							   float minSpan, float maxSpan, Color color, float size, float growthRate)
		{
			particles = new();
			emissionShape = _emissionShape;
			receptionShape = _receptionshape;

			(MinV, MaxV, Acceleration) = (minV, maxV, acceleration);
			(MinSpan, MaxSpan) = (minSpan, maxSpan);
			PColor = color;
			(Size, GrowthRate) = (size, growthRate);
		}

		protected override void CustomUpdate(GameTime gameTime)
		{
			ParticleTemplate p = new()
			{
				minVelocity = 4,
				maxVelocity = 5
			}
				;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			
		}

	}
}
