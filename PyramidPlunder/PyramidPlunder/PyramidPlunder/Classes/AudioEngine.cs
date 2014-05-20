using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Pyramid_Plunder.Classes
{

    class SoundJump
    {
        private SoundEffect jump1;
        private SoundEffect jump2;
        private SoundEffect jump3;

        private string objType;

        private Random rnd = new Random();

        public SoundJump(ContentManager content, Pyramid_Plunder.GameObjectList tempObjType)
        {
            objType = tempObjType.ToString();

            switch (objType)
            {
                case "Player":
                    jump1 = content.Load<SoundEffect>("Sounds/hup1");
                    jump2 = content.Load<SoundEffect>("Sounds/hup2");
                    jump3 = content.Load<SoundEffect>("Sounds/hup3");
                    break;
                default:

                    break;
            }

        }

        public void Play()
        {
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
        }
    }


    
}
