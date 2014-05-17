using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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

        protected float velocityX;
        protected float velocityY;
        protected float displacementX;
        protected float displacementY;

        protected bool isGravityAffected;
        protected bool isOnGround;
        protected bool wallOnLeft;
        protected bool wallOnRight;
        protected bool isSpawned;
        protected Alignments alignment;
        protected int maxHealth;
        protected int damage;
        protected float armor;
        protected int movementSpeed;
        protected Vector2[] collisionPointsArray;
        protected short[] collisionXs;
        protected short[] collisionYs;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        /// <param name="spawnPosition">The default starting position.</param>
        public PhysicsObject(GameObjectList objType, ContentManager content)
            : base(objType, content)
        {
            velocityX = 0;
            velocityY = 0;
        }

        /// <summary>
        /// Overrides the draw method in order to add the clause that checks to see if the object is spawned yet.
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw to.</param>
        public override void Draw(SpriteBatch batch, GameTime time)
        {
            if (isSpawned)
                base.Draw(batch, time);
        }

        /// <summary>
        /// Determines this frame's velocity increment based on the elapsed game time.
        /// </summary>
        /// <param name="time">The elapsed game time.</param>
        public virtual void Update(GameTime time)
        {
            //velocityX *= time.ElapsedGameTime.Seconds;
            //velocityY *= time.ElapsedGameTime.Seconds;
        }

        /// <summary>
        /// Actually moves the object and changes the Position based on the Velocity values
        /// </summary>
        public void Move()
        {
            base.Position = new Vector2(Position.X + displacementX, Position.Y + displacementY);
            displacementX = 0;
            displacementY = 0;
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
        public bool checkGround(Room room, int dX = 0, int dY = 0)
        {
            int intY = collisionYs.Last() + 1;
            if (Position.Y + dY + intY < 0 || Position.Y + dY + intY >= room.CollisionMap.Height)
                return false;
            foreach (int intX in collisionXs)
            {
                if (room.collisionColors[(int)(Position.X + dX + intX + (Position.Y + dY + intY) * room.CollisionMap.Width)].R == 0)
                    return true;
            }
            return false;
        }

        public bool checkWallRight(Room room)
        {
            int intX = collisionXs.Last() + 1;
            if (Position.X + intX >= room.CollisionMap.Width)
                return false;
            foreach (int intY in collisionYs)
            {
                if (Position.Y + intY >= 0 && Position.Y + intY < room.CollisionMap.Height &&
                    room.collisionColors[(int)(Position.X + intX + (Position.Y + intY) * room.CollisionMap.Width)].R == 0)
                    return true;
            }
            return false;
        }

        public bool checkWallLeft(Room room)
        {
            int intX = collisionXs.First() - 1;
            if (Position.X + intX < 0)
                return false;
            foreach (int intY in collisionYs)
            {
                if (Position.Y + intY >= 0 && Position.Y + intY < room.CollisionMap.Height &&
                    room.collisionColors[(int)(Position.X + intX + (Position.Y + intY) * room.CollisionMap.Width)].R == 0)
                    return true;
            }
            return false;
        }

        public bool isStuckAt(Room room, int dX = 0, int dY = 0)
        {
            if (Position.X + dX + collisionXs.First() < 0 ||
                Position.X + dX + collisionXs.Last() >= room.CollisionMap.Width)
                return true;
            foreach (int intY in collisionYs)
            {
                if (Position.Y + dY + intY >= 0 && Position.Y + dY + intY < room.CollisionMap.Height)
                {
                    foreach (int intX in collisionXs)
                    {
                        if (room.collisionColors[(int)(Position.X + dX + intX + (Position.Y + dY + intY) * room.CollisionMap.Width)].R == 0)
                            return true;
                    }
                }
            }
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

        public virtual void Land()
        {
            isOnGround = true;
            velocityY = 0;
        }

        public virtual void HitWall()
        {
            velocityX = 0;
        }

        public virtual void HitCeiling()
        {
            velocityY = 0;
        }

        public virtual void BecomeAirborne()
        {
            isOnGround = false;
        }

        public float VelocityY
        {
            get { return velocityY; }
            set { velocityY = value; }
        }

        public float DisplacementX
        {
            get { return displacementX; }
            set { displacementX = value; }
        }

        public float DisplacementY
        {
            get { return displacementY; }
            set { displacementY = value; }
        }

        public bool IsGravityAffected
        {
            get { return isGravityAffected; }
        }

        public bool IsOnGround
        {
            get { return isOnGround; }
        }

        public bool WallOnLeft
        {
            get { return wallOnLeft; }
            set { wallOnLeft = value; }
        }

        public bool WallOnRight
        {
            get { return wallOnRight; }
            set { wallOnRight = value; }
        }

        //TODO: Add appropriate public properties to access members.  If it shouldn't be written to from outside the class, it should be Read-Only!
    }
}
