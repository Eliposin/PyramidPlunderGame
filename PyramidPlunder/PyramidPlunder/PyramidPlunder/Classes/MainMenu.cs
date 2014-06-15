using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pyramid_Plunder.Classes
{
    /// <summary>
    /// The main menu for the game.
    /// </summary>
    public class MainMenu : GameMenu
    {
        private MenuNode StartButton;
        private MenuNode LoadButton;
        private MenuNode OptionsButton;
        private MenuNode CreditsButton;
        private MenuNode QuitButton;

        private DelMenu callback;

        private GameManager.GameSettings gameSettings;

        private OptionsMenu optionsMenu;
        private Credits credits;
        private bool isOptionsDisplayed;
        private bool isCreditsDisplayed;

        /// <summary>
        /// Constructor call.
        /// </summary>
        /// <param name="c">The callback method to use</param>
        /// <param name="settings">The game settings to write to</param>
        public MainMenu(DelMenu c, GameManager.GameSettings settings) : base()
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
            nodeList.Add(CreditsButton);
            nodeList.Add(QuitButton);

            hasFocus = true;
            isOptionsDisplayed = false;
        }

        /// <summary>
        /// Draws the menu to the spritebatch
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to draw to</param>
        /// <param name="gameTime">The gametime to use</param>
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            
            if (isOptionsDisplayed)
                optionsMenu.Draw(spriteBatch, gameTime);

            if (isCreditsDisplayed)
                credits.Draw(spriteBatch, gameTime);
        }

        /// <summary>
        /// Updates the state of the menu
        /// </summary>
        /// <param name="gameTime">The gametime to use</param>
        public override void Update(GameTime gameTime)
        {
            if (isOptionsDisplayed)
            {
                if (optionsMenu.HasFocus)
                    optionsMenu.Update(gameTime);
                else
                {
                    isOptionsDisplayed = false;
                    hasFocus = true;
                    optionsMenu.Dispose();
                    optionsMenu = null;
                }
            }

            if (isCreditsDisplayed)
            {
                if (credits.HasFocus)
                    credits.Update(gameTime);
                else
                {
                    isCreditsDisplayed = false;
                    hasFocus = true;
                    credits.Dispose();
                    credits = null;
                    StaticBGM.SwitchMusic("Menu");

                    selectedNode = StartButton;
                    StartButton.IsSelected = true;
                    CreditsButton.IsSelected = false;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Sets up the buttons the menu will use.
        /// </summary>
        protected override void InitializeButtons()
        {
            StartButton = new MenuNode();
            LoadButton = new MenuNode();
            OptionsButton = new MenuNode();
            CreditsButton = new MenuNode();
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
            OptionsButton.Down = CreditsButton;
            OptionsButton.Left = null;
            OptionsButton.Right = null;

            CreditsButton.Name = "Credits";
            CreditsButton.Selectable = true;
            CreditsButton.IsSelected = false;
            CreditsButton.Position = new Vector2(150, 400);
            CreditsButton.Up = OptionsButton;
            CreditsButton.Down = QuitButton;
            CreditsButton.Left = null;
            CreditsButton.Right = null;

            QuitButton.Name = "Quit";
            QuitButton.Selectable = true;
            QuitButton.IsSelected = false;
            QuitButton.Position = new Vector2(150, 500);
            QuitButton.Up = CreditsButton;
            QuitButton.Down = StartButton;
            QuitButton.Left = null;
            QuitButton.Right = null;
        }

        /// <summary>
        /// Determines the action to be taken when a menu option is selected
        /// </summary>
        /// <param name="button">The button that was selected</param>
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
            else if (button == CreditsButton)
            {
                credits = new Credits();
                isCreditsDisplayed = true;
                hasFocus = false;
                StaticBGM.SwitchMusic("Credits");
            }
        }
    }
}
