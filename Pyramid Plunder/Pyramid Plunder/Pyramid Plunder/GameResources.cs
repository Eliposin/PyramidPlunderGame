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
        Rock = 0,
        SomethingElse = 1
    }

    public enum MenuCallbacks : byte
    {
        PlayGame = 0,
        LoadGame = 1,
        Quit = 2
    }

    delegate void DelVoid();
    delegate void DelMenu(MenuCallbacks menuCallback);
}
