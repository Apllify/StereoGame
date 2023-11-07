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
	public interface IParticleShape
	{
		/// <returns> A position within the defined shape</returns>
		public Vector2 NextPosition();

		public void NextPosition(out float x, out float y);

	}
}
