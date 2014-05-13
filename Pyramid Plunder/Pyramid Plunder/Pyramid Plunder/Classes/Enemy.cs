using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Pyramid_Plunder.Classes
{
    public class Enemy : PhysicsObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="objType"></param>
        public Enemy(String filepath, GameObjectList objType, ContentManager content)
            : base(objType, content)
        {
            
        }
    }
}
