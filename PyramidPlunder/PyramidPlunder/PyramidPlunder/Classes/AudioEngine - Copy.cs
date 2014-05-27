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
            Dash = 2
        }

        private SoundEffect jump1;
        private SoundEffect jump2;
        private SoundEffect jump3;
        private SoundEffect dash;

        private float volume = 1;
        private float zero = 0;

        private GameObjectList objType;

        private Random rnd = new Random();

        public AudioEngine(ContentManager content, GameObjectList tempObjType)
        {
            objType = tempObjType;

            switch (objType)
            {
                case GameObjectList.Player:
                    jump1 = content.Load<SoundEffect>("Sounds/hup1");
                    jump2 = content.Load<SoundEffect>("Sounds/hup2");
                    jump3 = content.Load<SoundEffect>("Sounds/hup3");
                    dash = content.Load<SoundEffect>("Sounds/woosh");
                    break;
                default:

                    break;
            }

        }

        public void Play(SoundEffects effect)
        {
            switch (effect)
            {
                case SoundEffects.Jump:
                    int i = rnd.Next(0, 3);
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
            }
            
        }
    }
}
