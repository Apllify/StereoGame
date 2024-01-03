using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace StereoGame.UI
{
	public class FLabel : TextBox
	{
		const String DefaultFormat = "0.00";

		private float value;
		

		public FLabel(Vector2 position, SpriteFont font, ref float _value) :
			base(position, _value.ToString(DefaultFormat), font, 100)
		{
			value = _value;
		}

		public float GetValue()
		{
			return value;
		}

		public void SetValue(ref float newValue)
		{
			value = newValue;
		}

		protected override void CustomUpdate(GameTime gameTime)
		{
			base.CustomUpdate(gameTime);

			if (value.ToString(DefaultFormat) != TextContent)
			{
				TextContent = value.ToString(DefaultFormat);
			}
		}

	}
}
