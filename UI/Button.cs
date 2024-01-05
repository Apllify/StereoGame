using Microsoft.Xna.Framework.Graphics;
using StereoGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace StereoGame.UI
{
	public class Button:
		SpritedEntity
	{
		public bool IsPressed { get; private set; } = false;

		public event EventHandler<Action> PressedEvent;
		public event EventHandler<Action> ReleasedEvent;

		public RectangleF Hitbox { get; set; }

		public Button(Vector2 position, RectangleF hitbox, Texture2D texture, SpriteAnchor spriteAnchor):
			base(position, texture, spriteAnchor)
		{
			Hitbox = hitbox;
		}

		public Button(Vector2 position, RectangleF hitbox, Texture2D texture):
			this(position, hitbox, texture, SpriteAnchor.Center)
		{	}

		protected override void CustomUpdate(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.CustomUpdate(gameTime);

			//click detection logic
			Vector2 mousePos = InputHandler.CurrentHandler.GetMousePos();
		}
	}
}
