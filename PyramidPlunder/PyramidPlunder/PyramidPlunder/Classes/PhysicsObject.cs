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
        //Here, P means Pixels. e.g. P/sec means Pixels per second.

        //These are the only variables that need to be changed or set if the object
        //moves without acceleration.
        protected float velocityX;          //The current x-velocity, in P/sec.
        protected float velocityY;          //The current y-velocity, in P/sec.
        
        //These next four variables are related to gradual increases/decreases to velocity.
        //See the notes below that describe their implementation.
        protected float velocityLimitX;     //The max/min x-velocity obtainable in the current action, in P/sec.
        protected float velocityLimitY;     //The max/min y-velocity obtainable in the current action, in P/sec.
        protected float accelerationX;  //The current x-acceleration, in P/sec/sec.
        protected float accelerationY;  //The current y-acceleration, in P/sec/sec.

        //These variables should never be altered outside of PhysicsObject.Update, PhysicsObject.Move, or
        //PhysicsEngine.Update.
        private float displacementX;        //The amount by which to move in the x-direction this frame, in P.
        private float displacementY;        //The amount by which to move in the y-direction this frame, in P.

        protected int damage;               //How much damage has been taken.

        //These 3 variables should never be altered outside of PhysicsObject.Land() or PhysicsEngine.Update().
        protected bool isOnGround;          //Whether or not the object is on the ground.
        protected bool wallOnLeft;          //Whether or not there is an obstruction to the left.
        protected bool wallOnRight;         //Whether or not there is an obstruction to the right.

        //This can be kept at one constant value, or it can be altered under certain conditions if you
        //see fit. The player currently does this for mid-air dashing.
        protected bool isGravityAffected;   //Whether or not being ungrounded will cause downward movement.
        
        protected Alignments alignment;     //Is this a friend, enemy or neutral party to the player?
        protected int maxHealth;            //The most health points the object can hold at one time.
                                            //0 if the object is inanimate or invulnerable.
        protected float armor;              //The amount or percentage by which to reduce damage from attacks.
        protected int movementSpeed;        //If an object only ever moves at one horizontal speed,
                                            //here's where to store it.
        protected int interactionDistance;
   
        //These next two arrays of shorts contain all the x- and y-coordinates of the object's
        //collision points. See the notes below that describe their implementation.
        protected short[] collisionXs;
        protected short[] collisionYs;

        //*******************************************************************************************
        //NOTES ON ACCELERATION AND VELOCITY-LIMIT VARIABLES:
        //*******************************************************************************************
        //
        //*****TLDR:*****
        //-If your never want your object (namely an enemy) to accelerate in the x directions,
        // always have your object's accelerationX value set to 0. Your object's overridden
        // Update() function will just consist of your object's AI determining which x-direction
        // to move in, and then setting velocityX to a value in that direction. You needn't
        // bother with velocityLimitX. If you want an enemy to run right, for example, you
        // can simply set its velocityX to a positive value. If you ever want it to start moving
        // in the opposite (left) direction, you just set its velocityX to a negative value.
        // The PhysicsEngine class will take care of its collision into walls: upon collision,
        // the PhysicsEngine class will call CollideX(), which by default resets velocityX to 0
        // However, you can override this so that it multiplies velocityX by -1, thereby making
        // the enemy turn around. Or you could even set velocityY to a negative value if you want
        // the enemy to try and jump over whatever obstacle is in its way.
        //
        //-If your never want your object (namely an enemy) to accelerate in the y directions,
        // always have your object's accelerationY value set to 0. Your object's overridden
        // Update() function will just consist of your object's AI determining which y-direction
        // to move in, and then setting velocityY to a value in that direction. You needn't
        // bother with velocityLimitY. If you want an enemy to jump, for example, it's as simple
        // as setting its velocityY variable to a negative value. The PhysicsEngine class will
        // take care of the jump arc and descent. When your enemy touches a ground again, the
        // PhysicsEngine class will call the enemy's Land() method, resetting its velocityY to 0.
        //
        //-If you *do* want acceleration in either of these directions, then it's best to
        // coordinate each new assignment to the acceleration variable with an assignment of
        // an appropriate value to the related velocityLimit variable. If you don't want to bother
        // with that, then you can keep those variables set at high absolute values that the
        // object is unlikely to reach in its environment.
        //***************
        //
        //*********************
        //THE FULL EXPLANATION:
        //*********************
        //The accelerations will change the above velocities each frame, stopping if and when
        //the velocity limits are reached.
        //
        //If you want an object to accelerate in a direction,
        //it's smart to assign a value to velocityLimit so that your object never goes over/
        //under that limit. Otherwise, your object will move dangerously crazy-fast. Unless
        //you want that, in which case you can just set velocityLimit to a very high value.
        //
        //To clarify, the accelerationY variable has nothing to do with the GRAVITY constant 
        //in the PhysicsEngine class -- accelerationY is the acceleration of the object's 
        //own *intended, controlled* motions, whereas GRAVITY is an *external* acceleration
        //applied to a gravity-affected, ungrounded object regardless of its intended motions.
        //There is no need to account for gravity with this accelerationY: that accounting will
        //be done in the Update() method of the PhysicsEngine class.
        //
        //NOTE: These 4 variables are only necessary for objects that *do* accelerate/decelerate,
        //such as the player's character. Some objects, such as the vast majority of enemies in
        //past action or platforming games, don't accelerate or decelerate; they simply move at
        //one constant horizontal magnitude, and if they collide into a formation or decide
        //to turn around then their current velocity is simply assigned a new value. If the x-
        //and/or y-movement of your object fits this description, you can simply keep that
        //acceleration variable set to 0 at all times. The update method of that object can simply
        //change its velocity values when collisions or AI requires it. If you do this, then the
        //displacement for that frame will be determined entirely by the product of the velocity
        //and the total elapsed seconds.
        //*******************************************************************************************

        //*******************************************************************************************
        //NOTES ON THE ARRAYS collisionXs AND collisionYs
        //*******************************************************************************************
        //
        //*****TLDR:*****
        //To Create collisionXs:
        //-First element is the coordinate of the left edge of the hit box.
        //-Last element is the coordinate of the right edge of the hit box.
        //-If the agreed minimum object width is I, include every Ith integer
        // after the left edge's value, stopping at the first integer to
        // equal or exceed then right edge's value.
        //
        ////To Create collisionYs:
        //-First element is the coordinate of the top edge of the hit box.
        //-Last element is the coordinate of the bottom edge of the hit box.
        //-If the minimum agreed object height is J, include every Jth integer
        // after the top edge's value, stopping at the first integer to
        // equal or exceed then bottom edge's value.
        //***************
        //
        //*********************
        //THE FULL EXPLANATION:
        //*********************
        //These arrays contain all the x- and y-coordinates, respectively, of the all object's
        //collision points.
        //
        //A collision point will be created from each possible combination of one element in
        //collisionXs and one element in collisionYs. Thus for each element in collisionXs,
        //there will be a number of collision points, equal to the number of elements in
        //collisionYs, whose x-coordinate is equal to that element from collisionXs. Thus if
        //collisionXs has m elements and collisionYs has n elements, the object will have
        //m * n collision points.
        //
        //As an example, suppose collisionXs == {0, 5, 10} and collisionYs == {0, 5, 10}.
        //Then the object has the collision points (0, 0), (0, 5), (0, 10), (5, 0), (5, 5),
        //(5, 10), (10, 0), (10, 5), and (10, 10). Checking if the object is stuck will
        //involve testing these nine points on the object's sprite.
        //
        //The first two absolutely necessary values in collisionXs are those corresponding to
        //the left and right ends of hitbox, and the first two absolutely necessary values in
        //collisionYs are the top and bottom ends of the hitbox. From these four values, the
        //four corner collision points can be obtained. But there may be some more values
        //required, as described below.
        //
        //Requiring the check of only a small handful of points is very efficient, but it has
        //certain precautions. Since this method of collison tests only a handful of spread-out
        //pixels on the object's sprite, it is possible for a skinny (or short) enough formation
        //to sneak in between two of the x (or y) values in the array without touching either one,
        //thereby causing the object to move completely through the formation with no collision
        //detected. To prevent this, the programmers will agree on a minimum width and height
        //for all collision-causing objects, floors, walls, ceilings and platforms in this game.
        //None of these may be skinnier than the chosen width or shorter than the chosen height.
        //From this we can determine a maximum horizontal space between two horizontally-adjacent
        //collision points and a maximum vertical space between two vertically-adjacent collision
        //points.
        //
        //With these maximums for horizontal and vertical space between points, we can determine
        //all other values the arrays need. Take the smallest value in the array, which represents
        //the left (or top) edge of the hit box, and keep adding to it the value for the minimum
        //width (or height) until the you get a number is greater than or equal to the largest
        //value in the array, which represents the right (or bottom) edge of the hitbox. Every
        //value obtained by this repeated addition that lies between the low and high values
        //must be included in the array.
        //
        //As an example, suppose our minimum height is 40 pixels (just as an example -- the real
        //height could be greater than this), and an object whose sprite is 90 pixels tall has the
        //top of its hitbox at y = 4 and the bottom of its hitbox at y = 86. Then the minimum
        //necessary y-coordinates in collisionYs are 4, 44, 84 and 86. No two coordinates
        //are more than 40 pixels apart, so no object or formation with a height of 40 pixels
        //or more can be inside the object's hitbox without touching a collision point with one
        //of these y-coordinates. Actually, the last value before the bottom of the hit box,
        //84 in this case, can be decreased a little if you think it's weird to have to values
        //that are only two pixels apart. You could replace 84 with something like 65, which is
        //roughly equidistant from 44 and 86. The important thing is that you just don't simply
        //omit that second-to-last element. If you did that, and had an array with only 4, 44,
        //and 86, then any obstacle that is 40 or 41 pixels tall could sneak through the 44 and
        //86 without causing a collision. Determining the values for collisionXs follows the
        //same reasoning.
        //
        //It is important that these arrays are arranged in increasing order -- or, at the very
        //least, arranged such that the smallest value is the first element in the array and
        //the largest value is the last element. That way, methods that test for walls, ground,
        //or ceilings can easily access the edges of the hitbox by using the first() and last()
        //properties in the appropriate array. It will also make geometric collision detection
        //between two objects easier, since first() and last() can be used similarly.
        //
        //Obviously, the collision points generated by these two arrays form a rectangular
        //shape, so the assumption for now is that no object has a hitbox in the shape of
        //something unusual like a rhombus. If we do get that far, then we can go back to
        //the original idea of using one array of vector2's which represents all the collision
        //points of the object. Then maybe we can store variables for left-most, right-most,
        //top-most, and bottom-most coordinates of collision points, to simplify checking for
        //ground/walls/ceilings. Or if we don't wanna do that, then we can use some functions
        //which find min or max x- or y-coordinates in an array.
        //*******************************************************************************************

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
            accelerationX = 0;
            accelerationY = 0;
            velocityLimitX = 0;
            velocityLimitY = 0;
            displacementX = 0;
            displacementY = 0;
            LoadObjectData(objType);
        }
                
        /// <summary>
        /// Loads in the object's data from the data file associated with that object
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        private void LoadObjectData(GameObjectList objType)
        {
            // TODO: Determine which file to open and load in the object data
            string filepath;
            switch (objectType)
            {
                case GameObjectList.Player:
                    filepath = "../Data/PhysicsObjectData/Player.txt";
                    break;
                default:
                    filepath = "";
                    break;
            }
            if (filepath != "" && filepath != null)
            {
                try
                {
                    StreamReader sr = new StreamReader(filepath);
                    
                    String line = GameResources.getNextDataLine(sr, "#");
                    isGravityAffected = Convert.ToBoolean(int.Parse(line));
                    
                    line = GameResources.getNextDataLine(sr, "#");
                    alignment = (Alignments)int.Parse(line);
                    
                    line = GameResources.getNextDataLine(sr, "#");
                    maxHealth = int.Parse(line);
                    
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
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("An error occurred: " + e.Message);
                    isGravityAffected = false;
                    alignment = 0;
                    maxHealth = 0;
                    armor = 0;
                    movementSpeed = 0;
                    interactionDistance = 0;
                    collisionXs = new short[1] { 0 };
                    collisionYs = new short[1] { 0 };
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("File not found for object: " + objectType);
            }
        }

        /// <summary>
        /// Overrides the draw method in order to add the clause that checks to see if the object is spawned yet.
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw to.</param>
        public new void Draw(SpriteBatch batch, GameTime time)
        {
            if (isSpawned)
                base.Draw(batch, time);
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

        /// <summary>
        /// Determines this frame's intended x- and y- displacements based on 
        /// the elapsed game time and current velocities, as well as current
        /// accelerations and maximum velocities if acceleration is nonzero.
        /// </summary>
        /// <param name="time">The elapsed game time.</param>
        // Overriding this for a derived class of the object function, such
        // as for an enemy, is pretty simple, especially if the object is
        // not meant to accelerate. In that case, the acceleration variable
        // should be set to 0 and kept at that value at all times.
        // 
        // If and when your object should move or decides it will move in a
        // particular direction, you just set the appropriate velocity variable
        // to a value in that direction (e.g. velocityX > 0 for moving right,
        // velocityX < 0 for moving left, velocityY < 0 for moving up,
        // velocityY > 0 for moving down.) The only time the velocity needs
        // to be set again is when it needs to change to a new value.
        //
        // As an example, if you want the object to jump when it is on the
        // ground and some other conditions are met, you simply create an
        // if statement based on those conditions, and inside the block the
        // velocityY variable is set to a negative value. There is no need
        // to set velocityY again until the object hits the ground, which
        // *will* happen for all gravity-affected objects thanks to the
        // PhysicsEngine class. Keeping accelerationY set at 0 will cause
        // the object to move in the exact way you want it to.
        //
        // In the event the object collides with a wall/floor/ceiling, you
        // have two options. The first is to override the CollideX(), Land(),
        // and/or HitCeiling() methods so that the velocity changes to a
        // more appropriate value. E.g. for an enemy with a constant x-
        // velocity, you can override CollideX() so that its velocityX is
        // multiplied by -1 when a wall is hit. The other option is to
        // keep the current definitions of those functions and write
        // code that handles collisions inside this Update() method. For
        // example, you can write an if statement that tests whether
        // wallOnLeft or wallOnRight is true, and if it is then you can
        // multiply velocityX by -1 inside that block.
        //
        // If and when you *do* want your object to move in a direction with
        // acceleration, the code is a little different. Instead of setting
        // the velocity variable outright to a particular value, you must set 
        // the velocityLimit variable to the value you want the velocity
        // variable to reach, and you must set the acceleration to the value
        // that yields the acceleration you want for the object. If you want
        // the object to speed up, acceleration and velocityLimit must have
        // the same sign (positive or negative). If you want the object to
        // slow down, acceleration and velocityLimit must have opposite
        // signs. The absolute value of the acceleration depends on how
        // quickly you want the object to reach this limit.
        //
        // As an example, if you want an object to reach a velocityX of 500
        // pixels per second, you set velocitLimitX to 500 and choose a
        // positive value for accelerationX. If you then want the object to
        // skid to a stop after reaching this speed, you set velocityLimitX
        // to 0 and accelerationX to a negative value.
        //
        // There is no need to calculate displacementX or displacementY. As long as
        // you set the velocity, acceleration and sometimes velocityLimit to the
        // correct values, then PhysicsObject.Update() will calculate those PROVIDED
        // YOU CALL THE BASE DEFINITION of Update() at the very end of the definition.
        // In fact:
        // 
        // All derived objects must have "base.Update(time);" as the very last line
        // in their own, overridden Update() method!!!
        //
        // Only the GameManager class should ever call this method!!!
        public virtual void Update(GameTime time)
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

        /// <summary>
        /// Changes all of the object's relevant properties to reflect that
        /// the top end of its hitbox has collided with a ceiling.
        /// 
        /// For many objects, this just means to set velocityY and
        /// accelerationY to zero.
        /// But for some objects, like the player class, more variables need
        /// to change based on the object's current circumstances. In that
        /// case, this function may be overridden so that all relevant
        /// variables are set to the appropriate values.
        /// 
        /// So far the only method that should call this is PhysicsEngine.Update().
        /// It calls it when it detects that an upward moving object has collided
        /// with a ceiling.
        /// There should be little to no need to call this method when defining a
        /// new method.
        /// </summary>
        public virtual void HitCeiling()
        {
            velocityY = 0;
            accelerationY = 0;
        }

        /// <summary>
        /// Changes all of the object's relevant properties to reflect that
        /// it is now airborne (i.e. not on ground).
        /// 
        /// For many objects, this just means to set isOnGround to false.
        /// But for some objects, like the player class, more variables need
        /// to change based on the object's current circumstances. In that
        /// case, this function may be overridden so that all relevant
        /// variables are set to the appropriate values.
        /// 
        /// So far the only method that should call this is PhysicsEngine.Update().
        /// It calls it to change an object to "airborne mode" when isOnGround == true
        /// but it detects that there is no ground beneath said object.
        /// There should be little to no need to call this method when defining a
        /// new method.
        /// </summary>
        public virtual void BecomeAirborne()
        {
            isOnGround = false;
        }

        //checkGround, checkWallRight, checkWallLeft, and isStuckAt are the collision detection
        //functions. Their main test is to evaluate the color on the collision map of the pixels
        //(to be) occupied by the object's collision points. But you may also notice that there
        //are also tests to check whether certain coordinates are outside of the collision map's
        //boundaries. For example, these tests prevent objects from moving off the left or right
        //edges of the board. On the other hand, they do allow objects to move through the top
        //edge or fall through the bottom edge of the board.
        //
        //You know how in plenty of Mario games it's possible to walk on top of the board,
        //out of the view of the viewport, to access secrets? Like in Level 1-2 of Super Mario
        //Bros. where you hit the bricks above and then walk over the rest of the level? Can we
        //do something like that? I think there can be a few fun rooms where the player has to
        //jump up really high to get on top of the ceiling, then walk a little ways and fall
        //through a hole in the ceiling to find some treasure.
        //
        //Also, this makes it possible to fall into a pit outside of the viewport, to cause
        //either permanent death to the object/player or a damage and respawn the player
        //(Like in many Zelda games).
        //
        //The only thing that would need to be modified to make these things happen is the
        //function that controls how the viewport moves: it would need to stop scrolling when
        //it gets to the edges of the map. Of course, this modification would be really easy.
        //I have an example on the program I made a few weeks ago, if anyone wants to see.
        
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
        public bool checkGround(Room room, int dX = 0, int dY = 0)
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
            return false;
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
        public bool checkWallRight(Room room, int dX = 0, int dY = 0)
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
        public bool checkWallLeft(Room room, int dX = 0, int dY = 0)
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
        public bool isStuckAt(Room room, int dX = 0, int dY = 0)
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

        
    }
}
