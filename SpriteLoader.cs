using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StereoGame
{
    public class SpriteLoader
    {
        public static bool isInitialized = false; 
        public static ContentManager contentManager;
        public const string fallbackSprite = "Box";

        public static void InitializeSpriteLoader(ContentManager _contentManager)
        {
            //save the content manager that allows us to load sprites
            contentManager = _contentManager;

            isInitialized = true;
        }

        public static T LoadContent<T>(string textureName)
        {
            return contentManager.Load<T>(textureName);

        }

        public static Texture2D LoadTexture2D(string spriteName)
        {
            Texture2D sprite = contentManager.Load<Texture2D>(spriteName);
            sprite ??= contentManager.Load<Texture2D>(fallbackSprite);

            return sprite;
        }

        public static SpriteFont LoadFont(string fontName)
        {
            SpriteFont font = contentManager.Load<SpriteFont>(fontName);
            return font;
        }

    }
}
