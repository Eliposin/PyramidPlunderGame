using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pyramid_Plunder.Classes
{
    public class GameGraphic
    {
        protected Texture2D sprite;
        protected Vector2 coordinates;
        protected Rectangle hitBox;

        private String filepath;
        private int numAnimations;
        private int currentAnimation;
        private int[] animationLocation;
        private Vector2[] animationDimensions;
        private float[] animationSpeed;
        private int[] numberOfFrames;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented</param>
        public GameGraphic(GameObjectList objType)
        {
            currentAnimation = 0;
            LoadGraphicsData(objType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch, GameTime time)
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
                case GameObjectList.TestRoom:
                    filepath = "../Data/GraphicsData/TestRoom.gdf";
                    break;
                default:
                    filepath = "";
                    break;
            }

            if (filepath != "" && filepath != null)
            {
                StreamReader sr = new StreamReader(filepath);

                String line = GameResources.getNextDataLine(sr, "#");

                numAnimations = int.Parse(line);

                animationLocation = new int[numAnimations];
                animationDimensions = new Vector2[numAnimations];
                animationSpeed = new float[numAnimations];
                numberOfFrames = new int[numAnimations];

                for (byte i = 0; i < numAnimations; i++)
                {
                    animationLocation[i] = int.Parse(GameResources.getNextDataLine(sr, "#"));

                    animationDimensions[i] = new Vector2(int.Parse(GameResources.getNextDataLine(sr, "#")),
                        int.Parse(GameResources.getNextDataLine(sr, "#")));

                    animationSpeed[i] = float.Parse(GameResources.getNextDataLine(sr, "#"));

                    numberOfFrames[i] = int.Parse(GameResources.getNextDataLine(sr, "#"));
                }

                sr.Close();
                
                //System.Diagnostics.Debug.WriteLine("Number of animations for " + objType + ": " + numAnimations);
                //System.Diagnostics.Debug.WriteLine("Animation Location for " + objType + ": " + animationLocation[0]);
                //System.Diagnostics.Debug.WriteLine("Animation Dimensions for " + objType + ": " + animationDimensions[0]);
                //System.Diagnostics.Debug.WriteLine("Animation Speed for " + objType + ": " + animationSpeed[0]);
                //System.Diagnostics.Debug.WriteLine("Number of Frames for " + objType + ": " + numberOfFrames[0]);
            }
            else
            {
                
            }
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
