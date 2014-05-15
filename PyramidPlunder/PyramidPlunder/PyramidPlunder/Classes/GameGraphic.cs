using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pyramid_Plunder.Classes
{
    public class GameGraphic
    {
        public const int DEFAULT_ANIMATION_FPS = 8;
        public const float DEFAULT_ANIMATION_TIME = (float)1 / DEFAULT_ANIMATION_FPS * 1000;

        protected ContentManager Content;

        protected Texture2D sprite;
        protected Vector2 coordinates;
        protected Rectangle hitBox;
        protected GameObjectList objectType;

        private String filepath;
        private String spriteName;
        private int numAnimations;
        protected int currentAnimation;
        private int previousAnimation;
        private int currentFrame;
        private double elapsedMilliseconds;
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
            objectType = objType;
            LoadGraphicsData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch, GameTime time)
        {
            // TODO: Drawing code
            if (isLoaded)
            {
                DetermineAnimationFrame(time);

                Vector2 drawVector = new Vector2(coordinates.X, coordinates.Y);

                Rectangle sourceRectangle = new Rectangle(currentFrame * (int)animationDimensions[currentAnimation].X, 
                    animationLocation[currentAnimation], (int)animationDimensions[currentAnimation].X, 
                    (int)animationDimensions[currentAnimation].Y);

                spriteBatch.Draw(sprite, drawVector, sourceRectangle, Color.White);
            }
        }

        /// <summary>
        /// Loads in the graphics data for the object from the object type
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        private void LoadGraphicsData()
        {
            switch (objectType)
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

                    currentAnimation = 0;
                    previousAnimation = 0;
                    currentFrame = 0;
                    elapsedMilliseconds = 0;

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
        /// Checks the GameTime to see how much time has passed since the last frame changed and 
        /// compares that to the speed of the animation to determine if the frame needs to be incremented
        /// </summary>
        /// <param name="time">The GameTime to use.</param>
        private void DetermineAnimationFrame(GameTime time)
        {
            // TODO: make this method do what it's supposed to do

            if (previousAnimation != currentAnimation)
            {
                previousAnimation = currentAnimation;
                currentFrame = 0;
                elapsedMilliseconds = 0;
            }
            else
            {
                if (animationSpeed[currentAnimation] > 0)
                {
                    elapsedMilliseconds += time.ElapsedGameTime.TotalMilliseconds;
                    if (elapsedMilliseconds >= DEFAULT_ANIMATION_TIME / animationSpeed[currentAnimation])
                    {
                        if (currentFrame < numberOfFrames[currentAnimation] - 1)
                            currentFrame++;
                        else
                            currentFrame = 0;

                        elapsedMilliseconds = 0;
                    }
                }
            }

        }

        /// <summary>
        /// Accessor for the object's Coordinates (position on the screen)
        /// </summary>
        public Vector2 Coordinates
        {
            get { return coordinates; }
        }

        public GameObjectList ObjectType
        {
            get { return objectType; }
        }
    }
}
