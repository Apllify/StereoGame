using StereoGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace StereoGame.UI
{
	public class Slider : SpritedEntity
	{

		//properties
		public float Progress { get; set; } = 0;

		public RectangleF Hitbox { get; set; }

		public float SlideLength { get; set; } = 0;

		public bool IsPressed { get; private set; } = false;



		//fields
		private Vector2 lastMouseMov = Vector2.Zero;

		public Slider(Vector2 position, Vector2 sliderSize, float slideLength, Texture2D texture, SpriteAnchor spriteAnchor):
			base(position, texture, spriteAnchor, ForegroundDepth)
		{
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

			IsPressed = Hitbox.Contains(cursorPos) && cursorPressed;

			//follow mouse or not depending on whether we're being held 


		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			//draw axis of slide
			Vector2 sliderCenter = Hitbox.Center;
			sliderCenter.X -= Progress * SlideLength;


			//draw slider itself
			base.Draw(spriteBatch);


		}

	}
}
