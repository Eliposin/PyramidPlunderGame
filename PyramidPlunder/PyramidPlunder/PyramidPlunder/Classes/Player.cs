using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pyramid_Plunder.Classes
{
    public class Player : PhysicsObject
    {
        private int DEFAULT_SCREEN_POSITIONX = 610;
        private int DEFAULT_SCREEN_POSITIONY = 420;
        //private bool isSpawned;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType"></param>
        public Player(GameObjectList objType, ContentManager content)
            : base(objType, content)
        {
            isSpawned = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        public new void Spawn(Vector2 location)
        {
            base.Spawn(location);
            coordinates = new Vector2(DEFAULT_SCREEN_POSITIONX, DEFAULT_SCREEN_POSITIONY);
        }
    }
}
