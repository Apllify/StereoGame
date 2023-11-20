using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;

namespace StereoGame.Extensions
{

    /// <summary>
    /// WARNING  : this is a mutable struct.
    /// An alternative to Monogame.Color which internally represents the channels
    /// as floats, useful for incrementing colors by small amounts while retaining 
    /// performance.
    /// </summary>
    public struct ColorF
    {

        private float red = 0;
        public float Red { 
            get => red; 
            set => red = Math.Clamp(value, 0, 1); 
        }
        public float R { get => Red; set => Red = value; }

        private float green = 0;
        public float Green
        {
			get => green;
			set => green = Math.Clamp(value, 0, 1);
		}
        public float G { get => Green; set => Green = value; }


        private float blue = 0;
        public float Blue { 
            get => blue; 
            set => blue = Math.Clamp(value, 0, 1); 
        }
        public float B { get => Blue; set => Blue = value; }

        private float alpha = 0;
        public float Alpha { 
            get => alpha; 
            set => alpha = Math.Clamp(value, 0, 1); 
        }
        public float A { get => Alpha; set => Alpha = value; }

        public ColorF(float _red, float _green, float _blue, float _alpha)
        {
            Red = _red;
            Green = _green;
            Blue = _blue;
            Alpha = _alpha;
        }

        public ColorF(float red, float green, float blue) :
            this(red, green, blue, 1)
        { }

        public ColorF(int red, int green, int blue, int alpha) :
            this(red / 255f, green / 255f, blue / 255f, alpha / 255f)
        { }

        public ColorF(int red, int green, int blue) :
            this(red / 255f, green / 255f, blue / 255f)
        { }


        //Operator overloads
        public static ColorF operator +(ColorF c1, ColorF c2)
            => new ColorF(c1.R + c2.R,
                          c1.G + c2.G,
                          c1.B + c2.B,
                          c1.A + c2.A);

        public static ColorF operator +(ColorF c, Vector3 v)
            => new ColorF(c.R + v.X,
                          c.G + v.Y,
                          c.B + v.Z,
                          c.A);
        public static ColorF operator +(Vector3 v, ColorF c)
            => c + v;

        public static ColorF operator *(ColorF c1, float l)
            => new(c1.R * l, c1.G * l, c1.B * l);

        public static ColorF operator *(float l, ColorF c1)
            => c1 * l;


        /// <summary>
        /// Fastest way to increment fields of struct
        /// </summary>
        public static void Increment(ref ColorF color, Vector4 move)
        {
            color.R += move.X;
            color.G += move.Y;
            color.B += move.Z;
            color.A += move.W;
        }

		/// <summary>
		/// Fastest way to increment fields of struct
		/// </summary>
		public static void Increment(ref ColorF color, Vector3 move)
        {
            Increment(ref color, new Vector4(move, 0));
        }


        //Conversion to built-in monogame color type
        public static explicit operator Color(ColorF c)
            => new Color(c.Red, c.Green, c.Blue, c.Alpha);

        public Color ToColor()
            => (Color)this;

    }
}
