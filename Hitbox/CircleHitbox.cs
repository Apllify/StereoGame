using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StereoGame.Hitbox
{
	public class CircleHitbox : IHitbox
	{
		public float X { get; private set; }
		public float Y { get; private set; }

		public float Radius { get; private set; }

		private Vector2 lastFrameMovement;



		public CircleHitbox(float _x, float _y, float _radius)
		{
			X = _x;
			Y = _y;

			Radius = _radius;

			lastFrameMovement = new Vector2(0, 0);
		}

		public int GetTypeId()
		{
			return 1;
		}

		public void Shift(float shiftX, float shiftY)
		{
			X += shiftX;
			Y += shiftY;

			lastFrameMovement.X = shiftX;
			lastFrameMovement.Y = shiftY;
		}

		public Vector2 GetLastMove()
		{
			return lastFrameMovement;
		}

		public IHitbox Shifted(float shiftX, float shiftY)
		{
			CircleHitbox clone = new CircleHitbox(X, Y, Radius);
			clone.Shift(shiftX, shiftY);

			return clone;
		}

		public RectangleF GetBoundingBox()
		{
			float topLeftX = X - Radius;
			float topLeftY = Y - Radius;

			return new RectangleF(topLeftX, topLeftY, Radius * 2, Radius * 2);
		}


		public bool Intersects(IHitbox other)
		{
			if (other.GetTypeId() < GetTypeId())
			{
				return other.Intersects(this);
			}

			if (other is CircleHitbox)
			{
				CircleHitbox otherCircle = other as CircleHitbox;
				Vector2 centerToCenter = new Vector2(X - otherCircle.X, Y - otherCircle.Y);

				return (centerToCenter.Length() < (Radius + otherCircle.Radius));
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public Vector2 SolveCollision(IHitbox other)
		{
			if (other.GetTypeId() < GetTypeId())
			{
				return -other.SolveCollision(this);
			}

			if (other is CircleHitbox)
			{
				CircleHitbox otherCircle = other as CircleHitbox;
				Vector2 centerToCenter = new Vector2(X - otherCircle.X, Y - otherCircle.Y);

				float overlapLength = Math.Abs(centerToCenter.Length() - (Radius + otherCircle.Radius));

				centerToCenter.Normalize();
				return centerToCenter * overlapLength;

			}
			else
			{
				throw new NotImplementedException();
			}


		}


	}
}
