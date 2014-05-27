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
        private const int HP_BAR_X = 440;
        private const int HP_BAR_Y = 50;
        private const int KEYCHAIN_X = 590;
        private const int KEYCHAIN_Y = 100;

        private ContentManager Content;
        private GameGraphic healthBar;
        private GameGraphic keyChain;
        private Texture2D grayTexture;
        private Texture2D redTexture;

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

            healthBar.Coordinates = new Vector2(HP_BAR_X, HP_BAR_Y);
            keyChain.Coordinates = new Vector2(KEYCHAIN_X, KEYCHAIN_Y);

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
            DrawHealthBar(batch, time);
            DrawKeys(batch, time);
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
                (int)currentHP * healthBar.HitBox.Width, healthBar.HitBox.Height), Color.White);

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
                        //case 1:
                        //    color = blueTexture;
                        //case 2:
                        //    color = yellowTexture;
                        //case 3:
                        //    color = orangeTexture;
                    }
                    if (color != null)
                        batch.Draw(color, new Rectangle((int)keyChain.Coordinates.X + 5 + (i * 19),
                            (int)keyChain.Coordinates.Y + 1, 14, 28), Color.White);
                }
            }

            keyChain.Draw(batch, time);
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
