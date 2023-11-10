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

		/// <summary>
		/// Returns the shortest Vector2 connecting the point to the segment.
		/// </summary>
		/// <param name="lineP1"></param>
		/// <param name="lineP2"></param>
		/// <param name="point"></param>
		/// <returns>A vector from the line to the argument point</returns>
		public static Vector2 LinePointShortest(Vector2 lineP1, Vector2 lineP2, Vector2 point)
		{
			//remove edge case 
			if (lineP1 == lineP2)
			{
				return (point - lineP1);
			}

			//project the point on the line 
			float lineProjection = (point - lineP1).Dot((lineP2 - lineP1)) / ((lineP2 - lineP1).Length());
			lineProjection /= (lineP2 - lineP1).Length();

			lineProjection = Math.Clamp(lineProjection, 0, 1);

			return -((point - lineP1) - lineProjection * (lineP2 - lineP1));

		}

		public static Vector2 RectanglePointShortest(RectangleF rec, Vector2 point)
		{
			Vector2 recCenter = rec.Center;

			//determine closest horizontal and vertical edges of the rectangle
			//edges are represented in rectangles so 
			var (verticalLineP1, verticalLineP2) = (point.X > recCenter.X) ?
										(rec.TopRight, rec.BottomRight) :
										(rec.TopLeft, rec.BottomLeft);

			var (horizontalLineP1, horizontalLineP2) = (point.Y > recCenter.Y) ?
										(rec.BottomRight, rec.BottomLeft) :
										(rec.TopRight, rec.TopLeft);

			//compute the shortest paths to the edges now
			Vector2 vertShortest = LinePointShortest(verticalLineP1, verticalLineP2, point);
			Vector2 horShortest = LinePointShortest(horizontalLineP1, horizontalLineP2, point);

			return (vertShortest.Length() > horShortest.Length()) ?
						horShortest :
						vertShortest;

		}



		public Vector2 SolveCollision(IHitbox other)
		{
			if (other.GetTypeId() > GetTypeId())
			{
				return -other.SolveCollision(this);
			}

			if (other is CircleHitbox)
			{
				CircleHitbox otherCircle = other as CircleHitbox;
				return SolveCircleCollision(otherCircle);

			}
			else if (other is RectangleHitbox)
			{
				RectangleHitbox otherRectangle = other as RectangleHitbox;
				return SolveBoxCollision(otherRectangle);
			}
			else
			{
				throw new NotImplementedException();
			}


		}

		public Vector2 SolveCircleCollision(CircleHitbox other)
		{
			Vector2 centerToCenter = new Vector2(X - other.X, Y - other.Y);

			float overlapLength = Math.Max(Radius + other.Radius - centerToCenter.Length(), 0);

			centerToCenter.Normalize();
			return centerToCenter * overlapLength;
		}


		/// <summary>
		/// Handles box-circle collisions.
		/// Just does standard displacing along collision axis.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		private Vector2 SolveBoxCollision(RectangleHitbox other)
		{
			//similar behavior to intersection checking
			Vector2 circleCenter = new Vector2(X, Y);
			RectangleF otherHitbox = other.GetBoundingBox();

			//compute the shortest path between the center of the circle and the rec
			Vector2 penetrationVector = RectanglePointShortest(otherHitbox, circleCenter);
			float pLength = penetrationVector.Length();
			penetrationVector.Normalize();


			//CASE 0 : no collision
			if (pLength > Radius)
			{
				return Vector2.Zero;
			}


			//CASE 1 : the center of the circle is inside of the rectangle 
			if (otherHitbox.Contains(circleCenter))
			{
				return (pLength + Radius) * penetrationVector;
			}



			//CASE 2 : the center of the circle is NOT in the rectangle 
			return (pLength - Radius) * penetrationVector;
		}






	}

}

