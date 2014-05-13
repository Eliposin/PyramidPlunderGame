using System;
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
                    PhysicsEngine.Update(player, currentRoom); //Checks for collisions and modifies Velocity
                    player.Move(); //Actually sets the new position of the object

                    //Do the same thing for each physics object in the current room.
                    while (currentRoom.HasMoreObjects)
                    {
                        PhysicsObject tempObject = currentRoom.GetNextObject();
                        PhysicsEngine.Update(tempObject, currentRoom);
                        tempObject.Move();
                    }
                    currentRoom.ResetObjectList();

                    //Finally, update the drawing position of the objects in the room.
                    currentRoom.UpdateCoordinates(player.Position, player.Coordinates);
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
                currentRoom.Draw(spriteBatch, time);
                player.Draw(spriteBatch, time);
            }
        }

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

        private void LoadGameSettings()
        {

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

            currentRoom = new Room("TestRoom", roomContent);
            player = new Player(GameObjectList.Player, gameContent);
            player.Spawn(currentRoom.SpawnLocation);
            
            isPaused = false;
            inGame = true;
        }
    }
}
