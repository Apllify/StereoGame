using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace StereoGame.Particles
{
	/// <summary>
	/// A class that defines a shape for particle generation
	/// </summary>
	public abstract class ParticleShape
	{
		/// <summary>
		/// Computes and stores a position within the enclosed shape
		/// MUST be implemented by any children of this class 
		/// </summary>
		/// <param name="x">Variable for x coordinate</param>
		/// <param name="y">Variable for y coordinate</param>
		public abstract void NextPosition(out float x, out float y);



		/// <returns> The vector variant of the result of NextPosition(out x, out y) 
		/// </returns>
		public Vector2 NextPosition()
		{
			float x, y;
			NextPosition(out x, out y);
			return new(x, y);
		}
	}
}
