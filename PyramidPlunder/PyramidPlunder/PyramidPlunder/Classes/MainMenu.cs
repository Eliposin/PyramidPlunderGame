using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pyramid_Plunder.Classes
{
    public class MainMenu : GameMenu
    {
        private MenuNode StartButton;
        private MenuNode LoadButton;
        private MenuNode OptionsButton;
        private MenuNode QuitButton;

        private DelMenu callback;

        private GameManager.GameSettings gameSettings;

        private OptionsMenu optionsMenu;
        private bool isOptionsDisplayed;

        public MainMenu(DelMenu c, GameManager.GameSettings settings)
        {
            gameSettings = settings;
            callback = c;
            InitializeButtons();
            font = Content.Load<SpriteFont>("Fonts/MenuFont");
            background = new GameGraphic("MainMenuBackground", Content);

            selectedNode = StartButton;
            StartButton.IsSelected = true;

            nodeList = new List<MenuNode>();
            nodeList.Add(StartButton);
            nodeList.Add(LoadButton);
            nodeList.Add(OptionsButton);
            nodeList.Add(QuitButton);

            hasFocus = true;
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
            StartButton = new MenuNode();
            LoadButton = new MenuNode();
            OptionsButton = new MenuNode();
            QuitButton = new MenuNode();


            StartButton.Name = "Start New Game";
            StartButton.Selectable = true;
            StartButton.IsSelected = false;
            StartButton.Position = new Vector2(150, 100);
            StartButton.Up = QuitButton;
            StartButton.Down = LoadButton;
            StartButton.Left = null;
            StartButton.Right = null;

            LoadButton.Name = "Load Saved Game";
            LoadButton.Selectable = true;
            LoadButton.IsSelected = false;
            LoadButton.Position = new Vector2(150, 200);
            LoadButton.Up = StartButton;
            LoadButton.Down = OptionsButton;
            LoadButton.Left = null;
            LoadButton.Right = null;

            OptionsButton.Name = "Options...";
            OptionsButton.Selectable = true;
            OptionsButton.IsSelected = false;
            OptionsButton.Position = new Vector2(150, 300);
            OptionsButton.Up = LoadButton;
            OptionsButton.Down = QuitButton;
            OptionsButton.Left = null;
            OptionsButton.Right = null;

            QuitButton.Name = "Quit";
            QuitButton.Selectable = true;
            QuitButton.IsSelected = false;
            QuitButton.Position = new Vector2(150, 400);
            QuitButton.Up = OptionsButton;
            QuitButton.Down = StartButton;
            QuitButton.Left = null;
            QuitButton.Right = null;
        }

        protected override void Select(MenuNode button)
        {
            if (button == StartButton)
                callback(MenuCallbacks.NewGame);
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
    }
}
