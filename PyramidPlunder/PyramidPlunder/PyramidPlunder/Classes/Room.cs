using System;
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
        private Texture2D collisionMap;
        private bool hasMoreObjects;
        private int totalObjects;
        private GameObject background;     //Encapsulate the drawing requirements into the class already designed to do that
                                          //Needs to be a GameObject because it needs to move around with the player
        private string roomName;
        private ContentManager Content;
        
        public Color[] collisionColors;

        /// <summary>
        /// Creates a new Room object.
        /// </summary>
        /// <param name="roomName">The name of the room that is to be created.</param>
        /// <param name="content">The content manager to use for assets.</param>
        /// <param name="doorIndex">The index of the door to enter from.  If no index is specified, or the index is -1,
        /// the room's default spawn location will be used.</param>
        public Room(String roomName, ContentManager content, int doorIndex = -1)
        {
            this.roomName = roomName;
            Content = content;
            Load("../Data/Rooms/" + roomName + ".room", doorIndex);
            
            collisionMap = Content.Load<Texture2D>("Images/" + roomName + "Collisions");
            collisionColors = new Color[collisionMap.Width * collisionMap.Height];
            collisionMap.GetData<Color>(collisionColors);
            background = new GameObject(whichRoom(roomName), Content);
            background.Spawn(new Vector2(0, 0));
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
                    //spawnLocation = new Vector2(1100, 900); //Eventually should be read from file!
                    //totalObjects = 0;
                    //hasMoreObjects = false;
                    return GameObjectList.TestRoom;
                case "SaveRoom":
                    return GameObjectList.SaveRoom;
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
        /// <param name="doorIndex">Represents which door the player is entering from.
        /// If set to -1, player will enter at the default spawning location.</param>
        public void Load(string filePath, int doorIndex)
        {
            if (filePath != "" && filePath != null)
            {
                try
                {
                    int numberOfDoors;
                    int numberOfEnemies;
                    
                    StreamReader sr = new StreamReader(filePath);
                    spawnLocation = new Vector2(Convert.ToInt16(GameResources.getNextDataLine(sr, "#")), 
                                                Convert.ToInt16(GameResources.getNextDataLine(sr, "#")));
                    totalObjects = Convert.ToInt16(GameResources.getNextDataLine(sr, "#"));

                    if (totalObjects > 0)
                        hasMoreObjects = true;
                    else
                        hasMoreObjects = false;

                    numberOfDoors = Convert.ToInt16(GameResources.getNextDataLine(sr, "#"));
                    doorArray = new Door[numberOfDoors];
                    for (int i = 0; i < numberOfDoors; i++)
                    {
                        Door.DoorOrientations orientation = (Door.DoorOrientations)byte.Parse(GameResources.getNextDataLine(sr, "#"));
                        string roomName = GameResources.getNextDataLine(sr, "#");
                        int connectedDoorIndex = Convert.ToInt16(GameResources.getNextDataLine(sr, "#"));
                        Door.LockTypes lockType = (Door.LockTypes)byte.Parse(GameResources.getNextDataLine(sr, "#"));

                        doorArray[i] = new Door(Content, orientation, roomName, connectedDoorIndex, lockType);

                        doorArray[i].Spawn(new Vector2(float.Parse(GameResources.getNextDataLine(sr, "#")),
                                                       float.Parse(GameResources.getNextDataLine(sr, "#"))));
                    }

                    numberOfEnemies = Int16.Parse(GameResources.getNextDataLine(sr, "#"));
                    for (int i = 0; i < numberOfEnemies; i++)
                    {
                        String enemyType = GameResources.getNextDataLine(sr, "#");

                        switch (enemyType)
                        {
                            case "Mummy":
                                enemyArray[i] = new Enemy(GameObjectList.Mummy, Content);
                                break;
                            case "Skeleton":
                                enemyArray[i] = new Enemy(GameObjectList.Skeleton, Content);
                                break;
                            case "Scarab":
                                enemyArray[i] = new Enemy(GameObjectList.Scarab, Content);
                                break;
                        }

                        enemyArray[i].Spawn(new Vector2(Int16.Parse(GameResources.getNextDataLine(sr, "#")),
                            Int16.Parse(GameResources.getNextDataLine(sr, "#"))));

                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("An error occurred: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Iterates through the various objects inside the room and updates their coordinates relative to the player.
        /// </summary>
        /// <param name="playerPosition">The player's position.</param>
        /// <param name="playerCoordinates">The player's coordinates.</param>
        public void UpdateCoordinates(Vector2 playerPosition, Vector2 playerCoordinates, Rectangle roomDimensions)
        {
            background.UpdateCoordinates(playerPosition, playerCoordinates, roomDimensions);
            for (int i = 0; i < doorArray.Length; i++)
                doorArray[i].UpdateCoordinates(playerPosition, playerCoordinates, roomDimensions);
            // TODO: Iterate through and update ALL objects
        }

        /// <summary>
        /// Draws the contents of the room to the designated SpriteBatch
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw to.</param>
        public void Draw(SpriteBatch batch, GameTime time)
        {
            background.Draw(batch, time);
            for (int i = 0; i < doorArray.Length; i++)
                doorArray[i].Draw(batch, time);

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
