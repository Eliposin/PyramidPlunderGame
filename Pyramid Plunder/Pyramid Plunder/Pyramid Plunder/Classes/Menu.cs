using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pyramid_Plunder.Classes
{
    class Menu
    {
        public enum MenuTypes
        {
            Main = 0,
            Options
        }

        DelMenu menuCallback;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="menuType">The type of menu to load.</param>
        /// <param name="menuCB">The callback method to call.</param>
        public Menu(MenuTypes menuType, DelMenu menuCB)
        {
            menuCallback = menuCB;

            LoadMenu(menuType);
        }

        /// <summary>
        /// Loads the menu based on which type of menu is being loaded.
        /// </summary>
        /// <param name="menuType">The type of menu to load.</param>
        private void LoadMenu(MenuTypes menuType)
        {

        }

        /// <summary>
        /// Unloads the menu object from memory.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
