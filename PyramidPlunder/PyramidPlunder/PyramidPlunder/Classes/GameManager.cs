using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Pyramid_Plunder.Classes
{
    /// <summary>
    /// The GameManager class is the top managing class for the game.  Its purpose is to 
    /// handle the structure and flow of the classes and program.
    /// </summary>
    class GameManager
    {
        private bool isPaused;
        private bool inGame;
        private bool isSaving;
        private KeyboardState keyState;
        private GamePadState gamePadState;
        private GameSettings gameSettings;
        private DelVoid exitCallback;

        private Menu gameMenu;
        private Room currentRoom;
        private Room oldRoom;
        private Player player;
        private HUD gameHUD;
        private BGM musicManager;
        private ContentManager gameContent;
        private StorageDevice currentStorageDevice;

        private SpriteFont fpsFont;
        private int fpsCount;
        private float oldCount;
        private int drawCalls;

        private struct GameSettings
        {

        }

        /// <summary>
        /// Constructor call
        /// </summary>
        public GameManager(DelVoid exitDel)
        {
            isPaused = false;
            inGame = false;
            exitCallback = exitDel;
        }

        /// <summary>
        /// Loads the default settings for the game from the settings file
        /// </summary>
        public void Initialize(ContentManager gContent, GameServiceContainer services)
        {
            gameContent = gContent;
            GameResources.GameServices = services;

            keyState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);
            LoadGameSettings();

            gameMenu = new Menu("MenuFont", MenuCallback);


            musicManager = new BGM(gContent);

            fpsFont = gameContent.Load<SpriteFont>("Fonts/FPSFont");
            fpsCount = 0;
            oldCount = 0;
            drawCalls = 0;

        }

        /// <summary>
        /// Calls the Update method on all content managed by the GameManager.
        /// </summary>
        /// <param name="gameTime">The GameTime to use when calculating change over time.</param>
        public void Update(GameTime gameTime)
        {
            if (inGame)
            {
                CheckPaused();
                if (isPaused)
                {

                }
                else
                {
                    player.Update(gameTime); //Determines what the Player is trying to do (this is where the gameTime is taken into account)
                    PhysicsEngine.Update(player, currentRoom, gameTime); //Checks for collisions and modifies Velocity
                    player.Move(); //Actually sets the new position of the object

                    //Do the same thing for each physics object in the current room.
                    for (int i = 0; i < currentRoom.ObjectArray.Length; i++)
                    {
                        if (currentRoom.ObjectArray[i].IsPhysicsObject)
                        {
                            PhysicsEngine.Update((PhysicsObject)currentRoom.ObjectArray[i], currentRoom, gameTime);
                            ((PhysicsObject)currentRoom.ObjectArray[i]).Move();
                        }
                    }

                    //Finally, update the drawing position of the objects in the room.
                    player.UpdateCoordinates(currentRoom.CollisionMap.Bounds);

                    currentRoom.UpdateCoordinates(player.Position, player.Coordinates, currentRoom.CollisionMap.Bounds);
                    player.updateControlFlags(); //new

                    //Check to see if the player is trying to do something
                    if (player.InteractionFlag)
                    {
                        GameObject tempObject = FindInteractionObject(player);
                        if (tempObject != null)
                            player.InteractWith(tempObject, InteractionTypes.PlayerAction);
                    }

                    for (int i = 0; i < currentRoom.ObjectArray.Length; i++)
                    {
                        if (currentRoom.ObjectArray[i].IsSpawned)
                        {
                            if (PhysicsEngine.CheckBoundingBoxCollision(player, currentRoom.ObjectArray[i]))
                                player.InteractWith(currentRoom.ObjectArray[i], InteractionTypes.Collision);
                        }
                    }

                    gameHUD.Update(gameTime, player);
                }
            }
            else
            {
                if (gameMenu != null)
                    gameMenu.Update(gameTime);
            }

            

        }

        /// <summary>
        /// Calls the draw method on all content managed by the GameManager.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime time)
        {
            if (!inGame)
            {
                gameMenu.Draw(spriteBatch);
            }
            else
            {
                currentRoom.DrawBackground(spriteBatch, time);
                player.Draw(spriteBatch, time);
                currentRoom.DrawForeground(spriteBatch, time);

                gameHUD.Draw(spriteBatch, time);
            }

            oldCount += (float)time.ElapsedGameTime.TotalSeconds;
            drawCalls++;
            if (oldCount >= 1)
            {
                fpsCount = drawCalls;
                oldCount -= 1;
                drawCalls = 0;
            }

            spriteBatch.DrawString(fpsFont, "FPS: " + fpsCount, new Vector2(10, 10), new Color(0,255,0));

        }

        /// <summary>
        /// Searches the current room for the nearest valid interactable object and returns it if
        /// one is in range
        /// </summary>
        /// <returns>The nearest object</returns>
        private GameObject FindInteractionObject(GameObject initiator)
        {
            GameObject nearestObject = currentRoom.GetNearestObject(initiator.Position, player.InteractionDistance);
            return nearestObject;
        }

        /// <summary>
        /// Checks to see if the game is paused.
        /// </summary>
        private void CheckPaused()
        {
            KeyboardState tempKeyState = Keyboard.GetState();
            if (tempKeyState.IsKeyDown(Keys.Escape) && keyState.IsKeyUp(Keys.Escape))
            {
                if (isPaused == true)
                    isPaused = false;
                else
                    isPaused = true;
            }
        }

        /// <summary>
        /// Loads the game settings from a file
        /// </summary>
        private void LoadGameSettings()
        {
            // TODO: Actually load the game settings
        }

        /// <summary>
        /// The callback method from the menu class.  Takes the required action.
        /// </summary>
        /// <param name="action">The type of action to take.</param>
        private void MenuCallback(MenuCallbacks action)
        {
            switch (action)
            {
                case MenuCallbacks.NewGame:
                    StartNewGame();
                    break;
                case MenuCallbacks.LoadGame:
                    LoadGame();
                    break;
                case MenuCallbacks.Quit:
                    exitCallback();
                    break;
            }
        }

        /// <summary>
        /// Starts a new game, getting rid of the menu, deleting old saves, and opening up the starting room.
        /// </summary>
        private void StartNewGame()
        {
            gameMenu.Dispose();
            gameMenu = null;

            DeleteSave();

            musicManager.SwitchMusic("Menu");

            currentRoom = new Room("StartRoom", -1);
            player = new Player(gameContent, SaveGame, SwitchRooms);
            player.Spawn(currentRoom.SpawnLocation);
            gameHUD = new HUD(gameContent, player);
            gameHUD.DisplayRoomName(currentRoom.LongName);
            
            isPaused = false;
            inGame = true;
        }

        /// <summary>
        /// Loads the previously saved game
        /// </summary>
        private void LoadGame()
        {
            try
            {
                currentStorageDevice = GetStorageDevice();
                using (StorageContainer container = GetStorageContainer(currentStorageDevice))
                {
                    using (Stream stream = container.OpenFile("SaveGame.txt", FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            string line = GameResources.getNextDataLine(sr, "#");
                            currentRoom = new Room(line, -1);
                            int playerHealth = int.Parse(GameResources.getNextDataLine(sr, "#"));
                            bool[] playerItems = new bool[int.Parse(GameResources.getNextDataLine(sr, "#"))];
                            for (int i = 0; i < playerItems.Length; i++)
                                playerItems[i] = bool.Parse(GameResources.getNextDataLine(sr, "#"));

                            player = new Player(gameContent, SaveGame, SwitchRooms);
                            player.Spawn(currentRoom.SpawnLocation);
                            player.LoadSave(playerHealth, playerItems);
                            gameHUD = new HUD(gameContent, player);

                            isPaused = false;
                            inGame = true;

                            sr.Close();
                        }
                    }

                    musicManager.SwitchMusic("Menu");

                }
            }
            catch (FileNotFoundException e)
            {
                System.Diagnostics.Debug.WriteLine("The save file could not be found: " + e.Message);
                StartNewGame();
            }
        }

        /// <summary>
        /// Saves the game to a file
        /// </summary>
        private void SaveGame()
        {
            isSaving = true;

            string[] saveData = new string[player.CurrentItems.Length + 3];

            saveData[0] = currentRoom.RoomName;
            saveData[1] = player.CurrentHealth.ToString();
            saveData[2] = player.CurrentItems.Length.ToString();

            for (int i = 0; i < player.CurrentItems.Length; i++)
                saveData[i + 3] = player.CurrentItems[i].ToString();

            if (currentStorageDevice == null)
                currentStorageDevice = GetStorageDevice();

            using (StorageContainer container = GetStorageContainer(currentStorageDevice))
            {

                using (StreamWriter file = new StreamWriter(container.CreateFile("SaveGame.txt")))
                {
                    foreach (string line in saveData)
                        file.WriteLine(line);
                }
            }

            isSaving = false;
        }

        /// <summary>
        /// Retrieves the desired storage container for saving or loading data
        /// </summary>
        /// <param name="device">The storage device to search in</param>
        /// <returns>The container to use for saving and loading</returns>
        private StorageContainer GetStorageContainer(StorageDevice device)
        {
                IAsyncResult result = device.BeginOpenContainer("PyramidPlunder", null, null);

                while (!result.IsCompleted)
                    result.AsyncWaitHandle.WaitOne();

                StorageContainer container = device.EndOpenContainer(result);

                result.AsyncWaitHandle.Close();

                return container;
        }

        /// <summary>
        /// Retrieves the desired storage device to open a storage container from
        /// </summary>
        /// <returns></returns>
        private StorageDevice GetStorageDevice()
        {
            #if XBOX 
            if (Guide.IsVisible)
                return null;
            #endif

            try
            {
                IAsyncResult result;

                result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);

                while (!result.IsCompleted)
                    result.AsyncWaitHandle.WaitOne();

                StorageDevice device = StorageDevice.EndShowSelector(result);

                result.AsyncWaitHandle.Close();

                return device;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
            
        }

        /// <summary>
        /// Deletes any saved data files
        /// </summary>
        private void DeleteSave()
        {
            currentStorageDevice = GetStorageDevice();

            using (StorageContainer container = GetStorageContainer(currentStorageDevice))
            {

                string[] fileNames = container.GetFileNames();

                foreach (string file in fileNames)
                    container.DeleteFile(file);
            }
        }

        /// <summary>
        /// Switches the current room to the designated room, unloading the old room.
        /// </summary>
        /// <param name="whichRoom">The room to switch to.</param>
        private void SwitchRooms(Room whichRoom)
        {
            oldRoom = currentRoom;
            currentRoom = whichRoom;

            musicManager.SwitchMusic(currentRoom.MusicName);
            
            player.Spawn(currentRoom.SpawnLocation);
            gameHUD.DisplayRoomName(currentRoom.LongName);

            System.Threading.Thread roomDisposeThread = new System.Threading.Thread(DisposeRoom);
            roomDisposeThread.Start();
        }

        /// <summary>
        /// Removes the previously loaded room from memory.
        /// </summary>
        private void DisposeRoom()
        {
            if (oldRoom != null)
            {
                oldRoom.Dispose();
                oldRoom = null;
            }   
        }
    }
}
