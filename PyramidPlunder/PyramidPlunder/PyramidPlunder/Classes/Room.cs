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
        private GameObject[] environmentArray;
        private GameObject[] objectArray;
        private Vector2 spawnLocation;
        private Texture2D collisionMap;
        private GameObject background;     //Encapsulate the drawing requirements into the class already designed to do that
                                          //Needs to be a GameObject because it needs to move around with the player
        private string roomName;
        private ContentManager Content;
        
        public Color[] collisionColors;

        private bool isPersistant;

        /// <summary>
        /// Creates a new Room object.
        /// </summary>
        /// <param name="roomName">The name of the room that is to be created.</param>
        /// <param name="content">The content manager to use for assets.</param>
        /// <param name="doorIndex">The index of the door to enter from.  If no index is specified, or the index is -1,
        /// the room's default spawn location will be used.</param>
        public Room(String roomName, int doorIndex = -1)
        {
            this.roomName = roomName;
            Content = new ContentManager(GameResources.GetServices(), "Content");
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
                case "StartRoom":
                    return GameObjectList.StartRoom;
                case "SaveRoom":
                    return GameObjectList.SaveRoom;
                case "Vault":
                    return GameObjectList.Vault;
                case "Lobby":
                    return GameObjectList.Lobby;
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

        /// <summary>
        /// Accessor for the EnvironmentArray
        /// </summary>
        public GameObject[] EnvironmentArray
        {
            get { return environmentArray; }
        }

        /// <summary>
        /// Whether or not the room is reset when the player leaves.
        /// </summary>
        public bool IsPersistant
        {
            get { return isPersistant; }
            
            set { isPersistant = value; }

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
                    int numberOfEnvironmentObjects;
                    
                    StreamReader sr = new StreamReader(filePath);
                    spawnLocation = new Vector2(Convert.ToInt16(GameResources.getNextDataLine(sr, "#")), 
                                                Convert.ToInt16(GameResources.getNextDataLine(sr, "#")));

                    isPersistant = bool.Parse(GameResources.getNextDataLine(sr, "#"));

                    numberOfDoors = Convert.ToInt16(GameResources.getNextDataLine(sr, "#"));
                    doorArray = new Door[numberOfDoors];
                    for (int i = 0; i < numberOfDoors; i++)
                    {
                        Door.DoorOrientations orientation = (Door.DoorOrientations)byte.Parse(GameResources.getNextDataLine(sr, "#"));
                        string roomName = GameResources.getNextDataLine(sr, "#");
                        int connectedDoorIndex = Convert.ToInt16(GameResources.getNextDataLine(sr, "#"));
                        Locks lockType = (Locks)byte.Parse(GameResources.getNextDataLine(sr, "#"));

                        doorArray[i] = new Door(Content, orientation, roomName, connectedDoorIndex, lockType);

                        doorArray[i].Spawn(new Vector2(float.Parse(GameResources.getNextDataLine(sr, "#")),
                                                       float.Parse(GameResources.getNextDataLine(sr, "#"))));
                    }

                    numberOfEnemies = Int16.Parse(GameResources.getNextDataLine(sr, "#"));
                    enemyArray = new Enemy[numberOfEnemies];

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

                    numberOfEnvironmentObjects = Int16.Parse(GameResources.getNextDataLine(sr, "#"));
                    environmentArray = new GameObject[numberOfEnvironmentObjects];

                    for (int i = 0; i < numberOfEnvironmentObjects; i++)
                    {
                        String objectType = GameResources.getNextDataLine(sr, "#");
                        switch (objectType)
                        {
                            case "SavePoint":
                                environmentArray[i] = new GameObject(GameObjectList.SavePoint, Content);
                                break;
                            default:
                                break;
                        }
                        environmentArray[i].IsSolid = bool.Parse(GameResources.getNextDataLine(sr, "#"));

                        environmentArray[i].Spawn(new Vector2(Int16.Parse(GameResources.getNextDataLine(sr, "#")),
                            Int16.Parse(GameResources.getNextDataLine(sr, "#"))));
                    }

                    objectArray = new GameObject[numberOfDoors + numberOfEnemies + numberOfEnvironmentObjects];
                    int arrayIndex = 0;

                    for (int i = 0; i < numberOfDoors; i++)
                    {
                        objectArray[i + arrayIndex] = doorArray[i + arrayIndex];
                    }
                    arrayIndex += numberOfDoors;

                    for (int i = 0; i < numberOfEnemies; i++)
                    {
                        objectArray[i + arrayIndex] = enemyArray[i];
                    }
                    arrayIndex += numberOfEnemies;

                    for (int i = 0; i < numberOfEnvironmentObjects; i++)
                    {
                        objectArray[i + arrayIndex] = environmentArray[i];
                    }

                    if (doorIndex != -1)
                    {
                        spawnLocation = doorArray[doorIndex].Position;
                        if (doorArray[doorIndex].Orientation == Door.DoorOrientations.FacingLeft)
                            spawnLocation.X -= Player.PLAYER_WIDTH;
                        else
                            spawnLocation.X += doorArray[doorIndex].HitBox.Width;
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
        /// Draws the parts of the room that should appear in front of the player to the spritebatch.
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw to.</param>
        public void DrawForeground(SpriteBatch batch, GameTime time)
        {
            for (int i = 0; i < doorArray.Length; i++)
                doorArray[i].Draw(batch, time);

            // TODO: Iterate through and update ALL objects
        }

        /// <summary>
        /// Draws the parts of the room that should appear behind the player to the spritebatch.
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw to.</param>
        public void DrawBackground(SpriteBatch batch, GameTime time)
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

            Content.Unload();

            //for (int i = 0; i < objectArray.Length; i++)
            //    objectArray[i].Dispose();
            //background.Dispose();
            //collisionMap.Dispose();
        }

        /// <summary>
        /// Saves room state to file when room IsPersistant.
        /// </summary>
        private void saveToFile()
        {
            //Should I open a new file here or what?  I'm not exactly sure, and if I do
            //where would it go.  Maybe a temp file.
            for(int i = 0; i < doorArray.Length; i++)
            {
                doorArray[i].

            }

            for (int i = 0; i < enemyArray.Length; i++ )
            {
                enemyArray[i]
            }  
        }

        /// <summary>
        /// Finds the nearest GameObject to the given location
        /// </summary>
        /// <param name="location">The location to check from.</param>
        /// <returns>The nearest object</returns>
        public GameObject GetNearestObject(Vector2 location, int maxDistance)
        {
            if ((doorArray.Length == 0) && (enemyArray.Length == 0) && (environmentArray.Length == 0))
                return null;
            else
            {
                GameObject nearestObject = null;
                float closestDistance = 0;
                float tempDistance = 0;
                for (int i = 0; i < doorArray.Length; i++)
                {
                    if (nearestObject == null)
                    {
                        nearestObject = doorArray[i];
                        closestDistance = Vector2.Distance(location, doorArray[i].Position);
                    }
                    
                    
                    tempDistance = Vector2.Distance(location, doorArray[i].Position);
                    if (tempDistance < closestDistance)
                    {
                        nearestObject = doorArray[i];
                        closestDistance = tempDistance;
                    }
                    
                }
                for (int i = 0; i < enemyArray.Length; i++)
                {
                    if (nearestObject == null)
                    {
                        nearestObject = enemyArray[i];
                        closestDistance = Vector2.Distance(location, enemyArray[i].Position);
                    }

                    tempDistance = Vector2.Distance(location, enemyArray[i].Position);
                    if (tempDistance < closestDistance)
                    {
                        nearestObject = enemyArray[i];
                        closestDistance = tempDistance;
                    }

                }
                for (int i = 0; i < environmentArray.Length; i++)
                {
                    if (nearestObject == null)
                    {
                        nearestObject = environmentArray[i];
                        closestDistance = Vector2.Distance(location, environmentArray[i].Position);
                    }

                    tempDistance = Vector2.Distance(location, environmentArray[i].Position);
                    if (tempDistance < closestDistance)
                    {
                        nearestObject = environmentArray[i];
                        closestDistance = tempDistance;
                    }

                }

                if (closestDistance <= maxDistance)
                    return nearestObject;
                else
                    return null;
            }
        }

        /// <summary>
        /// A list of ALL the objects in the current room.
        /// </summary>
        public GameObject[] ObjectArray
        {
            get { return objectArray; }
        }

        /// <summary>
        /// The name of the room.
        /// </summary>
        public string RoomName
        {
            get { return roomName; }
        }
    }
}
