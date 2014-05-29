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
        protected string objectName;
        
        protected int currentAnimation;
        protected int currentFrame;
        protected int animationOffset;
        protected bool looping = true;
        protected bool hasGraphics;
        protected float[] animationSpeed;
        protected float[] defaultAnimationSpeed;
        protected int[] numberOfFrames;

        private String filepath;
        private String spriteName;
        private int numAnimations;
        private int previousAnimation;
        
        private double elapsedMilliseconds;
        private int[] animationLocation;
        private Vector2[] animationDimensions;
        
        
        private bool isLoaded;

        /// <summary>
        /// Constructor call
        /// </summary>
        /// <param name="objType">The type of object that is represented</param>
        public GameGraphic(string objName, ContentManager content, bool graphics)
        {
            isLoaded = false;
            Content = content;

            currentAnimation = 0;
            animationOffset = 0;
            objectName = objName;
            hasGraphics = graphics;
            if (hasGraphics)
                LoadGraphicsData();
        }


        public GameGraphic(string objName, ContentManager content) : this(objName, content, true) { }

        /// <summary>
        /// Draws the graphic to the spritebatch based on the properties about the spritesheet and current animation.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        public virtual void Draw(SpriteBatch spriteBatch, GameTime time)
        {
            // TODO: Drawing code
            if (isLoaded && hasGraphics)
            {
                DetermineAnimationFrame(time);

                Vector2 drawVector = new Vector2(coordinates.X, coordinates.Y);

                Rectangle sourceRectangle = new Rectangle(currentFrame * (int)animationDimensions[currentAnimation].X,
                    animationLocation[currentAnimation + animationOffset], (int)animationDimensions[currentAnimation].X,
                    (int)animationDimensions[currentAnimation + animationOffset].Y);

                spriteBatch.Draw(sprite, drawVector, sourceRectangle, Color.White);
            }
        }

        /// <summary>
        /// Loads in the graphics data for the object from the object type
        /// </summary>
        /// <param name="objType">The type of object that is represented.</param>
        private void LoadGraphicsData()
        {
            try
            {
                using (Stream stream = TitleContainer.OpenStream("Data/GraphicsData/" + objectName + ".txt"))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {

                        String line = GameResources.getNextDataLine(sr, "#");

                        numAnimations = int.Parse(line);

                        spriteName = "Images/" + GameResources.getNextDataLine(sr, "#");

                        animationLocation = new int[numAnimations];
                        animationDimensions = new Vector2[numAnimations];
                        defaultAnimationSpeed = new float[numAnimations];
                        animationSpeed = new float[numAnimations];
                        numberOfFrames = new int[numAnimations];

                        for (byte i = 0; i < numAnimations; i++)
                        {
                            animationLocation[i] = int.Parse(GameResources.getNextDataLine(sr, "#"));

                            animationDimensions[i] = new Vector2(int.Parse(GameResources.getNextDataLine(sr, "#")),
                                int.Parse(GameResources.getNextDataLine(sr, "#")));

                            defaultAnimationSpeed[i] = float.Parse(GameResources.getNextDataLine(sr, "#"));
                            animationSpeed[i] = defaultAnimationSpeed[i];

                            numberOfFrames[i] = int.Parse(GameResources.getNextDataLine(sr, "#"));
                        }

                        sr.Close();
                    }
                }

                sprite = Content.Load<Texture2D>(spriteName);

                currentAnimation = 0;
                previousAnimation = 0;
                currentFrame = 0;
                elapsedMilliseconds = 0;

                isLoaded = true;
            }
            catch (FileNotFoundException e)
            {
                System.Diagnostics.Debug.WriteLine("The file was not found for object: \n" + objectName + filepath + "\n" + e.Message);
                numAnimations = 0;
                spriteName = "";
                animationLocation = new int[0];
                animationDimensions = new Vector2[0];
                defaultAnimationSpeed = new float[0];
                animationSpeed = new float[0];
                numberOfFrames = new int[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("An error occurred in object: " + objectName + e.Message);
                numAnimations = 0;
                spriteName = "";
                animationLocation = new int[0];
                animationDimensions = new Vector2[0];
                defaultAnimationSpeed = new float[0];
                animationSpeed = new float[0];
                numberOfFrames = new int[0];
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
                        {
                            if (looping)
                                currentFrame = 0;
                            else
                            {
                                animationSpeed[currentAnimation] = 0;
                            }
                        }

                        elapsedMilliseconds = 0;
                    }
                }
            }

        }

        /// <summary>
        /// Removes the object from memory
        /// </summary>
        public virtual void Dispose()
        {
            sprite.Dispose();
        }

        /// <summary>
        /// Accessor for the object's Coordinates (position on the screen)
        /// </summary>
        public Vector2 Coordinates
        {
            get { return coordinates; }
            set { coordinates = value; }
        }

        /// <summary>
        /// The type of object represented in the GameObjectList
        /// </summary>
        public string ObjectName
        {
            get { return objectName; }
        }

        /// <summary>
        /// The area considered a positive match for collision detection.
        /// </summary>
        public Rectangle HitBox
        {
            get 
            {
                if (sprite != null)
                    return new Rectangle((int)coordinates.X, (int)coordinates.Y,
                        (int)animationDimensions[currentAnimation].X,
                        (int)animationDimensions[currentAnimation].Y);
                else
                    return new Rectangle(0, 0, 0, 0);
            }
        }
    }
}
