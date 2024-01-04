using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

namespace StereoGame.UI
{
	/// <summary>
	/// A special Textbox meant to display a float value which
	/// updates over time 
	/// </summary>
	public class FLabel : TextBox
	{
		const String DefaultFormat = "0.00";
		public delegate float ValueGetter();


		//instance members
		private float value;
		private ValueGetter valueGetter;

		private String format;
		public String Format { 
			get => format; 
			set => SetFormat(value);
		}
		

		public FLabel(Vector2 position, SpriteFont font, ValueGetter _valueGetter, String _format):
			base(position, "", font, float.PositiveInfinity)
		{
			Format = _format;

			//request value for first time
			valueGetter = _valueGetter;
			value = valueGetter.Invoke();

			TextContent = value.ToString(Format);
		}

		public FLabel(Vector2 position, SpriteFont font, ValueGetter valueGetter) :
			this(position, font, valueGetter, DefaultFormat)
		{
		}

		public FLabel(Vector2 position, SpriteFont font, float constValue, String format):
			this(position, font, ()=>constValue, format)
		{
		}


		public FLabel(Vector2 position, SpriteFont font, float constValue) :
			this(position, font, () => constValue)
		{ }


		public float GetValue()
		{
			return valueGetter.Invoke();
		}

		public ValueGetter GetValueGetter()
		{
			return valueGetter;
		}

		public void SetValueGetter(ValueGetter newGetter)
		{
			valueGetter = newGetter;
		}

		public void SetFormat(String _format)
		{
			//check input is valid float format
			try
			{
				value.ToString(_format);
				format = _format;
			}
			catch (FormatException e)
			{
				throw new FormatException("FLabel received invalid float to string format");
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			value = valueGetter.Invoke();

			if (value.ToString(DefaultFormat) != TextContent)
			{
				TextContent = value.ToString(DefaultFormat);
			}
			base.Draw(spriteBatch);
		}

	}
}
