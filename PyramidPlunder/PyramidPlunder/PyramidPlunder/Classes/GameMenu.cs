using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Pyramid_Plunder.Classes
{
    public abstract class GameMenu
    {
        public const float THUMBSTICK_THRESHOLD = 0.3f;

        /// <summary>
        /// A simple class representing a selectable button or node
        /// </summary>
        public class MenuNode
        {
            public MenuNode Up;
            public MenuNode Down;
            public MenuNode Left;
            public MenuNode Right;

            public Vector2 Position;
            public String Name;
            public bool Selectable;
            public bool IsSelected;
            
            public void Draw(SpriteBatch spriteBatch, SpriteFont font, Color color)
            {
                if (Name != null && Position != null)
                    spriteBatch.DrawString(font, Name, Position, color);
            }
        }

        private enum Directions : byte
        {
            Up, Down, Left, Right
        }

        protected MenuNode selectedNode;

        protected ContentManager Content;
        protected AudioEngine soundEngine;
        protected GameGraphic background;
        protected List<MenuNode> nodeList;
        protected SpriteFont font;
        protected bool hasFocus;

        protected Color unselectedColor = Color.White;
        protected Color selectedColor = Color.Yellow;
        protected Color unavailableColor = Color.Gray;

        protected KeyboardState newKeyState;
        protected KeyboardState oldKeyState;
        protected GamePadState newGamePadState;
        protected GamePadState oldGamePadState;

        

        public GameMenu()
        {
            Content = new ContentManager(GameResources.GameServices, "Content");
            soundEngine = new AudioEngine(Content, "Menu");

            newKeyState = Keyboard.GetState();
            newGamePadState = GamePad.GetState(PlayerIndex.One);
            oldKeyState = Keyboard.GetState();
            oldGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //if (hasFocus)
            {
                if (background != null)
                    background.Draw(spriteBatch, gameTime);

                foreach (MenuNode node in nodeList)
                {
                    if (node.IsSelected)
                        node.Draw(spriteBatch, font, selectedColor);
                    else
                        node.Draw(spriteBatch, font, unselectedColor);
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            newKeyState = Keyboard.GetState();
            newGamePadState = GamePad.GetState(PlayerIndex.One);

            if (hasFocus)
            {
                if (GameResources.CheckInputButton(Keys.Up, Buttons.DPadUp, oldKeyState, newKeyState, oldGamePadState, newGamePadState) ||
                    (newGamePadState.ThumbSticks.Left.Y >= THUMBSTICK_THRESHOLD && oldGamePadState.ThumbSticks.Left.Y < THUMBSTICK_THRESHOLD))
                    MoveSelection(Directions.Up);
                else if (GameResources.CheckInputButton(Keys.Down, Buttons.DPadDown, oldKeyState, newKeyState, oldGamePadState, newGamePadState) ||
                    (newGamePadState.ThumbSticks.Left.Y <= -THUMBSTICK_THRESHOLD && oldGamePadState.ThumbSticks.Left.Y > -THUMBSTICK_THRESHOLD))
                    MoveSelection(Directions.Down);

                if (GameResources.CheckInputButton(Keys.Left, Buttons.DPadLeft, oldKeyState, newKeyState, oldGamePadState, newGamePadState) ||
                    (newGamePadState.ThumbSticks.Left.X < -THUMBSTICK_THRESHOLD && oldGamePadState.ThumbSticks.Left.X >= -THUMBSTICK_THRESHOLD))
                    MoveSelection(Directions.Left);
                else if (GameResources.CheckInputButton(Keys.Right, Buttons.DPadRight, oldKeyState, newKeyState, oldGamePadState, newGamePadState) ||
                    (newGamePadState.ThumbSticks.Left.X > THUMBSTICK_THRESHOLD && oldGamePadState.ThumbSticks.Left.X <= THUMBSTICK_THRESHOLD))
                    MoveSelection(Directions.Right);
                
                if (GameResources.CheckInputButton(Keys.Enter, Buttons.A, oldKeyState, newKeyState, oldGamePadState, newGamePadState))
                    Select(selectedNode);
            }

            oldKeyState = newKeyState;
            oldGamePadState = newGamePadState;
        }

        public virtual void Dispose()
        {
            Content.Unload();
            Content.Dispose();
            Content = null;
        }

        private void MoveSelection(Directions direction)
        {
            switch (direction)
            {
                case Directions.Up:
                    if (selectedNode.Up != null)
                    {
                        selectedNode.IsSelected = false;
                        selectedNode = selectedNode.Up;
                        selectedNode.IsSelected = true;
                        soundEngine.Play(AudioEngine.SoundEffects.MenuClick);
                    }
                    break;
                case Directions.Down:
                    if (selectedNode.Down != null)
                    {
                        selectedNode.IsSelected = false;
                        selectedNode = selectedNode.Down;
                        selectedNode.IsSelected = true;
                        soundEngine.Play(AudioEngine.SoundEffects.MenuClick);
                    }
                    break;
                case Directions.Left:
                    if (selectedNode.Left != null)
                    {
                        selectedNode.IsSelected = false;
                        selectedNode = selectedNode.Left;
                        selectedNode.IsSelected = true;
                        soundEngine.Play(AudioEngine.SoundEffects.MenuClick);
                    }
                    break;
                case Directions.Right:
                    if (selectedNode.Right != null)
                    {
                        selectedNode.IsSelected = false;
                        selectedNode = selectedNode.Right;
                        selectedNode.IsSelected = true;
                        soundEngine.Play(AudioEngine.SoundEffects.MenuClick);
                    }
                    break;
            }
        }

        public bool HasFocus
        {
            get { return hasFocus; }
            set { hasFocus = value; }
        }

        protected abstract void InitializeButtons();
        protected abstract void Select(MenuNode button);
    }
}
