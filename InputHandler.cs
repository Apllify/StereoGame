using Microsoft.Xna.Framework;
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


		//Non-static members
		private KeyboardState lastState;
		private KeyboardState curState;

		private MouseState curMouseState;

		

		public static void Initialize()
		{
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
			CurrentHandler = new InputHandler();
		}

		public InputHandler()
		{
			lastState = Keyboard.GetState();
			curState = Keyboard.GetState();

			curMouseState = Mouse.GetState();
		}

		public void Update()
		{
			lastState = curState;
			curState = Keyboard.GetState();
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


		public Vector2 GetMousePos()
		{
			throw new NotImplementedException();
		}
	}
}

