using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pyramid_Plunder.Classes
{
    public class Enemy : PhysicsObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="objType"></param>
        public Enemy(String filepath, GameObjectList objType) : base(objType)
        {
            
        }
    }
}
