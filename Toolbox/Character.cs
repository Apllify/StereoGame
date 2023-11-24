using Microsoft.Xna.Framework.Graphics;
using StereoGame.Entities;
using StereoGame.Hitbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace StereoGame.Toolbox
{
	/// <summary>
	/// A class MADE to be extended from for creating a controllable character.
	/// Implements input handling and a basic stats feature
	/// </summary>
	public class Character : CollisionEntity
	{
		public String moveUp, moveDown, moveLeft, moveRight;
		public float Speed;

		private Vector2 movementVec;

		
		public Character(Vector2 pos, float speed, Texture2D texture, IHitbox hitbox):
			base(pos, hitbox, texture)
		{
			Speed = speed; 

			//player should be a bit heavier than default entities
			CollisionWeight = 3;
		}


		/// <summary>
		/// Assign input actions to character movement.
		/// </summary>
		/// <param name="up">The action corresponding to upwards movement</param>
		/// <param name="down">The action corresponding to downwards movement</param>
		/// <param name="left">The action corresponding to leftwards movement</param>
		/// <param name="right">The action corresponding to rightwards movement</param>
		public void SetControls(String up, String down, String left, String right)
		{
			this.moveUp = up;
			this.moveDown = down;
			this.moveLeft = left;
			this.moveRight = right;
		}

		protected override void CustomUpdate(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.CustomUpdate(gameTime);

			//make sure that we do have proper controls
			bool bound = (moveUp != null &&
						 moveDown != null &&
						 moveLeft != null &&
						 moveRight != null);

			if (bound)
			{
				float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
				Vector2 inputVec = new();

				if (InputHandler.CurrentHandler.IsActionDown(moveUp))
				{
					inputVec.Y = -1;
				}
				else if (InputHandler.CurrentHandler.IsActionDown(moveDown))
				{
					inputVec.Y = 1;
				}

				if (InputHandler.CurrentHandler.IsActionDown(moveRight))
				{
					inputVec.X = 1;
				}
				else if (InputHandler.CurrentHandler.IsActionDown(moveLeft))
				{
					inputVec.X = -1;
				}

				//only process movement if inputs were made
				if (inputVec != Vector2.Zero)
				{
					inputVec.Normalize();
					movementVec = inputVec * deltaT * Speed;

					ShiftPosition(movementVec);
				}
			}
		}
	}
}
