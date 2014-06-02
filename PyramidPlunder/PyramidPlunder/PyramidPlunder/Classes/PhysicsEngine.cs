using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pyramid_Plunder.Classes
{
    static class PhysicsEngine
    {
        //Variable GameTime
        public const float GRAVITY = 4320f;         //acceleration due to gravity, in Pixels/sec/sec.
        public const int MAX_FALLING_SPEED = 3000;  //Maximum falling velocity obtainable, in Pixels/sec.
        
        /// <summary>
        /// Tests to see if an object's intended x- and y-displacements are fully achievable in its
        /// current room and position. Causes a collision and modifies the displacement value if
        /// it would send the object into a collideable surface.
        /// </summary>
        /// <param name="obj">The object to be evaluated.</param>
        /// <param name="room">The room in which the object is located and trying to move.</param>
        /// <param name="time">The amount of time since the last frame.</param>
        public static void Update(PhysicsObject obj, Room room, GameTime time)
        {
            float totalTime = (float)(time.ElapsedGameTime.TotalSeconds);
            
            //All ungrounded, gravity-affected objects will have the displacement caused by
            //gravity in this frame added to their y-displacement.
            if (obj.IsGravityAffected && !obj.IsOnGround)
                obj.DisplacementY = Math.Min(obj.DisplacementY + (GRAVITY / 2) * (totalTime * totalTime), MAX_FALLING_SPEED * totalTime);
            
            obj.DisplacementX = (float)((int)(obj.DisplacementX));
            obj.DisplacementY = (float)((int)(obj.DisplacementY));

            //If the object will get stuck trying to move the originally intended amount, a collision
            //has occurred. The appropriate velocity & acceleration variables as well as boolean flags
            //will be reset, by calling the appropriate collision function (HitCeiling(), Land(), or
            //HitWall()).
            //Then the intended displacement is decreased by one until the object finds a coordinate
            //at which it will no longer be stuck.
            if (obj.DisplacementY < 0)
            {
                if (obj.IsStuck(room, 0, (int)obj.DisplacementY))
                {
                    obj.HitCeiling();
                    obj.DisplacementY += 1;
                    while (obj.DisplacementY != 0 && obj.IsStuck(room, 0, (int)obj.DisplacementY))
                        obj.DisplacementY += 1;
                }
            }
            else if (obj.DisplacementY > 0)
            {
                if (obj.IsStuck(room, 0, (int)obj.DisplacementY))
                {
                    obj.Land();
                    obj.DisplacementY -= 1;
                    while (obj.DisplacementY != 0 && obj.IsStuck(room, 0, (int)obj.DisplacementY))
                        obj.DisplacementY -= 1;
                }
            }

            if (obj.DisplacementX > 0)
            {
                if (obj.IsStuck(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                {
                    obj.CollideRight();
                    obj.DisplacementX -= 1;
                    while (obj.DisplacementX != 0 && obj.IsStuck(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                        obj.DisplacementX -= 1;
                }
            }

            if (obj.DisplacementX < 0)
            {
                if (obj.IsStuck(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                {
                    obj.CollideLeft();
                    obj.DisplacementX += 1;
                    while (obj.DisplacementX != 0 && obj.IsStuck(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                        obj.DisplacementX += 1;
                }
            }

            //Even if a displacement value goes unchanged in the above test, it's still possible that the object
            //will (not) have a wall beside or ground beneath them after moving their full displacements. This last
            //test checks to see if this is the case, and changes the appropriate boolean flags accordingly.
            //If the object is touching a floor in its new position, the appropriate collision-causing function
            //is called.
            //If an object was airborne at the beginning of the frame and is still airborne, its y-velocity
            //is increased by the acceleration due to gravity.
            //If it's determined that the object is now in the air, it will become airborne.

            if (!obj.WallOnLeft)
            {
                if (obj.CheckWallLeft(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                    obj.CollideLeft();
            }
            else
            {
                if (!obj.CheckWallLeft(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                    obj.WallOnLeft = false;
            }

            if (!obj.WallOnRight)
            {
                if (obj.CheckWallRight(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                    obj.CollideRight();
            }
            else
            {
                if (!obj.CheckWallRight(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                    obj.WallOnRight = false;
            }

            if (!obj.IsOnGround)
            {
                if (obj.CheckGround(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                    obj.Land();
                else if (obj.IsGravityAffected)
                    obj.VelocityY = Math.Min(obj.VelocityY + GRAVITY * totalTime, MAX_FALLING_SPEED);
            }
            else
            {
                if (!obj.CheckGround(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                    obj.LeaveGround();
            }

            if (!obj.CeilingAbove)
            {
                if (obj.CheckCeiling(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                    obj.HitCeiling();
            }
            else
            {
                if (!obj.CheckCeiling(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                    obj.LeaveCeiling();
            }
        }

        /// <summary>
        /// Checks two GameObject's to see if they collide.
        /// </summary>
        /// <param name="obj1">The first object.</param>
        /// <param name="obj2">The second object.</param>
        /// <returns>True if there is a collision.  False otherwise.</returns>
        public static bool CheckBoundingBoxCollision(GameObject obj1, GameObject obj2)
        {
            if (((obj1.HitBox.X + obj1.HitBox.Width >= obj2.HitBox.X) && (obj1.HitBox.X <= obj2.HitBox.X + obj2.HitBox.Width)) &&
                ((obj1.HitBox.Y + obj1.HitBox.Height >= obj2.HitBox.Y) && (obj1.HitBox.Y <= obj2.HitBox.Y + obj2.HitBox.Height)))
                return true;

            return false;
        }
    }
}
