using Microsoft.Xna.Framework.Graphics;
using StereoGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace StereoGame.UI
{
	public class Button:
		SpritedEntity
	{
		public bool IsPressed { get; private set; } = false;
		public RectangleF Hitbox { get; set; }

		public Color FlickerColor { get; set; } = Color.LightGray;

		public event Action PressedEvent;
		public event Action ReleasedEvent;


		public Button(Vector2 position, Vector2 size, Texture2D texture, SpriteAnchor spriteAnchor):
			base(position, texture, spriteAnchor)
		{
			Vector2 tlPos = GetTopLeftPosFromAnchor(Position, size, spriteAnchor);
			Hitbox = new RectangleF(tlPos, size);
		}

		public Button(Vector2 position, Vector2 size, Texture2D texture):
			this(position, size, texture, SpriteAnchor.Center)
		{	}

		public override void ShiftPosition(float shiftX, float shiftY)
		{
			base.ShiftPosition(shiftX, shiftY);
			Hitbox = new RectangleF(Hitbox.X + shiftX, Hitbox.Y + shiftY, Hitbox.Width, Hitbox.Height);
		}

		protected override void CustomUpdate(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.CustomUpdate(gameTime);

			//click detection logic
			Vector2 mousePos = InputHandler.CurrentHandler.GetMousePos();

			if (InputHandler.CurrentHandler.IsMouseJustDown() 
				&& Hitbox.Contains(mousePos))
			{
				//call press event
				PressedEvent?.Invoke();
				IsPressed = true;

			}
			else if (IsPressed && InputHandler.CurrentHandler.IsMouseUp())
			{
				//call release event
				ReleasedEvent?.Invoke();
				IsPressed = false;
			}

			if (IsPressed)
			{
				Flicker(FlickerColor, 0.5f);
			}
		}
	}
}
