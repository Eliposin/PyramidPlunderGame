using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pyramid_Plunder.Classes
{
    public class GameGraphic
    {
        protected ContentManager Content;

        protected Texture2D sprite;
        protected Vector2 coordinates;
        protected Rectangle hitBox;

        private String filepath;
        private String spriteName;
        private int numAnimations;
        private int currentAnimation;
        private int[] animationLocation;
        private Vector2[] animationDimensions;
        private float[] animationSpeed;
        private int[] numberOfFrames;

        private bool isLoaded;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented</param>
        public GameGraphic(GameObjectList objType, ContentManager content)
        {
            isLoaded = false;
            Content = content;

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
            if (isLoaded)
                spriteBatch.Draw(sprite, new Rectangle((int)coordinates.X, (int)coordinates.Y, sprite.Width, sprite.Height), Color.White);
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
                case GameObjectList.Player:
                    filepath = "../Data/GraphicsData/Player.gdf";
                    break;
                default:
                    filepath = "";
                    break;
            }

            if (filepath != "" && filepath != null)
            {
                try
                {
                    StreamReader sr = new StreamReader(filepath);

                    String line = GameResources.getNextDataLine(sr, "#");

                    numAnimations = int.Parse(line);

                    spriteName = "Images/" + GameResources.getNextDataLine(sr, "#");

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
                    
                    sprite = Content.Load<Texture2D>(spriteName);

                    isLoaded = true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("An error occurred: " + e.Message);
                    numAnimations = 0;
                    spriteName = "";
                    animationLocation = new int[0];
                    animationDimensions = new Vector2[0];
                    animationSpeed = new float[0];
                    numberOfFrames = new int[0];
                }

                

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
