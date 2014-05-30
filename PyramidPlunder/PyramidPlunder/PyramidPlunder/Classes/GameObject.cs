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
        protected Vector2 position;
        protected ItemList itemType;
        protected bool isSpawned;
        protected bool isPhysicsObject;
        protected bool isSolid;
        protected bool isItem;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        /// <param name="content">The content manager to use.</param>
        /// <param name="spawnPosition">The default spawning position.</param>
        public GameObject(string objName, ContentManager content, Vector2 spawnPosition, bool graphics) : base(objName, content, graphics)
        {
            position = spawnPosition;
            isPhysicsObject = false;
            Initialize();
        }

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        /// <param name="content">The content manager to use.</param>
        public GameObject(string objName, ContentManager content)
            : this(objName, content, new Vector2(0, 0), true) { }

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        /// <param name="content">The content manager to use.</param>
        /// <param name="graphics">Whether or not the object has a graphic.</param>
        public GameObject(string objName, ContentManager content, bool graphics)
            : this(objName, content, new Vector2(0, 0), graphics) { }

        /// <summary>
        /// Overrides the draw method in order to add the clause that checks to see if the object is spawned yet.
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw to.</param>
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch, GameTime time)
        {
            if (isSpawned)
                base.Draw(batch, time);
        }

        /// <summary>
        /// Updates the GameObject's coordinates based on its position and the location of the player in the room
        /// </summary>
        /// <param name="player">The player to reference</param>
        public void UpdateCoordinates(Vector2 playerPosition, Vector2 playerCoordinates, Rectangle roomDimensions)
        {
            //coordinates.X = playerCoordinates.X - playerPosition.X + position.X;
            //coordinates.Y = playerCoordinates.Y - playerPosition.Y + position.Y;
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
            if (objectName == "RedKey")
            {
                isItem = true;
                itemType = ItemList.RedKey;

            }
            else
            {
                isItem = false;
                itemType = ItemList.NullItem;
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

        /// <summary>
        /// Despawns the object and then removes it from memory by calling the Dispose method.
        /// </summary>
        public virtual void Despawn()
        {
            isSpawned = false;
        }

        /// <summary>
        /// Deals with object interactions between each other.
        /// </summary>
        /// <param name="otherObject">The other object to interact with.</param>
        public virtual void InteractWith(GameObject otherObject, InteractionTypes interactionType)
        {
            // TODO: Add the interaction chart for each possible object interaction.
            // The owner of this method is considered the "initiator" of the interaction
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
                else
                    return new Rectangle(0, 0, 0, 0);
            }
        }
    }
}