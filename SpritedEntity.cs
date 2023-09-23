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
			Vector2 origin;
			float spriteWidth = sprite.Width;


			if (spriteAnchor == SpriteAnchor.Center)
			{
				origin = new Vector2(sprite.Width/2, sprite.Height/2);
			}
			else if (spriteAnchor == SpriteAnchor.TopRight)
			{
				origin = new Vector2(sprite.Width, 0);
			}
			else
			{
				origin = new Vector2(0, 0);
			}


			spriteBatch.Draw(sprite, drawPosition, null, colorMask, 0, origin, 1, SpriteEffects.None, layerDepth);
		}

		public static void SpriteDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Texture2D sprite, SpriteAnchor spriteAnchor, float layerDepth)
		{
			SpriteDraw(spriteBatch, drawPosition, sprite, spriteAnchor, layerDepth, Color.White, 1);
		}


	}
}
