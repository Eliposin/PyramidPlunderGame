using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Pyramid_Plunder.Classes
{
    public class Door : GameObject
    {
        private DoorOrientations orientation;
        private LockTypes lockType;

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

        public Door(ContentManager content, DoorOrientations orient, string roomName, int connectedDoorIndex, LockTypes lockT)
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

            if (lockType != Door.LockTypes.Unlocked)
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

        private void unlock(LockTypes thisLock)
        {
            //Something to check if the player has met the
            //correct circumstances to unlock this door.
            
        }
        

       
       /// <summary>
       /// an enum representing the lockType the player needs to open the door.
       /// I am still unsure of how to impliment this.  Need to ask Ryan what his
       /// thoughts are on Monday.
       /// </summary>
        public enum LockTypes : byte
        {
            //Not sure what other types of lock we might have.
            Unlocked = 0, RedKey, BlueKey,
            DoubleJump, Dash, WallHang, BossKill, RoomClear
        }

        public enum DoorOrientations : byte
        {
            FacingRight = 0,
            FacingLeft = 1
        }

        /// <summary>
        /// Lock Type property
        /// </summary>
        public LockTypes LockType
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

        Boolean Open(Player player)
        {
            // TODO: Check to see if player has authorization
            try
            {
                Room nextRoom = new Room(connectedRoom, Content);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("An error occured opening the door or loading the room: " + e.Message);
                return false;
            }
        }
    }
}
    



