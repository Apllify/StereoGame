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

		public float Width { get; }
		public float Height { get; }

		//the true coordinates of the top left point no MATTER the rectangle anchor
		private Vector2 topLeftPosition;


		public RectangleEntity(float x, float y, float _width, float _height, SpriteAnchor rectangleAnchor):
			base(new Vector2(x, y), null, rectangleAnchor){

			//set the dimensions
			Width = _width;
			Height = _height;

			//set the coordinates of the top left point
			topLeftPosition = SpritedEntity.GetTopLeftPosFromAnchor(new Vector2(x, y), new Vector2(Width, Height), rectangleAnchor);
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
			return new RectangleEntity(topLeftPosition.X + shiftX, topLeftPosition.Y + shiftY, Width, Height, SpriteAnchor.TopLeft);
		}

		public Vector2 GetTopLeftPosition()
		{
			return topLeftPosition;
		}

		public Vector2 GetDimensions()
		{
			return new Vector2(Width, Height);
		}

		public Vector2 GetCenterPosition()
		{
			return topLeftPosition + new Vector2(Width / 2, Height/ 2);
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
			float otherWidth = otherRec.Width;
			float otherHeight = otherRec.Height;

			return (topLeftPosition.X < (otherTopLeft.X + otherWidth)) &&
					((topLeftPosition.X + Width) > otherTopLeft.X) &&
					(topLeftPosition.Y < (otherTopLeft.Y + otherHeight)) &&
					((topLeftPosition.Y + Height) > otherTopLeft.Y);

		}

		//AABB collision check BUT i use the on-screen discrete coordinates 
		public bool VisuallyIntersects(RectangleEntity otherRec)
		{
			return ToRectangle().Intersects(otherRec.ToRectangle());
		}

		public Rectangle ToRectangle()
		{
			return new Rectangle((int)Math.Round(topLeftPosition.X), (int)Math.Round(topLeftPosition.Y), 
								 (int)Math.Round(Width), (int)Math.Round(Height));
		}


		public Rectangle VisualIntersectionRectangle(RectangleEntity other)
		{
			return Rectangle.Intersect(ToRectangle(), other.ToRectangle());
		}


	}
}
