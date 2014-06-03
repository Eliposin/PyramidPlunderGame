using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pyramid_Plunder.Classes
{
    class HUD
    {
        private const int HP_BAR_X = 100;
        private const int HP_BAR_Y = 10;
        private const int KEYCHAIN_X = 520;
        private const int KEYCHAIN_Y = 15;
        private const float ROOM_DISPLAY_TIME = 3; //In Seconds

        private ContentManager Content;
        private GameGraphic healthBar;
        private GameGraphic keyChain;
        private Texture2D grayTexture;
        private Texture2D redTexture;
        private Texture2D darkGrayTexture;
        private SpriteFont roomFont;
        private SpriteFont saveFont;

        private string roomDisplay;
        private bool displayingRoom;
        private double roomDisplayTime;

        private string saveIndicator;
        private bool displayingSave;
        private double saveDisplayTime;

        private float currentHP;
        private bool isVisible;
        private bool[] keyArray;

        /// <summary>
        /// Creates a new HUD object.
        /// </summary>
        /// <param name="content">The content manager to use for assets.</param>
        /// <param name="player">The player to attack this HUD to.</param>
        public HUD(ContentManager content, Player player)
        {
            Content = content;

            healthBar = new GameGraphic("HealthBar", Content);
            keyChain = new GameGraphic("KeyChain", Content);

            grayTexture = new Texture2D(GameResources.Device, 1, 1);
            grayTexture.SetData(new[] { Color.Gray });

            redTexture = new Texture2D(GameResources.Device, 1, 1);
            redTexture.SetData(new[] { Color.Red });

            darkGrayTexture = new Texture2D(GameResources.Device, 1, 1);
            darkGrayTexture.SetData(new[] { new Color(102, 102, 102) });

            healthBar.Coordinates = new Vector2(HP_BAR_X, HP_BAR_Y);
            keyChain.Coordinates = new Vector2(KEYCHAIN_X, KEYCHAIN_Y);

            roomFont = Content.Load<SpriteFont>("Fonts/Philo42");
            saveFont = Content.Load<SpriteFont>("Fonts/Philo18");

            keyArray = new bool[1];
            for (int i = 0; i < keyArray.Length; i++)
                keyArray[i] = false;
                
        }

        /// <summary>
        /// Draws the HUD to the designated spritebatch.
        /// </summary>
        /// <param name="batch">The spritebatch to draw to.</param>
        /// <param name="time">The gametime to use.</param>
        public void Draw(SpriteBatch batch, GameTime time)
        {
            batch.Draw(darkGrayTexture, new Rectangle(0, 0, 1280, 60), Color.White);
            DrawHealthBar(batch, time);
            DrawKeys(batch, time);
            
            if (displayingRoom)
            {
                roomDisplayTime += time.ElapsedGameTime.TotalSeconds;
                batch.DrawString(roomFont, roomDisplay, new Vector2(700, 0), Color.DarkBlue);
                if (roomDisplayTime >= ROOM_DISPLAY_TIME)
                    displayingRoom = false;
            }

            if (displayingSave)
            {
                saveDisplayTime += time.ElapsedGameTime.TotalSeconds;
                batch.DrawString(saveFont, saveIndicator, new Vector2(525, 70), Color.Black);
                if (saveDisplayTime >= ROOM_DISPLAY_TIME)
                    displayingSave = false;
            }
        }

        /// <summary>
        /// Update the state of the HUD based on the state of the player.
        /// </summary>
        /// <param name="time">The GameTime to use.</param>
        /// <param name="player">The player to reference.</param>
        public void Update(GameTime time, Player player)
        {
            currentHP = (float)player.CurrentHealth / (float)player.MaximumHealth;

            for (int i = 0; i < keyArray.Length; i++)
            {
                if (keyArray[i] == false && player.CurrentItems[i + 1] == true)
                    keyArray[i] = true;
            }
        }

        private void DrawHealthBar(SpriteBatch batch, GameTime time)
        {
            batch.Draw(grayTexture, new Rectangle((int)healthBar.Coordinates.X, (int)healthBar.Coordinates.Y,
                healthBar.HitBox.Width, healthBar.HitBox.Height), Color.White);

            batch.Draw(redTexture, new Rectangle((int)healthBar.Coordinates.X, (int)healthBar.Coordinates.Y,
                (int)(currentHP * healthBar.HitBox.Width), healthBar.HitBox.Height), Color.White);

            healthBar.Draw(batch, time);
        }

        private void DrawKeys(SpriteBatch batch, GameTime time)
        {
            batch.Draw(grayTexture, new Rectangle((int)keyChain.Coordinates.X, (int)keyChain.Coordinates.Y,
                keyChain.HitBox.Width, keyChain.HitBox.Height), Color.White);

            for (int i = 0; i < keyArray.Length; i++)
            {
                if (keyArray[i] == true)
                {
                    Texture2D color = null;
                    switch (i)
                    {
                        case 0:
                            color = redTexture;
                            break;
                    }
                    if (color != null)
                        batch.Draw(color, new Rectangle((int)keyChain.Coordinates.X + 5 + (i * 19),
                            (int)keyChain.Coordinates.Y + 1, 14, 28), Color.White);
                }
            }

            keyChain.Draw(batch, time);
        }

        public void DisplayRoomName(string longName)
        {
            roomDisplay = longName;
            roomDisplayTime = 0;
            displayingRoom = true;
        }

        public void DisplaySaveIndicator(string saveString)
        {
            saveIndicator = saveString;
            saveDisplayTime = 0;
            displayingSave = true;
        }


        /// <summary>
        /// Whether or not the HUD should be displayed.
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }
    }
}
