using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnfinishedBusinessman.StereoGame.Hitbox
{
	public interface IHitbox
	{
		// The hitbox priority helps choose which of two handles solving a collision
		// If rectangle hitbox has lower type id than circle, then calling :
		// circle.SolveCollision(rectangle) will immediately call
		// rectangle.SolveCollision(circle) 
		public int GetTypeId();

		//Current Hitbox IDs are : 
		//0 : rectangle
		//1 : circle



		/// <summary>
		/// Function that returns the displacement of the hitbox over the last frame.
		/// </summary>
		/// <returns></returns>
		public Vector2 GetLastMove();



		public void Shift(float shiftX, float shiftY);
		public IHitbox Shifted(float shiftX, float shiftY);

		public RectangleF GetBoundingBox();

		public bool Intersects(IHitbox other);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns>The smallest normed vector that would get the caller out of collision.
		/// Reversing that vector allows the callee to get out of collision.</returns>
		public Vector2 SolveCollision(IHitbox other);
	}
}
