using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Pyramid_Plunder.Classes
{
    public class Enemy : PhysicsObject
    {
        protected bool isChasingPlayer = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="objType"></param>
        public Enemy(string objName, ContentManager content)
            : base(objName, content)
        {
            LoadEnemyData();
            
        }

        private void LoadEnemyData()
        {

        }

        public override bool HasInteraction(InteractionTypes interactionType)
        {
            return false;
        }

        public void Update(GameTime time, Player player)
        {
            switch (objectName)
            {
                case "Mummy":
                    {
                        if ((Math.Abs(player.Position.X + player.CollisionXs.First() - position.X + collisionXs.Last()) < 450) ||
                        ((Math.Abs(player.Position.X + player.CollisionXs.Last() - position.X + collisionXs.First()) < 450)))
                            isChasingPlayer = true;
                        else if (isChasingPlayer)
                        {
                            isChasingPlayer = false;
                            velocityX /= 2;
                        }
                        
                        if (isChasingPlayer)
                        {
                            if (isOnGround)
                            {
                                if (player.Position.X + player.CollisionXs.First() > position.X + collisionXs.Last())
                                    velocityX = movementSpeed;
                                else if (player.Position.X + player.CollisionXs.Last() < position.X + collisionXs.First())
                                    velocityX = -movementSpeed;
                                else
                                    velocityX = 0;

                                if ((wallOnLeft && velocityX < 0) || (wallOnRight && velocityX > 0))
                                    velocityY = -900;
                            }
                        }
                        else if (velocityX == 0)
                        {
                            velocityX = -movementSpeed / 2;
                        }
                    }
                    break;
                default:
                    break;
            }
            base.Update(time);
        }

        public override void CollideX()
        {
            if (!isChasingPlayer)
                velocityX *= -1;
        }
    }
}
