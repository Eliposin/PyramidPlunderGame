﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        private KeyboardState keyState;
        private GamePadState gamePadState;
        private GameSettings gameSettings;
        private DelVoid exitCallback;

        private Menu gameMenu;
        private Room currentRoom;
        private Player player;

        private ContentManager gameContent;
        private ContentManager roomContent;
        

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
        public void Initialize(ContentManager gContent, ContentManager rContent)
        {
            gameContent = gContent;
            roomContent = rContent;

            keyState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);
            LoadGameSettings();
            gameMenu = new Menu(MenuTypes.Main, MenuCallback, gContent);
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
                    while (currentRoom.HasMoreObjects)
                    {
                        PhysicsObject tempObject = currentRoom.GetNextObject();
                        PhysicsEngine.Update(tempObject, currentRoom, gameTime);
                        tempObject.Move();
                    }
                    currentRoom.ResetObjectList();

                    //Finally, update the drawing position of the objects in the room.
                    //if (CheckRoomBounds(player.Position, currentRoom.CollisionMap.Bounds))
                    player.UpdateCoordinates(currentRoom.CollisionMap.Bounds);

                    currentRoom.UpdateCoordinates(player.Position, player.Coordinates, currentRoom.CollisionMap.Bounds);
                    player.updateControlFlags(); //new

                    if (player.InteractionFlag)
                    {
                        GameObject tempObject = FindInteractionObject(player);
                        if (tempObject != null)
                            player.InteractWith(tempObject, InteractionTypes.PlayerAction);
                    }
                }
            }
            else
            {
                KeyboardState tempKeyState = Keyboard.GetState();
                if (tempKeyState.IsKeyDown(Keys.Enter) && keyState.IsKeyUp(Keys.Enter))
                {
                    if (!inGame)
                        StartNewGame();
                }
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
                //System.Diagnostics.Debug.WriteLine(player.Coordinates);
            }
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
                if (isPaused)
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
                case MenuCallbacks.PlayGame:
                    StartNewGame();
                    break;
                case MenuCallbacks.LoadGame:
                    break;
                case MenuCallbacks.Quit:
                    exitCallback();
                    break;
            }
        }

        private void StartNewGame()
        {
            gameMenu.Dispose();
            gameMenu = null;

            currentRoom = new Room("SaveRoom", roomContent);
            player = new Player(gameContent);
            player.Spawn(currentRoom.SpawnLocation);
            
            isPaused = false;
            inGame = true;
        }
    }
}
