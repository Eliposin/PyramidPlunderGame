﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pyramid_Plunder.Classes
{
    public class Player : PhysicsObject
    {
        public const int DEFAULT_SCREEN_POSITIONX = 610; //610
        public const int DEFAULT_SCREEN_POSITIONY = 420; //420

        public const int PLAYER_WIDTH = 60;
        public const int PLAYER_HEIGHT = 120;
        //private bool isSpawned;

        private KeyboardState keyState;
        private GamePadState gpState;

        private AudioEngine soundEngine;
        
        const short MAX_JUMP_HEIGHT = -250;
        float JUMP_V;
        float WALL_JUMP_V_X;
        float WALL_JUMP_V_Y;
        const float JUMP_DECAY = 14400;
        const float WALL_FRICTION_DEC = -2160f;
        const float MAX_WALL_SLIDE_V = 360;
        const short MAX_FALL_V = 3000;
        const byte MAX_MIDAIR_JUMPS = 1;

        const short MAX_DASH_LENGTH = 300;
        const float DASH_V = MAX_RUN_V * 3;
        const float MAX_DASH_TIME = (MAX_DASH_LENGTH / DASH_V);
        const sbyte MAX_MIDAIR_DASHES = 1;

        const sbyte DASH_NOT_ALLOWED = -1;
        const float DASH_LAG_START = DASH_NOT_ALLOWED - 0.1f;
        const sbyte DASH_ALLOWED = 0;
        const float DASH_HELD = .001f;
        const sbyte INFINITE_DASHES = -1;

        const short MAX_RUN_V = 480;
        const float RUN_ACC = 2700f;
        const float TOO_FAST_DEC = -2700f;
        const float STOP_DEC = -2160f;
        const float BRAKE_DEC = 4320f;

        

        public enum XDirection
        {
            None = 0,
            Right = 1,
            Left = 2
        };

        private enum JumpState
        {
            NotAllowed = 0,
            Allowed = 1,
            Holding = 2
        };

        

        private XDirection LatestXArrow = XDirection.None;
        private XDirection PlayerXFacing = XDirection.Right;
        private XDirection WallSlideDirection = XDirection.None;
        private JumpState PlayerJumpState = JumpState.Allowed;
        private byte midairJumps = MAX_MIDAIR_JUMPS;
        private sbyte dashes = INFINITE_DASHES;
        private float dashStatus = DASH_ALLOWED;

        private bool upBtnFlag = false;
        private bool downBtnFlag = false;
        private bool leftBtnFlag = false;
        private bool rightBtnFlag = false;
        private bool jumpBtnFlag = false;
        private bool dashBtnFlag = false;
        private bool interactBtnFlag = false;

        private bool[] itemArray;

        private DelVoid saveCallback;
        private DelRoom roomCallback;


        /// <summary>
        /// Creates a new Player object
        /// </summary>
        public Player(ContentManager content, DelVoid saveMethod, DelRoom roomMethod)
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

            soundEngine = new AudioEngine(content, GameObjectList.Player);

            saveCallback = saveMethod;
            roomCallback = roomMethod;
            
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

        public override void Update(GameTime time)
        {
            float totalTime = (float)(time.ElapsedGameTime.TotalSeconds);
            
            if (dashStatus < DASH_HELD)
            {
                if (LatestXArrow == XDirection.Left && PlayerXFacing == XDirection.Right)
                    PlayerXFacing = XDirection.Left;
                else if (LatestXArrow == XDirection.Right && PlayerXFacing == XDirection.Left)
                    PlayerXFacing = XDirection.Right;
            }

            if (isOnGround == false)
            {
                if (velocityY >= 0)
                {
                    if (wallOnRight && rightBtnFlag)
                    {
                        if (WallSlideDirection == XDirection.None)
                            soundEngine.Play(AudioEngine.SoundEffects.WallLand);
                        WallSlideDirection = XDirection.Right;
                    }
                    else if (wallOnLeft && leftBtnFlag == true)
                    {
                        if (WallSlideDirection == XDirection.None)
                            soundEngine.Play(AudioEngine.SoundEffects.WallLand);
                        WallSlideDirection = XDirection.Left;
                    }
                    else
                        WallSlideDirection = XDirection.None;
                }
            }
            else
                WallSlideDirection = XDirection.None;

            if (PlayerJumpState == JumpState.NotAllowed && jumpBtnFlag == false)
                PlayerJumpState = JumpState.Allowed;

            if (jumpBtnFlag == true //&& (InvincibleTimer < 0 || cInvincibleTimer >= STUN_END_TIME)
                && PlayerJumpState == JumpState.Allowed && (isOnGround || midairJumps > 0 || WallSlideDirection != XDirection.None))
            {
                if (isOnGround == false)
                {
                    if (WallSlideDirection != XDirection.None)
                    {
                        velocityY = WALL_JUMP_V_Y;
                        velocityX = WALL_JUMP_V_X;
                        if (WallSlideDirection == XDirection.Left)
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
                    BecomeAirborne();
                    velocityY = JUMP_V;
                    soundEngine.Play(AudioEngine.SoundEffects.Jump);
                }
                if (dashStatus >= DASH_HELD)
                {
                    dashStatus = DASH_LAG_START;
                    isGravityAffected = true;
                }
                PlayerJumpState = JumpState.Holding;
            }

            if (PlayerJumpState == JumpState.Holding)
            {
                if (jumpBtnFlag == false)
                    PlayerJumpState = JumpState.Allowed;
            }
            else if (velocityY < 0)
                velocityY = Math.Min(velocityY + JUMP_DECAY * totalTime, 0);

            if (dashStatus < DASH_HELD)
            {
                if (WallSlideDirection != XDirection.None)
                {
                    accelerationY = WALL_FRICTION_DEC;
                    velocityLimitY = MAX_WALL_SLIDE_V;
                }
                else
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
            base.Update(time);
        }

        public void UpdateCoordinates()
        {
            //This only works properly if the edge is on the left side
            coordinates.X = position.X;
        }

        /// <summary>
        /// Interacts with the given object and interaction type.
        /// </summary>
        /// <param name="otherObject">The object to interaction type.</param>
        /// <param name="interactionType">The type of interaction to take place.</param>
        public override void InteractWith(GameObject otherObject, InteractionTypes interactionType)
        {
            if (otherObject.ItemType != ItemList.NullItem)
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
                                if (itemArray[(byte)door.LockType] == true)
                                    door.Open();
                            }
                            else
                            {
                                if (door.IsRoomLoaded)
                                    roomCallback(door.LinkedRoom);
                            }
                        }
                        break;

                    case "SavePoint":
                        saveCallback();
                        break;
                    default:
                        break;
                }
            }
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
        /// <param name="key"></param>
        private void PickUpItem(GameObject item)
        {
            itemArray[(byte)item.ItemType] = true;
            item.Despawn();
        }

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
            base.Land();
        }

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
            base.CollideX();
        }

        public override void HitCeiling()
        {
            if (jumpBtnFlag == false)
                PlayerJumpState = JumpState.Allowed;
            else
                PlayerJumpState = JumpState.NotAllowed;
            base.HitCeiling();
        }

        public override void BecomeAirborne()
        {
            dashes = MAX_MIDAIR_DASHES;
            if (dashStatus >= DASH_HELD)
            {
                dashStatus = DASH_LAG_START;
                isGravityAffected = true;
            }
            base.BecomeAirborne();
        }

        public void updateControlFlags()
        {
            KeyboardState newKeyState = Keyboard.GetState();
            GamePadState newGPState = GamePad.GetState(PlayerIndex.One);


            if ((keyState.IsKeyUp(Keys.Right) && newKeyState.IsKeyDown(Keys.Right)) ||
                    (keyState.IsKeyUp(Keys.D) && newKeyState.IsKeyDown(Keys.D)) ||
                    (gpState.DPad.Right == ButtonState.Released && newGPState.DPad.Right == ButtonState.Pressed) ||
                    (gpState.ThumbSticks.Left.X < 0 && newGPState.ThumbSticks.Left.X > 0))
            {
                rightBtnFlag = true;
                LatestXArrow = XDirection.Right;
            }
            else if ((keyState.IsKeyUp(Keys.Left) && newKeyState.IsKeyDown(Keys.Left)) ||
                    (keyState.IsKeyUp(Keys.A) && newKeyState.IsKeyDown(Keys.A)) ||
                    (gpState.DPad.Left == ButtonState.Released && newGPState.DPad.Left == ButtonState.Pressed) ||
                    (gpState.ThumbSticks.Left.X > 0 && newGPState.ThumbSticks.Left.X < 0))
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

            if ((keyState.IsKeyUp(Keys.Z) && newKeyState.IsKeyDown(Keys.Z)) ||
                    (keyState.IsKeyUp(Keys.Q) && newKeyState.IsKeyDown(Keys.Q)))
                dashBtnFlag = true;
            else if (gpState.Triggers.Right == 0 && newGPState.Triggers.Right > 0)
            {
                PlayerXFacing = XDirection.Right;
                dashBtnFlag = true;
            }
            else if (gpState.Triggers.Left == 0 && newGPState.Triggers.Left > 0)
            {
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

            if ((keyState.IsKeyDown(Keys.Z) && newKeyState.IsKeyUp(Keys.Z)) ||
                (keyState.IsKeyDown(Keys.Q) && newKeyState.IsKeyUp(Keys.Q)) ||
                (gpState.Triggers.Right > 0 && newGPState.Triggers.Right == 0) ||
                (gpState.Triggers.Left > 0 && newGPState.Triggers.Left == 0))

                dashBtnFlag = false;

            //if (keyState.IsKeyDown(Keys.E) && newState.IsKeyUp(Keys.E))
            //    interactBtnFlag = false;

            keyState = newKeyState;
            gpState = newGPState;

        }

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

        public bool[] CurrentItems
        {
            get { return itemArray; }
        }
    }
}
