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
    public class Player : PhysicsObject
    {
        private int DEFAULT_SCREEN_POSITIONX = 610;
        private int DEFAULT_SCREEN_POSITIONY = 420;
        //private bool isSpawned;

        private KeyboardState keyState;

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
              
        /// <summary>
        /// Creates a new Player object
        /// </summary>
        public Player(ContentManager content)
            : base(GameObjectList.Player, content)
        {
            isSpawned = false;
            JUMP_V = (float)(-Math.Sqrt(-2 * PhysicsEngine.GRAVITY * MAX_JUMP_HEIGHT));
            WALL_JUMP_V_X = (float)(JUMP_V * 0.7071);
            WALL_JUMP_V_Y = (float)(JUMP_V * 0.7071);
            //isGravityAffected = true;
            //collisionXs = new short[3] { 11, 26, 41 };
            //collisionYs = new short[3] { 22, 65, 108 };

            soundEngine = new AudioEngine(content, GameObjectList.Player);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
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
                            //soundLand.Play();
                        WallSlideDirection = XDirection.Right;
                    }
                    else if (wallOnLeft && leftBtnFlag == true)
                    {
                        if (WallSlideDirection == XDirection.None)
                            //soundLand.Play();
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
                        //soundWallJump.Play();
                        
                    }
                    else
                    {
                        midairJumps = (byte)Math.Max(0, midairJumps - 1);
                        velocityY = JUMP_V;
                        soundEngine.Play(AudioEngine.SoundEffects.Jump);
                        
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
                    dashStatus = DASH_NOT_ALLOWED;
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
            else if (dashStatus <= DASH_NOT_ALLOWED && dashBtnFlag == false)
                dashStatus = DASH_ALLOWED;

            if (dashStatus >= DASH_HELD)
            {
                dashStatus += totalTime;
                if (dashStatus > MAX_DASH_TIME || dashBtnFlag == false)
                {
                    velocityX = 0;
                    isGravityAffected = true;
                    dashStatus = DASH_NOT_ALLOWED;
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

        public override void Land()
        {
            //soundLand.Play();
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
                dashStatus = DASH_NOT_ALLOWED;
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
                dashStatus = DASH_NOT_ALLOWED;
                isGravityAffected = true;
            }
            base.BecomeAirborne();
        }

        public void updateControlFlags()
        {
            KeyboardState newState = Keyboard.GetState();

            if ((keyState.IsKeyUp(Keys.Right) && newState.IsKeyDown(Keys.Right)) ||
                (keyState.IsKeyUp(Keys.D) && newState.IsKeyDown(Keys.D)))
            {
                rightBtnFlag = true;
                LatestXArrow = XDirection.Right;
            }
            else if ((keyState.IsKeyUp(Keys.Left) && newState.IsKeyDown(Keys.Left)) ||
                (keyState.IsKeyUp(Keys.A) && newState.IsKeyDown(Keys.A)))
            {
                leftBtnFlag = true;
                LatestXArrow = XDirection.Left;
            }

            if ((keyState.IsKeyUp(Keys.Up) && newState.IsKeyDown(Keys.Up)) ||
                (keyState.IsKeyUp(Keys.W) && newState.IsKeyDown(Keys.W)))
                upBtnFlag = true;

            if ((keyState.IsKeyUp(Keys.Space) && newState.IsKeyDown(Keys.Space)) ||
                (keyState.IsKeyUp(Keys.X) && newState.IsKeyDown(Keys.X)))
                jumpBtnFlag = true;

            if ((keyState.IsKeyUp(Keys.Z) && newState.IsKeyDown(Keys.Z)) ||
                (keyState.IsKeyUp(Keys.Q) && newState.IsKeyDown(Keys.Q)))
                dashBtnFlag = true;

            if ((keyState.IsKeyDown(Keys.Left) && newState.IsKeyUp(Keys.Left)) ||
                (keyState.IsKeyDown(Keys.A) && newState.IsKeyUp(Keys.A)))
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
            if ((keyState.IsKeyDown(Keys.Right) && newState.IsKeyUp(Keys.Right)) ||
                (keyState.IsKeyDown(Keys.D) && newState.IsKeyUp(Keys.D)))
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

            if ((keyState.IsKeyDown(Keys.Up) && newState.IsKeyUp(Keys.Up)) ||
                (keyState.IsKeyDown(Keys.W) && newState.IsKeyUp(Keys.W)))
            {
                upBtnFlag = false;
            }

            if ((keyState.IsKeyDown(Keys.Space) && newState.IsKeyUp(Keys.Space)) ||
                (keyState.IsKeyDown(Keys.X) && newState.IsKeyUp(Keys.X)))
            {
                jumpBtnFlag = false;
            }

            if ((keyState.IsKeyDown(Keys.Z) && newState.IsKeyUp(Keys.Z)) ||
                (keyState.IsKeyDown(Keys.Q) && newState.IsKeyUp(Keys.Q)))
                dashBtnFlag = false;
            keyState = newState;
        }
    }
}
