using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StereoGame.Toolbox
{
	public class CustomCursor : SpritedEntity
	{
		public Texture2D Sprite { get; set; }

		public CustomCursor(Texture2D sprite):
			base(new(), sprite, SpriteAnchor.Center, DepthLayer(450))
		{
		}


		protected override void CustomUpdate(GameTime gameTime)
		{
			Position = InputHandler.CurrentHandler.GetMousePos();
		}

	}
}
