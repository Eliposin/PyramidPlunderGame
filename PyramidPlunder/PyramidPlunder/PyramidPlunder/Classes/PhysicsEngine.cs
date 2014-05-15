using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pyramid_Plunder.Classes
{
    static class PhysicsEngine
    {
        //changed
        //public const int GRAVITY = 72;
        //public const int MAX_FALLING_SPEED = 3000;
        public const float GRAVITY = 1.2f;
        public const int MAX_FALLING_SPEED = 50;

        public static void Update(PhysicsObject obj, Room room)
        {
            if (obj.IsGravityAffected && !obj.IsOnGround)
                obj.DisplacementY = Math.Min(obj.DisplacementY + GRAVITY / 2, MAX_FALLING_SPEED);
            obj.DisplacementX = (float)Math.Truncate(obj.DisplacementX);
            obj.DisplacementY = (float)Math.Truncate(obj.DisplacementY);

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
                    obj.HitWall();
                    obj.DisplacementX += (obj.DisplacementX > 0) ? -1 : 1;
                    while (obj.isStuckAt(room, (int)obj.DisplacementX, (int)obj.DisplacementY) && obj.DisplacementX != 0)
                        obj.DisplacementX += (obj.DisplacementX > 0) ? -1 : 1;
                }
            }

            obj.WallOnLeft = obj.checkWallLeft(room);
            obj.WallOnRight = obj.checkWallRight(room);
            if (!obj.IsOnGround)
            {
                if (obj.checkGround(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                    obj.Land();
                else if (obj.IsGravityAffected)
                    obj.VelocityY = Math.Min(obj.VelocityY + GRAVITY, MAX_FALLING_SPEED);
            }
            else
            {
                if (!obj.checkGround(room, (int)obj.DisplacementX, (int)obj.DisplacementY))
                    obj.BecomeAirborne();
            }
        }
    
    }
}
