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
		private Vector2 linePointShortest(Vector2 lineP1, Vector2 lineP2, Vector2 point)
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


		public bool Intersects(IHitbox other)
		{
			if (other.GetTypeId() > GetTypeId())
			{
				return other.Intersects(this);
			}

			if (other is CircleHitbox)
			{
				CircleHitbox otherCircle = other as CircleHitbox;
				Vector2 centerToCenter = new Vector2(X - otherCircle.X, Y - otherCircle.Y);

				return (centerToCenter.Length() < (Radius + otherCircle.Radius));
			}
			else if (other is RectangleHitbox)
			{
				RectangleF otherHitbox = other.GetBoundingBox();

				Vector2 circleCenter = new Vector2(X, Y);

				//INTERSECTION CASE 1 : the center of the circle is in the rectangle
				if (otherHitbox.Contains(circleCenter))
				{
					return true;
				}

				//INTERSECTION CASE 2 : the center of circle is outside the rectangle but still collides
				Vector2 horizontalLineP1;
				Vector2 horizontalLineP2;

				Vector2 verticalLineP1;
				Vector2 verticalLineP2;

				(horizontalLineP1, horizontalLineP2) = (Y > otherHitbox.Y) ?
							(otherHitbox.BottomRight, otherHitbox.BottomLeft) :
							(otherHitbox.TopRight, otherHitbox.TopLeft);



				(verticalLineP1, verticalLineP2) = (X > otherHitbox.X) ?
											(otherHitbox.TopRight, otherHitbox.BottomRight) :
											(otherHitbox.TopLeft, otherHitbox.BottomLeft);


				//compute the shortest distances to the edges now
				float vertDistance = linePointShortest(verticalLineP1, verticalLineP2, circleCenter).Length();
				float horDistance = linePointShortest(horizontalLineP1, horizontalLineP2, circleCenter).Length();


				//check if one of the distance is smaller than the radius of the circle
				return (vertDistance < Radius || horDistance < Radius);
			}
			else
			{
				throw new NotImplementedException();
			}
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

			float overlapLength = Math.Abs(centerToCenter.Length() - (Radius + other.Radius));

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

			//CASE 1 : the center of the circle is inside of the rectangle 
			if (otherHitbox.Contains(circleCenter))
			{
				//TODO : get a better algorithm for this 
				return ((Vector2)otherHitbox.Center - circleCenter) / 10;
			}



			//CASE 2 : the center of the circle is NOT in the rectangle 

			//determine closest horizontal and vertical edges of the rectangle
			//edges are represented in rectangles so 
			var (verticalLineP1, verticalLineP2) = (X > otherHitbox.X) ?
										(otherHitbox.TopRight, otherHitbox.BottomRight) :
										(otherHitbox.TopLeft, otherHitbox.BottomLeft);

			var (horizontalLineP1, horizontalLineP2) = (circleCenter.Y > otherHitbox.Y) ?
										(otherHitbox.BottomRight, otherHitbox.BottomLeft) :
										(otherHitbox.TopRight, otherHitbox.TopLeft);

			//compute the shortest paths to the edges now
			float vertShortest = linePointShortest(verticalLineP1, verticalLineP2, circleCenter).Length();
			float horShortest = linePointShortest(horizontalLineP1, horizontalLineP2, circleCenter).Length();

			float finalDisp = Radius - Math.Min(vertShortest, horShortest);
			Vector2 centerToCenter = circleCenter - (Vector2)otherHitbox.Center;




			//since the vector goes from rec edge to circle, we return the opposite
			return finalDisp * centerToCenter.NormalizedCopy();
		}






	}

}

