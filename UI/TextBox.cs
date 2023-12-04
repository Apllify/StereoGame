using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StereoGame.Entities;

namespace StereoGame.UI
{
	/// <summary>
	/// A text box class which supports standard text operations
	/// like centering, or coloring.
	/// </summary>
	public class TextBox : SpritedEntity
	{
		public SpriteFont Font { get; set; }
		public string TextContent { get; set; }

		/// <summary>
		/// Initialize a textbox with the given values
		/// </summary>
		public TextBox(Vector2 topleftPosition, string textContent, SpriteFont textFont) :
			base(topleftPosition, null, SpriteAnchor.TopLeft)
		{
			TextContent = textContent;
			Font = textFont;
		}


		/// <summary>
		/// Initialize a textbox at (0, 0) with the given values
		/// </summary>

		public TextBox(string textContent, SpriteFont textFont):
			this(new Vector2(0, 0), textContent, textFont)
		{

		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawString(Font, TextContent, Position, Color.White);
		}
	}
}
