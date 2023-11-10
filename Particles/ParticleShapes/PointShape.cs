using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace StereoGame.Particles.ParticleShapes
{
	public class PointShape : ParticleShape
	{
		public float X { get; set; }
		public float Y { get; set; }
		public PointShape(float x, float y)
		{
			(X, Y) = (x,y);
		}

		/// <summary>
		/// Just returns the same point every-time
		/// </summary>
		public override void NextPosition(out float x, out float y)
		{
			(x, y) = (X, Y);
		}
	}
}
