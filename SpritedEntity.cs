using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace StereoGame
{
	public class SpritedEntity : Entity
	{

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
		public static Vector2 GetTopLeftPos(Vector2 position, Vector2 dimensions, SpriteAnchor spriteAnchor)
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
				topLeft = new Vector2(0, 0);
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



		public static void SpriteDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Texture2D sprite, SpriteAnchor spriteAnchor, float layerDepth, Color colorMask, float scale)
		{
			//TODO : take scale into account when drawing this 

			//determine how to draw the sprite based on the anchor
			Vector2 topLeftPos = SpritedEntity.GetTopLeftPos(drawPosition, new Vector2(sprite.Width, sprite.Height), spriteAnchor);


			spriteBatch.Draw(sprite, topLeftPos, null, colorMask, 0, Vector2.Zero, 1, SpriteEffects.None, layerDepth);
		}

		public static void SpriteDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Texture2D sprite, SpriteAnchor spriteAnchor, float layerDepth)
		{
			SpriteDraw(spriteBatch, drawPosition, sprite, spriteAnchor, layerDepth, Color.White, 1);
		}


	}
}
