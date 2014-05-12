using System;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pyramid_Plunder.Classes
{
    public class Room
    {
        private Enemy[] enemyArray;
        private Door[] doorArray;
        private Vector2 spawnLocation;
        private String filePath;

        /// <summary>
        /// Constructor to build a room from a file.
        /// </summary>
        /// <param name="path">the path to the file to load from.</param>
        public Room(String roomName)
        {
            this.filePath = "Data/RoomsAndDoors/" + roomName;
            //this.filePath = path;  
        }

        

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
           
            /*
             *PsudoCode For What Should Happen:
             *
             * if(next StringFromFile == Doors){
             * while(next line != EndDoors)
             * load info into Door Array
             * }else if(next StringFromFile == Enemies){
             * while(nextLine != EndEnemies){
             * load enemy info into Enemy Array
             * }
             * close room file.
             * 
             * load Texture2D for Collision Map
             * close file
             * load Texture2D for Map
             * close file
             * load Enemy Sprites
             * close file
             * 
             *
             * 
             */

        }

        public void Draw(SpriteBatch batch)
        {

        }

        public Vector2 SpawnLocation
        {
            get { return SpawnLocation; }
        }

        /// <summary>
        /// Clears all memory assets related to the room.  Saves to a saved state 
        /// file if IsPersistant is set to true.
        /// </summary>
        public void Dispose()
        {
            NotImplementedException e = new NotImplementedException; 
            if (this.IsPersistant)
            {
                saveToFile();
            }
            throw e;
        }


        /// <summary>
        /// Saves room state to file when room IsPersistant.
        /// </summary>
        private void saveToFile()
        {
           
        }
    }
}
