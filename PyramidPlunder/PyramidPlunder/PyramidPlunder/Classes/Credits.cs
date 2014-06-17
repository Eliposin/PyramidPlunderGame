using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Pyramid_Plunder.Classes
{
    class Credits
    {
        private const double MAX_TIME = 27; //In total seconds
        private const double CREDITS_TRAVEL_SPEED = 72; //In pixels-per-second

        private ContentManager Content;
        private Texture2D blackTexture;
        private Rectangle backgroundRectangle;
        private SpriteFont font;
        private SpriteFont biggerFont;
        private GameGraphic mummy;

        private KeyboardState newKeyState;
        private KeyboardState oldKeyState;
        private GamePadState newGPState;
        private GamePadState oldGPState;

        private float basePosition;
        private string jobList;
        private string nameList;
        private bool hasFocus;
        private double currentTime;

        public Credits()
        {
            Content = new ContentManager(GameResources.GameServices, "Content");
            backgroundRectangle = new Rectangle(0, 0, 1280, 720);

            blackTexture = new Texture2D(GameResources.Device, 1, 1);
            blackTexture.SetData(new[] { Color.Black });

            font = Content.Load<SpriteFont>("Fonts/Philo14");
            biggerFont = Content.Load<SpriteFont>("Fonts/Philo24");

            mummy = new GameGraphic("Mummy", Content);

            hasFocus = true;

            basePosition = 720;

            newKeyState = Keyboard.GetState();
            oldKeyState = Keyboard.GetState();
            oldGPState = GamePad.GetState(PlayerIndex.One);
            newGPState = GamePad.GetState(PlayerIndex.One);

            InitializeCredits();
        }

        private void InitializeCredits()
        {
            jobList = "Pyramid Plunder\n" +
                      "Edmonds Community College CS185\n\n\n" +
                      "Creator\n\n\n" +
                      "Executive Producer\n\n\n" +
                      "Lead Designer\n\n\n" +
                      "Lead Developer\n\n\n" +
                      "Character Artist\n\n\n" +
                      "Sound SFX Artst\n\n\n" +
                      "Other Designers\n\n\n\n\n\n\n" +
                      "Other Developers\n\n\n\n\n\n\n" +
                      "Xbox 360 Expert\n\n\n" +
                      "GitHub Dealie-Person\n\n\n";
            nameList = "\n\n\n\n" +
                       "Jon Ekdahl\n\n\n" +
                       "Ryan Berge\n\n\n" +
                       "Jon Ekdahl\n\n\n" +
                       "Ryan Berge\n\n\n" +
                       "Huy Ngo\n\n\n" +
                       "Brandon Lasater\n\n\n" +
                       "Josh Stratton\nBradley Pellegrini\nHuy Ngo\nBrandon Lasater\nRyan Berge\n\n\n" +
                       "Josh Stratton\nBradley Pellegrini\nHuy Ngo\nBrandon Lasater\nJon Ekdahl\n\n\n" +
                       "Brandon Lasater\n\n\n" +
                       "Bradley Pellegrini";
        }

        public void Update(GameTime gameTime)
        {
            newKeyState = Keyboard.GetState();
            newGPState = GamePad.GetState(PlayerIndex.One);

            currentTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (currentTime >= MAX_TIME)
                hasFocus = false;
            else
            {
                basePosition -= (float)(CREDITS_TRAVEL_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
                mummy.Coordinates = new Vector2(800, basePosition + 1100);
            }

            if (GameResources.CheckInputButton(Keys.Escape, Buttons.B, oldKeyState, newKeyState, oldGPState, newGPState))
            {
                hasFocus = false;
            }

            oldKeyState = newKeyState;
            oldGPState = newGPState;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(blackTexture, backgroundRectangle, Color.White);

            spriteBatch.DrawString(font, jobList, new Vector2(400, basePosition), Color.White);
            spriteBatch.DrawString(font, nameList, new Vector2(700, basePosition), Color.White);

            spriteBatch.DrawString(biggerFont, "Special Thanks to John Chenoweth", new Vector2(390, basePosition + 900), Color.White);
            spriteBatch.DrawString(biggerFont, "For his original soundtrack", new Vector2(440, basePosition + 940), Color.White);

            spriteBatch.DrawString(biggerFont, "And to Tim Cleavenger", new Vector2(470, basePosition + 1050), Color.White);
            spriteBatch.DrawString(biggerFont, "For his mummy thing", new Vector2(480, basePosition + 1090), Color.White);

            mummy.Draw(spriteBatch, gameTime);
        }

        public void Dispose()
        {
            Content.Unload();
            Content = null;
        }

        public bool HasFocus
        {
            get { return hasFocus; }
        }
    }
}
