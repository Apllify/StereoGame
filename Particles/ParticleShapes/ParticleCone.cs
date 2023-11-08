using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace StereoGame.Particles.ParticleShapes
{
	public class ParticleCone : IParticleShape
	{
		Random rng;
		Vector2 Center { get;  set; }
		Vector2 Direction { get; set; }

		float Radius { get; set; }
		float Angle { get; set; }


		/// <param name="angle"> The angle of the cone from direction vec, clockwise rotation.</param>
		public ParticleCone (Vector2 center, Vector2 directionVec, float radius, float angle)
		{
			rng = new Random();

			Center = center; 
			Direction = directionVec;
			Direction.Normalize();

			Radius = radius;
			Angle = angle;
		}


		public Vector2 NextPosition()
		{
			float curAngle = (float)rng.NextDouble() * Angle;
			float length = (float)rng.NextDouble() * Radius;

			return (Direction * length).Rotate(curAngle) + Center;
		}

		/// <summary>
		/// Slightly slower than vector2 nextPosition(), only for convenience
		/// </summary>
		public void NextPosition(out float x, out float y)
		{
			Vector2 current = NextPosition();
			x = current.X;
			y = current.Y;
		}
	}
}
