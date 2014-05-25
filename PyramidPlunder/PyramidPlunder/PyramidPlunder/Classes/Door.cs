using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Pyramid_Plunder.Classes
{
    public class Door : GameObject
    {
        private const float DOOR_ANIMATION_SPEED = 0.2f;

        private DoorOrientations orientation;
        private Locks lockType;

        private bool opening = false;

        /// <summary>
        /// the String that represents the name of the connected Room.
        /// </summary>
        public string connectedRoom;

        /// <summary>
        /// the int that represents the position in the connected room's door[]
        /// that this door connects to.
        /// </summary>
        public int connectedDoor;
        public bool isActivated;
        //private ContentManager Content;
        private bool locked;

        public Door(ContentManager content, DoorOrientations orient, string roomName, int connectedDoorIndex, Locks lockT)
            : base(GameObjectList.Door, content)
        {
            Content = content;
            orientation = orient;
            connectedRoom = roomName;
            connectedDoor = connectedDoorIndex;
            lockType = lockT;

            if (orientation == DoorOrientations.FacingLeft)
                animationOffset = 1;
            else
                animationOffset = 0;

            if (lockType != Locks.Unlocked)
                locked = true;
            else
                locked = false;

            isActivated = false;
        }


        
        public bool IsLocked
        {
            get { return locked; }
            //set { locked = value; }
        }
        

        public enum DoorOrientations : byte
        {
            FacingRight = 0,
            FacingLeft = 1
        }

        /// <summary>
        /// Lock Type property
        /// </summary>
        public Locks LockType
        {
            get { return lockType; }
            set { lockType = value; }
        }

        /// <summary>
        /// A representation of which direction the door is facing
        /// </summary>
        public DoorOrientations Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime time)
        {
            base.Draw(spriteBatch, time);
            if ((currentFrame == numberOfFrames[currentAnimation] - 1) && opening)
                opening = false;
        }

        public void Open()
        {
            opening = true;
            animationSpeed[currentAnimation] = DOOR_ANIMATION_SPEED;
            looping = false;

            // I'm hijacking this method
            //try
            //{
            //    Room nextRoom = new Room(connectedRoom, Content);
            //    return true;
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine("An error occured opening the door or loading the room: " + e.Message);
            //    return false;
            //}
        }

        
    }
}
    



