using System;
using System.IO;
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
        
        //These are the only variables that need to be changed or set if the object
        //moves without acceleration.
        protected float velocityX;          //The current x-velocity, in Pixels/sec.
        protected float velocityY;          //The current y-velocity, in Pixels/sec.
        
        //These next four variables are related to gradual increases/decreases to velocity.
        //See the notes below that describe their implementation.
        protected float velocityLimitX;     //The max/min x-velocity obtainable in the current action, in P/sec.
        protected float velocityLimitY;     //The max/min y-velocity obtainable in the current action, in P/sec.
        protected float accelerationX;      //The current x-acceleration, in P/sec/sec.
        protected float accelerationY;      //The current y-acceleration, in P/sec/sec.

        //These variables should never be altered outside of PhysicsObject.Update, PhysicsObject.Move, or
        //PhysicsEngine.Update.
        private float displacementX;        //The amount by which to move in the x-direction this frame, in P.
        private float displacementY;        //The amount by which to move in the y-direction this frame, in P.

        protected int damage;               //How much damage has been taken.
        protected int currentHealth;        //-1 if the object is inanimate or invulnerable.
        
        //These 3 variables should never be altered outside of PhysicsObject.Land() or PhysicsEngine.Update().
        protected bool isOnGround;          //Whether or not the object is on the ground.
        protected bool ceilingAbove;        //Whether or not there is an obstruction above the object.
        protected bool wallOnLeft;          //Whether or not there is an obstruction to the left.
        protected bool wallOnRight;         //Whether or not there is an obstruction to the right.

        //This can be kept at one constant value, or it can be altered under certain conditions if you
        //see fit. The player currently does this for mid-air dashing.
        protected bool isGravityAffected;   //Whether or not being ungrounded will cause downward movement.
        protected bool movesOffEdges;
        protected bool sticksToSurfaces;
        protected bool isEnemy;
        
        protected Alignments alignment;     //Is this a friend, enemy or neutral party to the player?
        protected int maxHealth;            //The most health points the object can hold at one time.
        protected float armor;              //The amount or percentage by which to reduce damage from attacks.
        protected int movementSpeed;        //If an object only ever moves at one horizontal speed,
                                            //here's where to store it.
        protected int interactionDistance;
   
        //These next two arrays of shorts contain all the x- and y-coordinates of the object's
        //collision points. See the notes below that describe their implementation.
        protected short[] collisionXs;
        protected short[] collisionYs;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        /// <param name="content">The content manager to load assets to.</param>
        public PhysicsObject(string objName, ContentManager content)
            : this(objName, content, new Vector2(0, 0), true, null, false) { }

        public PhysicsObject(string objName, ContentManager content, Vector2 spawnPosition, bool graphics, AudioEngine audioEngine, bool enemy)
            : base (objName, content, spawnPosition, graphics, audioEngine)
        {
            CheckObjectType();
            if (isPhysicsObject)
            {
                velocityX = 0;
                velocityY = 0;
                accelerationX = 0;
                accelerationY = 0;
                velocityLimitX = 0;
                velocityLimitY = 0;
                displacementX = 0;
                displacementY = 0;
                isOnGround = false;
                currentHealth = maxHealth;
                LoadObjectData();
            }
        }
                
        /// <summary>
        /// Loads in the object's data from the data file associated with that object
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        private void LoadObjectData()
        {
            try
            {
                using (Stream stream = TitleContainer.OpenStream("Data/PhysicsObjectData/" + objectName + ".txt"))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        String line = GameResources.getNextDataLine(sr, "#");
                        isGravityAffected = Convert.ToBoolean(int.Parse(line));

                        line = GameResources.getNextDataLine(sr, "#");
                        movesOffEdges = Convert.ToBoolean(int.Parse(line));

                        line = GameResources.getNextDataLine(sr, "#");
                        sticksToSurfaces = Convert.ToBoolean(int.Parse(line));

                        line = GameResources.getNextDataLine(sr, "#");
                        alignment = (Alignments)int.Parse(line);

                        line = GameResources.getNextDataLine(sr, "#");
                        maxHealth = int.Parse(line);

                        currentHealth = maxHealth;

                        line = GameResources.getNextDataLine(sr, "#");
                        armor = float.Parse(line);

                        line = GameResources.getNextDataLine(sr, "#");
                        movementSpeed = int.Parse(line);

                        line = GameResources.getNextDataLine(sr, "#");
                        interactionDistance = int.Parse(line);

                        line = GameResources.getNextDataLine(sr, "#");
                        string[] numbers = line.Split(' ');
                        collisionXs = new short[numbers.Count()];
                        for (int i = 0; i < numbers.Count(); i++)
                            collisionXs[i] = short.Parse(numbers[i]);

                        line = GameResources.getNextDataLine(sr, "#");
                        numbers = line.Split(' ');
                        collisionYs = new short[numbers.Count()];
                        for (int i = 0; i < numbers.Count(); i++)
                            collisionYs[i] = short.Parse(numbers[i]);

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
                isGravityAffected = false;
                movesOffEdges = false;
                alignment = 0;
                maxHealth = 0;
                armor = 0;
                movementSpeed = 0;
                interactionDistance = 0;
                collisionXs = new short[1] { 0 };
                collisionYs = new short[1] { 0 };
            }
        }

        private void CheckObjectType()
        {
            if (isEnemy)
            {
                isPhysicsObject = true;
            }
            else
            {
                switch (objectName)
                {
                    case "FallingSpikePlatform":
                    case "Player":
                    case "FloorThing":
                    case "Mummy":
                    case "WallThing":
                        isPhysicsObject = true;
                        break;
                    default:
                        isPhysicsObject = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the object's velocity in the y-direction.
        /// </summary>
        // So far the only method that calls this is PhysicsEngine.Update().
        // It calls it to set and get the velocityY variable when the object
        // is affected by gravity.
        // The only other time I imagine another method may want to call this
        // property is if another object with AI wants to determine what this
        // object's vertical velocity is. Otherwise, not many other methods
        // should call this.
        public float VelocityY
        {
            get { return velocityY; }
            set { velocityY = value; }
        }

        public float VelocityX
        {
            get { return velocityX; }
            set { velocityX = value; }
        }

        /// <summary>
        /// Gets or sets the amount by which the object will move in the
        /// x-direction during the current frame.
        /// </summary>
        // So far the only method that should call this is PhysicsEngine.Update().
        // It calls it to set and get the displacementX variable.
        // The only other time I imagine another method may want to call this
        // property is if another object with AI wants to determine if, when, and
        // how much this object is moving during a frame. Otherwise, not many other
        // methods should call this.
        public float DisplacementX
        {
            get { return displacementX; }
            set { displacementX = value; }
        }

        /// <summary>
        /// Gets or sets the amount by which the object will move in the
        /// y-direction during the current frame.
        /// </summary>
        // So far the only method that should call this is PhysicsEngine.Update().
        // It calls it to set and get the displacementY variable.
        // The only other time I imagine another method may want to call this
        // property is if another object with AI wants to determine if, when, and
        // how much this object is moving during a frame. Otherwise, not many other
        // methods should call this.
        public float DisplacementY
        {
            get { return displacementY; }
            set { displacementY = value; }
        }

        /// <summary>
        /// Returns the flag which keeps track of whether the object is affected by
        /// gravity.
        /// </summary>
        // So far the only method that should call this is PhysicsEngine.Update().
        // It calls it to access the isGravityAffected flag and determine if gravity
        // should be applied to the object.
        // There should be little to no need to call this method when defining a
        // new method.
        public bool IsGravityAffected
        {
            get { return isGravityAffected; }
            set { isGravityAffected = value; }
        }

        public bool MovesOffEdges
        {
            get { return movesOffEdges; }
        }

        public bool SticksToSurfaces
        {
            get { return sticksToSurfaces; }
        }

        /// <summary>
        /// Returns the flag which keeps track of whether the object is grounded.
        /// </summary>
        // So far the only method that should call this is PhysicsEngine.Update().
        // It calls it to access the isOnGround flag and determine if gravity should
        // be applied to the object.
        // The only other time I imagine another method may want to call this
        // property is if another object with AI wants to determine if this object
        // is currently on the ground. Otherwise, not many other methods should call this.
        public bool IsOnGround
        {
            get { return isOnGround; }
            set { isOnGround = value; }
        }

        public bool CeilingAbove
        {
            get { return ceilingAbove; }
        }

        /// <summary>
        /// Gets or sets the flag which keeps track of whether the object is touching
        /// a wall on its left side.
        /// </summary>
        // So far the only method that should call this is PhysicsEngine.Update().
        // It calls it to access and set the wallOnLeft flag accordingly.
        // The only other time I imagine another method may want to call this
        // property is if another object with AI wants to determine if this object
        // is currently up against a wall. Otherwise, not many other methods should
        // call this.
        public bool WallOnLeft
        {
            get { return wallOnLeft; }
            set { wallOnLeft = value; }
        }

        /// <summary>
        /// Gets or sets the flag which keeps track of whether the object is touching
        /// a wall on its right side.
        /// </summary>
        // So far the only method that should call this is PhysicsEngine.Update().
        // It calls it to access and set the wallOnRight flag accordingly.
        // The only other time I imagine another method may want to call this
        // property is if another object with AI wants to determine if this object
        // is currently up against a wall. Otherwise, not many other methods should
        // call this.
        public bool WallOnRight
        {
            get { return wallOnRight; }
            set { wallOnRight = value; }
        }

        public short[] CollisionXs
        {
            get { return collisionXs; }
        }

        public short[] CollisionYs
        {
            get { return collisionYs; }
        }

        /// <summary>
        /// Determines this frame's intended x- and y- displacements based on 
        /// the elapsed game time and current velocities, as well as current
        /// accelerations and maximum velocities if acceleration is nonzero.
        /// </summary>
        /// <param name="time">The elapsed game time.</param>

        public override void Update(GameTime time)
        {
            if (isSpawned)
            {
                float totalTime = (float)(time.ElapsedGameTime.TotalSeconds);

                //If you set your object's acceleration to 0, then displacement is simply
                //determined by velocity * time. As long as you keep acceleration to zero,
                //the next two else if blocks involving acceleration and velocityLimits
                //will be ignored.
                if (accelerationX == 0)
                {
                    displacementX = velocityX * totalTime;
                }
                //If your object's acceleration is nonzero, there's a little more math involved.
                //The displacement caused by acceleration is added to the above formula, but only
                //if it does not pass the limits on velocity you have set. Otherwise, displacement
                //becomes equal to the limit velocity * time.
                //After the displacement for this frame is determined, the velocity for the
                //upcoming frame is set to either the sum of the current velocity and
                //acceleration * time, or to the velocity limit. The value chosen is the one
                //with the smaller absolute value, as a way to prevent the object from moving
                //faster than you want it to.
                else if (accelerationX > 0)
                {
                    displacementX = totalTime * Math.Min(velocityX + (accelerationX / 2) * totalTime, velocityLimitX);
                    velocityX = Math.Min(velocityX + accelerationX * totalTime, velocityLimitX);
                }
                else //if (accelerationX < 0)
                {
                    displacementX = totalTime * Math.Max(velocityX + (accelerationX / 2) * totalTime, velocityLimitX);
                    velocityX = Math.Max(velocityX + accelerationX * totalTime, velocityLimitX);
                }

                //Same description as above, but this time for the Y direction.
                if (accelerationY == 0)
                {
                    displacementY = velocityY * totalTime;
                }
                else if (accelerationY > 0)
                {
                    displacementY = totalTime * Math.Min(velocityY + (accelerationY / 2) * totalTime, velocityLimitY);
                    velocityY = Math.Min(velocityY + accelerationY * totalTime, velocityLimitY);
                }
                else //if (accelerationY < 0)
                {
                    if (accelerationY + PhysicsEngine.GRAVITY <= 0)
                    {
                        displacementY = totalTime * Math.Max(velocityY + (accelerationY / 2) * totalTime, velocityLimitY);
                        velocityY = Math.Max(velocityY + accelerationY * totalTime, velocityLimitY);
                    }
                    else
                    {
                        displacementY = totalTime * Math.Min(velocityY + (accelerationY / 2) * totalTime, velocityLimitY);
                        velocityY = Math.Min(velocityY + accelerationY * totalTime, velocityLimitY);
                    }
                }
            }

            base.Update(time);
        }

        /// <summary>
        /// Changes all of the object's relevant properties to reflect that
        /// the bottom side of its hitbox has collided with a ground.
        /// </summary>
        // For many objects, this just means to set velocityY and
        // accelerationY to zero, and isOnGround to true.
        // But for some objects, like the player class, more variables need
        // to change based on the object's current circumstances. In that
        // case, this function may be overridden so that all relevant
        // variables are set to the appropriate values.
        // 
        // So far the only method that should call this is PhysicsEngine.Update().
        // It calls it when it detects that a vertically-moving object has
        // collided with a ground.
        // There should be little to no need to call this method when defining a
        // new method.
        public virtual void Land()
        {
            isOnGround = true;
            velocityY = 0;
            accelerationY = 0;
        }

        /// <summary>
        /// Changes all of the object's relevant properties to reflect that
        /// the left or right side of its hitbox has collided with a wall
        /// or a horizontal edge of the map.
        /// </summary>
        // For many objects, this just means to set velocityX and
        // accelerationX to zero.
        // But for some objects, like the player class, more variables need
        // to change based on the object's current circumstances. In that
        // case, this function may be overridden so that all relevant
        // variables are set to the appropriate values.
        // 
        // So far the only method that should call this is PhysicsEngine.Update().
        // It calls it when it detects that a horizontally-moving object has
        // collided with a ceiling.
        // There should be little to no need to call this method when defining a
        // new method.
        public virtual void CollideX()
        {
            velocityX = 0;
            accelerationX = 0;
        }

        public void CollideLeft()
        {
            CollideX();
            wallOnLeft = true;
        }

        public void CollideRight()
        {
            CollideX();
            wallOnRight = true;
        }

        /// <summary>
        /// Changes all of the object's relevant properties to reflect that
        /// the top end of its hitbox has collided with a ceiling.
        /// </summary>
        // For many objects, this just means to set velocityY and
        // accelerationY to zero.
        // But for some objects, like the player class, more variables need
        // to change based on the object's current circumstances. In that
        // case, this function may be overridden so that all relevant
        // variables are set to the appropriate values.
        // 
        // So far the only method that should call this is PhysicsEngine.Update().
        // It calls it when it detects that an upward moving object has collided
        // with a ceiling.
        // There should be little to no need to call this method when defining a
        // new method.
        public virtual void HitCeiling()
        {
            ceilingAbove = true;
            velocityY = 0;
            accelerationY = 0;
        }

        /// <summary>
        /// Changes all of the object's relevant properties to reflect that
        /// it is now airborne (i.e. not on ground).
        /// </summary>
        // For many objects, this just means to set isOnGround to false.
        // But for some objects, like the player class, more variables need
        // to change based on the object's current circumstances. In that
        // case, this function may be overridden so that all relevant
        // variables are set to the appropriate values.
        // 
        // So far the only method that should call this is PhysicsEngine.Update().
        // It calls it to change an object to "airborne mode" when isOnGround == true
        // but it detects that there is no ground beneath said object.
        // There should be little to no need to call this method when defining a
        // new method.
        public virtual void LeaveGround()
        {
            isOnGround = false;
        }

        public virtual void LeaveCeiling()
        {
            ceilingAbove = false;
        }
        
        /// <summary>
        /// Checks to see if if there is ground beneath the object at its current coordinates,
        /// OR if there *will be* ground beneath the object if its coordinates are adjusted
        /// by specified amounts in the x and/or y directions.
        /// Non-zero values may be input for dX and/or dY to test for ground at the coordinates
        /// equal to the object's coordinates plus the vector (dX, dY).
        /// </summary>
        /// <param name="room">The room in which to test for ground</param>
        /// <param name="dX">The value by which to adjust the x-coordinate in the test</param>
        /// <param name="dY">The value by which to adjust the y-coordinate in the test</param>
        /// <returns></returns>
        // This method is only called by PhysicsEngine.Update(). If you need to see if this
        // object is on the ground, use the IsOnGround property or even the isOnGround member
        // bool for methods inside this class.
        public virtual bool CheckGround(Room room, int dX, int dY)
        {
            //row is the y-coordinate just beneath the object's hitbox at its
            //(adjusted) coordinates.
            int row = (int)Position.Y + dY + collisionYs.Last() + 1;
            //All space above or below the map is not considered ground.
            if (row < 0 || row >= room.CollisionMap.Height)
                return false;
            //If the row is on the collision map, do a color test on
            //all collision points in the row:
            int coordinateX = (int)Position.X + dX;
            foreach (int intX in collisionXs)
            {
                //Walls/grounds/ceilings are black on the collision map, i.e. the R value is 0
                if (room.collisionColors[(int)(coordinateX + intX + row * room.CollisionMap.Width)].R == 0)
                    return true;

                
            }
            //If none of the collision points are in a black region, there is no ground.

            foreach (GameObject obj in room.ObjectArray)
            {
                if (obj.IsSolid && obj.IsSpawned)
                {
                    if ((coordinateX + collisionXs.First() <= obj.Position.X + obj.HitBox.Width) &&
                        (coordinateX + collisionXs.Last() >= obj.Position.X) &&
                        (row >= obj.Position.Y) && (row <= obj.Position.Y + obj.HitBox.Height))
                    {
                        obj.StandingOn(objectName);
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool CheckCeiling(Room room, int dX, int dY)
        {
            //row is the y-coordinate just above the object's hitbox at its
            //(adjusted) coordinates.
            int row = (int)Position.Y + dY + collisionYs.First() - 1;
            //All space above or below the map is not considered ground.
            if (row < 0 || row >= room.CollisionMap.Height)
                return false;
            //If the row is on the collision map, do a color test on
            //all collision points in the row:
            int coordinateX = (int)Position.X + dX;
            foreach (int intX in collisionXs)
            {
                //Walls/grounds/ceilings are black on the collision map, i.e. the R value is 0
                if (room.collisionColors[(int)(coordinateX + intX + row * room.CollisionMap.Width)].R == 0)
                    return true;
            }
            //If none of the collision points are in a black region, there is no ground.

            foreach (GameObject obj in room.ObjectArray)
            {
                if (obj.IsSolid && obj.IsSpawned)
                {
                    if ((coordinateX + collisionXs.First() <= obj.Position.X + obj.HitBox.Width) &&
                        (coordinateX + collisionXs.Last() >= obj.Position.X) &&
                        (row >= obj.Position.Y) && (row <= obj.Position.Y + obj.HitBox.Height))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the object will walk over a ledge if they move
        /// with their attempted horizontal displacement.
        /// </summary>
        /// <param name="room">The room the object is in.</param>
        /// <returns></returns>
        public bool WillMovePastEdge(Room room)
        {
            int column;

            if (displacementX > 0)
                column = (int)(Position.X + displacementX + collisionXs.Last());
            else if (displacementX < 0)
                column = (int)(Position.X + displacementX + collisionXs.First());
            else
                return false;

            int row = (int)(Position.Y + displacementY + collisionYs.Last() + 1);

            if (row < 0 || row >= room.CollisionMap.Height)
                return false;

            if (room.collisionColors[column + row * room.CollisionMap.Width].R == 0)
                return false;

            foreach (GameObject obj in room.ObjectArray)
            {
                if (obj.IsSolid && obj.IsSpawned)
                {
                    if ((column >= obj.Position.X) && (column <= obj.Position.X + obj.HitBox.Width) &&
                        (row >= obj.Position.Y) && (row <= obj.Position.Y + obj.HitBox.Height))
                        return false;
                }
            }
            return true;
        }

        public bool WillLoseSurface(Room room)
        {
            int column = (int)position.X;
            int row = (int)position.Y;

            if (displacementX != 0)
            {
                if (isOnGround)
                {
                    if (CheckGround(room, (int)displacementX, 0))
                        return false;
                    row += collisionYs.Last() + 1;
                }
                else if (ceilingAbove)
                {
                    if (CheckCeiling(room, (int)displacementX, 0))
                        return false;
                    row += collisionYs.First() - 1;
                }
                else
                    return false;

                column += (int)displacementX;

                if (displacementX > 0)
                    column += collisionXs.First() - 1;
                else
                    column += collisionXs.Last() + 1;
            }
            else if (displacementY != 0)
            {
                if (wallOnRight)
                {
                    if (CheckWallRight(room, 0, (int)displacementY))
                        return false;
                    column += collisionXs.Last() + 1;
                }
                else if (wallOnLeft)
                {
                    if (CheckWallLeft(room, 0, (int)displacementY))
                        return false;
                    column += collisionXs.First() - 1;
                }
                else
                    return false;

                row += (int)displacementY;

                if (displacementY > 0)
                    row += collisionXs.First() - 1;
                else
                    row += collisionXs.Last() + 1;
            }
            else
                return false;

            foreach (GameObject obj in room.ObjectArray)
            {
                if (obj.IsSolid && obj.IsSpawned)
                {
                    if ((column >= obj.Position.X) && (column <= obj.Position.X + obj.HitBox.Width) &&
                        (row >= obj.Position.Y) && (row <= obj.Position.Y + obj.HitBox.Height))
                        return false;
                }
            }

            if (room.collisionColors[column + row * room.CollisionMap.Width].R == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Checks to see that the object is fully grounded: instead of checking that at least
        /// one collision point in the bottom row is sitting on either a collision region of the
        /// map or a solid GameObject, it checks that *each* collision point in the bottom row is
        /// sitting on either a collision region or a solid GameObject.
        /// </summary>
        /// <param name="room">The room in to test for groundedness.</param>
        /// <param name="dX">The value by which to adjust the x-coordinate in the test</param>
        /// <param name="dY">The value by which to adjust the y-coordinate in the test</param>
        /// <returns></returns>
        public bool CheckFullyGrounded(Room room, int dX, int dY)
        {
            int row = (int)Position.Y + dY + collisionYs.Last() + 1;
            
            if (row < 0 || row >= room.CollisionMap.Height)
                return false;

            bool pointOnSolid = false;

            foreach (int intX in collisionXs)
            {
                int coordinateX = (int)Position.X + dX + intX;

                if (room.collisionColors[coordinateX + row * room.CollisionMap.Width].R == 0)
                    continue;

                pointOnSolid = false;
                foreach (GameObject obj in room.ObjectArray)
                {
                    if (obj.IsSolid && obj.IsSpawned)
                    {
                        if ((coordinateX >= obj.Position.X) &&
                            (coordinateX <= obj.Position.X + obj.HitBox.Width) &&
                            (row >= obj.Position.Y) && (row <= obj.Position.Y + obj.HitBox.Height))
                        {
                            pointOnSolid = true;
                            break;
                        }
                    }
                }
                if (!pointOnSolid)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks to see if if there is a wall right of the object at its current coordinates,
        /// OR if there *will be* a wall right of the object if its coordinates are adjusted
        /// by specified amounts in the x and/or y directions.
        /// Non-zero values may be input for dX and/or dY to test for a wall at the coordinates
        /// equal to the object's coordinates plus the vector (dX, dY).
        /// </summary>
        /// <param name="room">The room in which to test for a wall</param>
        /// <param name="dX">The value by which to adjust the x-coordinate in the test</param>
        /// <param name="dY">The value by which to adjust the y-coordinate in the test</param>
        /// <returns></returns>
        public virtual bool CheckWallRight(Room room, int dX, int dY)
        {
            //column is the x-coordinate just right of the object's hitbox
            //at its (adjusted) coordinates.
            int column = (int)Position.X + dX + collisionXs.Last() + 1;
            //All space to the right of the map is not considered a wall.
            if (column >= room.CollisionMap.Width)
                return false;
            //If the column is on the collision map, do a color test on
            //all collision points in the column:
            int coordinateY = (int)Position.Y + dY;
            foreach (int intY in collisionYs)
            {
                //All space in the column that is above or below the map will not be considered
                //a wall. But any collision points in the column and on the map *will* be tested.
                //Walls/grounds/ceilings are black on the collision map, i.e. the R value is 0
                if (coordinateY + intY >= 0 && coordinateY + intY < room.CollisionMap.Height &&
                    room.collisionColors[column + (coordinateY + intY) * room.CollisionMap.Width].R == 0)
                    return true;
            }

            foreach (GameObject obj in room.ObjectArray)
            {
                if (obj.IsSolid && obj.IsSpawned)
                {
                    if ((coordinateY + collisionYs.First() <= obj.Position.Y + obj.HitBox.Height) &&
                        (coordinateY + collisionYs.Last() >= obj.Position.Y) &&
                        (column >= obj.Position.X) && (column <= obj.Position.X + obj.HitBox.Width))
                        return true;
                }
            }
            //If none of the collision points are in a black region, there is no wall.
            return false;
        }

        /// <summary>
        /// Checks to see if if there is a wall left of the object at its current coordinates,
        /// OR if there *will be* a wall left of the object if its coordinates are adjusted by
        /// specified amounts in the x and/or y directions.
        /// Non-zero values may be input for dX and/or dY to test for a wall at the coordinates
        /// equal to the object's coordinates plus the vector (dX, dY).
        /// </summary>
        /// <param name="room">The room in which to test for a wall</param>
        /// <param name="dX">The value by which to adjust the x-coordinate in the test</param>
        /// <param name="dY">The value by which to adjust the y-coordinate in the test</param>
        /// <returns></returns>
        public virtual bool CheckWallLeft(Room room, int dX, int dY)
        {
            //column is the x-coordinate just left of the object's hitbox
            //at its (adjusted) coordinates.
            int column = (int)Position.X + dX + collisionXs.First() - 1;
            //All space to the left of the map is not considered a wall.
            if (column < 0)
                return false;
            //If the column is on the collision map, do a color test on
            //all collision points in the column:
            int coordinateY = (int)Position.Y + dY;
            foreach (int intY in collisionYs)
            {
                //All space in the column that is above or below the map will not be considered
                //a wall. But any collision points in the column and on the map *will* be tested.
                //Walls/grounds/ceilings are black on the collision map, i.e. the R value is 0
                if (coordinateY + intY >= 0 && coordinateY + intY < room.CollisionMap.Height &&
                    room.collisionColors[column + (coordinateY + intY) * room.CollisionMap.Width].R == 0)
                    return true;
            }

            foreach (GameObject obj in room.ObjectArray)
            {
                if (obj.IsSolid && obj.IsSpawned)
                {
                    if ((coordinateY + collisionYs.First() <= obj.Position.Y + obj.HitBox.Height) &&
                        (coordinateY + collisionYs.Last() >= obj.Position.Y) &&
                        (column >= obj.Position.X) && (column <= obj.Position.X + obj.HitBox.Width))
                        return true;
                }
            }
            //If none of the collision points are in a black region, there is no wall.
            return false;
        }
        
        /// <summary>
        /// Checks to see if an object is stuck at its current coordinates, OR if it *will be*
        /// stuck if its coordinates are adjusted by specified amounts in the x and/or y
        /// positions.
        /// Non-zero values may be input for dX and/or dY to test for stuckness at the
        /// coordinates equal to the object's coordinates plus the vector (dX, dY).
        /// </summary>
        /// <param name="room">The room in which to test for stuckness</param>
        /// <param name="dX">The value by which to adjust the x-coordinate in the test</param>
        /// <param name="dY">The value by which to adjust the y-coordinate in the test</param>
        /// <returns></returns>
        public virtual bool IsStuck(Room room, int dX, int dY)
        {
            //If dX != 0, it's added to the test x-coordinate
            int CoordinateX = (int)Position.X + dX;
            //If the leftmost collision points are past the left of the map, or
            //if the rightmost collision points are past the right of the map,
            //the object is stuck.
            if (CoordinateX + collisionXs.First() < 0 ||
                CoordinateX + collisionXs.Last() >= room.CollisionMap.Width)
                return true;
            //If dY != 0, it's added to the test y-coordinate
            int CoordinateY = (int)Position.Y + dY;
            foreach (int intY in collisionYs)
            {
                //Collision points above or below the map will not cause stuckness.
                if (CoordinateY + intY >= 0 && CoordinateY + intY < room.CollisionMap.Height)
                {
                    foreach (int intX in collisionXs)
                    {
                        //Walls/grounds/ceilings are black on the collision map, i.e. the R value is 0
                        if (room.collisionColors[CoordinateX + intX + (CoordinateY + intY) * room.CollisionMap.Width].R == 0)
                            return true;
                    
                            
                    }
                }
            }
            foreach (GameObject obj in room.ObjectArray)
            {
                if (obj.IsSolid && obj.IsSpawned)
                {
                    if ((CoordinateX + collisionXs.First() <= obj.Position.X + obj.HitBox.Width) &&
                        (CoordinateX + collisionXs.Last() >= obj.Position.X) &&
                        (CoordinateY + collisionYs.First() <= obj.Position.Y + obj.HitBox.Height) &&
                        (CoordinateY + collisionYs.Last() >= obj.Position.Y))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Actually moves the object and changes the Position based on the displacement values
        /// </summary>
        //Only the GameManager class should call this method. No other methods should.
        public void Move()
        {
            //After the physics engine sets the displacements to the amounts the object actually
            //*can* move, it adds those displacements to its current position.
            base.Position = new Vector2(Position.X + displacementX, Position.Y + displacementY);
            //Then the displacement values will start at zero for the next frame.
            displacementX = 0;
            displacementY = 0;
        }

        /// <summary>
        /// The maximum distance at which the object can interact with other objects.
        /// </summary>
        public int InteractionDistance
        {
            get { return interactionDistance; }
        }

        /// <summary>
        /// This object's maximum health.
        /// </summary>
        public int MaximumHealth
        {
            get { return maxHealth; }
        }

        /// <summary>
        /// This object's current health.
        /// </summary>
        public int CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }
    }
}
