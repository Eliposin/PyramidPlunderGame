using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pyramid_Plunder.Classes
{
    class Options
    {
        public const float THUMBSTICK_THRESHOLD = 0.3f;
        protected GamePadState gp, oldGp;
        protected KeyboardState keys, oldKeys;
        public bool debugMode;
        bool paused;
        DelMenu mc;
        Texture2D optBackground;
        int menuTemp, menuSelect;
        Vector2[] vecOptions = new Vector2[5];
        private AudioEngine soundEngine;
        //its necessary to use two different options menus because you can't resume a paused game from the main menu
        //only from a pause menu
        string[] pauseOptions = { "Resume", "Debug Mode = Off", "Volume Up", "Volume Down", "Exit" };
        string[] menuOptions = { "Main Menu", "Debug Mode = Off", "Volume Up", "Volume Down", "Exit" };
        Color colorSelected = Color.Red;
        SpriteFont optionsFont;
        //default struct
        public Options() { }
        //struct to pull necessary information
        public Options(SpriteFont sf, int temp, DelMenu menuC, Texture2D t2d)
        {
            //gS = GameStates.Options;
            optBackground = t2d;
            optionsFont = sf;
            menuTemp = temp;
            LoadMenuButtons();
            mc = menuC;
        }
        private void LoadMenuButtons()
        {
            vecOptions = new Vector2[pauseOptions.Length];
            for (int i = 0; i < vecOptions.Length; i++)
                vecOptions[i] = new Vector2(375, 100 + (50 * i));
        }
        public void Update(GameTime gameTime)
        {
            keys = Keyboard.GetState();
            gp = GamePad.GetState(PlayerIndex.One);

          
                menuSelect = MenuOptionsSelect();
                if (menuSelect == 0)
                {
                    
                }
                else if (menuSelect == 1 && keys.IsKeyDown(Keys.Enter) && oldKeys.IsKeyDown(Keys.Enter)
                    || gp.IsButtonDown(Buttons.A) && oldGp.IsButtonUp(Buttons.A))
                {
                    if (debugMode == true)
                    {
                        debugMode = false;
                        pauseOptions[1] = "Debug Mode = Off";
                    }
                    else
                    {
                        debugMode = true;
                        pauseOptions[1] = "Debug Mode = On";
                    }
                    
                }
                //these next two else if statements are for volume changing
                else if (menuSelect == 2)
                {
                }
                else if (menuSelect == 3)
                {
                }
                //exit call
                else if (menuSelect == 4 && keys.IsKeyDown(Keys.Enter) && oldKeys.IsKeyDown(Keys.Enter)
                    || gp.IsButtonDown(Buttons.A) && oldGp.IsButtonUp(Buttons.A))
                {
                    mc(MenuCallbacks.Quit);
                }
            

            oldGp = gp;
            oldKeys = keys;
        }
       
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(optBackground, new Rectangle(0, 0, 1280, 720), Color.White);
                for (int i = 0; i < vecOptions.Length; i++)
                {
                    if (i == menuSelect)
                    {
                        spriteBatch.DrawString(optionsFont, pauseOptions[i], vecOptions[i], colorSelected);
                    }
                    else spriteBatch.DrawString(optionsFont, pauseOptions[i], vecOptions[i], Color.Black);
                }
            
        }
        protected int MenuOptionsSelect()
        {

            if ((keys.IsKeyDown(Keys.Down) && oldKeys.IsKeyUp(Keys.Down)) ||
                (gp.IsButtonDown(Buttons.DPadDown) && oldGp.IsButtonUp(Buttons.DPadDown)) ||
                (gp.ThumbSticks.Left.Y <= -THUMBSTICK_THRESHOLD && oldGp.ThumbSticks.Left.Y > -THUMBSTICK_THRESHOLD))
            {
                menuTemp++;
                if (menuTemp >= vecOptions.Length) menuTemp = 0;
                //soundEngine.Play(AudioEngine.SoundEffects.MenuClick);
            }

            else if ((keys.IsKeyDown(Keys.Up) && oldKeys.IsKeyUp(Keys.Up)) ||
                (gp.IsButtonDown(Buttons.DPadUp) && oldGp.IsButtonUp(Buttons.DPadUp)) ||
                (gp.ThumbSticks.Left.Y >= THUMBSTICK_THRESHOLD && oldGp.ThumbSticks.Left.Y < THUMBSTICK_THRESHOLD))
            {
                menuTemp--;
                if (menuTemp < 0) menuTemp = vecOptions.Length - 1;
                //soundEngine.Play(AudioEngine.SoundEffects.MenuClick);
            }

            return menuTemp;
        }
    }
}
