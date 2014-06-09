using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Pyramid_Plunder.Classes
{
    public class GameObject : GameGraphic
    {
        private const float DISINTEGRATE_TIME = 1;
        private const float DISINTEGRATE_ANIMATION_SPEED = 0.75f;

        protected Vector2 position;
        protected ItemList itemType;
        protected AudioEngine soundEngine;
        protected bool isSpawned;
        protected bool isPhysicsObject;
        protected bool isSolid;
        protected bool isItem;
        protected bool isKey;
        protected bool isPowerup;
        protected bool isHazard;
        protected bool isDisintegrating;

        private double disintegrateTimer;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        /// <param name="content">The content manager to use.</param>
        /// <param name="spawnPosition">The default spawning position.</param>
        public GameObject(string objName, ContentManager content, Vector2 spawnPosition,
            bool graphics, AudioEngine audioEngine) : base(objName, content, graphics)
        {
            soundEngine = audioEngine;
            position = spawnPosition;
            isPhysicsObject = false;
            isDisintegrating = false;
            disintegrateTimer = 0;
            Initialize();
        }

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        /// <param name="content">The content manager to use.</param>
        public GameObject(string objName, ContentManager content)
            : this(objName, content, new Vector2(0, 0), true, null) { }

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        /// <param name="content">The content manager to use.</param>
        /// <param name="graphics">Whether or not the object has a graphic.</param>
        public GameObject(string objName, ContentManager content, bool graphics)
            : this(objName, content, new Vector2(0, 0), graphics, null) { }

        /// <summary>
        /// Overrides the draw method in order to add the clause that checks to see if the object is spawned yet.
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw to.</param>
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch, GameTime time)
        {
            if (isSpawned)
                base.Draw(batch, time);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameTime time, bool playAnimations)
        {
            if (isSpawned)
                base.Draw(spriteBatch, time, playAnimations);
        }

        public virtual void Update(GameTime time)
        {
            if (isDisintegrating)
            {
                disintegrateTimer += time.ElapsedGameTime.TotalSeconds;
                if (disintegrateTimer >= DISINTEGRATE_TIME)
                {
                    Despawn();
                }
            }
        }

        /// <summary>
        /// Updates the GameObject's coordinates based on its position and the location of the player in the room
        /// </summary>
        /// <param name="player">The player to reference</param>
        public void UpdateCoordinates(Vector2 playerPosition, Vector2 playerCoordinates, Rectangle roomDimensions)
        {
            int xLine = Player.DEFAULT_SCREEN_POSITIONX;
            int yLine = Player.DEFAULT_SCREEN_POSITIONY;
            int viewWidth = Main.DEFAULT_RESOLUTION_X;
            int viewHeight = Main.DEFAULT_RESOLUTION_Y;
            int bgWidth = roomDimensions.Width;
            int bgHeight = roomDimensions.Height;

            //x
            if (bgWidth <= viewWidth)
                coordinates.X = position.X + (viewWidth - bgWidth) / 2;
            else
            {
                if (playerPosition.X >= bgWidth - viewWidth + xLine)
                    coordinates.X = position.X - bgWidth + viewWidth;
                else if (playerPosition.X <= xLine)
                    coordinates.X = position.X;
                else
                    coordinates.X = position.X - playerPosition.X + playerCoordinates.X;
            }

            //y
            if (bgHeight <= viewHeight)
                coordinates.Y = position.Y + (viewHeight - bgHeight) / 2;
            else
            {
                if (playerPosition.Y >= bgHeight - viewHeight + yLine)
                    coordinates.Y = position.Y - bgHeight + viewHeight;
                else if (playerPosition.Y <= yLine)
                    coordinates.Y = position.Y;
                else
                    coordinates.Y = position.Y - playerPosition.Y + playerCoordinates.Y;
            }
        }

        /// <summary>
        /// Sets up the object based on its objectType
        /// </summary>
        protected virtual void Initialize()
        {
            switch (objectName)
            {
                case "RedKey":
                {
                    isItem = true;
                    isKey = true;
                    isPowerup = false;
                    isHazard = false;
                    itemType = ItemList.RedKey;
                    break;
                }
                case "DashPowerup":
                {
                    isItem = true;
                    isKey = false;
                    isPowerup = true;
                    isHazard = false;
                    itemType = ItemList.Dash;
                    break;
                }
                case "Lava":
                {
                    isItem = false;
                    isKey = false;
                    isPowerup = false;
                    isHazard = true;
                    itemType = ItemList.NullItem;
                    break;
                }
                case "FloorSpikes":
                {
                    isItem = false;
                    isKey = false;
                    isPowerup = false;
                    isHazard = true;
                    itemType = ItemList.NullItem;
                    break;
                }
                default:
                {
                    isItem = false;
                    isKey = false;
                    isPowerup = false;
                    isHazard = false;
                    itemType = ItemList.NullItem;
                    break;
                }
            }
        }

        /// <summary>
        /// Causes the object to be "spawned," and therefore drawable and interactable.
        /// </summary>
        /// <param name="location">The location to spawn the object.</param>
        public virtual void Spawn(Vector2 location)
        {
            isSpawned = true;
            Position = location;
        }

        public virtual void Spawn()
        {
            isSpawned = true;
            if (objectName == "DisintegratingPlatform")
            {
                disintegrateTimer = 0;
                isDisintegrating = false;
                currentFrame = 0;
                animationSpeed[currentAnimation] = 0;
            }
        }

        /// <summary>
        /// Despawns the object and then removes it from memory by calling the Dispose method.
        /// </summary>
        public virtual void Despawn()
        {
            isSpawned = false;
        }

        public virtual bool HasInteraction(InteractionTypes interactionType)
        {
            switch (interactionType)
            {
                case InteractionTypes.PlayerAction:
                    switch (objectName)
                    {
                        case "SavePoint":
                            return true;
                        case "Lever":
                            return true;
                        default:
                            return false;
                    }
                case InteractionTypes.Collision:
                    if (isItem || isHazard)
                        return true;
                    break;
            }

            return false;
        }

        public virtual void StandingOn(string obj)
        {
            if (obj == "Player" && objectName == "DisintegratingPlatform" && !isDisintegrating)
            {
                soundEngine.Play(AudioEngine.SoundEffects.PlatformCrumble);
                isDisintegrating = true;
                animationSpeed[currentAnimation] = DISINTEGRATE_ANIMATION_SPEED;
            }
        }

        public void SwitchLever()
        {
            if (objectName == "Lever")
            {
                if (currentFrame == 0)
                    currentFrame = 1;
                else
                    currentFrame = 0;
            }
        }

        /// <summary>
        /// Deals with object interactions between each other.
        /// </summary>
        /// <param name="otherObject">The other object to interact with.</param>
        public virtual InteractionActions InteractWith(GameObject otherObject, InteractionTypes interactionType)
        {
            // TODO: Add the interaction chart for each possible object interaction.
            // The owner of this method is considered the "initiator" of the interaction
            return InteractionActions.None;
        }

        /// <summary>
        /// The position of the object relative to the room.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Whether or not the object is a PhysicsObject
        /// </summary>
        public bool IsPhysicsObject
        {
            get { return isPhysicsObject; }
        }

        /// <summary>
        /// Whether or not the object is solid
        /// </summary>
        public bool IsSolid
        {
            get { return isSolid; }
            set { isSolid = value; }
        }

        public bool IsHazard
        {
            get { return isHazard; }
        }

        /// <summary>
        /// What type of item is represented by this object.
        /// </summary>
        public ItemList ItemType
        {
            get { return itemType; }
        }

        /// <summary>
        /// Whether or not the object is currently spawned.
        /// </summary>
        public bool IsSpawned
        {
            get { return isSpawned; }
        }

        public bool IsKey
        {
            get { return isKey; }
        }

        public bool IsPowerup
        {
            get { return isPowerup; }
        }

        /// <summary>
        /// Get the interaction point for the object
        /// </summary>
        public virtual Vector2 InteractionPoint
        {
            get 
            {
                if (hasGraphics)
                    return new Vector2((HitBox.Width / 2) + position.X, (HitBox.Height / 2) + position.Y);
                else
                    return position;
            }
        }

        /// <summary>
        /// The area considered a positive match for collision detection.
        /// </summary>
        public override Rectangle HitBox
        {
            get
            {
                if (sprite != null)
                    return new Rectangle((int)position.X, (int)position.Y,
                        (int)animationDimensions[currentAnimation].X,
                        (int)animationDimensions[currentAnimation].Y);
                else if (!hasGraphics)
                    return new Rectangle((int)position.X, (int)position.Y, 0, 0);
                else
                    return new Rectangle(0, 0, 0, 0);
            }
        }
    }
}