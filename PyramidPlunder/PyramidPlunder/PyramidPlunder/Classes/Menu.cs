using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Pyramid_Plunder.Classes
{
    public class Menu
    {
        #region
        //private DelMenu menuCallback;
        //private ContentManager Content;

        ///// <summary>
        ///// Constructor call
        ///// </summary>
        ///// <param name="menuType">The type of menu to load.</param>
        ///// <param name="menuCB">The callback method to call.</param>
        //public Menu(MenuTypes menuType, DelMenu menuCB, ContentManager content)
        //{
        //    Content = content;

        //    menuCallback = menuCB;

        //    LoadMenu(menuType);

        //    //menuCallback(MenuCallbacks.LoadGame);
        //}

        //public void Draw(SpriteBatch batch)
        //{

        //}

        ///// <summary>
        ///// Loads the menu based on which type of menu is being loaded.
        ///// </summary>
        ///// <param name="menuType">The type of menu to load.</param>
        //private void LoadMenu(MenuTypes menuType)
        //{

        //}

        ///// <summary>
        ///// Unloads the menu object from memory.
        ///// </summary>
        //public void Dispose()
        //{

        //}
        #endregion
        //for the menu we need to use the keyboard and the 
        //gamepad
        GamePadState gp, oldGp;
        KeyboardState keys, oldKeys;
        //the menu will use a spritefont instead of images
        SpriteFont menuFont;
        //the main is basically the main game class, and since
        //I cannot figure out how to make the menu work with the game 
        //manager class, I am just going to make it work through the main
        //class

        ContentManager Content;

        //For the menu we need to set up different game states, which
        // will be enumerated through the main class
        public GameStates gameStates;

        //this integer will work for choosing the menu option
        int menuSelect;
        //this is the color to show what the current menu option is
        Color ColorSelected = Color.Red;
         //Gonna try to use a vector array to help make things a little bit more simple
        Vector2[] vecMenuOpts = new Vector2[4];
        //this will be used for the menuSelect method
        int menuTemp;
        //set up menu strings
        String[] menuOptionsArray = { "New Game", "Load", "Options", "Exit" };
        //empty struct

        DelMenu menuCallback;

        private bool isRunning;

        //not an empty struct, go back over old code to remember exactly why this is necessary, same with the empty struct
        public Menu(string fontName, DelMenu menuC)
        {

            menuCallback = menuC;

            Content = new ContentManager(GameResources.GameServices, "Content");

            LoadMenuButtons();

            //vecMenuOpts[0] = VecNew;
            //vecMenuOpts[1] = VecLoad;
            //vecMenuOpts[2] = VecOptions;
            //vecMenuOpts[3] = VecExit;
            menuFont = Content.Load<SpriteFont>("Fonts/" + fontName);

            isRunning = true;
        }

        private void LoadMenuButtons()
        {
            try
            {
                StreamReader sr = new StreamReader("../Data/MainMenu.menu");

                String line = GameResources.getNextDataLine(sr, "#");

                int numberOfButtons = Convert.ToInt16(line);

                vecMenuOpts = new Vector2[numberOfButtons];

                for (int i = 0; i < numberOfButtons; i++)
                {
                    vecMenuOpts[i] = new Vector2(int.Parse(GameResources.getNextDataLine(sr, "#")),
                        int.Parse(GameResources.getNextDataLine(sr, "#")));
                }
            }
            catch (FileNotFoundException e)
            {
                System.Diagnostics.Debug.WriteLine("The file could not be found: " + e.Message);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("There was an error: " + e.Message);
            }
        }

        public void Load()
        {
            menuTemp = 0;
            gameStates = GameStates.Menu;
        }

        public void Dispose()
        {
            Content.Unload();
            Content = null;
        }

        public void Update(GameTime gameTime)
        {
            if (isRunning)
            {
                keys = Keyboard.GetState();
                
                #region//Menu
                if (gameStates == GameStates.Menu)
                {
                    menuSelect = MenuOptionsSelect(keys, oldKeys);
                    if (menuSelect == 0 && (keys.IsKeyDown(Keys.Enter) && oldKeys.IsKeyUp(Keys.Enter)))
                    {
                        isRunning = false;
                        menuCallback(MenuCallbacks.NewGame);
                    }
                    else if (menuSelect == 1 && (keys.IsKeyDown(Keys.Enter) && oldKeys.IsKeyUp(Keys.Enter)))
                    {
                        //gameStates = GameStates.LoadGame;
                    }
                    else if (menuSelect == 2 && (keys.IsKeyDown(Keys.Enter) && oldKeys.IsKeyUp(Keys.Enter)))
                    {
                        //gameStates = GameStates.Options;
                    }
                    else if (menuSelect == 3 && (keys.IsKeyDown(Keys.Enter) && oldKeys.IsKeyUp(Keys.Enter)))
                    {
                        menuCallback(MenuCallbacks.Quit);
                    }
                }

                #endregion

                //set the oldKeys equal to keys, this makes it so the key presses only register once per use
                oldKeys = keys;
            }
        }
        public void Draw(SpriteBatch spritebatch)
        {
            if (isRunning)
            {
                //use an array and a for loop to draw out the menu options
                Color normal = Color.Black;
                if (gameStates == GameStates.Menu)
                {
                    for (int i = 0; i < vecMenuOpts.Length; i++)
                    {
                        if (i == menuSelect)
                        {
                            spritebatch.DrawString(menuFont, menuOptionsArray[i], vecMenuOpts[i], ColorSelected);
                        }
                        else spritebatch.DrawString(menuFont, menuOptionsArray[i], vecMenuOpts[i], normal);
                    }
                    spritebatch.DrawString(menuFont, menuSelect.ToString(), new Vector2(300, 100), normal);
                }
            }
        }
        //this Function allows the user to select menu Options using keyboard keys
        //TODO: add in functionality to use gamepad DPad
        private int MenuOptionsSelect(KeyboardState keys, KeyboardState oldKeys)
        {
           
            if (keys.IsKeyDown(Keys.Down) && oldKeys.IsKeyUp(Keys.Down))
            {
                menuTemp++;
                if (menuTemp >= vecMenuOpts.Length) menuTemp = 0;
            }

            else if (keys.IsKeyDown(Keys.Up) && oldKeys.IsKeyUp(Keys.Up))
            {
                menuTemp--;
                if (menuTemp < 0) menuTemp = vecMenuOpts.Length - 1;
            }

            return menuTemp;
        }
    }
}
