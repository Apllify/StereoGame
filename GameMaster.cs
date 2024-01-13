using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UnfinishedBusinessman;

namespace StereoGame
{
	public class GameMaster : Game
	{
		//Game dimensions
		public readonly int VirtualWidth;
		public readonly int VirtualHeight;
		public int VW { get => VirtualWidth;  }
		public int VH { get => VirtualHeight; }


		//Monogame things
		protected GraphicsDeviceManager _graphics;
		protected SpriteBatch _spriteBatch;
		protected RenderTarget2D _renderTarget;

		public Effect ElementShader { get; set; }
		public Effect ScreenShader { get; set; }


		private Dictionary<String, List<Keys>> actionMapping;
		private Scene currentScene;

		/// <summary>
		/// Set the main properties of the game
		/// </summary>
		/// <param name="virtualWidth">The game width (independent of window width)</param>
		/// <param name="virtualHeight">The game height (independent of window height)</param>
		public GameMaster(int virtualWidth, int virtualHeight, 
						  Dictionary<String, List<Keys>> _actionMapping):
			base()
		{
			(VirtualWidth, VirtualHeight) = (virtualWidth, virtualHeight);
			actionMapping = _actionMapping;

			_graphics = new GraphicsDeviceManager(this);
			Window.AllowUserResizing = true;

			IsMouseVisible = true;
		}

		protected Scene GetScene()
		{
			return currentScene;
		}	
		protected void SetScene(Scene newS)
		{
			currentScene = newS;
			currentScene.Load();
		}

		protected void SetElementShader(Effect newShader)
		{
			ElementShader = newShader; 
		}
		protected void SetScreenShader(Effect newShader)
		{
			ScreenShader= newShader;
		}

		protected override void Initialize()
		{
			_renderTarget = new RenderTarget2D(GraphicsDevice, VW, VH);



			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			SpriteLoader.InitializeSpriteLoader(Content);
			InputHandler.Initialize(GraphicsDevice, new Vector2(VW, VH), actionMapping);

			base.LoadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			currentScene?.Update(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{

			//virtual rendering
			GraphicsDevice.SetRenderTarget(_renderTarget);
			GraphicsDevice.Clear(Color.Transparent);

			_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp,
							   effect : ElementShader) ;


			currentScene.Draw(_spriteBatch);

			_spriteBatch.End();


			//make black bars around game when resolution demands it
			float w = GraphicsDevice.PresentationParameters.BackBufferWidth;
			float h = GraphicsDevice.PresentationParameters.BackBufferHeight;
			float virtualRatio = (float)GameConstants.VirtualWidth / (float)GameConstants.VirtualHeight;

			float blackWidth = Math.Max(0, w - h * virtualRatio);
			float blackHeight = Math.Max(0, h - w / virtualRatio);

			Rectangle drawRec = new((int)(blackWidth / 2), (int)(blackHeight / 2),
									(int)Math.Min(w, h * virtualRatio),
									(int)Math.Min(h, w / virtualRatio));


			//scaling of render target 
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.LightSkyBlue);
			_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp,
							   effect : ScreenShader);

			_spriteBatch.Draw(_renderTarget, drawRec, Color.White);
			_spriteBatch.End();


			base.Draw(gameTime);
		}
	}
}
