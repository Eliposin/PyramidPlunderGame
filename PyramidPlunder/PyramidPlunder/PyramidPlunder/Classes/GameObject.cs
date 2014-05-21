﻿using System;
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
        protected bool isSpawned;
        protected bool isUpgrade;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        /// <param name="spawnPosition">The default spawning position.</param>
        public GameObject(GameObjectList objType, ContentManager content, Vector2 spawnPosition) : base(objType, content)
        {
            objectType = objType;
            position = spawnPosition;
            Initialize();
        }

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        public GameObject(GameObjectList objType, ContentManager content)
            : base(objType, content)
        {
            objectType = objType;
            position = new Vector2(0, 0);
            Initialize();
        }

        /// <summary>
        /// Updates the GameObject's coordinates based on its position and the location of the player in the room
        /// </summary>
        /// <param name="player">The player to reference</param>
        public void UpdateCoordinates(Vector2 playerPosition, Vector2 playerCoordinates)
        {
            coordinates.X = playerCoordinates.X - playerPosition.X + position.X;
            coordinates.Y = playerCoordinates.Y - playerPosition.Y + position.Y;
        }

        /// <summary>
        /// Sets up the object based on its objectType
        /// </summary>
        protected virtual void Initialize()
        {
            switch (objectType)
            {
                case GameObjectList.Dash:
                    isUpgrade = true;
                    break;
                case GameObjectList.DoubleJump:
                    isUpgrade = true;
                    break;
                default:
                    isUpgrade = false;
                    break;
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
            Dispose();
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
        /// Whether the object is an upgrade
        /// </summary>
        public bool IsUpgrade
        {
            get { return isUpgrade; }
        }
    }
}
