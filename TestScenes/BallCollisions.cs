using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using StereoGame.Hitbox;
using StereoGame.Particles.ParticleShapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnfinishedBusinessman;
using UnfinishedBusinessman.Entities.Characters;
using StereoGame.Entities;

namespace StereoGame.TestScenes
{
    public class BallCollisions : Scene
	{

		private const float dirChangeInterval = 2f;
		private float dirChangeTimer = 0;
		private Vector2 windDirection = new Vector2(1, 1).NormalizedCopy();
		private float windStrength = 40;

		private List<CollisionEntity> ballsList = new();

		public BallCollisions():
			base()
		{

		}

		public override void Load()
		{
			Texture2D ballSprite = SpriteLoader.LoadTexture2D("Ball");
			CircleHitbox hitbox = new(0, 0, 10);


			//create many radius=10 balls in random locations
			float x, y;
			for(int i = 0; i<100; i++)
			{
				(x,  y) = (RNG.NextFloat(InputHandler.GameWidth), 
						   RNG.NextFloat(InputHandler.GameHeight));
				CollisionEntity ball = new(new(x, y), hitbox.Shifted(x, y), null);
				ballsList.Add(ball);
				AddCollisionEntity(ball);
			}


			//create edges for the scene
			RectangleHitbox topBound, bottomBound, leftBound, rightBound;
			topBound = new(0, -20, InputHandler.GameWidth, 20);
			bottomBound = new(0, InputHandler.GameHeight, InputHandler.GameWidth, 20);
			leftBound = new(-20, 0, 20, InputHandler.GameHeight);
			rightBound = new(InputHandler.GameWidth, 0, 20, InputHandler.GameHeight);
			List<IHitbox> boundaries = new()
			{
				topBound, bottomBound, leftBound, rightBound
			};

			foreach(IHitbox recHitbox in boundaries)
			{
				CollisionEntity current = new(new(), recHitbox, null);
				current.CollisionWeight = float.PositiveInfinity;

				AddCollisionEntity(current);
			}

		}

		protected override void CustomUpdate(GameTime gameTime)
		{
			//update wind direction if applicable
			dirChangeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (dirChangeTimer >= dirChangeInterval)
			{
				dirChangeTimer = 0f;
				windDirection = windDirection.PerpendicularClockwise();
			}

			//move all balls
			foreach (CollisionEntity ce in ballsList)
			{
				ce.ShiftPosition((float)gameTime.ElapsedGameTime.TotalSeconds * windStrength * windDirection);
			}
		}
	}


}
