using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pyramid_Plunder.Classes
{
    /// <summary>
    /// A list of the different types of objects the game has.
    /// </summary>
    public enum GameObjectList : byte
    {
        Rock = 0,
        SomethingElse = 1
    }

    class GameObject : GameGraphic
    {
        private GameObjectList objectType;
        private Vector2 position;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="objType"></param>
        public GameObject(String filepath, GameObjectList objType) : base(filepath)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        public void UpdateCoordinates(Player player, Room room)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherObject"></param>
        public void Interact(GameObject otherObject)
        {

        }
    }
}
