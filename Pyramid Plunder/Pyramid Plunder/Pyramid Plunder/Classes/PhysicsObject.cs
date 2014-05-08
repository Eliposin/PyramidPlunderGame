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

        public PhysicsObject(String filepath, GameObjectList objType) : base(filepath, objType)
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

        //TODO: Add appropriate public properties to access members.  If it shouldn't be written to from outside the class, it should be Read-Only!
    }
}
