using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StereoGame.Hitbox;

namespace StereoGame.Entities
{
    public class CollisionEntity : SpritedEntity
    {
        //List of entities attached to this one AKA their positions are locked relative to this
        private List<SpritedEntity> attachedEntities;
        protected IHitbox hitbox;

        //Keep track of the position from the last frame
        protected Vector2 oldPosition;
        protected Vector2 lastFrameMovement;

        //Keep track of our position on the collision grid (32 x 32)
        public const int CollisionTileSize = 32;

        public uint GridXPos { get; private set; }
        public uint GridYPos { get; private set; }



        /// <summary>
        /// Don't make this zero under ANY circumstances.
        /// </summary>
        public float CollisionWeight { get; set; }


        public CollisionEntity(Vector2 position, IHitbox _hitbox, Texture2D _sprite, SpriteAnchor _spriteAnchor, float _layerDepth) :
            base(position, _sprite, _spriteAnchor, _layerDepth)
        {
            //stuff common to all entity subclasses
            Tag = "untagged-collision-entity";
            CollisionWeight = 1;

            //instantiate attached entities list
            attachedEntities = new List<SpritedEntity>();

            //collision entity stuff
            if (_hitbox != null)
            {
                SetHitbox(_hitbox);
				ComputeGridPos();

			}

		}

        public CollisionEntity(float x, float y, IHitbox _hitbox, Texture2D _sprite, SpriteAnchor _spriteAnchor, float _layerDepth) :
            this(new Vector2(x, y), _hitbox, _sprite, _spriteAnchor, _layerDepth)
        { }

        public CollisionEntity(Vector2 position, IHitbox _hitbox, Texture2D _sprite, SpriteAnchor _spriteAnchor) :
            this(position, _hitbox, _sprite, _spriteAnchor, ActiveDepth)
        { }

        public CollisionEntity(Vector2 position, IHitbox _hitbox, Texture2D _sprite) :
            this(position, _hitbox, _sprite, SpriteAnchor.Center)
        { }

        public CollisionEntity(float x, float y, IHitbox _hitbox, Texture2D _sprite, SpriteAnchor _spriteAnchor) :
            this(new Vector2(x, y), _hitbox, _sprite, _spriteAnchor, ActiveDepth)
        { }

        private void ComputeGridPos()
        {
            if (GetHitbox() is null)
                return;

            IHitbox hitbox = GetHitbox();
			int lBound = (int)(hitbox.GetBoundingBox().Left / CollisionTileSize);
			int rBound = (int)(hitbox.GetBoundingBox().Right / CollisionTileSize);
			int bBound = (int)(hitbox.GetBoundingBox().Bottom / CollisionTileSize);
			int tBound = (int)(hitbox.GetBoundingBox().Top / CollisionTileSize);

			GridXPos = ~(1u << (rBound - lBound)) << lBound;
			GridYPos = ~(1u << (bBound - tBound)) << tBound;
		}

        public override void ShiftPosition(float shiftX, float shiftY)
        {
            // position change + moving children
            base.ShiftPosition(shiftX, shiftY);

            //move our children + hitbox(es)
            ShiftChildrenPositions(shiftX, shiftY);
            hitbox.Shift(shiftX, shiftY);

            //TODO : use shift to optimize the new grid coordinates
            ComputeGridPos();

		}



		public void SetHitbox(IHitbox _hitbox)
        {
            //set hitbox and add it to the list of attached entities
            hitbox = _hitbox;
        }

        /// <summary>
        /// Compute the default hitbox of this CollisionEntity
        /// Supports the available spriteAnchors
        /// </summary>
        /// <returns>The bounds of the entity sprite</returns>
        public IHitbox GetHitbox()
        {
            return hitbox;
        }

        public Vector2 GetLastPosition()
        {
            return oldPosition;
        }

        public Vector2 GetLastFrameMovement()
        {
            return lastFrameMovement;
        }

        public virtual void OnCollision(CollisionEntity otherEntity)
        {

        }

        protected void ShiftChildrenPositions(float shiftX, float shiftY)
        {
            foreach (SpritedEntity e in attachedEntities)
            {
                e.ShiftPosition(shiftX, shiftY);
            }
        }


        protected sealed override void PreUpdate(GameTime gameTime)
        {
            oldPosition = GetPosition();
        }

        protected sealed override void PostUpdate(GameTime gameTime)
        {
            lastFrameMovement = GetPosition() - oldPosition;
        }


    }
}
