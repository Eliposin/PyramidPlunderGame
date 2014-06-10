using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Pyramid_Plunder.Classes
{
    public class InfoBox
    {
        private ContentManager Content;
        private GameGraphic background;
        private SpriteFont font;
        private DelVoid callback;

        private KeyboardState oldKeyState;
        private KeyboardState newKeyState;
        private GamePadState oldGamePadState;
        private GamePadState newGamePadState;

        private string message;
        private string instruction;
        private bool isRemovable;

        public InfoBox(string m, DelVoid cb, bool removable)
        {
            Content = new ContentManager(GameResources.GameServices, "Content");
            background = new GameGraphic("InfoBox", Content);
            font = Content.Load<SpriteFont>("Fonts/Philo14");
            callback = cb;
            message = m;
            oldKeyState = Keyboard.GetState();
            newKeyState = Keyboard.GetState();
            oldGamePadState = GamePad.GetState(PlayerIndex.One);
            newGamePadState = GamePad.GetState(PlayerIndex.One);

            isRemovable = removable;
            if (isRemovable)
                instruction = "A/Enter: Okay";
            else
                instruction = "";
        }

        public void Update(GameTime gameTime)
        {
            if (isRemovable)
            {
                newKeyState = Keyboard.GetState();
                newGamePadState = GamePad.GetState(PlayerIndex.One);

                if (GameResources.CheckInputButton(Keys.Enter, Buttons.A, oldKeyState, newKeyState, oldGamePadState, newGamePadState))
                    callback();

                oldKeyState = newKeyState;
                oldGamePadState = newGamePadState;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            background.Draw(spriteBatch, gameTime);
            spriteBatch.DrawString(font, message, new Vector2(400, 275), Color.White);
            spriteBatch.DrawString(font, instruction, new Vector2(400, 400), Color.White);
        }

        public void Dispose()
        {
            Content.Unload();
            Content.Dispose();
        }
    }
}
