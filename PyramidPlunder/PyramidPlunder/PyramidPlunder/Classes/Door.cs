using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Pyramid_Plunder.Classes
{
    public class Door : GameObject
    {
        /// <summary>
        /// the String that represents the name of the connected Room.
        /// </summary>
        public String connectedRoom;

        /// <summary>
        /// the int that represents the position in the connected room's door[]
        /// that this door connects to.
        /// </summary>
        public int connectedDoor;
        public bool isActivated;
        private ContentManager content;
        private bool locked;
        public Door(GameObjectList objType, ContentManager content)
            : base(objType, content)
        {
            locked = true;
            isActivated = false;


        }


        
        public bool isLocked
        {
            get { return locked; }
            set { locked = value; }
        }

        private void unlock(lockType thisLock)
        {
            //Something to check if the player has met the
            //correct circumstances to unlock this door.
            
        }
        

       
       /// <summary>
       /// an enum representing the lockType the player needs to open the door.
       /// I am still unsure of how to impliment this.  Need to ask Ryan what his
       /// thoughts are on Monday.
       /// </summary>
        private enum lockType : byte
        {
            //Not sure what other types of lock we might have.
            DoubleJump, Dash, WallHang, BossKill, RoomClear


        }

        

        Boolean Open(Player player)
        {
            Room nextRoom = new Room(connectedRoom, content);
            try
            {
                nextRoom.Load(connectedDoor);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("An error occurred: " + e.Message);
            }
            return true;
        }
    }
}
    



