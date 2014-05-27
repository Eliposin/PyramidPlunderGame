using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Pyramid_Plunder.Classes
{

    class AudioEngine
    {
        public enum SoundEffects : byte
        {
            Jump = 0,
            WallJump = 1,
            Dash = 2,
            WallLand = 3,
            Attack = 4,
            Land = 5
        }

        private SoundEffect jump1;
        private SoundEffect jump2;
        private SoundEffect jump3;
        private SoundEffect dash;
        private SoundEffect land;
        private SoundEffect wallLand;
        private SoundEffect wallJump;

        private float volume = 1f; //TODO: Call menu's volume parameter to get the user defined varaible.
        private const float zero = 0f;

        // Stores the called object type for reference.
        private GameObjectList objType;

        // Used to select a random soundeffect from a list.
        private Random rnd = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content">Pass the content manager.</param>
        /// <param name="tempObjType">Pass the desired object type.</param>
        public AudioEngine(ContentManager content, GameObjectList tempObjType)
        {
            objType = tempObjType;

            switch (objType)
            {
                case GameObjectList.Player:
                    jump1 = content.Load<SoundEffect>("Sounds/hup1");
                    jump2 = content.Load<SoundEffect>("Sounds/hup2");
                    jump3 = content.Load<SoundEffect>("Sounds/hup3");
                    dash = content.Load<SoundEffect>("Sounds/wooshal2");
                    land = content.Load<SoundEffect>("Sounds/land");
                    wallLand = content.Load<SoundEffect>("Sounds/wallland");

                    break;
                default:

                    break;
            }

        }

        /// <summary>
        /// Plays called soundeffect.
        /// </summary>
        public void Play(SoundEffects effect)
        {
            int i;
            switch (effect)
            {
                case SoundEffects.Jump:
                    i = rnd.Next(0, 3);
                    switch (i)
                    {
                        case 0:
                            jump1.Play();
                            break;
                        case 1:
                            jump2.Play();
                            break;
                        case 2:
                            jump3.Play();
                            break;
                        default:
                            break;
                    }
                    break;

                case SoundEffects.WallJump:
                    i = rnd.Next(0, 3);
                    switch (i)
                    {
                        case 0:
                            jump1.Play();
                            break;
                        case 1:
                            jump2.Play();
                            break;
                        case 2:
                            jump3.Play();
                            break;
                        default:
                            break;
                    }
                    break;

                case SoundEffects.Dash:
                    dash.Play(volume, zero, zero);
                    break;

                case SoundEffects.Land:
                    land.Play(volume, zero, zero);
                    break;

                case SoundEffects.WallLand:
                    wallLand.Play(volume, zero, zero);
                    break;

                default: break;
            }
        }
    }
 
    ///Initiates background music and way of testing
    ///  to see if a change of music is needed.
    class BGM
    {
        private SoundEffect menuSong;
        private SoundEffect mainTheme;

        private float volume = 1f;
        private float zero = 0f;

        public BGM(ContentManager content)
        {
            menuSong = content.Load<SoundEffect>("Sounds/MenuScreen");
            mainTheme = content.Load<SoundEffect>("Sounds/MainTitle");
            play();
        }
        public void play()
        {
            mainTheme.Play(volume, zero, zero);
        }
    }
}
