using Microsoft.Xna.Framework;
using MonoGame.Extended;
using StereoGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            else if (otherHitbox is CircleHitbox)
            {
                CircleHitbox otherCircle = otherHitbox as CircleHitbox;
                Vector2 circleCenter = new Vector2(otherCircle.X, otherCircle.Y);

				//determine closest horizontal and vertical edges of the rectangle
				//edges are represented in rectangles so 
				Vector2 horizontalLineP1;
				Vector2 horizontalLineP2;

				Vector2 verticalLineP1;
                Vector2 verticalLineP2;

				(horizontalLineP1, horizontalLineP2) = (circleCenter.Y > hitboxRectangle.Y) ?
							(hitboxRectangle.BottomRight, hitboxRectangle.BottomLeft) :
							(hitboxRectangle.TopRight, hitboxRectangle.TopLeft);



				(verticalLineP1, verticalLineP2) = (circleCenter.X > hitboxRectangle.X) ?
                                            (hitboxRectangle.TopRight, hitboxRectangle.BottomRight) :
                                            (hitboxRectangle.TopLeft,  hitboxRectangle.BottomLeft);
                                            

                //compute the shortest distances to the edges now
                float vertDistance = linePointShortest(verticalLineP1, verticalLineP2, circleCenter).Length();
				float horDistance = linePointShortest(horizontalLineP1, horizontalLineP2, circleCenter).Length();


                //check if one of the distance is smaller than the radius of the circle
                return (vertDistance < otherCircle.Radius || horDistance < otherCircle.Radius);

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
                return SolveBoxCollision(other as RectangleHitbox);
			}
            else if (other is CircleHitbox)
            {
                return SolveCircleCollision(other as CircleHitbox);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns the shortest Vector2 connecting the point to the segment.
        /// </summary>
        /// <param name="lineP1"></param>
        /// <param name="lineP2"></param>
        /// <param name="point"></param>
        /// <returns>A vector from the line to the argument point</returns>
        private Vector2 linePointShortest(Vector2 lineP1, Vector2 lineP2, Vector2 point)
        {
            //remove edge case 
            if (lineP1 == lineP2)
            {
                return (point - lineP1);
            }

            //project the point on the line 
            float lineProjection = (point - lineP1).Dot((lineP2 - lineP1)) / ((lineP2 - lineP1).Length());
            lineProjection /= (lineP2 - lineP1).Length();

			lineProjection = Math.Clamp(lineProjection, 0, 1);

            return -((point - lineP1) - lineProjection * (lineP2 - lineP1));
            
        }

        /// <summary>
        /// Handles box-box collisions by using the intersection rectangle.
        /// </summary>
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



        /// <summary>
        /// Handles box-circle collisions.
        /// Just does standard displacing along collision axis.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private Vector2 SolveCircleCollision(CircleHitbox other)
        {
            //similar behavior to intersection checking
			Vector2 circleCenter = new Vector2(other.X, other.Y);

			//determine closest horizontal and vertical edges of the rectangle
			//edges are represented in rectangles so 
			var (verticalLineP1, verticalLineP2) = (circleCenter.X > hitboxRectangle.X) ?
										(hitboxRectangle.TopRight, hitboxRectangle.BottomRight) :
										(hitboxRectangle.TopLeft, hitboxRectangle.BottomLeft);

			var (horizontalLineP1, horizontalLineP2) = (circleCenter.Y > hitboxRectangle.Y) ?
										(hitboxRectangle.BottomRight, hitboxRectangle.BottomLeft) :
										(hitboxRectangle.TopRight, hitboxRectangle.TopLeft);

			//compute the shortest paths to the edges now
			Vector2 vertShortest = linePointShortest(verticalLineP1, verticalLineP2, circleCenter);
			Vector2 horShortest = linePointShortest(horizontalLineP1, horizontalLineP2, circleCenter);

            //TODO : fix this pls
            Vector2 vertDisplacement;
            Vector2 horDisplacement;




            //since the vector goes from rec edge to circle, we return the opposite
            return ((Vector2)hitboxRectangle.Center - new Vector2(other.X, other.Y)) / 10;   
		}






    }
}
