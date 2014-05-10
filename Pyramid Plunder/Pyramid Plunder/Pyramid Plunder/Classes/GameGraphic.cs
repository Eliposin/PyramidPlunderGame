using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pyramid_Plunder.Classes
{
    public class GameGraphic
    {
        private Texture2D sprite;
        private Vector2 coordinates;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented</param>
        public GameGraphic(GameObjectList objType)
        {
            LoadGraphicsData(objType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //TODO: Drawing code
        }

        /// <summary>
        /// Loads in the graphics data for the object from the object type
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        private void LoadGraphicsData(GameObjectList objType)
        {
            switch (objType)
            {
                // TODO: Specifiy the filepath for each objType
                case 0: break;
            }

            // TODO: Load the graphics data from the graphics file
        }
    }
}
