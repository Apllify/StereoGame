using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace StereoGame.Hitbox
{
	public class ConvexPHitbox: IHitbox
	{

		public List<Vector2> Edges { get; private set; }
		public List<Vector2> Normals { get; private set; }


		public Vector2 CenterOfMass { get; private set; }


		private RectangleF boundingBox;
		private Vector2 lastMove;

		public ConvexPHitbox(List<Vector2> edges) 
		{
			Edges = new();
			Normals = new();


			CenterOfMass = new Vector2(0, 0);
			boundingBox = new RectangleF(0, 0, 0, 0);
			lastMove = new Vector2(0, 0);

			//incomplete shape
			if (edges.Count <= 2)
			{
				return;
			}


			//keep track of bounding coordinates on first pass
			float minX, maxX, minY, maxY;
			minX = maxX = edges[0].X;
			minY = maxY = edges[0].Y;

			//1st PASS : we compute center of mass + bounding box
			for (int i = 0; i < edges.Count; i++)
			{
				Vector2 curEdge = edges[i];

				//check min-max coords
				if (curEdge.X < minX)
					minX = curEdge.X;
				else if (curEdge.X > maxX)
					maxX = curEdge.X;

				if (curEdge.Y < minY)
					minY = curEdge.Y;
				else if (curEdge.Y > maxY)
					maxY = curEdge.Y;
				

				//add the corresponding normal
				Normals.Add(edges[i].NormalizedCopy().PerpendicularClockwise());

				//compute center of mass
				CenterOfMass += edges[i];

				//compute bounding box 
				boundingBox = new RectangleF(minX, minY, maxX - minX, maxY - minY);
			}
			CenterOfMass /= edges.Count;



			//2nd PASS : we make sure that the shape is convex using center of mass
			for (int i = 0; i<edges.Count; i++)
			{
				Vector2 toCenter = CenterOfMass - edges[i];
				Vector2 e1 = edges[(i-1)%edges.Count] - edges[i];
				Vector2 e2 = edges[i + 1] - edges[i];

				float a1 = (float) Math.Acos( toCenter.Dot(e1) / (toCenter.Length() * e1.Length()));
				float a2 = (float) Math.Acos( toCenter.Dot(e2) / (toCenter.Length() * e2.Length()));

				//IF we're not convex, stop control flow (probably foolproof this eventually with out variable in construct)
				if (a1 + a2 > Math.PI)
				{
					throw new ArgumentException("Input shape is not convex !");
				}


			}


			//save those edges
			Edges = edges;

		}	

		public int GetTypeId()
		{
			return 2;
		}

		public void Shift(float shiftX, float shiftY)
		{
			Vector2 offset = new(shiftX, shiftY);

			for (int i = 0; i <  Edges.Count; i++)
			{
				Edges[i] += offset;
			}

			//also shift center of mass + bounding box
			CenterOfMass += offset;
			boundingBox.Offset(shiftX, shiftY);

			lastMove = offset;
		}

		public IHitbox Shifted(float shiftX, float shiftY)
		{
			ConvexPHitbox result = new ConvexPHitbox(Edges);
			result.Shift(shiftX, shiftY);

			return result;
		}


		public Vector2 GetLastMove()
		{
			return lastMove;
		}


		public Vector2 SolveCollision(IHitbox other)
		{
			throw new NotImplementedException();
		}

		public bool Intersects(IHitbox other)
		{
			return false;
		}

		public RectangleF GetBoundingBox()
		{
			return boundingBox;
		}
	}
}
