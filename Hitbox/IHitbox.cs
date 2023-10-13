using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StereoGame.Hitbox
{
	public interface IHitbox
	{
		// The hitbox priority helps choose which of two handles solving a collision
		// Later type IDs must implement collision methods for all earlier type IDs
		// If rectangle.intersects(circle) is called, it will in turn call circle.intersects(rectangle)
		public int GetTypeId();

		//Current Hitbox IDs are : 
		//0 : rectangle
		//1 : circle
		//2 : convex polygon



		/// <summary>
		/// Function that returns the displacement of the hitbox over the last frame.
		/// </summary>
		/// <returns></returns>
		public Vector2 GetLastMove();



		public void Shift(float shiftX, float shiftY);
		public IHitbox Shifted(float shiftX, float shiftY);

		public RectangleF GetBoundingBox();




		/// <summary>
		/// Computes the resolution of the collision of the two elements, through case analysis.
		/// </summary>
		/// <param name="other"></param>
		/// <returns>The smallest normed vector that would get the caller out of collision.
		/// Reversing that vector allows the callee to get out of collision.
		/// The return vector is Zero if there is no collision.</returns>
		public Vector2 SolveCollision(IHitbox other);
	}
}
