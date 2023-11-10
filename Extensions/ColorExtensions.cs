using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using StereoGame.Extras;

namespace StereoGame.Extensions
{

	/// <summary>
	/// Provides shorthand for working with colors
	/// </summary>
	public static class ColorExtensions
	{
		public static void Add(this Color c1, Color c2)
		{
			c1.R += c2.R;
			c1.G += c2.G;
			c1.B += c2.B;
		}

		public static Color Added(Color c1, Color c2)
		{
			return new Color(Math.Min(255, c1.R + c2.R), Math.Min(255, c1.G + c2.G), Math.Min(255, c1.B + c2.B));
		}

		public static ColorF ToColorF(this Color c)
		{
			return new ColorF(c.R, c.G, c.B);
		}

	}
}
