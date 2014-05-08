using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pyramid_Plunder.Classes
{
    class Door : GameObject
    {

        public Boolean isLocked
        {
            get { return isLocked; }
            private set { isLocked = value; }
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


        /*
         *Not sure what to put here for lock type.  Leaving it as Object
         *just so code will compile and shouldn't throw issues until I can
         *figureout what it really should be.
         * 
         */
        public Object lockType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="objType"></param>
        public Door(String filepath, GameObjectList objType)
            : base(filepath, objType)
        {


        }
    }
    

}

