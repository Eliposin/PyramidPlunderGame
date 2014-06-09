﻿using System;
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
using PyramidPlunder.Classes;

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
        private bool isDeathScreenUp;
        
        private GameSettings gameSettings;
        private DelVoid exitCallback;

        private KeyboardState oldKeyState;
        private KeyboardState newKeyState;
        private GamePadState oldGamePadState;
        private GamePadState newGamePadState;


        private Menu gameMenu;
        private GameGraphic deathScreen;
        private Room currentRoom;
        private Room oldRoom;
        private Door loadingDoor;
        private Player player;
        private HUD gameHUD;
        private BGM musicManager;
        private InfoBox infoBox;
        private ContentManager gameContent;
        private StorageDevice currentStorageDevice;

        private SpriteFont fpsFont;
        private int fpsCount;
        private float oldCount;
        private int drawCalls;
        private static bool isFrozen;
        private double freezeTimerMax;
        private double freezeTimer;

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

            oldKeyState = Keyboard.GetState();
            newKeyState = Keyboard.GetState();

            oldGamePadState = GamePad.GetState(PlayerIndex.One);
            newGamePadState = GamePad.GetState(PlayerIndex.One);

            LoadGameSettings();

            gameMenu = new Menu("MenuFont", MenuCallback);

            musicManager = new BGM(gContent);

            musicManager.SwitchMusic("Main");

            fpsFont = gameContent.Load<SpriteFont>("Fonts/FPSFont");
            fpsCount = 0;
            oldCount = 0;
            drawCalls = 0;

            isFrozen = false;
            freezeTimer = 0;

            isDeathScreenUp = false;
        }

        /// <summary>
        /// Calls the Update method on all content managed by the GameManager.
        /// </summary>
        /// <param name="gameTime">The GameTime to use when calculating change over time.</param>
        public void Update(GameTime gameTime)
        {
            newKeyState = Keyboard.GetState();
            newGamePadState = GamePad.GetState(PlayerIndex.One);

            if (inGame)
            {
                CheckPaused();
                if (isPaused)
                {

                }
                else
                {
                    if (!isFrozen)
                    {
                        if (player.DeathSequenceEnded)
                        {
                            ShowDeathScreen();
                        }
                        else if (player.IsDead)
                            player.StartDeathSequence();

                        if (player.Position.Y >= currentRoom.CollisionMap.Height + player.HitBox.Height)
                        {
                            currentRoom.Reset();
                            player.ReceivePitFallDamage();
                            player.Position = currentRoom.SpawnLocation;
                        }
                        else if (player.CheckHazards(currentRoom))
                        {
                            currentRoom.Reset();
                            player.ReceiveHazardDamage();
                            player.Position = currentRoom.SpawnLocation;
                        }
                        else
                        {
                            if (player.IsVulnerable)
                                player.DetectEnemyCollisions(currentRoom);
                            player.Update(gameTime); //Determines what the Player is trying to do (this is where the gameTime is taken into account)
                        }

                        for (int i = 0; i < currentRoom.ObjectArray.Length; i++)
                        {
                            if (currentRoom.ObjectArray[i].IsPhysicsObject && currentRoom.ObjectArray[i].IsSpawned &&
                                currentRoom.ObjectArray[i].Position.Y > currentRoom.CollisionMap.Height)
                                currentRoom.ObjectArray[i].Despawn();
                        }

                        //Determine where the room's enemies want to move, (possibly) based on where the player is currently
                        foreach (Enemy enemy in currentRoom.EnemyArray)
                            enemy.Update(gameTime, player);

                        foreach (GameObject obj in currentRoom.EnvironmentArray)
                            obj.Update(gameTime);

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
                            GameObject tempObject = FindInteractionObject(player, InteractionTypes.PlayerAction);
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
                    }
                    else
                    {
                        if (player.CheckLoadedRoom())
                            InfoBoxCallback();

                        if (isDeathScreenUp && GameResources.CheckInputButton(Keys.Enter, Buttons.Start, oldKeyState, newKeyState, oldGamePadState, newGamePadState))
                        {
                            ResetGame();
                            LoadGame();
                        }

                        if (infoBox != null)
                            infoBox.Update(gameTime);
                    }

                    gameHUD.Update(gameTime, player);
                }
            }
            else
            {
                if (gameMenu != null)
                    gameMenu.Update(gameTime);
            }

            oldKeyState = newKeyState;
            oldGamePadState = newGamePadState;

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
                if (isPaused)
                {
                    //pM.Draw(spriteBatch);
                }

                
                    
                currentRoom.DrawBackground(spriteBatch, time, !isFrozen);
                player.Draw(spriteBatch, time, !isFrozen);
                currentRoom.DrawForeground(spriteBatch, time, !isFrozen);
                    
                gameHUD.Draw(spriteBatch, time);

                if (isFrozen && infoBox != null)
                    infoBox.Draw(spriteBatch, time);

                if (isDeathScreenUp)
                    deathScreen.Draw(spriteBatch, time);
            }

            DrawFPS(spriteBatch, time);
            

        }

        public static void ToggleFreeze(bool frozen)
        {
            isFrozen = frozen;
        }

        private void DrawFPS(SpriteBatch spriteBatch, GameTime time)
        {
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
        private GameObject FindInteractionObject(GameObject initiator, InteractionTypes interactionType)
        {
            GameObject nearestObject = currentRoom.GetNearestObject(initiator.InteractionPoint, player.InteractionDistance, interactionType);
            return nearestObject;
        }

        

        /// <summary>
        /// Checks to see if the game is paused.
        /// </summary>
        private void CheckPaused()
        {
            if (!isDeathScreenUp)
            {
                if (GameResources.CheckInputButton(Keys.Escape, Buttons.Start, oldKeyState, newKeyState, oldGamePadState, newGamePadState))
                {
                    if (isPaused)
                    {
                        isPaused = false;
                        isFrozen = false;
                    }
                    else
                    {
                        isFrozen = true;
                        isPaused = true;
                        freezeTimerMax = -1;
                    }
                }
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

        private void ResetGame()
        {
            currentRoom.Dispose();
            player.ResetActionStates(Player.XDirection.Right);
            isDeathScreenUp = false;
            isFrozen = false;
        }

        /// <summary>
        /// Starts a new game, getting rid of the menu, deleting old saves, and opening up the starting room.
        /// </summary>
        private void StartNewGame()
        {
            if (gameMenu != null)
            {
                gameMenu.Dispose();
                gameMenu = null;
            }

            DeleteSave();

            GameResources.RoomSaves = new List<RoomSaveData>();

            musicManager.SwitchMusic("Level");

            currentRoom = new Room("StartRoom", -1);
            player = new Player(gameContent, SaveGame, SwitchRooms, ToggleGameFreeze, HudCallback);
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

                            RoomSaveData[] roomSaves = new RoomSaveData[int.Parse(GameResources.getNextDataLine(sr, "#"))];

                            for (int i = 0; i < roomSaves.Length; i ++)
                            {
                                roomSaves[i].roomName = GameResources.getNextDataLine(sr, "#");
                                bool[] objects = new bool[int.Parse(GameResources.getNextDataLine(sr, "#"))];
                                for (int j = 0; j < objects.Length; j++)
                                    objects[j] = bool.Parse(GameResources.getNextDataLine(sr, "#"));
                                roomSaves[i].objectsAreSpawned = objects;
                            }

                            GameResources.RoomSaves = new List<RoomSaveData>();
                            foreach (RoomSaveData save in roomSaves)
                                GameResources.RoomSaves.Add(save);

                            player = new Player(gameContent, SaveGame, SwitchRooms, ToggleGameFreeze, HudCallback);
                            player.Spawn(currentRoom.SpawnLocation);
                            player.LoadSave(playerHealth, playerItems);
                            gameHUD = new HUD(gameContent, player);

                            isPaused = false;
                            inGame = true;

                            sr.Close();
                        }
                    }

                    musicManager.SwitchMusic(currentRoom.MusicName);

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
            

            try
            {
                List<string> saveData = new List<string>();

                saveData.Add(currentRoom.RoomName);
                saveData.Add(player.CurrentHealth.ToString());
                saveData.Add(player.CurrentItems.Length.ToString());

                foreach (bool item in player.CurrentItems)
                    saveData.Add(item.ToString());

                saveData.Add(GameResources.RoomSaves.Count.ToString());

                foreach (RoomSaveData roomSave in GameResources.RoomSaves)
                {
                    saveData.Add(roomSave.roomName);
                    saveData.Add(roomSave.objectsAreSpawned.Length.ToString());
                    foreach (bool obj in roomSave.objectsAreSpawned)
                        saveData.Add(obj.ToString());
                }

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

                HudCallback("Your game was saved.", false, true);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("There was an error when saving the game: \n" + e.Message);
                gameHUD.DisplaySaveIndicator("Your game was not saved.");
            }
        }

        private void HudCallback(string input, bool pauseMusic, bool removable)
        {
            //gameHUD.DisplayInfo(input);
            infoBox = new InfoBox(input, InfoBoxCallback, removable);
            isFrozen = true;
            if (pauseMusic)
                musicManager.PauseMusic();
        }

        private void InfoBoxCallback()
        {
            infoBox.Dispose();
            infoBox = null;
            isFrozen = false;
            freezeTimerMax = -1;
            freezeTimer = 0;
            musicManager.UnpauseMusic();
        }

        private void ShowDeathScreen()
        {
            isFrozen = true;
            freezeTimerMax = -1;
            deathScreen = new GameGraphic("DeathScreen", gameContent);
            isDeathScreenUp = true;
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

        private void ToggleGameFreeze(bool frozen, double length)
        {
            if (frozen)
            {
                freezeTimerMax = length;
                isFrozen = true;
                musicManager.PauseMusic();
            }
            else
                isFrozen = false;
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
            currentRoom.PlaySoundInstance();


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
