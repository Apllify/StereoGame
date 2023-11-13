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
        public float Red { get; set; }
        public float R { get => Red; set => Red = value; }

        public float Green { get; set; }
        public float G { get => Green; set => Green = value; }

        public float Blue { get; set; }
        public float B { get => Blue; set => Blue = value; }

        public float Alpha { get; set; }
        public float A { get => Alpha; set => Alpha = value; }

        public ColorF(float red, float green, float blue, float alpha)
        {
            Red = Math.Clamp(red, 0, 1);
            Green = Math.Clamp(green, 0, 1);
            Blue = Math.Clamp(blue, 0, 1);
            Alpha = Math.Clamp(alpha, 0, 1);
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


        //Conversion to built-in monogame color type
        public static explicit operator Color(ColorF c)
            => new Color(c.Red, c.Green, c.Blue, c.Alpha);

        public Color ToColor()
            => (Color)this;

    }
}
