using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pyramid_Plunder.Classes
{
    public class PhysicsObject : GameObject
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

        protected float velocityX;
        protected float velocityY;
        protected bool isOnGround;
        protected bool isGravityAffected;
        protected bool isSpawned;
        protected Alignments alignment;
        protected int maxHealth;
        protected int damage;
        protected float armor;
        protected int movementSpeed;
        protected Vector2[] collisionPointsArray;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        /// <param name="spawnPosition">The default starting position.</param>
        public PhysicsObject(GameObjectList objType)
            : base(objType)
        {
            velocityX = 0;
            velocityY = 0;
        }

        /// <summary>
        /// Overrides the draw method in order to add the clause that checks to see if the object is spawned yet.
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw to.</param>
        public new void Draw(SpriteBatch batch)
        {
            if (isSpawned)
                base.Draw(batch);
        }

        /// <summary>
        /// Determines this frame's velocity increment based on the elapsed game time.
        /// </summary>
        /// <param name="time">The elapsed game time.</param>
        public virtual void Update(GameTime time)
        {
            velocityX *= time.ElapsedGameTime.Seconds;
            velocityY *= time.ElapsedGameTime.Seconds;
        }

        /// <summary>
        /// Causes the object to be "spawned," and therefore drawable and interactable.
        /// </summary>
        /// <param name="location"></param>
        public virtual void Spawn(Vector2 location)
        {
            isSpawned = true;
            Position = location;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
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
