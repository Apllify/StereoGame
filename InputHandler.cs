using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace StereoGame
{
	/// <summary>
	/// Singleton class 
	/// </summary>
	public class InputHandler
	{
		//Static members
		public static InputHandler CurrentHandler { 
			get => currentHandler; 
		}
		private static InputHandler currentHandler;

		public static Vector2 VirtualResolution { get; set; }
		public static float GameWidth {	get => VirtualResolution.X; }
		public static float GameHeight {  get => VirtualResolution.Y; }


		//Non-static members
		private static Dictionary<String, List<Keys>> actionMapping { get; set; }
		//every string represents some action like "move up"


		private KeyboardState lastState;
		private KeyboardState curState;

		private MouseState lastMouseState;
		private MouseState curMouseState;

		private GraphicsDevice gd;

		

		public static void Initialize(GraphicsDevice gd, Vector2 virtualResolution, 
								      Dictionary<String, List<Keys>> actionMapping)
		{

            if ( CurrentHandler != null) 
            {
				return;
            }

			//create the input handler instance
			VirtualResolution = virtualResolution;
			currentHandler = new InputHandler(gd, actionMapping);
		}

		public InputHandler(GraphicsDevice _gd, Dictionary<String, List<Keys>> _actionMapping)
		{
			lastState = Keyboard.GetState();
			curState = Keyboard.GetState();

			lastMouseState = Mouse.GetState();
			curMouseState = Mouse.GetState();

			gd = _gd;

			actionMapping = _actionMapping;
		}

		public bool AddAction(String actionName, List<Keys> actions)
		{
			return actionMapping.TryAdd(actionName, actions);
		}

		public void Update()
		{
			lastState = curState;
			curState = Keyboard.GetState();

			lastMouseState = curMouseState;
			curMouseState = Mouse.GetState();
		}


		/// <summary>
		/// Returns false if no action with this name exists
		/// </summary>
		public bool IsActionDown(String action)
		{
			List<Keys> keys;
			actionMapping.TryGetValue(action, out keys);

			if (keys == null)
				return false;

			foreach (Keys key in keys)
			{
				if (IsKeyDown(key))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns false if no action with this name exists
		/// </summary>
		public bool IsActionJustDown(String action)
		{
			List<Keys> keys;
			actionMapping.TryGetValue(action, out keys);

			if (keys == null)
				return false;

			foreach (Keys key in keys)
			{
				if (IsKeyJustDown(key))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns *true* if no action with this name exists
		/// </summary>
		public bool IsActionUp(String action)
		{
			return !IsActionDown(action);
		}



		public bool IsKeyDown(Keys key)
		{
			return curState.IsKeyDown(key);
		}

		public bool IsKeyJustDown(Keys key)
		{
			return (curState.IsKeyDown(key) && lastState.IsKeyUp(key));
		}

		public bool IsKeyUp(Keys key)
		{
			return !IsKeyDown(key);
		}

		/// <summary>
		/// Converts effective screen position into virtual position 
		/// (whose dimensions are chosen by player)
		/// </summary>
		public Vector2 RealToVirtualPos(Vector2 pos)
		{
			Vector2 virtualPos = new();

			float screenWidth = gd.PresentationParameters.BackBufferWidth;
			float screenHeight = gd.PresentationParameters.BackBufferHeight;

			virtualPos.X = ((float)pos.X / screenWidth)
			   * VirtualResolution.X;
			virtualPos.Y = ((float)pos.Y / screenHeight)
						   * VirtualResolution.Y;

			return virtualPos;
		}

		/// <summary>
		/// See <see cref="RealToVirtualPos(Vector2)"/>
		/// </summary>
		public Vector2 VirtualToRealPos(Vector2 pos)
		{
			Vector2 realPos = new();

			float screenWidth = gd.PresentationParameters.BackBufferWidth;
			float screenHeight = gd.PresentationParameters.BackBufferHeight;

			realPos.X = ((float)pos.X / VirtualResolution.X)
			   * screenWidth;
			realPos.Y = ((float)pos.Y / VirtualResolution.Y)
						   * screenHeight;

			return realPos;
		}


		/// <summary>
		/// Abbreviation for <see cref="RealToVirtualPos(Vector2)"/>
		/// </summary>
		public Vector2 RtV(Vector2 pos)
		{
			return RealToVirtualPos(pos);
		}

		/// <summary>
		/// Abbreviation for <see cref="VirtualToRealPos(Vector2)"/>
		/// </summary>
		public Vector2 VtR(Vector2 pos)
		{
			return VirtualToRealPos(pos);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>Coordinates of the mouse w.r.t the virtual resolution.</returns>
		public Vector2 GetMousePos()
		{
			return RealToVirtualPos(curMouseState.Position.ToVector2());
		}
	}
}

