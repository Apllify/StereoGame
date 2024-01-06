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

namespace StereoGame.UI
{
	public class Button:
		SpritedEntity
	{
		public bool IsPressed { get; private set; } = false;

		public event Action PressedEvent;
		public event Action ReleasedEvent;

		public RectangleF Hitbox { get; set; }

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
				PressedEvent?.Invoke();
				IsPressed = true;
			}
			else if (IsPressed && InputHandler.CurrentHandler.IsMouseUp())
			{
				ReleasedEvent?.Invoke();
				IsPressed = false;
			}
			


		}
	}
}
