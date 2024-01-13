using Microsoft.Xna.Framework;
using MonoGame.Extended;

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StereoGame
{
	public static class RNG
	{
		private static Random rng;
		public static Random Rng
		{
			get => rng ?? (rng = new Random()); 
		}

		/// <param name="min">Inclusive lower bound</param>
		/// <param name="max">Exclusive upper bound</param>
		public static int NextInt(int min, int max) 
			=> Rng.Next(min, max);

		public static double NextDouble()
			=> Rng.NextDouble();

		public static double NextDouble(double max)
			=> NextDouble() * max;

		public static double NextDouble(double min, double max)
			=> NextDouble(max-min) + min;

		public static float NextFloat()
			=> (float)Rng.NextDouble();

		public static float NextFloat(float max)
			=> NextFloat() * max;

		public static float NextFloat(float min, float max)
			=> NextFloat(max - min) + min;



		public static Vector2 RandomPos(ref Vector2 topLeft, ref Vector2 bottomRight)
			=> new Vector2(NextFloat(topLeft.X, bottomRight.X), NextFloat(topLeft.Y, bottomRight.Y));

		public static Vector2 RandomPos(ref RectangleF area)
			=> new Vector2(NextFloat(area.Left, area.Right), NextFloat(area.Top, area.Bottom));
	}
}
