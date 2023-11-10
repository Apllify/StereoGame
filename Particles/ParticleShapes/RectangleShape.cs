using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using MonoGame.Extended;

namespace StereoGame.Particles.ParticleShapes
{
	public class RectangleShape
	{
		public float Left {get; set;}
		public float Right { get; set; }
		public float Top { get; set; }
		public float Bottom { get; set; }


		public RectangleShape(float left, float right, float top, float bottom)
		{
			(Left, Right, Top, Bottom) = (left, right, top, bottom);
		}

		public RectangleShape(RectangleF rec) :
			this(rec.Left, rec.Right, rec.Top, rec.Bottom)
		{ }

		public void NextPosition(out float x, out float y)
		{
			x = RNG.NextFloat(Left, Right);
			y = RNG.NextFloat(Top, Bottom);
		}
	}
}
