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
		// If rectangle hitbox has lower type id than circle, then calling :
		// circle.SolveCollision(rectangle) will immediately call
		// rectangle.SolveCollision(circle) 
		public int GetTypeId();

		public void Shift(float shiftX, float shiftY);
		public IHitbox Shifted(float shiftX, float shiftY);

		public bool Intersects(IHitbox other);

		public RectangleF GetBoundingBox();


		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns>The smallest normed vector that would get the caller out of collision.
		/// Reversing that vector allows the callee to get out of collision.</returns>
		public Vector2 SolveCollision(IHitbox other);
	}
}
