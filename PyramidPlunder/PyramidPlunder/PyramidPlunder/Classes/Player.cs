using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pyramid_Plunder.Classes
{
    /// <summary>
    /// The class representing the character the player controls during the game.
    /// </summary>
    public class Player : PhysicsObject
    {
        public const int DEFAULT_SCREEN_POSITIONX = 610; //The player's ideal x-coordinates on the screen
        public const int DEFAULT_SCREEN_POSITIONY = 420; //The player's ideal y-coordinates on the screen

        public const int PLAYER_WIDTH = 60;

        public const double POWERUP_JINGLE_LENGTH = 7; //In seconds
        
        private KeyboardState keyState;
        private GamePadState gpState;

        private new AudioEngine soundEngine;

        const short MAX_JUMP_HEIGHT = -250;     //How high the player jumps if the button is held until the apex. (Pixels)
        float JUMP_V;                           //The resulting jump velocity from the max jump height. (Pixels/sec)
        float WALL_JUMP_V_X;                    //x-component of velocity when jumping off a wall. (Pixels/sec)
        float WALL_JUMP_V_Y;                    //y-component of velocity when jumping off a wall. (Pixels/sec)
        const float JUMP_DECAY = 14400;         //How rate at which y-velocity decreases if the jump button is released early.
                                                //(Pixels/sec/sec)
        const float WALL_FRICTION_DEC = -2160f; //Deceleration caused from friction on the wall while wall-sliding (Pixels/sec/sec)
        const float MAX_WALL_SLIDE_V = 360;     //The greatest y-velocity attainable while wall-sliding. (Pixels/sec)
        const byte MAX_MIDAIR_JUMPS = 1;        //The most times the player may jump while in midair.

        const short MAX_DASH_LENGTH = 300;      //How far the player dashes if the button is held until a forced stop. (Pixels)
        const float DASH_V = MAX_RUN_V * 3;     //How fast the player dashes. (Pixels/sec)
        const float MAX_DASH_TIME = (MAX_DASH_LENGTH / DASH_V); //The longest possible time the character can dash for (Sec)
        const sbyte MAX_MIDAIR_DASHES = 1;      //The most times the player may dash while in midair.

        //These constants are the various significant values that the dashStatus property is set to throughout play.
        const sbyte DASH_NOT_ALLOWED = -1;      //Pressing the dash button will not cause the player to dash.
        const float DASH_LAG_START = DASH_NOT_ALLOWED - 0.1f;   //The start of the time before another dash can be executed.
        const sbyte DASH_ALLOWED = 0;           //Pressing the dash button will cause the player to dash if other conditions allow.
        const float DASH_HELD = .001f;          //The dash button has just started to be held.
        const sbyte INFINITE_DASHES = -1;       //The player may dash as many times as they wish.

        const short MAX_RUN_V = 480;            //Fastest maintable running (not dashing) velocity by pressing the control stick. (Pixels/sec)
        const float RUN_ACC = 2700f;            //Rate of acceleration while running (Pixels/sec/sec)
        const float TOO_FAST_DEC = -2700f;      //Rate of deceleration when the player is running at a speed greater than its
                                                //maximum maintable running speed. (Pixels/sec/sec)
        const float STOP_DEC = -2160f;          //Rate of deceleration when the player stops pressing the arrow pad or
                                                //joystick. (Pixels/sec/sec)
        const float BRAKE_DEC = 4320f;          //Rate of deceleration when the player presses the arrowpad/joystick in the
                                                //direction opposite of their current running direction. (Pixels/sec/sec)
        const float KNOCK_BACK_V = 840;         //Velocity at which the character is knocked back after enemy collision. (Pixels/sec)

        const int PIT_FALL_DAMAGE = 2;          //Amount of Health points lost when falling into a pit.
        const int HAZARD_DAMAGE = 2;            //Amount of Health points lost when touching a hazard.
        
        //These constants are the various significant values that the damageStatus property attains during gameplay.
        const float VULNERABLE = -1;            //Player is susceptible to colliding with enemies.
        const float INVINCIBLE_START = 0;       //Player has just become invincible.
        const float STUN_END = 0.375F;          //How long (seconds) the object is stunned upon taking damage.
                                                //Should be zero if no stun.
        const float INVINCIBLE_END = 2;         //How long (seconds) the object is invincible upon taking damage.
                                                //Should be greater than stunTime.
        const float DEATH_SEQUENCE_END_NO_RAGDOLL = -5;     //The end of a death sequence in which no enemies ragdolled the player.
        const float DEATH_SEQUENCE_END_WITH_RAGDOLL = -2;   //The end of a death sequence in which enemies ragdolled the player.
        const float DEATH_SEQUENCE_START = -7;              //The player has just died and now the death sequence has begun.
        const float SWITCH_TO_DEAD_FRAME = -6.9f;           //The player is in the death sequence and is now switching to its lying sprite/hitbox.
        
        public enum XDirection
        {
            None = 0,
            Right = 1,
            Left = 2
        }

        private enum JumpState
        {
            NotAllowed = 0,
            Allowed = 1,
            Holding = 2
        }

        private enum Powerups
        {
            Dash = 7,
            DoubleJump = 8,
            WallJump = 9
        }

        private enum PlayerAnimations
        {
            WalkRight = 0,
            WalkLeft = 1,
            JumpRight = 2,
            JumpLeft = 3,
            WallSlideRight = 4,
            WallSlideLeft = 5,
            DashRight = 6,
            DashLeft = 7,
            DamageRight = 8,
            DamageLeft = 9,
            DyingRight = 10,
            DyingLeft = 11,
        }

        private bool drawnLastFrame = true;                 //Used to make the character blink during its invincibility phase.
        private XDirection LatestXArrow = XDirection.None;  //The latest horizontal direction (left or right) or lack thereof (none)
                                                            //pressed on the arrow pad or joystick.
        private XDirection PlayerXFacing = XDirection.Right;//The direction the player is facing
        private XDirection WallSlideDirection = XDirection.None;    //The side of the player that the wall it is sliding on is touching:
                                                                    //Left, right, or none.
        private JumpState PlayerJumpState = JumpState.Allowed;      //Whether the character is allowed to jump or is currently jumping.
        private byte midairJumps = MAX_MIDAIR_JUMPS;                //How many midair jumps the player is allowed to perform at any given moment.
        private sbyte dashes = INFINITE_DASHES;                     //How many dashes the player is allowed to perform at any given moment.
        private float dashStatus = DASH_ALLOWED;                    //Keeps track of if or when the character will dash in response to presses of
                                                                    //the dash button.
        protected float damageStatus = VULNERABLE;                  //Keeps track of if or for how long the player has been damaged and/or dead, or
                                                                    //if they are susceptible to damage.
        protected float deathSequenceEndTime = DEATH_SEQUENCE_END_NO_RAGDOLL;   //The damageStatus value at which the death sequence ends and
                                                                                //the game over screen appears.

        private double freezeTimerMax;

        //Flags which for each relevant button specifying if it it being pressed.
        private bool upBtnFlag = false;
        private bool downBtnFlag = false;
        private bool leftBtnFlag = false;
        private bool rightBtnFlag = false;
        private bool jumpBtnFlag = false;
        private bool dashBtnFlag = false;
        private bool interactBtnFlag = false;

        //Flags which specify which powerups the player has obtained.
        private bool[] itemArray;

        private DelVoid saveCallback;
        private DelRoom roomCallback;
        private DelSB hudCallback;

        private Door loadingDoor;

        //The arrays assigned to collisionXs and collisionYs when the player is alive
        private short[] lifeCollisionXs;
        private short[] lifeCollisionYs;

        //The arrays assigned to collisionXs and collisionYs when the player is dead
        private short[] deathCollisionXs;
        private short[] deathCollisionYs;
        
        /// <summary>
        /// Creates a new Player object
        /// </summary>
        public Player(ContentManager content, DelVoid saveMethod, DelRoom roomMethod, DelSB hudMethod)
            : base("Player", content)
        {
            isSpawned = false;
            JUMP_V = (float)(-Math.Sqrt(-2 * PhysicsEngine.GRAVITY * MAX_JUMP_HEIGHT));
            WALL_JUMP_V_X = (float)(JUMP_V * 0.7071);
            WALL_JUMP_V_Y = (float)(JUMP_V * 0.7071);

            itemArray = new bool[GameResources.NUM_ITEMS];
            for (int i = 0; i < itemArray.Length; i++)
                itemArray[i] = false;
            itemArray[0] = true;

            soundEngine = new AudioEngine(content, "Player");

            saveCallback = saveMethod;
            hudCallback = hudMethod;
            roomCallback = roomMethod;

            lifeCollisionXs = new short[collisionXs.Length];
            lifeCollisionYs = new short[collisionYs.Length];

            deathCollisionXs = new short[collisionYs.Length];
            deathCollisionYs = new short[collisionXs.Length];

            Array.Copy(collisionXs, lifeCollisionXs, collisionXs.Length);
            Array.Copy(collisionYs, lifeCollisionYs, collisionYs.Length);
            Array.Copy(collisionXs, deathCollisionYs, collisionXs.Length);
            Array.Copy(collisionYs, deathCollisionXs, collisionYs.Length);
        }

        /// <summary>
        /// Causes the object to become "spawned," and therefore drawable and interactable.
        /// </summary>
        /// <param name="location">The position to spawn the object at.</param>
        public override void Spawn(Vector2 location)
        {
            base.Spawn(location);
            coordinates = new Vector2(DEFAULT_SCREEN_POSITIONX, DEFAULT_SCREEN_POSITIONY);
        }

        /// <summary>
        /// Determines the player's intended x and y velocities based on the most recent user input,
        /// while updating other action-related properties.
        /// </summary>
        /// <param name="time">The amount of gametime that has elapsed since the last frame.</param>
        public override void Update(GameTime time)
        {
            float totalTime = (float)(time.ElapsedGameTime.TotalSeconds);

            if (damageStatus >= INVINCIBLE_END)
            {
                damageStatus = VULNERABLE;
            }
            else if (damageStatus >= STUN_END)
            {
                if (rightBtnFlag == true && leftBtnFlag == false)
                    LatestXArrow = XDirection.Right;
                else if (leftBtnFlag == true && rightBtnFlag == false)
                    LatestXArrow = XDirection.Left;
                else
                    LatestXArrow = XDirection.None;
            }

            if (currentHealth > 0 && (damageStatus < INVINCIBLE_START || damageStatus >= STUN_END))
            {
                if (dashStatus < DASH_HELD)
                {
                    if (LatestXArrow == XDirection.Left && PlayerXFacing == XDirection.Right)
                        PlayerXFacing = XDirection.Left;
                    else if (LatestXArrow == XDirection.Right && PlayerXFacing == XDirection.Left)
                        PlayerXFacing = XDirection.Right;
                }

                if (isOnGround == false && velocityY >= 0)
                {
                    if (wallOnRight && rightBtnFlag)
                    {
                        if (WallSlideDirection == XDirection.None)
                        {
                            soundEngine.Play(AudioEngine.SoundEffects.WallLand);
                            if (jumpBtnFlag == false)
                                PlayerJumpState = JumpState.Allowed;
                            else
                                PlayerJumpState = JumpState.NotAllowed;
                            if (!dashBtnFlag)
                                dashStatus = DASH_ALLOWED;
                            else
                                dashStatus = DASH_NOT_ALLOWED;
                        }
                        WallSlideDirection = XDirection.Right;
                    }
                    else if (wallOnLeft && leftBtnFlag == true)
                    {
                        if (WallSlideDirection == XDirection.None)
                        {
                            soundEngine.Play(AudioEngine.SoundEffects.WallLand);
                            if (jumpBtnFlag == false)
                                PlayerJumpState = JumpState.Allowed;
                            else
                                PlayerJumpState = JumpState.NotAllowed;
                            if (!dashBtnFlag)
                                dashStatus = DASH_ALLOWED;
                            else
                                dashStatus = DASH_NOT_ALLOWED;
                        }
                        WallSlideDirection = XDirection.Left;
                    }
                    else
                        WallSlideDirection = XDirection.None;
                }
                else
                    WallSlideDirection = XDirection.None;

                if (PlayerJumpState == JumpState.NotAllowed && jumpBtnFlag == false)
                    PlayerJumpState = JumpState.Allowed;

                if (jumpBtnFlag == true && PlayerJumpState == JumpState.Allowed &&
                    (isOnGround || midairJumps > 0 || wallOnLeft || wallOnRight))
                {
                    if (isOnGround == false)
                    {
                        if (wallOnRight || wallOnLeft)
                        {
                            velocityY = WALL_JUMP_V_Y;
                            velocityX = WALL_JUMP_V_X;
                            if (wallOnLeft)
                            {
                                velocityX *= -1;
                                PlayerXFacing = XDirection.Right;
                            }
                            else
                            {
                                PlayerXFacing = XDirection.Left;
                            }
                            WallSlideDirection = XDirection.None;
                            soundEngine.Play(AudioEngine.SoundEffects.WallJump);
                        }
                        else
                        {
                            midairJumps = (byte)Math.Max(0, midairJumps - 1);
                            velocityY = JUMP_V;
                            soundEngine.Play(AudioEngine.SoundEffects.WallJump);
                        }
                    }
                    else
                    {
                        LeaveGround();
                        velocityY = JUMP_V;
                        soundEngine.Play(AudioEngine.SoundEffects.Jump);
                    }
                    if (dashStatus >= DASH_HELD)
                    {
                        dashStatus = DASH_LAG_START;
                        isGravityAffected = true;
                    }
                    PlayerJumpState = JumpState.Holding;
                    accelerationY = 0;
                }

                if (!isOnGround)
                {
                    if (PlayerJumpState == JumpState.Holding)
                    {
                        if (jumpBtnFlag == false)
                        {
                            PlayerJumpState = JumpState.Allowed;
                            if (velocityY <= 0)
                            {
                                accelerationY = JUMP_DECAY;
                                velocityLimitY = 0;
                            }
                        }
                    }
                    else if (WallSlideDirection != XDirection.None)
                    {
                        accelerationY = WALL_FRICTION_DEC;
                        velocityLimitY = MAX_WALL_SLIDE_V;
                    }
                    else if (velocityY >= 0)
                    {
                        accelerationY = 0;
                    }
                }

                if (dashStatus == DASH_ALLOWED && dashBtnFlag == true &&
                    (dashes != 0 || WallSlideDirection != XDirection.None))
                {
                    dashStatus = DASH_HELD;
                    accelerationX = 0;
                    if (WallSlideDirection == XDirection.Right)
                    {
                        velocityX = -DASH_V;
                        PlayerXFacing = XDirection.Left;
                    }
                    else if (WallSlideDirection == XDirection.Left)
                    {
                        velocityX = DASH_V;
                        PlayerXFacing = XDirection.Right;
                    }
                    else
                    {
                        if (dashes > 0)
                            dashes--;
                        if (PlayerXFacing == XDirection.Right)
                        {
                            velocityX = DASH_V;
                            velocityLimitX = DASH_V;
                        }
                        else
                        {
                            velocityX = -DASH_V;
                            velocityLimitX = -DASH_V;
                        }
                    }
                    if (!isOnGround)
                    {
                        isGravityAffected = false;
                        velocityY = 0;
                        accelerationY = 0;
                    }
                    soundEngine.Play(AudioEngine.SoundEffects.Dash);
                }
                else if (dashStatus < DASH_NOT_ALLOWED)
                    dashStatus = Math.Min(dashStatus + totalTime, DASH_NOT_ALLOWED);
                else if (dashStatus == DASH_NOT_ALLOWED && dashBtnFlag == false)
                    dashStatus = DASH_ALLOWED;

                if (dashStatus >= DASH_HELD)
                {
                    dashStatus += totalTime;
                    if (dashStatus > MAX_DASH_TIME || dashBtnFlag == false)
                    {
                        if (LatestXArrow == XDirection.Right)
                            velocityX = MAX_RUN_V;
                        else if (LatestXArrow == XDirection.Left)
                            velocityX = -MAX_RUN_V;
                        else
                            velocityX = 0;
                        isGravityAffected = true;
                        dashStatus = DASH_LAG_START;
                    }
                }
                else
                {
                    if (LatestXArrow == XDirection.Right)
                    {
                        if (velocityX < 0)
                        {
                            accelerationX = BRAKE_DEC;
                            velocityLimitX = 0;
                        }
                        else if (velocityX <= MAX_RUN_V)
                        {
                            accelerationX = RUN_ACC;
                            velocityLimitX = MAX_RUN_V;
                        }
                        else
                        {
                            accelerationX = TOO_FAST_DEC;
                            velocityLimitX = MAX_RUN_V;
                        }
                    }
                    else if (LatestXArrow == XDirection.Left)
                    {
                        if (velocityX > 0)
                        {
                            accelerationX = -BRAKE_DEC;
                            velocityLimitX = 0;
                        }
                        else if (velocityX >= -MAX_RUN_V)
                        {
                            accelerationX = -RUN_ACC;
                            velocityLimitX = -MAX_RUN_V;
                        }
                        else
                        {
                            accelerationX = -TOO_FAST_DEC;
                            velocityLimitX = -MAX_RUN_V;
                        }
                    }
                    else if (velocityX != 0)
                    {
                        if (velocityX > MAX_RUN_V)
                        {
                            accelerationX = TOO_FAST_DEC;
                            velocityLimitX = MAX_RUN_V;
                        }
                        else if (velocityX > 0)
                        {
                            accelerationX = STOP_DEC;
                            velocityLimitX = 0;
                        }
                        else if (velocityX < -MAX_RUN_V)
                        {
                            accelerationX = -TOO_FAST_DEC;
                            velocityLimitX = -MAX_RUN_V;
                        }
                        else if (velocityX < 0)
                        {
                            accelerationX = -STOP_DEC;
                            velocityLimitX = 0;
                        }
                    }
                }
            }

            if ((damageStatus >= SWITCH_TO_DEAD_FRAME) && (damageStatus <= deathSequenceEndTime) &&
                (currentFrame == 0))
            {
                SwitchToDeathCollision();
                currentFrame++;
            }

            if (damageStatus >= INVINCIBLE_START)
                damageStatus += totalTime;
            else if (damageStatus < deathSequenceEndTime)
                damageStatus = Math.Min(deathSequenceEndTime, damageStatus + totalTime);
            
            if (PlayerXFacing == XDirection.Right)
            {
                if (currentHealth <= 0)
                    currentAnimation = currentAnimation = (int)PlayerAnimations.DyingRight;
                else if (damageStatus >= INVINCIBLE_START && damageStatus < STUN_END)
                    currentAnimation = (int)PlayerAnimations.DamageRight;
                else if (dashStatus >= DASH_HELD)
                    currentAnimation = (int)PlayerAnimations.DashRight;
                else if (WallSlideDirection == XDirection.Right)
                    currentAnimation = (int)PlayerAnimations.WallSlideRight;
                else if (!isOnGround)
                    currentAnimation = (int)PlayerAnimations.JumpRight;
                else
                {
                    currentAnimation = (int)PlayerAnimations.WalkRight;
                    if (damageStatus >= STUN_END)
                        animationSpeed[currentAnimation] = 2 * velocityX / MAX_RUN_V;
                    else
                        animationSpeed[currentAnimation] = velocityX / MAX_RUN_V;
                }
            }
            else
            {
                if (currentHealth <= 0)
                    currentAnimation = currentAnimation = (int)PlayerAnimations.DyingLeft;
                else if (damageStatus >= INVINCIBLE_START && damageStatus < STUN_END)
                    currentAnimation = (int)PlayerAnimations.DamageLeft;
                else if (dashStatus >= DASH_HELD)
                    currentAnimation = (int)PlayerAnimations.DashLeft;
                else if (WallSlideDirection == XDirection.Left)
                    currentAnimation = (int)PlayerAnimations.WallSlideLeft;
                else if (!isOnGround)
                    currentAnimation = (int)PlayerAnimations.JumpLeft;
                else
                {
                    currentAnimation = (int)PlayerAnimations.WalkLeft;
                    if (damageStatus >= STUN_END)
                        animationSpeed[currentAnimation] = 2 * -velocityX / MAX_RUN_V;
                    else
                        animationSpeed[currentAnimation] = -velocityX / MAX_RUN_V;
                }
            }
            if (velocityX == 0 && currentHealth > 0)
                currentFrame = 0;

            if (currentAnimation == (int)PlayerAnimations.DashLeft || currentAnimation == (int)PlayerAnimations.DashRight ||
                currentAnimation == (int)PlayerAnimations.DyingLeft || currentAnimation == (int)PlayerAnimations.DyingRight)
                looping = false;
            else
                looping = true;

            base.Update(time);
        }
                
        /// <summary>
        /// Interacts with the given object and interaction type.
        /// </summary>
        /// <param name="otherObject">The object to interaction type.</param>
        /// <param name="interactionType">The type of interaction to take place.</param>
        public override InteractionActions InteractWith(GameObject otherObject, InteractionTypes interactionType)
        {
            if (otherObject.ItemType != ItemList.NullItem && interactionType == InteractionTypes.Collision)
            {
                PickUpItem(otherObject);
            }
            else
            {
                switch (otherObject.ObjectName)
                {
                    case "Door":
                        if (interactionType == InteractionTypes.PlayerAction)
                        {
                            Door door = (Door)otherObject;
                            
                            if (!door.IsOpen)
                            {
                                if (!itemArray[(byte)door.LockType])
                                    hudCallback("This door is locked.", false, true);
                                else if (door.LinkedRoomName == "(unassigned)")
                                    hudCallback("This door appears to go nowhere.", false, true);
                                else
                                    door.Open();

                                return InteractionActions.None;
                            }
                        }
                        else if (!otherObject.IsSolid && interactionType == InteractionTypes.Collision)
                        {
                            Door door = (Door)otherObject;
                            if (door.IsRoomLoaded)
                                roomCallback(door.LinkedRoom);
                            else
                            {
                                loadingDoor = door;
                                GameManager.ToggleFreeze(true);
                                hudCallback("Loading room...", false, false);
                            }

                            if (door.Orientation == Door.DoorOrientations.FacingLeft)
                                ResetActionStates(XDirection.Right);
                            else
                                ResetActionStates(XDirection.Left);

                            return InteractionActions.None;
                        }
                        break;

                    case "SavePoint":
                        if (interactionType == InteractionTypes.PlayerAction)
                        {
                            currentHealth = maxHealth;
                            saveCallback();
                            return InteractionActions.None;
                        }
                        break;
                    case "Lever":
                        if (interactionType == InteractionTypes.PlayerAction)
                        {
                            otherObject.SwitchLever();
                            return InteractionActions.Lever;
                        }
                        break;
                    default:
                        return InteractionActions.None;
                }
            }
            return InteractionActions.None;
        }
                
        public bool CheckLoadedRoom()
        {
            if (loadingDoor != null && loadingDoor.IsRoomLoaded)
            {
                GameManager.ToggleFreeze(false);
                roomCallback(loadingDoor.LinkedRoom);
                loadingDoor = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resets all relevant properties to their "fresh"/initial states.
        /// Used for when the player enters rooms, spawns/respawns, or starts
        /// a new game.
        /// </summary>
        /// <param name="direction">The direction the player should face.</param>
        public void ResetActionStates(XDirection direction)
        {
            isOnGround = true;
            isGravityAffected = true;
            PlayerJumpState = JumpState.NotAllowed;
            PlayerXFacing = direction;
            midairJumps = MAX_MIDAIR_JUMPS;
            dashes = INFINITE_DASHES;
            if (rightBtnFlag == true && leftBtnFlag == false)
                LatestXArrow = XDirection.Right;
            else if (leftBtnFlag == true && rightBtnFlag == false)
                LatestXArrow = XDirection.Left;
            else
                LatestXArrow = XDirection.None;
            WallSlideDirection = XDirection.None;
            dashStatus = DASH_NOT_ALLOWED;
            velocityY = 0;
            velocityX = 0;
            accelerationX = 0;
            accelerationY = 0;
            drawnLastFrame = true;
        }

        /// <summary>
        /// Loads in a saved player state
        /// </summary>
        /// <param name="health">The health to set for the player.</param>
        /// <param name="items">The items the player should have.</param>
        public void LoadSave(int health, bool[] items)
        {
            currentHealth = health;
            itemArray = items;
        }

        /// <summary>
        /// Picks up the designated item, despawning it and adding it to the player's inventory.
        /// </summary>
        /// <param name="item">The item that was picked up.</param>
        private void PickUpItem(GameObject item)
        {
            itemArray[(byte)item.ItemType] = true;
            item.Despawn();
            if (item.IsKey)
                soundEngine.Play(AudioEngine.SoundEffects.KeyGet);
            else if (item.IsPowerup)
            {
                soundEngine.Play(AudioEngine.SoundEffects.ItemGet);
                string info = null;
                switch (item.ObjectName)
                {
                    case "DashPowerup":
                        info = "You got the dash powerup!\nUse the triggers (or Q on a keyboard) to dash, even in midair.";
                        break;
                }
                if (info != null)
                    hudCallback(info, true, true);
            }
        }

        /// <summary>
        /// Changes all of the player's relevant properties to reflect that
        /// the bottom side of its collision box has just hit a floor.
        /// </summary>
        public override void Land()
        {
            soundEngine.Play(AudioEngine.SoundEffects.Land);
            if (jumpBtnFlag == false)
                PlayerJumpState = JumpState.Allowed;
            else
                PlayerJumpState = JumpState.NotAllowed;

            if (dashStatus >= DASH_HELD)
            {
                dashStatus = DASH_NOT_ALLOWED;
                isGravityAffected = true;
            }

            if (dashBtnFlag == false)
                dashStatus = DASH_ALLOWED;
            else
                dashStatus = DASH_NOT_ALLOWED;

            midairJumps = MAX_MIDAIR_JUMPS;
            dashes = INFINITE_DASHES;

            if (currentHealth > 0)
                base.Land();
            else
            {
                isOnGround = true;
                accelerationY = 0;
                if (DisplacementY <= 2)
                    velocityY = 0;
                else
                    velocityY *= -0.5f;
            }
        }

        /// <summary>
        /// Changes all of the player's relevant properties to reflect that
        /// the left or right side of its collision box has just hit a wall
        /// or a horizontal edge of the map.
        /// </summary>
        public override void CollideX()
        {
            if (dashStatus >= DASH_HELD)
            {
                dashStatus = DASH_LAG_START;
                isGravityAffected = true;
                velocityY = 0;
                if (jumpBtnFlag == true)
                    PlayerJumpState = JumpState.NotAllowed;
                else
                    PlayerJumpState = JumpState.Allowed;
            }
            if (currentHealth < 0)
            {
                accelerationX = 0;
                velocityX *= -0.5f;
            }
            else
                base.CollideX();
        }

        /// <summary>
        /// Changes all of the player's relevant properties to reflect that
        /// the top end of its collision box has just hit a ceiling.
        /// </summary>
        public override void HitCeiling()
        {
            if (jumpBtnFlag == false)
                PlayerJumpState = JumpState.Allowed;
            else
                PlayerJumpState = JumpState.NotAllowed;
            base.HitCeiling();
        }

        /// <summary>
        /// Changes all of the player's relevant properties to reflect that
        /// it has just now become airborne (i.e. no longer on ground).
        /// </summary>
        public override void LeaveGround()
        {
            dashes = MAX_MIDAIR_DASHES;
            if (dashStatus >= DASH_HELD)
            {
                dashStatus = DASH_LAG_START;
                isGravityAffected = true;
            }
            base.LeaveGround();
        }

        /// <summary>
        /// Analyzes user input and updates the control flags for the player character.
        /// </summary>
        public void updateControlFlags()
        {
            KeyboardState newKeyState = Keyboard.GetState();
            GamePadState newGPState = GamePad.GetState(PlayerIndex.One);


            if ((keyState.IsKeyUp(Keys.Right) && newKeyState.IsKeyDown(Keys.Right)) ||
                    (keyState.IsKeyUp(Keys.D) && newKeyState.IsKeyDown(Keys.D)) ||
                    ((newGPState.DPad.Right == ButtonState.Pressed) && (gpState.DPad.Right == ButtonState.Released)) ||
                    ((newGPState.ThumbSticks.Left.X > 0) && (gpState.ThumbSticks.Left.X <= 0)))
            {
                rightBtnFlag = true;
                LatestXArrow = XDirection.Right;
            }
            else if ((keyState.IsKeyUp(Keys.Left) && newKeyState.IsKeyDown(Keys.Left)) ||
                    (keyState.IsKeyUp(Keys.A) && newKeyState.IsKeyDown(Keys.A)) ||
                    ((gpState.DPad.Left == ButtonState.Released) && (newGPState.DPad.Left == ButtonState.Pressed)) ||
                    ((gpState.ThumbSticks.Left.X >= 0) && (newGPState.ThumbSticks.Left.X < 0)))
            {
                leftBtnFlag = true;
                LatestXArrow = XDirection.Left;
            }

            if ((keyState.IsKeyUp(Keys.Up) && newKeyState.IsKeyDown(Keys.Up)) ||
                    (keyState.IsKeyUp(Keys.W) && newKeyState.IsKeyDown(Keys.W)) ||
                    (gpState.DPad.Up == ButtonState.Released && newGPState.DPad.Up == ButtonState.Pressed))
                upBtnFlag = true;

            if ((keyState.IsKeyUp(Keys.Space) && newKeyState.IsKeyDown(Keys.Space)) ||
                    (keyState.IsKeyUp(Keys.X) && newKeyState.IsKeyDown(Keys.X)) ||
                    (gpState.Buttons.A == ButtonState.Released && newGPState.Buttons.A == ButtonState.Pressed))
                jumpBtnFlag = true;

            if (itemArray[(int)Powerups.Dash] &&
                ((keyState.IsKeyUp(Keys.Z) && newKeyState.IsKeyDown(Keys.Z)) ||
                    (keyState.IsKeyUp(Keys.Q) && newKeyState.IsKeyDown(Keys.Q))))
                dashBtnFlag = true;
            else if (itemArray[(int)Powerups.Dash] && (gpState.Triggers.Right == 0 && newGPState.Triggers.Right > 0))
            {
                if (currentHealth > 0 && dashStatus < DASH_HELD && dashStatus >= DASH_ALLOWED)
                    PlayerXFacing = XDirection.Right;
                dashBtnFlag = true;
            }
            else if (itemArray[(int)Powerups.Dash] && (gpState.Triggers.Left == 0 && newGPState.Triggers.Left > 0))
            {
                if (currentHealth > 0 && dashStatus < DASH_HELD && dashStatus >= DASH_ALLOWED)
                    PlayerXFacing = XDirection.Left;
                dashBtnFlag = true;
            }
            
            if (keyState.IsKeyUp(Keys.E) && newKeyState.IsKeyDown(Keys.E) ||
                (gpState.Buttons.X == ButtonState.Released && newGPState.Buttons.X == ButtonState.Pressed))
                interactBtnFlag = true;
            else
                interactBtnFlag = false;

            //
            //-----------------------------------------------------------------------------
            //

            if ((keyState.IsKeyDown(Keys.Left) && newKeyState.IsKeyUp(Keys.Left)) ||
                (keyState.IsKeyDown(Keys.A) && newKeyState.IsKeyUp(Keys.A)) ||
                (gpState.DPad.Left == ButtonState.Pressed && newGPState.DPad.Left == ButtonState.Released) ||
                (gpState.ThumbSticks.Left.X != 0 && newGPState.ThumbSticks.Left.X == 0))
            {
                leftBtnFlag = false;
                if (LatestXArrow == XDirection.Left)
                {
                    if (rightBtnFlag == true)
                        LatestXArrow = XDirection.Right;
                    else
                        LatestXArrow = XDirection.None;
                }
            }
            if ((keyState.IsKeyDown(Keys.Right) && newKeyState.IsKeyUp(Keys.Right)) ||
                (keyState.IsKeyDown(Keys.D) && newKeyState.IsKeyUp(Keys.D)) ||
                (gpState.DPad.Right == ButtonState.Pressed && newGPState.DPad.Right == ButtonState.Released) ||
                (gpState.ThumbSticks.Left.X != 0 && newGPState.ThumbSticks.Left.X == 0))
            {
                rightBtnFlag = false;
                if (LatestXArrow == XDirection.Right)
                {
                    if (leftBtnFlag == true)
                        LatestXArrow = XDirection.Left;
                    else
                        LatestXArrow = XDirection.None;
                }
            }

            if ((keyState.IsKeyDown(Keys.Up) && newKeyState.IsKeyUp(Keys.Up)) ||
                (keyState.IsKeyDown(Keys.W) && newKeyState.IsKeyUp(Keys.W)) ||
                (gpState.DPad.Up == ButtonState.Pressed && newGPState.DPad.Up == ButtonState.Released))
            {
                upBtnFlag = false;
            }

            if ((keyState.IsKeyDown(Keys.Space) && newKeyState.IsKeyUp(Keys.Space)) ||
                (keyState.IsKeyDown(Keys.X) && newKeyState.IsKeyUp(Keys.X)) ||
                (gpState.Buttons.A == ButtonState.Pressed && newGPState.Buttons.A == ButtonState.Released))
            {
                jumpBtnFlag = false;
            }

            if (itemArray[(int)Powerups.Dash] &&
                (keyState.IsKeyDown(Keys.Z) && newKeyState.IsKeyUp(Keys.Z)) ||
                (keyState.IsKeyDown(Keys.Q) && newKeyState.IsKeyUp(Keys.Q)) ||
                (gpState.Triggers.Right > 0 && newGPState.Triggers.Right == 0) ||
                (gpState.Triggers.Left > 0 && newGPState.Triggers.Left == 0))
                dashBtnFlag = false;

            keyState = newKeyState;
            gpState = newGPState;

        }

        /// <summary>
        /// Updates the coordinates on the screen at which the player will be
        /// drawn.
        /// </summary>
        /// <param name="roomDimensions">The dimensions of the room the player is in.</param>
        public void UpdateCoordinates(Rectangle roomDimensions)
        {
            int xLine = Player.DEFAULT_SCREEN_POSITIONX;
            int yLine = Player.DEFAULT_SCREEN_POSITIONY;
            int viewWidth = Main.DEFAULT_RESOLUTION_X;
            int viewHeight = Main.DEFAULT_RESOLUTION_Y;
            int bgWidth = roomDimensions.Width;
            int bgHeight = roomDimensions.Height;

            //x
            if (bgWidth <= viewWidth)
                coordinates.X = position.X + (viewWidth - bgWidth) / 2;
            else
            {
                if (position.X >= bgWidth - viewWidth + xLine)
                    coordinates.X = position.X - bgWidth + viewWidth;
                else if (position.X <= xLine)
                    coordinates.X = position.X;
                else
                    coordinates.X = xLine;
            }

            //y
            if (bgHeight <= viewHeight)
                coordinates.Y = position.Y + (viewHeight - bgHeight) / 2;
            else
            {
                if (position.Y >= bgHeight - viewHeight + yLine)
                    coordinates.Y = position.Y - bgHeight + viewHeight;
                else if (position.Y <= yLine)
                    coordinates.Y = position.Y;
                else
                    coordinates.Y = yLine;
            }
        }
                
        /// <summary>
        /// Whether or not the player is attempting an interaction
        /// </summary>
        public bool InteractionFlag
        {
            get { return interactBtnFlag; }
        }

        /// <summary>
        /// Returns the array of flags which keeps track of the powerups the
        /// Player has obtained.
        /// </summary>
        public bool[] CurrentItems
        {
            get { return itemArray; }
        }

        /// <summary>
        /// Returns true if the player is susceptible to collision with enemies;
        /// false otherwise.
        /// </summary>
        public bool IsVulnerable
        {
            get { return (damageStatus < 0); }
        }

        /// <summary>
        /// Returns true if the player's health has reached zero and it has
        /// finished flinching; false otherwise.
        /// </summary>
        public bool IsDead
        {
            get { return (currentHealth <= 0 && damageStatus == VULNERABLE); }
        }
                
        /// <summary>
        /// Begin the sequence that occurs right after the player's health has reached
        /// zero and the player is officially dying.
        /// </summary>
        public void StartDeathSequence()
        {
            damageStatus = DEATH_SEQUENCE_START;
        }

        /// <summary>
        /// Returns whether the death sequence has ended; if it has, the
        /// player's collision coordinates and deathSequence end time are
        /// reset to their living counterparts.
        /// </summary>
        public bool DeathSequenceEnded
        {
            get
            {
                if (damageStatus == deathSequenceEndTime)
                {
                    SwitchToLifeCollision();
                    deathSequenceEndTime = DEATH_SEQUENCE_END_NO_RAGDOLL;
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Initiate damage, respawn and invincibility due falling through the bottom of the map.
        /// </summary>
        public void ReceivePitFallDamage()
        {
            currentHealth = Math.Max(0, currentHealth - PIT_FALL_DAMAGE);
            if (currentHealth > 0)
                damageStatus = STUN_END;
            else
                damageStatus = VULNERABLE;
            ResetActionStates(PlayerXFacing);
        }

        /// <summary>
        /// Initiate damage, respawn and invincibility due to touching a hazard
        /// like lava or spikes.
        /// </summary>
        public void ReceiveHazardDamage()
        {
            currentHealth = Math.Max(0, currentHealth - HAZARD_DAMAGE);
            if (currentHealth > 0)
                damageStatus = STUN_END;
            else
                damageStatus = VULNERABLE;
            ResetActionStates(PlayerXFacing);
        }

        /// <summary>
        /// Initiate damage and flinching caused by collision with a specific
        /// enemy object from a specific direction.
        /// </summary>
        /// <param name="enemy">The enemy to collide with.</param>
        /// <param name="direction">The side of the player that the enemy is touching.</param>
        private void CollideWithEnemy(Enemy enemy, XDirection direction)
        {
            soundEngine.Play(AudioEngine.SoundEffects.Hurt);
            currentHealth = Math.Max(0, currentHealth - enemy.ContactDamage);
            if (direction == XDirection.Left)
            {
                velocityX = KNOCK_BACK_V;
                accelerationX = STOP_DEC;
            }
            else
            {
                velocityX = -KNOCK_BACK_V;
                accelerationX = -STOP_DEC;
            }
            velocityLimitX = 0;
            accelerationY = 0;
            LatestXArrow = XDirection.None;
            isGravityAffected = true;
            if (currentHealth > 0)
                damageStatus = INVINCIBLE_START;
            else
            {
                velocityY -= 500;
                deathSequenceEndTime = DEATH_SEQUENCE_END_WITH_RAGDOLL;
            }
        }

        /// <summary>
        /// Checks for geometric collision with all of the enemies in the room.
        /// </summary>
        /// <param name="room">The room the player is currently in.</param>
        public void DetectEnemyCollisions(Room room)
        {
            foreach (Enemy enemy in room.EnemyArray)
            {
                if (enemy.IsSpawned &&
                    (position.Y + collisionYs.Last() >= enemy.Position.Y + enemy.CollisionYs.First()) &&
                    (position.Y + collisionYs.First() <= enemy.Position.Y + enemy.CollisionYs.Last()) &&
                    (position.X + collisionXs.Last() >= enemy.Position.X + enemy.CollisionXs.First()) &&
                    (position.X + collisionXs.First() <= enemy.Position.X + enemy.CollisionXs.Last()))
                {
                    if ((position.X + collisionXs.First() <= enemy.Position.X + enemy.CollisionXs.Last()) &&
                        position.X + collisionXs.First() >= enemy.Position.X + enemy.CollisionXs.First())
                    {
                        CollideWithEnemy(enemy, XDirection.Left);
                        return;
                    }
                    else
                    {
                        CollideWithEnemy(enemy, XDirection.Right);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the player is touching any hazards in the room.
        /// </summary>
        /// <param name="room">The room in which the character is in.</param>
        /// <returns></returns>
        public bool CheckHazards(Room room)
        {
            foreach (GameObject obj in room.EnvironmentArray)
            {
                if (obj.IsHazard)
                {
                    if ((position.X + collisionXs.Last() >= obj.Position.X) &&
                        (position.X + collisionXs.First() <= obj.Position.X + obj.HitBox.Width) &&
                        (position.Y + collisionYs.Last() >= obj.Position.Y) &&
                        (position.Y + collisionYs.First() <= obj.Position.Y + obj.HitBox.Height))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sets collisionXs and collisionYs to the arrays for collision for
        /// when the character is dead.
        /// </summary>
        private void SwitchToDeathCollision()
        {
            collisionXs = new short[deathCollisionXs.Length];
            collisionYs = new short[deathCollisionYs.Length];
            Array.Copy(deathCollisionXs, collisionXs, deathCollisionXs.Length);
            Array.Copy(deathCollisionYs, collisionYs, deathCollisionYs.Length);
        }

        /// <summary>
        /// Sets collisionXs and collisionYs to the arrays for collision for
        /// when the character is alive.
        /// </summary>
        private void SwitchToLifeCollision()
        {
            collisionXs = new short[lifeCollisionXs.Length];
            collisionYs = new short[lifeCollisionYs.Length];
            Array.Copy(lifeCollisionXs, collisionXs, lifeCollisionXs.Length);
            Array.Copy(lifeCollisionYs, collisionYs, lifeCollisionYs.Length);
        }

        /// <summary>
        /// Draws the player to the screen. Player will not be drawn every other frame when
        /// it is invincible.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        /// <param name="time">The game time elapsed since the last frame.</param>
        /// <param name="playAnimations"></param>
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameTime time, bool playAnimations)
        {
            if ((damageStatus < INVINCIBLE_START) || (damageStatus >= INVINCIBLE_END) || !drawnLastFrame)
            {
                base.Draw(spriteBatch, time, playAnimations);
                drawnLastFrame = true;
            }
            else
            {
                drawnLastFrame = false;
            }
        }

        /// <summary>
        /// Returns the direction (left or right) that the player is facing.
        /// </summary>
        public XDirection CurrentDirection
        {
            get { return PlayerXFacing; }
        }
    }
}
