using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StereoGame
{
	public class CollisionEntity : SpritedEntity
	{

		public const float ActiveDepth = 0.5f;
		public const float BackgroundDepth = 0.2f;
		public const float ForegroundDepth = 1f;

		public const int CollisionPixelPrecision = 1;

		protected RectangleEntity hitbox;

		//List of entities attached to this one AKA their positions are locked relative to this
		private List<SpritedEntity> attachedEntities;

		//Keep track of the position from the last frame
		protected Vector2 oldPosition;
		protected Vector2 lastFrameMovement;



		/// <summary>
		/// Don't make this zero under ANY circumstances.
		/// </summary>
		public double CollisionWeight { get; set; }

		public CollisionEntity(Vector2 position, RectangleEntity _hitbox,  Texture2D _sprite, SpriteAnchor _spriteAnchor, float _layerDepth):
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
			}

		}
		
		public CollisionEntity(float x, float y, RectangleEntity _hitbox, Texture2D _sprite, SpriteAnchor _spriteAnchor, float _layerDepth):
			this(new Vector2(x,y), _hitbox, _sprite, _spriteAnchor, _layerDepth)
		{ }

		public CollisionEntity(Vector2 position, RectangleEntity _hitbox, Texture2D _sprite, SpriteAnchor _spriteAnchor):
			this(position, _hitbox, _sprite, _spriteAnchor, CollisionEntity.ActiveDepth)
		{ }

		public CollisionEntity(float x, float y, RectangleEntity _hitbox, Texture2D _sprite, SpriteAnchor _spriteAnchor) :
			this(new Vector2(x, y), _hitbox, _sprite, _spriteAnchor, CollisionEntity.ActiveDepth)
		{ }


		public override void ShiftPosition(float shiftX, float shiftY)
		{
			//normal position change + moving children
			base.ShiftPosition(shiftX, shiftY);

			ShiftChildrenPositions(shiftX, shiftY);
		}



		public void SetHitbox(RectangleEntity _hitbox)
		{
			//remove older hitbox if necessary
			if (hitbox != null)
			{
				attachedEntities.Remove(hitbox);
			}


			//set hitbox and add it to the list of attached entities
			hitbox = _hitbox;
			attachedEntities.Add(hitbox);
		}

		/// <summary>
		/// Compute the default hitbox of this CollisionEntity
		/// Supports the available spriteAnchors
		/// </summary>
		/// <returns>The bounds of the entity sprite</returns>
		public RectangleEntity GetHitbox()
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


		public virtual void OnCollision(CollisionEntity otherEntity)
		{

		}

	}
}
