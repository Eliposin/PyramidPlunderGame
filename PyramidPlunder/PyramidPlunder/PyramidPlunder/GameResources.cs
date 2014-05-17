using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pyramid_Plunder
{
    /// <summary>
    /// A list of the different types of objects the game has.
    /// </summary>
    public enum GameObjectList : byte
    {
        NullObject,
        Player,
        Mummy,

        TestRoom
    }

    public enum MenuCallbacks : byte
    {
        PlayGame = 0,
        LoadGame = 1,
        Quit = 2
    }

    public enum MenuTypes
    {
        Main = 0,
        Options
    }

    public delegate void DelVoid();
    public delegate void DelMenu(MenuCallbacks menuCallback);

    

    public static class GameResources
    {
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
    }
}