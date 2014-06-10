using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pyramid_Plunder.Classes
{
    class OptionsMenu : GameMenu
    {
        GameManager.GameSettings gameSettings;

        private MenuNode BackButton;
        private MenuNode MusicVolume;
        private MenuNode SoundEffectsVolume;

        int musicVolume;
        int soundEffectsVolume;

        public OptionsMenu(GameManager.GameSettings settings)
        {
            gameSettings = settings;
            InitializeButtons();

            font = Content.Load<SpriteFont>("Fonts/MenuFont");
            background = new GameGraphic("OptionsMenuBackground", Content);

            nodeList = new List<MenuNode>();
            nodeList.Add(BackButton);
            nodeList.Add(MusicVolume);
            nodeList.Add(SoundEffectsVolume);

            selectedNode = MusicVolume;
            MusicVolume.IsSelected = true;

            hasFocus = true;

            musicVolume = (int)(gameSettings.MusicVolume * 100);
            soundEffectsVolume = (int)(gameSettings.SoundEffectsVolume * 100);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            spriteBatch.DrawString(font, musicVolume.ToString(), new Vector2(MusicVolume.Position.X + 400, MusicVolume.Position.Y), unselectedColor);
            spriteBatch.DrawString(font, soundEffectsVolume.ToString(), new Vector2(SoundEffectsVolume.Position.X + 400, SoundEffectsVolume.Position.Y), unselectedColor);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (selectedNode == MusicVolume)
            {
                if ((newKeyState.IsKeyDown(Keys.Right) || newGamePadState.IsButtonDown(Buttons.DPadRight) || newGamePadState.ThumbSticks.Left.X > 0) && musicVolume < 100)
                    musicVolume++;

                if ((newKeyState.IsKeyDown(Keys.Left) || newGamePadState.IsButtonDown(Buttons.DPadLeft) || newGamePadState.ThumbSticks.Left.X < 0) && musicVolume > 0)
                    musicVolume--;

                gameSettings.MusicVolume = (float)musicVolume / 100;
                gameSettings.UpdateVolume();
            }
            else if (selectedNode == SoundEffectsVolume)
            {
                if ((newKeyState.IsKeyDown(Keys.Right) || newGamePadState.IsButtonDown(Buttons.DPadRight) || newGamePadState.ThumbSticks.Left.X > 0) && soundEffectsVolume < 100)
                    soundEffectsVolume++;

                if ((newKeyState.IsKeyDown(Keys.Left) || newGamePadState.IsButtonDown(Buttons.DPadLeft) || newGamePadState.ThumbSticks.Left.X < 0) && soundEffectsVolume > 0)
                    soundEffectsVolume--;

                gameSettings.SoundEffectsVolume = (float)soundEffectsVolume / 100;
                gameSettings.UpdateVolume();
            }

            if (newGamePadState.IsButtonDown(Buttons.B) && oldGamePadState.IsButtonDown(Buttons.B))
                Select(BackButton);
        }

        protected override void InitializeButtons()
        {
            MusicVolume = new MenuNode();
            SoundEffectsVolume = new MenuNode();
            BackButton = new MenuNode();

            MusicVolume.Name = "Music";
            MusicVolume.Selectable = true;
            MusicVolume.IsSelected = false;
            MusicVolume.Position = new Vector2(350, 200);
            MusicVolume.Up = BackButton;
            MusicVolume.Down = SoundEffectsVolume;
            MusicVolume.Left = null;
            MusicVolume.Right = null;

            SoundEffectsVolume.Name = "Sound Effects";
            SoundEffectsVolume.Selectable = true;
            SoundEffectsVolume.IsSelected = false;
            SoundEffectsVolume.Position = new Vector2(350, 300);
            SoundEffectsVolume.Up = MusicVolume;
            SoundEffectsVolume.Down = BackButton;
            SoundEffectsVolume.Left = null;
            SoundEffectsVolume.Right = null;
            
            BackButton.Name = "Back";
            BackButton.Selectable = true;
            BackButton.IsSelected = false;
            BackButton.Position = new Vector2(350, 400);
            BackButton.Up = SoundEffectsVolume;
            BackButton.Down = MusicVolume;
            BackButton.Left = null;
            BackButton.Right = null;
        }

        protected override void Select(MenuNode button)
        {
            if (button == BackButton)
                hasFocus = false;
        }
    }
}