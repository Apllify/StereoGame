using StereoGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;

namespace StereoGame.UI
{
	public class ProgressBar : SpritedEntity
	{

		//Properties
		public Vector2 Size { get; set; }

		private float visualProgress;
		private float progress;
		/// <summary>
		/// Lies between 0 and 1 (inclusive)
		/// </summary>
		public float Progress { 
			get => GetProgress(); 
			set => SetProgress(value);
		}

		public bool IsSmooth { get; set; } = true;
		public float SmoothingSpeed { get; set; } = 2f;


		public Color BackdropColor { get; set; } = Color.White;
		public Color ProgressColor { get; set; } = Color.PaleVioletRed;


		/// <summary>
		/// Initializes a new progress bar
		/// </summary>
		public ProgressBar(Vector2 origin, Vector2 size, float initialProgress): 
			base(origin, null, SpriteAnchor.TopLeft, ForegroundDepth)
		{
			Size = size;
			Progress = initialProgress;
			visualProgress = Progress;
		}


		/// <summary>
		/// Initializes a progress bar with a progression of 0%
		/// </summary>
		public ProgressBar(Vector2 origin, Vector2 size):
			this(origin, size, 0)
		{ }

		public float GetProgress() => 
			progress;
		public void SetProgress(float newProg) => 
			progress = Math.Clamp(newProg, 0, 1);


		protected override void CustomUpdate(GameTime gameTime)
		{
			float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
			//update visual progress
			if (IsSmooth && visualProgress != Progress)
			{
				//snap to it
				if (Math.Abs(visualProgress - Progress) < SmoothingSpeed * dt)
				{
					visualProgress = Progress;
				}
				//or lerp instead
				else
				{
					visualProgress += SmoothingSpeed * dt * Math.Sign(Progress - visualProgress);
				}
			}
			else
			{
				visualProgress = Progress;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			//draw backdrop
			Vector2 endPoint = Position + new Vector2(Size.X, 0);
			LineDraw(spriteBatch, Position, endPoint, (int)Size.Y, BackdropColor, ForegroundDepth);

			//draw progress
			Vector2 endProgress = Position + new Vector2(visualProgress * Size.X, 0);
			LineDraw(spriteBatch, Position, endProgress, (int)Size.Y, ProgressColor, ForegroundDepth - DepthStep);
		}
	}
}
