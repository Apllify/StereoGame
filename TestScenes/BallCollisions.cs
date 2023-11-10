using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StereoGame.Hitbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnfinishedBusinessman;
using UnfinishedBusinessman.Entities.Characters;

namespace StereoGame.TestScenes
{
	public class BallCollisions : Scene
	{
		public BallCollisions():
			base()
		{

		}

		public override void Load()
		{
			//create many 10 radius balls in random locations
			Texture2D ball = SpriteLoader.LoadTexture2D("Ball");
			CircleHitbox hitbox = new(0, 0, 10);
			SpritedEntity.SpriteAnchor centerAnchor = SpritedEntity.SpriteAnchor.Center;
			SpritedEntity.SpriteAnchor topLeftAnchor = SpritedEntity.SpriteAnchor.TopLeft;



			float x, y;
			for(int i = 0; i<100; i++)
			{
				(x,  y) = (RNG.NextFloat(InputHandler.GameWidth), 
						   RNG.NextFloat(InputHandler.GameHeight));

				AddCollisionEntity(new(new(x, y), hitbox.Shifted(x, y), null, centerAnchor));
			}


			//create edges for the scene
			//RectangleHitbox topBound, bottomBound, leftBound, rightBound;
			//topBound = new(0, -20, InputHandler.GameWidth, 20);
			//bottomBound = new(0, InputHandler.GameHeight, InputHandler.GameWidth, 20);
			//leftBound = new(-20, 0, 20, InputHandler.GameHeight);
			//rightBound = new(InputHandler.GameWidth, 0, 20, InputHandler.GameHeight);

			//AddCollisionEntity(new(new(), topBound, null, centerAnchor));
			//AddCollisionEntity(new(new(), bottomBound, null, centerAnchor));
			//AddCollisionEntity(new(new(), leftBound, null, centerAnchor));
			//AddCollisionEntity(new(new(), rightBound, null, centerAnchor));

			//add the player 
			AddCollisionEntity(new Player(200, 100));



		}

		protected override void CustomUpdate(GameTime gameTime)
		{

		}
	}


}
