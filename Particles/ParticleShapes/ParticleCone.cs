using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Xna.Framework;

namespace StereoGame.Particles.ParticleShapes
{
	public class ParticleCone : IParticleShape
	{
		Random rng;
		Vector2 Center { get;  set; }
		Vector2 Direction { get; set; }

		float Radius { get; set; }
		float Angle { get; set; }

		public ParticleCone (Vector2 center, Vector2 directionVec, float radius, float angle)
		{
			rng = new Random();

			Center = center; 
			Direction = directionVec;

			Radius = radius;
			Angle = angle;
		}

		public void NextPosition(out float x, out float y)
		{
			float curAngle = (float)rng.NextDouble() * Angle;
			float length = (float)rng.NextDouble() * Radius;

			//TODO : finish this
		}

		public Vector2 NextPosition()
		{
			float x, y;
			NextPosition(out x, out y);

			return new Vector2(x, y);
		}
	}
}
