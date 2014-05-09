using System;
using Microsoft.Xna.Framework.Storage;

namespace Pyramid_Plunder.Classes
{
    public class Room
    {
        private Enemy[] enemyArray;
        private Door[] doorArray;


        /// <summary>
        /// Constructor to build a room from a file.
        /// </summary>
        /// <param name="path">the path to the file to load from.</param>
        Room(String path)
        {
            this.filePath = path;  
        }

        /// <summary>
        /// The path to the file that the Room object represents.
        /// </summary>
        /// 

        private String filePath;
        /// <summary>
        /// An Array of Door Objects representing all possible entrances
        /// and exits to the room.
        /// </summary>
        public Door[] DoorArray
        {
            get { return doorArray; }
            
        }

        /// <summary>
        /// An Array of Enemies representing all enemies that exist in the room.
        /// </summary>

        public Enemy[] EnemyArray
        {
            get { return enemyArray; }
        }


        public bool IsPersistant
        {
            get { return IsPersistant; }
            
            set { IsPersistant = value; }

        }
        /// <summary>
        ///  Loads the room into memory. 
        ///  If IsPersistant is set to true, loads up the previously saved State file
        /// </summary>
        /// <param name="doorIndex">  represents which door the player is entering from </param>
        public void Load(int doorIndex)
        {
           


        }

        /// <summary>
        /// Clears all memory assets related to the room.  Saves to a saved state 
        /// file if IsPersistant is set to true.
        /// </summary>
        public void Dispose()
        {
            if (this.IsPersistant)
            {
                saveToFile();
            }
        }


        /// <summary>
        /// Saves room state to file when room IsPersistant.
        /// </summary>
        private void saveToFile()
        {

        }
    }
}
