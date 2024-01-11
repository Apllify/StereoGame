using StereoGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Diagnostics;

namespace StereoGame.UI
{
	public class Slider : SpritedEntity
	{

		//properties
		public Vector2 Origin { get; set; }

		/// <summary>
		/// Always between 0 and 1 (inclusive)
		/// </summary>
		public float Progress { get; set; } = 0;

		public RectangleF Hitbox { get; set; }

		public float SlideLength { get; set; } = 0;

		public bool IsPressed { get; private set; } = false;

		public Color PressedColor { get; set; } = Color.Gray;

		public Color SlideColor { get; set; } = Color.DarkGray;

		//fields
		private Vector2 lastMouseMov = Vector2.Zero;

		public Slider(Vector2 position, Vector2 sliderSize, float slideLength, Texture2D texture, SpriteAnchor spriteAnchor):
			base(position, texture, spriteAnchor, ForegroundDepth)
		{
			Origin = position; 

			Vector2 tlPos = GetTopLeftPosFromAnchor(Position, sliderSize, spriteAnchor);
			Hitbox = new RectangleF(tlPos, sliderSize);

			SlideLength = slideLength;
		}

		public Slider(Vector2 position, Vector2 sliderSize, float slideLength, Texture2D texture) :
			this(position, sliderSize, slideLength, texture, SpriteAnchor.Center)
		{ }

		public override void ShiftPosition(float shiftX, float shiftY)
		{
			base.ShiftPosition(shiftX, shiftY);
			Hitbox = new RectangleF(Hitbox.X + shiftX, Hitbox.Y + shiftY, 
									Hitbox.Width, Hitbox.Height);
		}

		protected override void CustomUpdate(GameTime gameTime)
		{
			//check whether we're being held by the mouse
			Vector2 cursorPos = InputHandler.CurrentHandler.GetMousePos();
			bool cursorPressed = InputHandler.CurrentHandler.IsMouseDown();

			if (Hitbox.Contains(cursorPos) && cursorPressed)
			{
				IsPressed = true;
			}
			else if (!cursorPressed)
			{
				IsPressed = false;
			}

			//follow mouse or not depending on whether we're being held 
			if (IsPressed)
			{
				Flicker(PressedColor, 0.2f);

				//compute new progress percent
				float progressInc = lastMouseMov.X / SlideLength;
				float newProgress = Math.Clamp(Progress + progressInc, 0f, 1f);
				Progress = newProgress;

				//change x position
				float newX = Origin.X + Progress * SlideLength;
				SetPosition(new Vector2(newX, Origin.Y));

				//actualize mouse tracking
				lastMouseMov = InputHandler.CurrentHandler.GetMouseMov();
			}
			else
			{
				//ignore mouse when we're not selected
				if (lastMouseMov != Vector2.Zero)
				{
					lastMouseMov = new(0, 0);
				}
			}

		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			//draw axis of slide
			Vector2 sliderEnd = Origin + new Vector2(SlideLength, 0);
			LineDraw(spriteBatch, Origin, sliderEnd, 4, SlideColor);

			//draw slider itself
			base.Draw(spriteBatch);
		}

	}
}
