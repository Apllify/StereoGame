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
using StereoGame.TestScenes;
using StereoGame;
using System.Diagnostics;

namespace StereoGame.TestScenes
{
	/// <summary>
	/// Tests the collision engine by spawning 
	/// a LOT of balls with collisions at once
	/// </summary>
    public class BallCollisions : Scene
	{
		/// <summary>
		/// Helper class for testing circle collisions
		/// </summary>
		private class BouncyBall : CollisionEntity
		{
			public static int BallSize = 4;

			public static float MinMomentum = 30;
			public static float MaxMomentum = 50;


			private Vector2 momentum;

			public BouncyBall(Vector2 position) :
				base(position, new CircleHitbox(position.X, position.Y, BallSize), null)
			{
				//apply random start momentum
				float moment_angle = RNG.NextFloat(0, 2 * MathF.PI);
				float moment_strength = RNG.NextFloat(MinMomentum, MaxMomentum);

				momentum = new Vector2(MathF.Cos(moment_angle) * moment_strength,
									   MathF.Sin(moment_angle) * moment_strength);
			}

			protected override void CustomUpdate(GameTime gameTime)
			{
				ShiftPosition(momentum * (float)gameTime.ElapsedGameTime.TotalSeconds);
			}

			public override void OnCollision(CollisionEntity otherEntity)
			{
				//randomize direction again
				momentum *= -1;
			}
		}

		const int BallCount = 500;


		public BallCollisions():
			base()
		{

		}

		public override void Load()
		{
			Texture2D ballSprite = SpriteLoader.LoadTexture2D("Ball");
			RectangleF spawnArea = new RectangleF(0, 0, InputHandler.GameWidth, InputHandler.GameHeight);

			//create all of the bouncy balls
			for (int i = 0; i < BallCount; i++)
			{
				AddCollisionEntity(new BouncyBall(RNG.RandomPos(ref spawnArea)));
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
	}


}
