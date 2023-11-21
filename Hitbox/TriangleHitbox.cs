using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace StereoGame.Hitbox
{
	/// <summary>
	/// Represents a 2D triangle collider (3 vertices total)
	/// </summary>
	public class TriangleHitbox : IHitbox
	{

		//instance members
		private Vector2 p1;
		private Vector2 p2;
		private Vector2 p3;

		public Vector2 P1 { get => p1; }
		public Vector2 P2 { get => p2; }
		public Vector2 P3 { get => p3; }


		private Vector2 lastMove;

		public TriangleHitbox(Vector2 _p1, Vector2 _p2, Vector2 _p3)
		{
			p1 = _p1;
			p2 = _p2;
			p3 = _p3;
		}

		public TriangleHitbox(Vector2[] vertices)
		{
			if (vertices.Length> 3)
			{
				p1 = vertices[0];
				p2 = vertices[1];
				p3 = vertices[2];
			}
			else
			{
				throw new ArgumentException("Vertices array must contain at least 3 vertices");
			}
		}

		public TriangleHitbox(List<Vector2> vertices)
		{
			if (vertices.Count > 3)
			{
				p1 = vertices[0];
				p2 = vertices[1];
				p3 = vertices[2];
			}
			else
			{
				throw new ArgumentException("Vertices list must contain at least 3 vertices");
			}
		}

		public int GetTypeId()
			=> 3;
		public Vector2 GetLastMove()
		{
			//TODO : implement this 
			return Vector2.Zero;
		}

		public void Shift(float x, float y)
		{
			lastMove = new(x, y);

			//avoid duplicating structs
			Vector2.Add(ref p1, ref lastMove, out p1);
			Vector2.Add(ref p2, ref lastMove, out p2);
			Vector2.Add(ref p3, ref lastMove, out p3);
		}

		public IHitbox Shifted(float x, float y)
		{
			return new TriangleHitbox(p1 + new Vector2(x, y), 
									  p2 + new Vector2(x, y), 
									  p3 + new Vector2(x, y));
		}

		public RectangleF GetBoundingBox()
		{
			float minX, minY;
			float maxX, maxY;

			minX = Math.Min(Math.Min(p1.X, p2.X), p3.X);
			minY = Math.Min(Math.Min(p1.Y, p2.Y), p3.Y);

			maxX = Math.Max(Math.Max(p1.X, p2.X), p3.X);
			maxY = Math.Max(Math.Max(p1.Y, p2.Y), p3.Y);

			return new RectangleF(minX, minY, maxX - minX, maxY - minY);
		}

		public Vector2 SolveCollision(IHitbox other)
		{
			throw new NotImplementedException();
		}
	}
}
