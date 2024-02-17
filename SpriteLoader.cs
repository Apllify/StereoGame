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

    /// <summary>
    /// A thread safe class for loading resources
    /// </summary>
    public class SpriteLoader
    {
        private static object _lock = new object();


		public static string fallbackSprite { get; set; } = "Box";
		public static bool IsInitialized { get; private set; } = false;
        public static ContentManager ContentManager { get; private set; }
        

        public static void InitializeSpriteLoader(ContentManager _contentManager)
        {
            //save the content manager that allows us to load sprites
            ContentManager = _contentManager;

            IsInitialized = true;
        }


        /// <summary>
        /// Warning : unsafe method, use <see cref="LoadTexture2D(string)"/>
        /// or <see cref="LoadFont(string)"/> instead
        /// </summary>
        public static T LoadContent<T>(string textureName)
        {
            lock (_lock)
            {
				if (!IsInitialized)
				{
					throw new Exception("SpriteLoader not initialized !");
				}

				return ContentManager.Load<T>(textureName);

			}

		}

        public static Texture2D LoadTexture2D(string spriteName)
        {
            lock (_lock)
            {
				if (!IsInitialized)
				{
					throw new Exception("SpriteLoader not initialized !");
				}

				Texture2D sprite = ContentManager.Load<Texture2D>(spriteName);
                //try load default texture if load failed
				sprite ??= ContentManager.Load<Texture2D>(fallbackSprite);

				return sprite;
			}

        }

        public static SpriteFont LoadFont(string fontName)
        {
            lock (_lock)
            {
				if (!IsInitialized)
				{
					throw new Exception("SpriteLoader not initialized !");
				}

				SpriteFont font = ContentManager.Load<SpriteFont>(fontName);
				return font;
			}
        }
    }
}
