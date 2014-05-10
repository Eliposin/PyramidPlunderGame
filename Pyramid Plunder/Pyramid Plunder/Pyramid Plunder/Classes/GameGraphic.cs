using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pyramid_Plunder.Classes
{
    public class GameGraphic
    {
        protected Texture2D sprite;
        protected Vector2 coordinates;
        protected Rectangle hitBox;

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
        public virtual void Draw(SpriteBatch spriteBatch)
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

        /// <summary>
        /// Accessor for the object's Coordinates (position on the screen)
        /// </summary>
        public Vector2 Coordinates
        {
            get { return coordinates; }
        }
    }
}
