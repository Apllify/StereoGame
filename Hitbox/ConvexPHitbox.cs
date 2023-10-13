using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace StereoGame.Hitbox
{
	public class ConvexPHitbox: IHitbox
	{

		public List<Vector2> Vertices { get; private set; }
		public HashSet<Vector2> Normals { get; private set; }


		public Vector2 CenterOfMass { get; private set; }


		private RectangleF boundingBox;
		private Vector2 lastMove;

		public ConvexPHitbox(List<Vector2> vertices) 
		{
			Vertices = new();
			Normals = new();


			CenterOfMass = new Vector2(0, 0);
			boundingBox = new RectangleF(0, 0, 0, 0);
			lastMove = new Vector2(0, 0);

			//incomplete shape
			if (vertices.Count <= 2)
			{
				return;
			}


			//keep track of bounding coordinates on first pass
			float minX, maxX, minY, maxY;
			minX = maxX = vertices[0].X;
			minY = maxY = vertices[0].Y;

			//1st PASS : we compute center of mass + bounding box
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 curVertex = vertices[i];

				//check min-max coords
				if (curVertex.X < minX)
					minX = curVertex.X;
				else if (curVertex.X > maxX)
					maxX = curVertex.X;

				if (curVertex.Y < minY)
					minY = curVertex.Y;
				else if (curVertex.Y > maxY)
					maxY = curVertex.Y;
				

				//compute center of mass
				CenterOfMass += vertices[i];

				//compute bounding box 
				boundingBox = new RectangleF(minX, minY, maxX - minX, maxY - minY);
			}
			CenterOfMass /= vertices.Count;


			Vector2 p1 = vertices[0] - vertices[^1];
			Vector2 p2 = vertices[1] - vertices[0];
			float expectedCross = Math.Sign( p1.X * p2.Y - p1.Y * p2.X);


			//2nd PASS : we check convexity and compute normal vectors
			for (int i = 0; i<vertices.Count; i++)
			{
				p1 = vertices[i] - vertices[(i-1)>=0 ? (i-1) : ^1];
				p2 = vertices[(i + 1) % vertices.Count] - vertices[i];

				float cross = Math.Sign(p1.X * p2.Y - p1.Y * p2.X);

				//IF we're not convex, stop control flow (probably foolproof this eventually with out variable in construct)
				if (cross != expectedCross)
				{
					throw new ArgumentException("Input shape is not convex !");
				}

				//make sure every normal has a positive x (for easy duplicate checks)
				Vector2 normal = (p2 - p1).NormalizedCopy().PerpendicularClockwise();
				if (normal.X < 0)
				{
					normal *= -1;
				}
				else if (normal.X == 0 && normal.Y < 0)
                {
					normal *= -1;
                }


                Normals.Add(normal);


			}


			//save those vertices
			Vertices = vertices;

		}	

		public int GetTypeId()
		{
			return 2;
		}

		public void Shift(float shiftX, float shiftY)
		{
			Vector2 offset = new(shiftX, shiftY);

			for (int i = 0; i <  Vertices.Count; i++)
			{
				Vertices[i] += offset;
			}

			//also shift center of mass + bounding box
			CenterOfMass += offset;
			boundingBox.Offset(shiftX, shiftY);

			lastMove = offset;
		}

		public IHitbox Shifted(float shiftX, float shiftY)
		{
			ConvexPHitbox result = new ConvexPHitbox(Vertices);
			result.Shift(shiftX, shiftY);

			return result;
		}

		public RectangleF GetBoundingBox()
		{
			return boundingBox;
		}


		public Vector2 GetLastMove()
		{
			return lastMove;
		}



		/// <summary>
		/// Uses the SAT collision resolution algorithm to compute 
		/// the shortest penetration vector, and returns it.
		/// </summary>
		/// <param name="ps1"></param>
		/// <param name="ps2"></param>
		/// <param name="normals"></param>
		/// <returns>The vector such that applying it to e1 gets it out of collision.</returns>
		public static Vector2 ComputeSAT(List<Vector2> ps1, List<Vector2> ps2, List<Vector2> normals)
		{
			Vector2 smallestPenetrationVector = Vector2.Zero;

			//this is where the	 magic happens
			foreach (Vector2 normal in normals)
			{
				Segment2 proj1 = ShapeProjection(ps1, normal);
				Segment2 proj2 = ShapeProjection(ps2, normal);

				float start1Coord = ((Vector2) proj1.Start).Dot(normal);
				float start2Coord = ((Vector2) proj2.Start).Dot(normal);
				float maxStartCoord = Math.Max(start1Coord, start2Coord);


				float end1Coord = ((Vector2)proj1.End).Dot(normal);
				float end2Coord = ((Vector2)proj2.End).Dot(normal);
				float minEndCoord = Math.Min(end1Coord, end2Coord);


				if (minEndCoord <= maxStartCoord)
				{
					return Vector2.Zero;
				}

				Vector2 curPenetrationVector = (minEndCoord * normal - maxStartCoord * normal);

				//the collision vector on this axis is the intersection of the two projections
				if ((smallestPenetrationVector == Vector2.Zero) || curPenetrationVector.Length() < smallestPenetrationVector.Length())
				{
					//apply the proper sign to it depending on the relative positions of both 
					//(assumption : BOTH shapes are convex)
					float dirSign = (maxStartCoord == start1Coord) ? 1 : -1;
					smallestPenetrationVector = curPenetrationVector * dirSign;

				}
			}


			return smallestPenetrationVector;
		}


		/// <summary>
		/// Projects an entire set of points onto a vector, by computing the union
		/// of all the individual projections.
		/// </summary>
		/// <param name="shape"></param>
		/// <param name="projAxis"></param>
		/// <returns></returns>
		public static Segment2 ShapeProjection(List<Vector2> shape, Vector2 projAxis)
		{

			Segment2 result = new Segment2(shape[0].ProjectOnto(projAxis), shape[0].ProjectOnto(projAxis)) ;
			float curMin = shape[0].Dot(projAxis);
			float curMax = shape[0].Dot(projAxis);


			for (int i = 1; i < shape.Count; i++)
			{
				float dotProduct = shape[i].Dot(projAxis);
				
				if (dotProduct < curMin)
				{
					curMin = dotProduct;
					result.Start = shape[i].ProjectOnto(projAxis);
				}
				else if (dotProduct > curMax)
				{
					curMax = dotProduct;
					result.End = shape[i].ProjectOnto(projAxis);
				}				
			}


			return result;
		}


		public Vector2 SolveCollision(IHitbox other)
		{
			if (other is RectangleHitbox)
			{
				//regular sat call
				List<Vector2> otherVertices = new List<Vector2>() {
												(Vector2)other.GetBoundingBox().TopLeft,
												(Vector2)other.GetBoundingBox().TopRight,
												(Vector2)other.GetBoundingBox().BottomRight,
												(Vector2)other.GetBoundingBox().BottomLeft
												};


				return ComputeSAT(Vertices, otherVertices,
								  Normals.Union(RectangleHitbox.RectangleNormals).ToList());
			}
			else if (other is CircleHitbox)
			{
				return Vector2.Zero;
			}
			else if (other is ConvexPHitbox)
			{
				//just call SAT
				var otherCPoly = other as ConvexPHitbox;

				return ComputeSAT(Vertices, otherCPoly.Vertices,
								  Normals.Union(otherCPoly.Normals).ToList());
			}
			else
			{
				throw new NotImplementedException();
			}
		}


	}
}
