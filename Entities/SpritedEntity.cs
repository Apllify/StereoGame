using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

using StereoGame.Extensions;

namespace StereoGame.Entities
{
    public class SpritedEntity : Entity
    {
        //class members
        public static Texture2D WhiteRectangle { get; private set; }

        public const float ActiveDepth = 0.5f;
        public const float BackgroundDepth = 1f;
        public const float ForegroundDepth = 0.2f;

        public const float DepthStep = 0.001f;


        //instance members
        private Vector2 position;
        public Vector2 Position
        {
            get => position;
            set => ShiftPosition(value - position);
        }

        protected Texture2D sprite;

        public enum SpriteAnchor
        {
            Center,
            TopLeft,
            TopRight
        }
        protected SpriteAnchor spriteAnchor;

        public Color ColorMask { get; private set; }


        protected float scale = 1;

        public float LayerDepth { get; protected set; }



        //helpers for flicker behavior
        private Color flickerOrigin;
        private Color flickerTarget;
        private float flickerProgress = 0;
        private float flickerSpeed = 0;

        public SpritedEntity(Vector2 startingCoords, Texture2D _sprite, SpriteAnchor _spriteAnchor, float _layerDepth)
        {
            //properties/fields that have argument values
            position = startingCoords;

            sprite = _sprite;
            spriteAnchor = _spriteAnchor;

            LayerDepth = _layerDepth;

            //default values
            ColorMask = Color.White;
            flickerOrigin = ColorMask;
            scale = 1;

            //default tag for all sprited entities
            Tag = "untagged-sprited-entity";
        }

        public SpritedEntity(Vector2 startingCoords, Texture2D _sprite, SpriteAnchor _spriteAnchor) :
            this(startingCoords, _sprite, _spriteAnchor, CollisionEntity.ActiveDepth)
        { }

        public SpritedEntity(Vector2 startingCoords, Texture2D _sprite) :
            this(startingCoords, _sprite, SpriteAnchor.Center, CollisionEntity.ActiveDepth)
        { }

        public SpritedEntity(Vector2 startingCoords) :
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

            switch (spriteAnchor)
            {
                case SpriteAnchor.Center:
                    topLeft = position - dimensions / 2;
                    break;

                case SpriteAnchor.TopRight:
                    topLeft = position - new Vector2(dimensions.X, 0);
                    break;

                case SpriteAnchor.TopLeft:
                    topLeft = position;
                    break;

                default:
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
        /// Converts an int in [-500, +500] to a depth accepted by sprite batch draw.
        /// The higher the index, the more to the front it is.
        /// </summary>
        /// <param name="layerIndex">
        /// The draw priority as an int (0 = active layer) (1 = slightly in front) etc...
        /// </param>
        /// <returns></returns>
        public static float DepthLayer(int layerIndex)
        {
            return ActiveDepth - layerIndex * DepthStep;
        }

        /// <summary>
        /// Move this game object safely by the given offset
        /// </summary>
        public virtual void ShiftPosition(float shiftX, float shiftY)
        {
            position.X += shiftX;
            position.Y += shiftY;
        }


		/// <summary>
		/// Move this game object safely by the given offset
		/// </summary>
		public void ShiftPosition(Vector2 shiftVector)
        {
            ShiftPosition(shiftVector.X, shiftVector.Y);
        }


        /// <summary>
        /// Safely change the color of this object (flicker
        /// effects still remain)
        /// </summary>
        public void SetColor(Color newColor)
        {
            flickerOrigin = newColor;
            if (flickerProgress <= 0)
            {
                ColorMask = flickerOrigin;
            }
        }

        /// <summary>
        /// Make the entity flash to the given color 
        /// for a specified duration
        /// </summary>
        public void Flicker(Color flickerColor, float flickerDuration)
        {
            flickerTarget = flickerColor;
            flickerProgress = 1f;
            flickerSpeed = 1/flickerDuration;
        }


        /// <summary>
        /// DO NOT override in any SpritedEntity
        /// subclasses.
        /// </summary>
		protected override void PreUpdate(GameTime gameTime)
		{
			//progress our flicker 
            if (flickerProgress > 0)
            {
                ColorMask = Color.Lerp(flickerOrigin, flickerTarget, flickerProgress);

                flickerProgress -= (float)gameTime.ElapsedGameTime.TotalSeconds*
                                    flickerSpeed;
            }


		}

		public override void Draw(SpriteBatch spriteBatch)
        {
            if (sprite != null)
            {
                SpriteDraw(spriteBatch, position, sprite, spriteAnchor, LayerDepth, ColorMask, scale);
            }

        }



        public static void RectangleDraw(SpriteBatch spriteBatch, RectangleF location, Color color, float layerDepth)
        {
            //make sure the singular rectangle texture has already been created
            if (WhiteRectangle == null)
            {
                WhiteRectangle = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                WhiteRectangle.SetData(new[] { Color.White });
            }

            spriteBatch.Draw(WhiteRectangle, location.TopLeft, null,
                color, 0f, Vector2.Zero, location.Size,
                SpriteEffects.None, layerDepth);
        }
        public static void RectangleDraw(SpriteBatch spriteBatch, RectangleF location, Color color)
        {
            RectangleDraw(spriteBatch, location, color, ActiveDepth);
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
            HRectangleDraw(spriteBatch, location, thickness, color, ActiveDepth);
        }



        public static void LineDraw(SpriteBatch spriteBatch, Vector2 p1, Vector2 p2, int thickness, Color color, float layerDepth)
        {
            //make sure the singular rectangle texture has already been created
            if (WhiteRectangle == null)
            {
                WhiteRectangle = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                WhiteRectangle.SetData(new[] { Color.White });
            }

            //compute width and height of the final segment
            float lineLength = new Vector2(p2.X - p1.X, p2.Y - p1.Y).Length() + thickness;


            RectangleF flatLine = new RectangleF(p1.X, p1.Y - thickness / 2, lineLength, thickness);


            float rotation = (p2 - p1).ToAngle() - MathF.PI / 2;


            //compute the offset to adjust for fact that rotation is centered at bottom left of sprite
            Vector2 centerOffset = new Vector2(thickness / (2 * flatLine.Width), thickness / (2 * flatLine.Height));



            spriteBatch.Draw(WhiteRectangle, flatLine.TopLeft, null, color, rotation, centerOffset,
                            flatLine.Size, SpriteEffects.None, layerDepth);

        }

        public static void LineDraw(SpriteBatch spriteBatch, Vector2 p1, Vector2 p2, int thickness, Color color)
        {
            LineDraw(spriteBatch, p1, p2, thickness, color, ActiveDepth);
        }

        public static void LineDraw(SpriteBatch spriteBatch, Segment2 segment, int thickness, Color color, float layerDepth)
        {
            LineDraw(spriteBatch, segment.Start, segment.End, thickness, color, layerDepth);
        }




        public static void CircleDraw(SpriteBatch spriteBatch, Vector2 center, float radius, int thickness, Color color, float layerDepth)
        {
            //make sure the singular rectangle texture has already been created
            if (WhiteRectangle == null)
            {
                WhiteRectangle = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                WhiteRectangle.SetData(new[] { Color.White });
            }


            //go all around the center drawing the circle pixel by pixel
            int angleCount = 75;
            Vector2 scale = new Vector2(thickness, thickness);

            for (int angle = 0; angle < angleCount; angle++)
            {
                float curRot = (float)((float)angle / angleCount * MathF.PI * 2);
                float curX = MathF.Cos(curRot) * radius;
                float curY = MathF.Sin(curRot) * radius;

                Vector2 offset = new Vector2(curX, curY);

                spriteBatch.Draw(WhiteRectangle, center + offset - scale / 2, null, color,
                                 0, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
            }
        }

        public static void CircleDraw(SpriteBatch spriteBatch, Vector2 center, float radius, int thickness, Color color)
            => CircleDraw(spriteBatch, center, radius, thickness, color, ActiveDepth);

        public static void SpriteDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Texture2D sprite, SpriteAnchor spriteAnchor, float layerDepth, Color colorMask, float scale)
        {
            //TODO : take scale into account when drawing this 

            //determine how to draw the sprite based on the anchor
            Vector2 topLeftPos = GetTopLeftPosFromAnchor(drawPosition, new Vector2(sprite.Width, sprite.Height), spriteAnchor);

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
