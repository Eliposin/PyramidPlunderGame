using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pyramid_Plunder.Classes
{
    public class GameObject : GameGraphic
    {
        private GameObjectList objectType;
        private Vector2 position;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        /// <param name="spawnPosition">The default spawning position.</param>
        public GameObject(GameObjectList objType, Vector2 spawnPosition) : base(objType)
        {
            objectType = objType;
            position = spawnPosition;
        }

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        public GameObject(GameObjectList objType) : base(objType)
        {
            objectType = objType;
            position = new Vector2(0, 0);
        }

        /// <summary>
        /// Updates the GameObject's coordinates based on its position and the location of the player in the room
        /// </summary>
        /// <param name="player">The player to reference</param>
        public void UpdateCoordinates(Vector2 playerPosition, Vector2 playerCoordinates)
        {
            coordinates.X = playerCoordinates.X - playerPosition.X;
            coordinates.Y = playerCoordinates.Y - playerPosition.Y;
        }

        /// <summary>
        /// Deals with object interactions between each other.
        /// </summary>
        /// <param name="otherObject">The other object to interact with.</param>
        public void Interact(GameObject otherObject)
        {

        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
    }
}
