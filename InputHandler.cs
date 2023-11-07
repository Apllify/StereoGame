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
	public class InputHandler
	{
		//Static members
		public static InputHandler CurrentHandler { get; set; }

		public static Dictionary<Action, List<Keys>> ActionMapping { get; set; }

		public enum Action
		{
			DebugToggle,

			MoveUp,
			MoveDown,
			MoveRight,
			MoveLeft,
			ChangeHitbox
		}

		public static Vector2 VirtualResolution { get; set; }


		//Non-static members
		private KeyboardState lastState;
		private KeyboardState curState;

		private MouseState lastMouseState;
		private MouseState curMouseState;

		private GraphicsDevice gd;

		

		public static void Initialize(GraphicsDevice gd, Vector2 virtualResolution)
		{

            if ( CurrentHandler != null) 
            {
				return;
            }

            //CHANGE THE MAPPINGS HERE 
            ActionMapping = new Dictionary<Action , List<Keys>>()
			{
				{Action.DebugToggle, new List<Keys>() {Keys.NumPad1 } },

				{Action.MoveUp, new List<Keys>(){Keys.Z} },
				{Action.MoveDown, new List<Keys>(){Keys.S} },
				{Action.MoveRight, new List<Keys>(){Keys.D} },
				{Action.MoveLeft, new List<Keys>(){Keys.Q } },

				{Action.ChangeHitbox, new List<Keys>(){ Keys.Space } }
			};


			//create the input handler instance
			VirtualResolution = virtualResolution;
			CurrentHandler = new InputHandler(gd);
		}

		public InputHandler(GraphicsDevice _gd)
		{
			lastState = Keyboard.GetState();
			curState = Keyboard.GetState();

			lastMouseState = Mouse.GetState();
			curMouseState = Mouse.GetState();

			gd = _gd;
		}

		public void Update()
		{
			lastState = curState;
			curState = Keyboard.GetState();

			lastMouseState = curMouseState;
			curMouseState = Mouse.GetState();
		}


		public bool IsActionDown(Action action)
		{
			List<Keys> keys = ActionMapping[action];


			foreach (Keys key in keys)
			{
				if (IsKeyDown(key))
				{
					return true;
				}
			}

			return false;
		}

		public bool IsActionJustDown(Action action)
		{
			List<Keys> keys = ActionMapping[action];

			foreach (Keys key in keys)
			{
				if (IsKeyJustDown(key))
				{
					return true;
				}
			}

			return false;
		}

		public bool IsActionUp(Action action)
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
		/// 
		/// </summary>
		/// <returns>Coordinates of the mouse w.r.t the virtual resolution.</returns>
		public Vector2 GetMousePos()
		{
			Vector2 virtualPos = new();

			float screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			float screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

			virtualPos.X = ((float)curMouseState.X / screenWidth)
						   * VirtualResolution.X;
			virtualPos.Y = ((float)curMouseState.Y / screenHeight)
						   * VirtualResolution.Y;



			Debug.WriteLine(screenWidth);

			return virtualPos;

		}
	}
}

