using System;
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

    delegate void DelVoid();
    delegate void DelMenu(MenuCallbacks menuCallback);
}