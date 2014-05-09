using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pyramid_Plunder.Classes
{
    public class Door : GameObject
    {

        public Door(String filepath, GameObjectList objType)
            : base(objType)
        {
            locked = true;


        }


        private bool locked;
        public bool isLocked
        {
            get { return locked; }
            set { locked = value; }
        }

        /// <summary>
        /// the String that represents the name of the connected Room.
        /// </summary>
        public String connectedRoom;

        /// <summary>
        /// the int that represents the position in the connected room's door[]
        /// that this door connects to.
        /// </summary>
        public int connectedDoor;

       /// <summary>
       /// an enum representing the lockType the player needs to open the door.
       /// </summary>
        private enum lockType : byte
        {

        }

        public bool isActivated;

        Boolean Open(Player player)
        {
            return true;
        }
    }
}
    



