using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace StereoGame.Particles.ParticleShapes
{
	/// <summary>
	/// A shape describing the boundary (furthest points from center)
	/// of a cone. 
	/// Notably, if angle = 2 pi, the shape is a ring.
	/// </summary>
	public class ConeBoundaryShape : ParticleShape
	{
		public Vector2 Center { get; set; }
		public Vector2 Direction { get; set; }

		public float MinRadius { get; set; }
		public float MaxRadius { get; set; }
		public float Angle { get; set; }

		public ConeBoundaryShape(Vector2 center, Vector2 direction, float minRadius, float maxRadius, float angle)
		{
			Center = center;
			Direction = direction;
			Direction.Normalize();

			MinRadius = minRadius;
			MaxRadius = maxRadius;
			Angle = angle;
		}

		public override void NextPosition(out float x, out float y)
		{
			float curLength = RNG.NextFloat(MinRadius, MaxRadius);
			float curAngle = RNG.NextFloat(Angle);

			Vector2 point = curLength * Direction.Rotate(curAngle) + Center;
			(x, y) = (point.X, point.Y);
		}
	}
}
