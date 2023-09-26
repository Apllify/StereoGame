using Microsoft.Xna.Framework;
using MonoGame.Extended;
using StereoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnfinishedBusinessman.StereoGame.Hitbox
{
    public class RectangleHitbox : IHitbox
    {

        protected RectangleF hitboxRectangle;


        public RectangleHitbox(float x, float y, float width, float height, SpritedEntity.SpriteAnchor rectangleAnchor)
        {
            //create the matching rectangleF
            Vector2 topLeftPosition = SpritedEntity.GetTopLeftPosFromAnchor(new Vector2(x, y), 
                                                                            new Vector2(width, height), rectangleAnchor);

            hitboxRectangle = new RectangleF(topLeftPosition.X, topLeftPosition.Y, width, height);
        }

        public RectangleHitbox(float x, float y, float _width, float _height) :
            this(x, y, _width, _height, SpritedEntity.SpriteAnchor.Center) { }

        public RectangleHitbox(RectangleF startingState):
            this(startingState.X, startingState.Y, startingState.Width, startingState.Height,
                SpritedEntity.SpriteAnchor.TopLeft)
        
        {

        }


        public int GetTypeId()
        {
            return 0;
        }

        public void ShiftPosition(float shiftX, float shiftY)
        {
            hitboxRectangle.Offset(shiftX, shiftY);
        }

		public RectangleF GetBoundingBox()
		{
			return hitboxRectangle;
		}

		public bool Intersects(IHitbox otherHitbox)
        {
            if (otherHitbox is RectangleHitbox)
            {
                return hitboxRectangle.Intersects(otherHitbox.GetBoundingBox());
            }
            else
            {
                throw new NotImplementedException();
            }

        }



        public void SolveCollision(IHitbox other)
        {
            return;
        }






    }
}
