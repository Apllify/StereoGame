using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace StereoGame
{
	public class SpritedEntity : Entity
	{
		//class members
		public static Texture2D WhiteRectangle;

		public const float ActiveDepth = 0.5f;
		public const float BackgroundDepth = 1f;
		public const float ForegroundDepth = 0.2f;

		public const float DepthStep = 0.001f; 


		//instance members
		private Vector2 position;

		protected Texture2D sprite;

		public enum SpriteAnchor
		{
			Center,
			TopLeft,
			TopRight
		}
		protected SpriteAnchor spriteAnchor; 

		protected Color colorMask;
		protected float scale = 1;


		public float LayerDepth { get; protected set; }



		public SpritedEntity(Vector2 startingCoords, Texture2D _sprite, SpriteAnchor _spriteAnchor, float _layerDepth)
		{
			//properties/fields that have argument values
			position = startingCoords;

			sprite = _sprite;
			spriteAnchor = _spriteAnchor;

			LayerDepth = _layerDepth;

			//default values
			colorMask = Color.White;
			scale = 1;

			//default tag for all sprited entities
			Tag = "untagged-sprited-entity";
		}

		public SpritedEntity(Vector2 startingCoords, Texture2D _sprite, SpriteAnchor _spriteAnchor):
			this(startingCoords, _sprite, _spriteAnchor, CollisionEntity.ActiveDepth)
		{ }

		public SpritedEntity(Vector2 startingCoords, Texture2D _sprite) :
			this(startingCoords, _sprite, SpriteAnchor.Center, CollisionEntity.ActiveDepth)
		{ }

		public SpritedEntity(Vector2 startingCoords):
			this(startingCoords, null)
		{

		}

		/// <summary>
		/// Takes a position + dimensions of a sprite anchor and 
		/// return the top left of the anchored object
		/// </summary>
		/// <param name="position"></param>
		/// <param name="dimensions"></param>
		/// <param name="spriteAnchor"></param>
		public static Vector2 GetTopLeftPosFromAnchor(Vector2 position, Vector2 dimensions, SpriteAnchor spriteAnchor)
		{
			Vector2 topLeft;

			if (spriteAnchor == SpriteAnchor.Center)
			{
				topLeft = position - (dimensions/2);
			}
			else if (spriteAnchor == SpriteAnchor.TopRight)
			{
				topLeft = position - new Vector2(dimensions.X, 0);
			}
			else if (spriteAnchor == SpriteAnchor.TopLeft)
			{
				topLeft = position;
			}
			else
			{
				throw new NotImplementedException();
			}

			return topLeft;
		}

		/// <summary>
		/// Have to implement manually otherwise modifying it gets annoying
		/// </summary>
		/// <returns>The current position of the game object.</returns>
		public Vector2 GetPosition()
		{
			return position;
		}


		/// <summary>
		/// Converts an int in (-inf, +inf) to a depth accepted by sprite batch draw.
		/// The higher the index, the more to the front it is.
		/// </summary>
		/// <param name="layerIndex">
		/// The draw priority as an int (0 = active layer) (1 = slightly in front) etc...
		/// </param>
		/// <returns></returns>
		public static float DepthLayer(int layerIndex)
		{
			return ActiveDepth - (layerIndex * DepthStep);
		}

		public virtual void ShiftPosition(float shiftX, float shiftY)
		{
			position.X += shiftX;
			position.Y += shiftY;
		}

		public virtual void ShiftPosition(Vector2 shiftVector)
		{
			ShiftPosition(shiftVector.X, shiftVector.Y);
		}
		

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (sprite != null)
			{
				SpritedEntity.SpriteDraw(spriteBatch, position, sprite, spriteAnchor, LayerDepth, colorMask, scale);
			}

		}



		public static void RectangleDraw(SpriteBatch spriteBatch, RectangleF location, Color color, float layerDepth)
		{
			//make sure the singular rectangle texture has already been created
			if (SpritedEntity.WhiteRectangle == null)
			{
				SpritedEntity.WhiteRectangle = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
				SpritedEntity.WhiteRectangle.SetData(new[] { Color.White });
			}

			spriteBatch.Draw(SpritedEntity.WhiteRectangle, location.TopLeft, null,
				color, 0f, Vector2.Zero, location.Size,
				SpriteEffects.None, layerDepth);
		}

		public static void RectangleDraw(SpriteBatch spriteBatch, RectangleF location, Color color)
		{
			RectangleDraw(spriteBatch, location, color, SpritedEntity.ActiveDepth);
		}

		public static void HRectangleDraw(SpriteBatch spriteBatch, RectangleF location, int thickness, Color color, float layerDepth)
		{
			//we use the rectangle draw function to make lines 
			RectangleF hor1 = new RectangleF(location.TopLeft, new Size2(location.Width, thickness));
			RectangleF hor2 = new RectangleF(location.Left, location.Bottom - thickness, location.Width, thickness);

			RectangleF ver1 = new RectangleF(location.TopLeft, new Size2(thickness, location.Height));
			RectangleF ver2 = new RectangleF(location.Right - thickness, location.Top, thickness, location.Height);

			RectangleDraw(spriteBatch, hor1, color, layerDepth);
			RectangleDraw(spriteBatch, hor2, color, layerDepth);
			RectangleDraw(spriteBatch, ver1, color, layerDepth);
			RectangleDraw(spriteBatch, ver2, color, layerDepth);

		}

		public static void HRectangleDraw(SpriteBatch spriteBatch, RectangleF location, int thickness, Color color)
		{
			HRectangleDraw(spriteBatch, location, thickness, color, SpritedEntity.ActiveDepth);
		}

		public static void LineDraw(SpriteBatch spriteBatch, Vector2 p1, Vector2 p2, int thickness, Color color, float layerDepth)
		{
			//make sure the singular rectangle texture has already been created
			if (SpritedEntity.WhiteRectangle == null)
			{
				SpritedEntity.WhiteRectangle = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
				SpritedEntity.WhiteRectangle.SetData(new[] { Color.White });
			}

			RectangleF flatLine = new RectangleF(p1.X, p1.Y - thickness/2f, p2.X - p1.X, thickness);
			float rotation = (p2-p1).ToAngle() - (float)Math.PI/2;

			//spriteBatch.Draw(WhiteRectangle, flatLine.TopLeft, null, color, (float)Math.PI/4, Vector2.Zero,
			//				flatLine.Size, SpriteEffects.None, layerDepth);


			spriteBatch.Draw(WhiteRectangle, flatLine.TopLeft, null, color, rotation, Vector2.Zero,
							flatLine.Size, SpriteEffects.None, layerDepth);



		}

		public static void LineDraw(SpriteBatch spriteBatch, Vector2 p1, Vector2 p2, int thickness, Color color)
		{
			LineDraw(spriteBatch, p1, p2, thickness, color, ActiveDepth);
		}


		public static void CircleDraw(SpriteBatch spriteBatch, Vector2 center, float radius, int thickness, Color color)
		{
			//make sure the singular rectangle texture has already been created
			if (SpritedEntity.WhiteRectangle == null)
			{
				SpritedEntity.WhiteRectangle = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
				SpritedEntity.WhiteRectangle.SetData(new[] { Color.White });
			}


			//go all around the center drawing the circle pixel by pixel
			int angleCount = 75;
			Vector2 scale = new Vector2(thickness, thickness);

			for (int angle = 0; angle < angleCount; angle++)
			{
				float curRot = (float) (((float)angle / angleCount) * Math.PI * 2);
				float curX = (float)Math.Cos(curRot) * radius;
				float curY = (float)Math.Sin(curRot) * radius;

				Vector2 offset = new Vector2(curX, curY);

				spriteBatch.Draw(SpritedEntity.WhiteRectangle, center + offset - (scale / 2), null, color, 
								 0, Vector2.Zero, scale, SpriteEffects.None, ForegroundDepth );
			}
		}

		public static void SpriteDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Texture2D sprite, SpriteAnchor spriteAnchor, float layerDepth, Color colorMask, float scale)
		{
			//TODO : take scale into account when drawing this 

			//determine how to draw the sprite based on the anchor
			Vector2 topLeftPos = SpritedEntity.GetTopLeftPosFromAnchor(drawPosition, new Vector2(sprite.Width, sprite.Height), spriteAnchor);

			//make sure to manually round the coordinates (just in case)
			topLeftPos.X = (int)Math.Round(topLeftPos.X);
			topLeftPos.Y = (int)Math.Round(topLeftPos.Y);

			spriteBatch.Draw(sprite, topLeftPos, null, colorMask, 0, Vector2.Zero, 1, SpriteEffects.None, layerDepth);
		}

		public static void SpriteDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Texture2D sprite, SpriteAnchor spriteAnchor, float layerDepth)
		{
			SpriteDraw(spriteBatch, drawPosition, sprite, spriteAnchor, layerDepth, Color.White, 1);
		}


	}
}
