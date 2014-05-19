﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.IO;

namespace Pyramid_Plunder.Classes
{
    public class Room
    {
        private Enemy[] enemyArray;
        private Door[] doorArray;
        private Vector2 spawnLocation;
        private String filePath;
        private Texture2D collisionMap;
        private bool hasMoreObjects;
        private int totalObjects;
        private GameObject background;     //Encapsulate the drawing requirements into the class already designed to do that
                                          //Needs to be a GameObject because it needs to move around with the player
        private string roomName;          
        private int SpawnX;
        private int SpawnY;
        private ContentManager Content;
        public Color[] collisionColors;

        /// <summary>
        /// Constructor to build a room from a file.
        /// </summary>
        /// <param name="path">the path to the file to load from.</param>
        public Room(String roomName, ContentManager content)
        {
            this.roomName = roomName;
            Content = content;
           // Load();
            this.filePath = "Data/RoomsAndDoors/" + roomName + ".room"; //Lets add a .room extension, shall we?  It can still be a plain text file.
            background = new GameObject(whichRoom(roomName), Content);

            //Added for testing!!!
            collisionMap = Content.Load<Texture2D>("Images/TestRoom");
            collisionColors = new Color[collisionMap.Width * collisionMap.Height];
            collisionMap.GetData<Color>(collisionColors);
        }

        /// <summary>
        /// Determines which room should be passed into the background constructor based on the room's name
        /// </summary>
        /// <param name="roomName">The name of the room as a String</param>
        /// <returns>Which room in the GameObjectList</returns>
        private GameObjectList whichRoom(String roomName)
        {
            switch (roomName)
            {
                case "TestRoom":
                    spawnLocation = new Vector2(1100, 900); //Eventually should be read from file!
                    totalObjects = 0;
                    hasMoreObjects = false;
                    return GameObjectList.TestRoom;
                default:
                    return GameObjectList.NullObject;
            }
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
        /// The bitmap containing collision information for the room
        /// </summary>
        public Texture2D CollisionMap
        {
            get { return collisionMap; }
        }

        /// <summary>
        ///  Loads the room into memory. 
        ///  If IsPersistant is set to true, loads up the previously saved State file
        /// </summary>
        /// <param name="doorIndex">  represents which door the player is entering from </param>
        public void Load(int doorIndex)
        {
           
            

            if (filePath != "" && filePath != null)
            {
                try
                {
                    int numberOfDoors;
                    int numberOfEnemies;
                    
                    StreamReader sr = new StreamReader(filePath);
                    SpawnX = Convert.ToInt16(GameResources.getNextDataLine(sr, "#"));
                    SpawnY = Convert.ToInt16(GameResources.getNextDataLine(sr, "#"));
                    spawnLocation = new Vector2(SpawnX, SpawnY);
                    numberOfDoors = Convert.ToInt16(GameResources.getNextDataLine(sr, "#"));
                    doorArray = new Door[numberOfDoors];
                    for (int i = 0; i < numberOfDoors; i++)
                    {
                        
                            doorArray[i].connectedRoom = GameResources.getNextDataLine(sr, "#");
                            doorArray[i].connectedDoor = Convert.ToInt16(GameResources.getNextDataLine(sr, "#"));
                            doorArray[i].isLocked = true;
                    }
                    numberOfEnemies = Convert.ToInt16(GameResources.getNextDataLine(sr, "#"));
                    for (int i = 0; i < numberOfEnemies; i++)
                    {
                        //TODO: GameObjectList What is it and why does enemy need it?  And where do I create it.
                        //enemyArray[i] = new Enemy(GameResources.getNextDataLine(sr, "#"), , Content);
                    }

                    collisionMap = Content.Load<Texture2D>("Images/" + roomName);
                    collisionColors = new Color[collisionMap.Width * collisionMap.Height];
                    collisionMap.GetData<Color>(collisionColors);

                    background = new GameObject(whichRoom(roomName), Content);

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("An error occurred: " + e.Message);
                }
            }

            
            {
            
            }
            //Notes from Ryan:
            //  The Texture2D makes sense for the collision map, but it makes better sense to use the background as a full GameGraphic
            //  Enemy sprites will be loaded inside the Enemy class; after all, they will inherit from GameGraphic as well

        }

        /// <summary>
        /// Iterates through the various objects inside the room and updates their coordinates relative to the player.
        /// </summary>
        /// <param name="playerPosition">The player's position.</param>
        /// <param name="playerCoordinates">The player's coordinates.</param>
        public void UpdateCoordinates(Vector2 playerPosition, Vector2 playerCoordinates)
        {
            background.UpdateCoordinates(playerPosition, playerCoordinates);
            // TODO: Iterate through and update ALL objects
        }

        /// <summary>
        /// Draws the contents of the room to the designated SpriteBatch
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw to.</param>
        public void Draw(SpriteBatch batch, GameTime time)
        {
            background.Draw(batch, time);
            // TODO: Iterate through and update ALL objects
        }

        /// <summary>
        /// The default spawning location if a player spawns inside this room
        /// </summary>
        public Vector2 SpawnLocation
        {
            get { return spawnLocation; }
        }

        /// <summary>
        /// Clears all memory assets related to the room.  Saves to a saved state 
        /// file if IsPersistant is set to true.
        /// </summary>
        public void Dispose()
        {
            if (IsPersistant)
            {
                saveToFile();
            }
            //throw new NotImplementedException("Not implemented.");
            //For this one, it's sufficient for the dispose to simply do nothing. - Ryan
        }


        /// <summary>
        /// Saves room state to file when room IsPersistant.
        /// </summary>
        private void saveToFile()
        {
           
        }

        /// <summary>
        /// Iterates through all the PhysicsObjects that the room is responsible,
        /// returning the next one each time the method is called as long as HasMoreObjects
        /// is true.
        /// </summary>
        /// <returns>The next PhysicsObject in the list.</returns>
        public PhysicsObject GetNextObject()
        {
            if (!hasMoreObjects)
                return null;
            else
            {
                return null; //This is the way we make it compile.
                // TODO: Define a way to iterate through the list of PhysicsObjects that this room is responsible for.
                // Each time this method is called, it should return the NEXT object down the list.
                // ... now that I think about it, you can probably use this structure in your destructor as well when
                // you need to Dispose.
            }
        }

        /// <summary>
        /// Whether there are more objects in the list of PhysicsObjects that need to be iterated through.
        /// </summary>
        public bool HasMoreObjects
        {
            get { return hasMoreObjects; }
        }

        /// <summary>
        /// Resets the iterator for the PhysicsObject list
        /// </summary>
        public void ResetObjectList()
        {
            if (totalObjects > 0)
                hasMoreObjects = true;
            else
                hasMoreObjects = false;
            // TODO: Whatever iterator you have set up, reset it to the first object here.
        }
    }
}
