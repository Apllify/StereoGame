using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StereoGame
{
	public class RectangleEntity : SpritedEntity
	{

		private float width;
		private float height;

		//the true coordinates of the top left point no MATTER the rectangle anchor
		private Vector2 topLeftPosition;


		public RectangleEntity(float x, float y, float _width, float _height, SpriteAnchor rectangleAnchor):
			base(new Vector2(x, y), null, rectangleAnchor){
			//set the dimensions
			width = _width;
			height = _height;

			//set the coordinates of the top left point
			topLeftPosition = SpritedEntity.GetTopLeftPos(new Vector2(x, y), new Vector2(width, height), rectangleAnchor);
		}

		public RectangleEntity(float x, float y, float _width, float _height):
			this(x, y, _width, _height, SpriteAnchor.Center)
		{

		}

		public override void ShiftPosition(float shiftX, float shiftY)
		{
			base.ShiftPosition(shiftX, shiftY);

			topLeftPosition.X += shiftX;
			topLeftPosition.Y += shiftY;
		}

		public RectangleEntity ShiftPositionClone(float shiftX, float shiftY)
		{
			return new RectangleEntity(GetPosition().X + shiftX, GetPosition().Y + shiftY, width, height, spriteAnchor);
		}

		public Vector2 GetTopLeftPosition()
		{
			return topLeftPosition;
		}

		public float GetWidth()
		{
			return width;
		}

		public float GetHeight()
		{
			return height;
		}

		public Vector2 GetDimensions()
		{
			return new Vector2(width, height);
		}

		public Vector2 GetCenter()
		{
			return topLeftPosition + new Vector2(width / 2, height / 2);
		}

		/// <summary>
		/// CAREFUL if using this, ALL normal collision events should use VisuallyIntersects()
		/// instead of Intersects()
		/// </summary>
		/// <param name="otherRec"></param>
		/// <returns></returns>
		public bool Intersects(RectangleEntity otherRec)
		{
			Vector2 otherTopLeft = otherRec.GetTopLeftPosition();
			float otherWidth = otherRec.GetWidth();
			float otherHeight = otherRec.GetHeight();

			return (topLeftPosition.X < (otherTopLeft.X + otherWidth)) &&
					((topLeftPosition.X + width) > otherTopLeft.X) &&
					(topLeftPosition.Y < (otherTopLeft.Y + otherHeight)) &&
					((topLeftPosition.Y + height) > otherTopLeft.Y);

		}

		//AABB collision check BUT i use the on-screen discrete coordinates 
		public bool VisuallyIntersects(RectangleEntity otherRec)
		{
			Vector2 otherTopLeft = otherRec.GetTopLeftPosition();
			float otherWidth = otherRec.GetWidth();
			float otherHeight = otherRec.GetHeight();

			return (Math.Round(topLeftPosition.X) < Math.Round(otherTopLeft.X + otherWidth)) &&
					(Math.Round(topLeftPosition.X + width) > Math.Round(otherTopLeft.X)) &&
					(Math.Round(topLeftPosition.Y) < Math.Round(otherTopLeft.Y + otherHeight)) &&
					(Math.Round(topLeftPosition.Y + height) > Math.Round(otherTopLeft.Y));
		}

		public Rectangle ToRectangle()
		{
			return new Rectangle((int)topLeftPosition.X, (int)topLeftPosition.Y, (int)width, (int)height);
		}



	}
}
