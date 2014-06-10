﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pyramid_Plunder.Classes
{
    public class PauseMenu : GameMenu
    {
        private MenuNode ResumeButton;
        private MenuNode LoadButton;
        private MenuNode OptionsButton;
        private MenuNode QuitButton;

        private DelMenu callback;

        private GameManager.GameSettings gameSettings;

        private OptionsMenu optionsMenu;
        private bool isOptionsDisplayed;
        private bool isPaused;

        public PauseMenu(DelMenu c, GameManager.GameSettings settings)
        {
            gameSettings = settings;
            callback = c;
            InitializeButtons();
            font = Content.Load<SpriteFont>("Fonts/MenuFont");
            background = new GameGraphic("OptionsMenuBackground", Content);

            selectedNode = ResumeButton;
            ResumeButton.IsSelected = true;

            nodeList = new List<MenuNode>();
            nodeList.Add(ResumeButton);
            nodeList.Add(LoadButton);
            nodeList.Add(OptionsButton);
            nodeList.Add(QuitButton);

            hasFocus = true;
            isPaused = true;
            isOptionsDisplayed = false;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            
            if (isOptionsDisplayed)
                optionsMenu.Draw(spriteBatch, gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (isOptionsDisplayed)
            {
                if (optionsMenu.HasFocus)
                    optionsMenu.Update(gameTime);
                else
                    DisposeOptions();
            }

            base.Update(gameTime);
        }

        protected override void InitializeButtons()
        {
            ResumeButton = new MenuNode();
            LoadButton = new MenuNode();
            OptionsButton = new MenuNode();
            QuitButton = new MenuNode();


            ResumeButton.Name = "Resume Game";
            ResumeButton.Selectable = true;
            ResumeButton.IsSelected = false;
            ResumeButton.Position = new Vector2(400, 100);
            ResumeButton.Up = QuitButton;
            ResumeButton.Down = LoadButton;
            ResumeButton.Left = null;
            ResumeButton.Right = null;

            LoadButton.Name = "Load Saved Game";
            LoadButton.Selectable = true;
            LoadButton.IsSelected = false;
            LoadButton.Position = new Vector2(400, 200);
            LoadButton.Up = ResumeButton;
            LoadButton.Down = OptionsButton;
            LoadButton.Left = null;
            LoadButton.Right = null;

            OptionsButton.Name = "Options...";
            OptionsButton.Selectable = true;
            OptionsButton.IsSelected = false;
            OptionsButton.Position = new Vector2(400, 300);
            OptionsButton.Up = LoadButton;
            OptionsButton.Down = QuitButton;
            OptionsButton.Left = null;
            OptionsButton.Right = null;

            QuitButton.Name = "Quit";
            QuitButton.Selectable = true;
            QuitButton.IsSelected = false;
            QuitButton.Position = new Vector2(400, 400);
            QuitButton.Up = OptionsButton;
            QuitButton.Down = ResumeButton;
            QuitButton.Left = null;
            QuitButton.Right = null;
        }

        protected override void Select(MenuNode button)
        {
            if (button == ResumeButton)
            {
                isPaused = false;
            }
            else if (button == LoadButton)
                callback(MenuCallbacks.LoadGame);
            else if (button == QuitButton)
                callback(MenuCallbacks.Quit);
            else if (button == OptionsButton)
            {
                optionsMenu = new OptionsMenu(gameSettings);
                isOptionsDisplayed = true;
                hasFocus = false;
            }
        }

        private void DisposeOptions()
        {
            isOptionsDisplayed = false;
            hasFocus = true;
            optionsMenu.Dispose();
            optionsMenu = null;
        }

        public bool IsPaused
        {
            get { return isPaused; }
        }
    }
}
