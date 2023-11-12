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
	/// A "cone" shape which looks like a set "slice" of a disc.
	/// In the case of angle = 2 pi, the shape is simply a filled disc/2D circle.
	/// </summary>
	public class ConeShape : ParticleShape
	{
		Vector2 Center { get;  set; }
		Vector2 Direction { get; set; }

		float Radius { get; set; }
		float Angle { get; set; }



		public ConeShape (Vector2 center, Vector2 directionVec, float radius, float angle)
		{
			Center = center; 
			Direction = directionVec;
			Direction.Normalize();

			Radius = radius;
			Angle = Math.Clamp(angle, 0, 2 * MathF.PI);
		}



		/// <summary>
		/// Randomly generate a point in the cone of this instance.
		/// </summary>
		public override void NextPosition(out float x, out float y)
		{
			float curAngle = RNG.NextFloat(Angle);
			float curLength = RNG.NextFloat(Radius);

			Vector2 curPoint = (curLength * Direction.Rotate(curAngle)) + Center;
			(x, y) = (curPoint.X, curPoint.Y);
		}
	}
}
