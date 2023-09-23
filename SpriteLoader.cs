using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnfinishedBusinessman.StereoGame
{
    public class SpriteLoader
    {
        public static ContentManager contentManager;
        public const string fallbackSprite = "Box";

        public static void InitializeSpriteLoader(ContentManager _contentManager)
        {
            contentManager = _contentManager;
        }

        public static T LoadContent<T>(string textureName)
        {
            return contentManager.Load<T>(textureName);

        }

        public static Texture2D LoadTexture2D<Texture2D>(string spriteName)
        {
            Texture2D sprite = contentManager.Load<Texture2D>(spriteName);
            sprite ??= contentManager.Load<Texture2D>(fallbackSprite);

            return sprite;
        }


    }
}
