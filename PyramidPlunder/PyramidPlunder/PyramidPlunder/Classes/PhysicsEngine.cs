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
        /// current room and position. Causes a collision and modifies the displacement value of
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
            
            obj.DisplacementX = (float)Math.Truncate(obj.DisplacementX);
            obj.DisplacementY = (float)Math.Truncate(obj.DisplacementY);

            //If the object will get stuck trying to move the originally intended amount, a collision
            //has occurred. The appropriate velocity & acceleration variables as well as boolean flags
            //will be reset, by calling the appropriate collision function (HitCeiling(), Land(), or
            //HitWall()).
            //Then the intended displacement is decreased by one until the object finds a coordinate
            //at which it will no longer be stuck.
            if (obj.DisplacementY < 0)
            {
                if (obj.isStuckAt(room, 0, (int)obj.DisplacementY))
                {
                    obj.HitCeiling();
                    obj.DisplacementY += 1;
                    while (obj.isStuckAt(room, 0, (int)obj.DisplacementY) && obj.DisplacementY != 0)
                        obj.DisplacementY += 1;
                }
            }
            else if (obj.DisplacementY > 0)
            {
                if (obj.isStuckAt(room, 0, (int)obj.DisplacementY))
                {
                    obj.Land();
                    obj.DisplacementY -= 1;
                    while (obj.isStuckAt(room, 0, (int)obj.DisplacementY) && obj.DisplacementY != 0)
                        obj.DisplacementY -= 1;
                }
            }

            if (obj.DisplacementX != 0)
            {
                if (obj.isStuckAt(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                {
                    obj.CollideX();
                    obj.DisplacementX += (obj.DisplacementX > 0) ? -1 : 1;
                    while (obj.isStuckAt(room, (int)obj.DisplacementX, (int)obj.DisplacementY) && obj.DisplacementX != 0)
                        obj.DisplacementX += (obj.DisplacementX > 0) ? -1 : 1;
                }
            }

            //Even if a displacement value goes unchanged in the above test, it's still possible that the object
            //will have a wall beside or ground beneath them after moving their full displacements. This last
            //test checks to see if this is the case, and changes the appropriate boolean flags accordingly.
            //If the object is touching a floor in its new position, the appropriate collision-causing function
            //is called.
            //If an object was airborne at the beginning of the frame and is still airborne, its y-velocity
            //is increased by the acceleration due to gravity.
            //If it's determined that the object is now in the air, it will become airborne.
            obj.WallOnLeft = obj.checkWallLeft(room, (int)obj.DisplacementX, (int)obj.DisplacementY);
            obj.WallOnRight = obj.checkWallRight(room, (int)obj.DisplacementX, (int)obj.DisplacementY);
            
            if (!obj.IsOnGround)
            {
                if (obj.checkGround(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                    obj.Land();
                else if (obj.IsGravityAffected)
                    obj.VelocityY = Math.Min(obj.VelocityY + GRAVITY * totalTime, MAX_FALLING_SPEED);
            }
            else
            {
                if (!obj.checkGround(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                    obj.BecomeAirborne();
            }
        }
    
    }
}
