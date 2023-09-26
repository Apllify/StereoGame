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

		public void ShiftPosition(float xShift, float yShift);

		public bool Intersects(IHitbox other);

		public RectangleF GetBoundingBox();

		public void SolveCollision(IHitbox other);
	}
}
