using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Triangulation;
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
		public StringBuilder TextContent { get; set; }

		/// <summary>
		/// Initialize a textbox with the given values
		/// </summary>
		public TextBox(Vector2 topleftPosition, string textContent, SpriteFont textFont) :
			base(topleftPosition, null, SpriteAnchor.TopLeft)
		{
			TextContent = new StringBuilder(textContent);
			Font = textFont;
		}

		/// <summary>
		/// Initialize a textbox at (0, 0) with the given values
		/// </summary>

		public TextBox(string textContent, SpriteFont textFont):
			this(new Vector2(0, 0), textContent, textFont)
		{

		}


		/// <summary>
		/// Wraps the text of the textbox to fit 
		/// within a given width.
		/// </summary>
		public void WrapText(float lineLength)
		{
			//isolate the newline chars 
			TextContent.Replace("\n", " \n ");
			string[] words = TextContent.ToString().Split(' ');

			float currentLength = 0;
			int builderIndex = -1;

			foreach (string word in words)
			{
				builderIndex += word.Length + 1;
				float wordLength = Font.MeasureString(word).X;
				
				if (wordLength + currentLength > lineLength)
				{
					TextContent.Insert(builderIndex, '\n');
					currentLength = 0;
				}
				else
				{
					currentLength += wordLength;
				}
			}

			//undo our newline isolation
			TextContent.Replace(" \n ", "\n");
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawString(Font, TextContent, Position, Color.White);
		}
	}
}
