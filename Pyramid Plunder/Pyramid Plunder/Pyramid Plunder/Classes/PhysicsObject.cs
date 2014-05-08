using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pyramid_Plunder.Classes
{
    class PhysicsObject : GameObject
    {
        public enum Alignments : byte
        {
            Friendly = 0,
            Enemy = 1,
            Neutral = 2
        }

        private enum CollisionPoints : byte
        {
            Right = 0,
            Left = 1,
            Top = 2,
            Bottom = 3,
            TopRight = 4,
            TopLeft = 5,
            BotRight = 6,
            BotLeft = 7
        }

        public const int MAX_FALLING_SPEED = 0;

        private int VelocityX;
        private int VelocityY;
        private bool isOnGround;
        private bool isGravityAffected;
        private Alignments alignment;
        private int maxHealth;
        private int damage;
        private float armor;
        private int movementSpeed;
        private Vector2[] collisionPointsArray;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        public PhysicsObject(GameObjectList objType) : base(objType)
        {

        }

        public void Update(GameTime time)
        {

        }

        private bool checkGround(Room room)
        {
            //TODO: Actualy check for the ground
            return false;
        }

        /// <summary>
        /// Loads in the object's data from the data file associated with that object
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        private void LoadObjectData(GameObjectList objType)
        {
            // TODO: Determine which file to open and load in the object data
        }

        //TODO: Add appropriate public properties to access members.  If it shouldn't be written to from outside the class, it should be Read-Only!
    }
}
