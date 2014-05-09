using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pyramid_Plunder.Classes
{
    public class GameObject : GameGraphic
    {
        private GameObjectList objectType;
        private Vector2 position;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        public GameObject(GameObjectList objType) : base(objType)
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
