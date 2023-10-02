using Microsoft.Xna.Framework;
using MonoGame.Extended;
using StereoGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StereoGame.Hitbox
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

        public void Shift(float shiftX, float shiftY)
        {
            hitboxRectangle.Offset(shiftX, shiftY);
        }

        public IHitbox Shifted(float shiftX, float shiftY)
        {
            return new RectangleHitbox(hitboxRectangle.X + shiftX, hitboxRectangle.Y + shiftY,
                                        hitboxRectangle.Width, hitboxRectangle.Height, 
                                        SpritedEntity.SpriteAnchor.TopLeft);
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



        public Vector2 SolveCollision(IHitbox other)
        {
            if (other is RectangleHitbox)
            {
                return SolveBoxCollision((RectangleHitbox) other);
			}
            else
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="w1">The weight of the entity whose hitbox SolveBoxCollision() is called from.</param>
        /// <param name="w2">The weight of the entity containing the hitbox passed as argument.</param>
        private Vector2 SolveBoxCollision(RectangleHitbox other)
        {


            //for now, don't care when an element is inside another
            RectangleF intersectionRec = GetBoundingBox().Intersection(other.GetBoundingBox());

			if (intersectionRec == GetBoundingBox() ||
                intersectionRec == other.GetBoundingBox()) 
			{
				return Vector2.Zero;
			}








			//displace the entities based on which of the dimensions of intersection is smaller
			if (intersectionRec.Width >= intersectionRec.Height)
			{
				//displace vertically 
				int e1Dir = (GetBoundingBox().TopLeft.Y > other.GetBoundingBox().TopLeft.Y) ? 1 : -1;

				return new Vector2(0, e1Dir * intersectionRec.Height);
			}
			else
			{
				//displace horizontally 
				int e1Dir = (GetBoundingBox().TopLeft.X > other.GetBoundingBox().TopLeft.X) ? 1 : -1;

				return new Vector2(e1Dir * intersectionRec.Width, 0);
			}
		}






    }
}
