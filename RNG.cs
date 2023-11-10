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
		public static Random currentRNG;

		/// <summary>
		/// Called by the top level game file before creating 
		/// any scenes or entities.
		/// </summary>
		public static void Initialize()
		{
			currentRNG = new Random();
		}


		/// <param name="min">Inclusive lower bound</param>
		/// <param name="max">Exclusive upper bound</param>
		public static int NextInt(int min, int max) 
			=> currentRNG.Next(min, max);

		public static double NextDouble()
			=> currentRNG.NextDouble();

		public static float NextFloat()
			=> (float)currentRNG.NextDouble();

		public static float NextFloat(float max)
			=> NextFloat() * max;

		public static float NextFloat(float min, float max)
			=> NextFloat(max - min) + min;
	}
}
