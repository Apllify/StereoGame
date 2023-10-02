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
        private Vector2 lastFrameMovement;


        public RectangleHitbox(float x, float y, float width, float height, SpritedEntity.SpriteAnchor rectangleAnchor)
        {
            //create the matching rectangleF
            Vector2 topLeftPosition = SpritedEntity.GetTopLeftPosFromAnchor(new Vector2(x, y), 
                                                                            new Vector2(width, height), rectangleAnchor);

            hitboxRectangle = new RectangleF(topLeftPosition.X, topLeftPosition.Y, width, height);


            //keep track of our movements
            lastFrameMovement = new Vector2(0, 0);
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

        public Vector2 GetLastMove()
        {
            return lastFrameMovement;
        }

        public void Shift(float shiftX, float shiftY)
        {
            hitboxRectangle.Offset(shiftX, shiftY);

            //keep track of this offset
            lastFrameMovement.X = shiftX;
            lastFrameMovement.Y = shiftY;
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

                //INTERSECTION CASE 1 : the center of the circle is in the rectangle
                if (GetBoundingBox().Contains(circleCenter)) {
                    return true;
                }

				//INTERSECTION CASE 2 : the center of circle is outside the rectangle but still collides
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
            //handle the case where one of the elements is contained within the other
            RectangleF selfRec = GetBoundingBox();
            RectangleF otherRec = other.GetBoundingBox();



            //OVERARCHING CASE 1 : one rectangle is inside the other
            RectangleF intersectionRec = selfRec.Intersection(otherRec);
            if (intersectionRec == selfRec|| intersectionRec == otherRec){

                //CASE 1 : we are inside of the other rectangle
                if (intersectionRec == selfRec)
                {
                    //TODO : probably implement this ? pretty please ?
                    if (GetLastMove() == Vector2.Zero)
                    {
                        return Vector2.Zero;
                    }

                    Vector2 exitVector = -GetLastMove().NormalizedCopy();

                    //compute the distances required to get out on either direction
                    float verDistance = Math.Min(Math.Abs(selfRec.Center.Y - otherRec.Top),
                                                 Math.Abs(selfRec.Y - otherRec.Bottom));

                    float horDistance = Math.Min(Math.Abs(selfRec.Center.X - otherRec.Left),
                                                 Math.Abs(selfRec.Center.X - otherRec.Right)) ;

                    verDistance += selfRec.Height / 2;
                    horDistance += selfRec.Width / 2;


                    //determine whether we need a diagonal or straight displacement
                    if (exitVector.ToAngle() % Math.PI/4 == 0) //straight
                    {
                        exitVector *= verDistance;
                        exitVector *= horDistance;

                        return exitVector;
                    }
                    else //diagonal / mixed angle
                    {
                        exitVector /= Math.Max(Math.Abs(exitVector.X), Math.Abs(exitVector.Y));
                        float multiplier = (Math.Abs(exitVector.X) > Math.Abs(exitVector.Y)) ?
                                           horDistance : verDistance;

                        return multiplier * exitVector;
					}




                }
                // CASE 2 : the other rectangle is inside of us
                else
                {
                    return -other.SolveBoxCollision(this);
                }
            }


            //OVERARCHING CASE 2 : the rectangles simply intersect
			//displace the entities based on which of the dimensions of intersection is smaller
			if (intersectionRec.Width >= intersectionRec.Height)
			{
				//displace vertically 
				int e1Dir = (selfRec.TopLeft.Y > otherRec.TopLeft.Y) ? 1 : -1;

				return new Vector2(0, e1Dir * intersectionRec.Height);
			}
			else
			{
				//displace horizontally 
				int e1Dir = (selfRec.TopLeft.X > otherRec.TopLeft.X) ? 1 : -1;

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

            //CASE 1 : the center of the circle is inside of the rectangle 
            if (GetBoundingBox().Contains(circleCenter))
            {
                //TODO : get a better algorithm for this 
				return ((Vector2)hitboxRectangle.Center - new Vector2(other.X, other.Y)) / 10;
			}



            //CASE 2 : the center of the circle is NOT in the rectangle 

			//determine closest horizontal and vertical edges of the rectangle
			//edges are represented in rectangles so 
			var (verticalLineP1, verticalLineP2) = (circleCenter.X > hitboxRectangle.X) ?
										(hitboxRectangle.TopRight, hitboxRectangle.BottomRight) :
										(hitboxRectangle.TopLeft, hitboxRectangle.BottomLeft);

			var (horizontalLineP1, horizontalLineP2) = (circleCenter.Y > hitboxRectangle.Y) ?
										(hitboxRectangle.BottomRight, hitboxRectangle.BottomLeft) :
										(hitboxRectangle.TopRight, hitboxRectangle.TopLeft);

			//compute the shortest paths to the edges now
			float vertShortest = linePointShortest(verticalLineP1, verticalLineP2, circleCenter).Length();
			float horShortest = linePointShortest(horizontalLineP1, horizontalLineP2, circleCenter).Length();

            float finalDisp = other.Radius - Math.Min(vertShortest, horShortest);
            Vector2 centerToCenter = GetBoundingBox().Center - circleCenter;




            //since the vector goes from rec edge to circle, we return the opposite
            return finalDisp * centerToCenter.NormalizedCopy();   
		}






    }
}
