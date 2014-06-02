using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Pyramid_Plunder.Classes
{
    public class Enemy : PhysicsObject
    {
        protected int contactDamage;
        protected bool bumpsOtherEnemies;
                
        protected bool isChasingPlayer;
        protected float timer1;             //Allows enemy to keep track of time, if necessary.
        protected float timer2;             //Allows enemy to keep track of time, if necessary.
        
        /// <summary>
        /// Constructs an enemy object of the type specified.
        /// </summary>
        /// <param name="objName">The type of enemy.</param>
        /// <param name="content">The Content Manager to load from.</param>
        public Enemy(string objName, ContentManager content)
            : base(objName, content)
        {
            isChasingPlayer = false;
            LoadEnemyData();
        }

        private void LoadEnemyData()
        {
            try
            {
                using (Stream stream = TitleContainer.OpenStream("Data/EnemyData/" + objectName + ".txt"))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        String line = GameResources.getNextDataLine(sr, "#");
                        contactDamage = int.Parse(line);

                        line = GameResources.getNextDataLine(sr, "#");
                        bumpsOtherEnemies = Convert.ToBoolean(int.Parse(line));

                        sr.Close();
                    }
                }

            }
            catch (FileNotFoundException e)
            {
                System.Diagnostics.Debug.WriteLine("The file could not be found: " + e.Message);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("An error occurred: " + e.Message);
                contactDamage = 1;
                bumpsOtherEnemies = false;
            }
        }

        public override bool HasInteraction(InteractionTypes interactionType)
        {
            return false;
        }

        /// <summary>
        /// Determines the velocities with which the Enemy will attempt to move, based on
        /// its current stimuli and (possibly) based on what the Player is currently doing.
        /// </summary>
        /// <param name="time">The time that's elapsed since the last frame.</param>
        /// <param name="player">The player object the Enemy is targetting.</param>
        public void Update(GameTime time, Player player)
        {
            switch (objectName)
            {
                case "Mummy":
                    {
                        if (velocityX == 0)
                            velocityX = -movementSpeed;
                    }
                    break;
                case "WallThing":
                    {
                        if (velocityX == 0 & velocityY == 0)
                        {
                            if (isOnGround)
                                velocityX = movementSpeed;
                            else if (wallOnRight)
                                velocityY = -movementSpeed;
                            else if (ceilingAbove)
                                velocityX = -movementSpeed;
                            else //if (wallOnLeft)
                                velocityY = movementSpeed;
                        }

                        if (isOnGround)
                        {
                            if (wallOnRight)
                                velocityY = -movementSpeed;
                        }
                        else if (wallOnRight)
                        {
                            if (ceilingAbove)
                                velocityX = -movementSpeed;
                        }
                        else if (ceilingAbove)
                        {
                            if (wallOnLeft)
                                velocityY = movementSpeed;
                        }
                        else if (wallOnLeft)
                        {
                            if (isOnGround)
                                velocityX = movementSpeed;
                        }
                        else
                        {
                            if (velocityX > 0)
                            {
                                velocityX = 0;
                                velocityY = movementSpeed;
                            }
                            else if (velocityY > 0)
                            {
                                velocityX = -movementSpeed;
                                velocityY = 0;
                            }
                            else if (velocityX < 0)
                            {
                                velocityX = 0;
                                velocityY = -movementSpeed;
                            }
                            else //if (velocityY < 0)
                            {
                                velocityX = movementSpeed;
                                velocityY = 0;
                            }
                        }
                    }
                    break;
                default:
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
            }
            base.Update(time);
        }

        /// <summary>
        /// Overrides the definition for PhysicsObject.CollideX(). Enemies who collide into
        /// a wall while on ground and not chasing the Player will simply turn around.
        /// </summary>
        public override void CollideX()
        {
            if (objectName == "WallThing")
            {
                base.CollideX();
            }
            else
            {
                if (!isChasingPlayer && isOnGround)
                    velocityX *= -1;
            }
            //if (objectName != "WallThing")
            //{
            //    if (!isChasingPlayer && isOnGround)
            //        velocityX *= -1;
            //}
        }

        /// <summary>
        /// Extension of PhysicsObject.IsStuckAt() for Enemies. Adds additional check for
        /// collisions with other enemy objects and walking off ledges before the other checks.
        /// </summary>
        /// <param name="room">The room the enemy is located in.</param>
        /// <param name="dX">An optional amount by which to adjust the Enemy's x-position for the test.</param>
        /// <param name="dY">An optional amount by which to adjust the Enemy's y-position for the test.</param>
        /// <returns></returns>
        public override bool IsStuck(Room room, int dX, int dY)
        {
            if (bumpsOtherEnemies)
            {
                for (int i = 0; i < room.EnemyArray.Length; i++)
                {
                    if (!room.EnemyArray[i].BumpsOtherEnemies)
                        continue;

                    if (this == room.EnemyArray[i])
                        continue;

                    if ((position.Y + dY + collisionYs.Last() >= room.EnemyArray[i].Position.Y + room.EnemyArray[i].collisionYs.First()) &&
                        (position.Y + dY + collisionYs.First() <= room.EnemyArray[i].Position.Y + room.EnemyArray[i].collisionYs.Last()) &&
                        (position.X + dX + collisionXs.Last() >= room.EnemyArray[i].Position.X + room.EnemyArray[i].collisionXs.First()) &&
                        (position.X + dX + collisionXs.First() <= room.EnemyArray[i].Position.X + room.EnemyArray[i].collisionXs.Last()))
                        return true;
                }
            }
            
            return base.IsStuck(room, dX, dY);
        }

        public int ContactDamage
        {
            get { return contactDamage; }
        }

        public bool BumpsOtherEnemies
        {
            get { return bumpsOtherEnemies; }
        }
    }
}
