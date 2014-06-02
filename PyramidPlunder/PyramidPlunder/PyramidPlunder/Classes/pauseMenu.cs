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
    public class pauseMenu : Menu
    {
        public bool isPaused = false;
        DelMenu menuCallBack;
        ContentManager content;
        string[] pauseOptions = { "Resume", "Options", "Exit" };


        public pauseMenu(DelMenu menuC)
        {
            menuCallBack = menuC;
            LoadMenuButtons();
            //Load();
            content = new ContentManager(GameResources.GameServices, "Content");
            menuFont = content.Load<SpriteFont>("Fonts/OptionsFont");
            
        }
        public override void Load()
        {
            menuTemp = 0;
            
            isPaused = true;
        }

        private void LoadMenuButtons()
        {
            vecMenuOpts = new Vector2[2];
            vecMenuOpts[0] = new Vector2(100, 100);
            vecMenuOpts[1] = new Vector2(100, 200);
        }

        public new void Update(GameTime gameTime)
        {
            keys = Keyboard.GetState();
            gp = GamePad.GetState(PlayerIndex.One);
            
            if (gameStates == GameStates.Options)
            {
                menuSelect = MenuOptionsSelect();
                if (menuSelect == 0)
                {
                    isPaused = false;
                    Unload();
                }
                else if (menuSelect == 1)
                {

                }
                else if (menuSelect == 2)
                {
                    Main m1 = new Main();
                    m1.Exit();
                }
                
            }
            oldGp = gp;
            oldKeys = keys;
        }
        public void Unload()
        {
            Content.Dispose();
            Content = null;
        }

        public new void Draw(SpriteBatch spritebatch)
        {
            if (isPaused == true)
            {
                //use an array and a for loop to draw out the menu options
                Color normal = Color.Black;
                if (gameStates == GameStates.Menu)
                {
                    for (int i = 0; i < vecMenuOpts.Length; i++)
                    {
                        if (i == menuSelect)
                        {
                            spritebatch.DrawString(this.menuFont, menuOptionsArray[i], vecMenuOpts[i], ColorSelected);
                        }
                        else spritebatch.DrawString(this.menuFont, menuOptionsArray[i], vecMenuOpts[i], normal);
                    }
                    //spritebatch.DrawString(menuFont, menuSelect.ToString(), new Vector2(300, 100), normal);
                }
            }
        }
    }
}
