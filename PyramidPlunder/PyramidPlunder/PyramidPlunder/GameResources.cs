using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pyramid_Plunder
{
    /// <summary>
    /// A list of the different types of objects the game has.
    /// </summary>
    public enum GameObjectList : byte
    {
        NullObject,
        Player,
        Mummy, Skeleton, Scarab,
        Dash, DoubleJump,
        StartRoom, SaveRoom, Lobby, Vault,
        Door, SavePoint,
        RedKey
    }

    public enum GameStates
    {
        Menu,
        NewGame,
        LoadGame,
        Options,
        Exit
    }

    public enum InteractionTypes : byte
    {
        None,
        Collision,
        Attack,
        PlayerAction
    }

    public enum MenuCallbacks : byte
    {
        NewGame = 0,
        LoadGame = 1,
        Quit = 2
    }

    public enum MenuTypes
    {
        Main = 0,
        Options
    }

    public enum Locks
    {
        Unlocked = 0,
        Red = 1
    }

    public enum ItemList : byte
    {
        NullItem = 0,
        RedKey = 1, BlueKey, YellowKey,
        Dash = 7, DoubleJump = 8, WallJump = 9
    }

    public struct RoomSaveData
    {
        public string roomName;
        public bool[] objectsAreSpawned;
    }

    public delegate void DelVoid();
    public delegate void DelString(string input);
    public delegate void DelSB(string input, bool option);
    public delegate void DelMenu(MenuCallbacks menuCallback);
    public delegate void DelRoom(Classes.Room whichRoom);
    public delegate void DelFreeze(bool frozen, double length);

    

    public static class GameResources
    {
        public const int NUM_ITEMS = 10;
        

        private static GameServiceContainer gameServices;
        private static GraphicsDevice graphicsDevice;

        private static List<RoomSaveData> roomSaves = new List<RoomSaveData>();

        /// <summary>
        /// Returns the next line in the given stream that does not start with the designated delimiter.
        /// </summary>
        /// <param name="line">The line to check and read into.</param>
        /// <param name="sr">The StreamReader to read from.</param>
        /// <param name="delimiter">The delimiter to check at the start of the line.</param>
        public static String getNextDataLine(StreamReader sr, String delimiter)
        {
            String line;
            do
                line = sr.ReadLine();
            while (line.Substring(0).StartsWith(delimiter));
            return line;
        }

        /// <summary>
        /// Checks a button to see if it has been newly pressed since the last update.
        /// </summary>
        /// <param name="key">The key on the keyboard to check.</param>
        /// <param name="button">The button on the gamepad to check.</param>
        /// <param name="oldKeyState">The old keyboard state to check against.</param>
        /// <param name="newKeyState">The new keyboard state to check against.</param>
        /// <param name="oldGamePadState">The old gamepad state to check against.</param>
        /// <param name="newGamePadState">The new gamepad state to check against.</param>
        /// <returns></returns>
        public static bool CheckInputButton(Keys key, Buttons button, KeyboardState oldKeyState, KeyboardState newKeyState,
            GamePadState oldGamePadState, GamePadState newGamePadState)
        {
            if ((newKeyState.IsKeyDown(key) && oldKeyState.IsKeyUp(key)) ||
                newGamePadState.IsButtonDown(button) && oldGamePadState.IsButtonUp(button))
                return true;
            else
                return false;
        }

        public static GameServiceContainer GameServices
        {
            get { return gameServices; }
            set { gameServices = value; }
        }


        public static List<RoomSaveData> RoomSaves
        {
            get { return roomSaves; }
            set { roomSaves = value; }
        }

        public static GraphicsDevice Device
        {
            get { return graphicsDevice; }
            set { graphicsDevice = value; }
        }
    }
}