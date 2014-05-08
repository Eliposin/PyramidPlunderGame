using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pyramid_Plunder.Classes
{
    /// <summary>
    /// The GameManager class is the top managing class for the game.  Its purpose is to 
    /// handle the structure and flow of the classes and program.
    /// </summary>
    class GameManager
    {
        private bool isPaused;
        private KeyboardState keyState;
        private GamePadState gamePadState;

        /// <summary>
        /// Constructor call
        /// </summary>
        public GameManager()
        {
            isPaused = false;
        }

        /// <summary>
        /// Loads the default settings for the game from the settings file
        /// </summary>
        public void Initialize()
        {
            keyState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// Calls the Update method on all content managed by the GameManager.
        /// </summary>
        /// <param name="gameTime">The GameTime to use when calculating change over time.</param>
        public void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Calls the draw method on all content managed by the GameManager.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
