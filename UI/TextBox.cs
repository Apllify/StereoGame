using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
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

		private String textContent;
		public String TextContent {
			get => GetText();
			set => SetText(value);
		}

		public Color TextColor { get; set; } = Color.White;

		public float MaxWidth { get; set; }

		/// <summary>
		/// Initialize a textbox with the given values
		/// </summary>
		public TextBox(Vector2 topleftPosition, string _textContent, SpriteFont textFont, float maxWidth) :
			base(topleftPosition, null, SpriteAnchor.TopLeft)
		{
			LayerDepth = ForegroundDepth;

			Font = textFont;

			MaxWidth = maxWidth;

			//set text content last to allow auto-wrapping
			TextContent = _textContent;

		}

		/// <summary>
		/// Initialize a textbox at (0, 0) with the given values
		/// </summary>

		public TextBox(string textContent, SpriteFont textFont):
			this(new Vector2(0, 0), textContent, textFont, float.PositiveInfinity)
		{

		}
 

		/// <summary>
		/// Returns the text currently in the text box
		/// </summary>
		public String GetText()
		{
			return textContent;
		}

		/// <summary>
		/// Updates the content of textbox and rewraps
		/// </summary>
		public void SetText(String newText)
		{
			textContent = newText;

			if (!float.IsInfinity(MaxWidth))
			{
				WrapText(MaxWidth);
			}
		}

		/// <summary>
		/// Wraps the text of the textbox to fit 
		/// within a given width (strict)
		/// </summary>
		private void WrapText(float lineLength)
		{
			//isolate the newline chars 
			StringBuilder contentBuilder = new StringBuilder(textContent);
			contentBuilder.Replace("\n", " \n ");
			string[] words = textContent.ToString().Split(' ');

			float currentLength = 0;
			int builderIndex = 0;

			foreach (string word in words)
			{
				//get the length of the current word in virtual coords
				float wordLength = InputHandler.CurrentHandler.RtV(Font.MeasureString(word)).X;

				//special case of pre-existing br
				if (word == "\n")
				{
					currentLength = 0;
					continue;
				}
				

				//manually wrap if applicable
				if (wordLength + currentLength > lineLength)
				{
					contentBuilder.Insert(builderIndex, '\n');
					currentLength = 0;

					builderIndex++;
				}

				currentLength += wordLength;
				builderIndex += word.Length + 1;

			}

			//undo our newline isolation + reconvert to string
			contentBuilder.Replace(" \n ", "\n");
			textContent = contentBuilder.ToString();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawString(Font, TextContent, Position, TextColor);
		}
	}
}
