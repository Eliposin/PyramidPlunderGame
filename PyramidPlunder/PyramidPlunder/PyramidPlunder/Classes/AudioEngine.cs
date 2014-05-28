using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Pyramid_Plunder.Classes
{

    public class AudioEngine
    {
        public enum SoundEffects : byte
        {
            Jump = 0,
            WallJump = 1,
            Dash = 2,
            WallLand = 3,
            Attack = 4,
            Land = 5,
            DoorOpen = 6,
            KeyGet = 7
        }

        private SoundEffect jump1;
        private SoundEffect jump2;
        private SoundEffect jump3;
        private SoundEffect dash;
        private SoundEffect land;
        private SoundEffect wallLand;
        private SoundEffect wallJump;
        private SoundEffect doorOpen;
        private SoundEffect keyGet;

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
                    dash = content.Load<SoundEffect>("Sounds/Wooshal2");
                    land = content.Load<SoundEffect>("Sounds/land");
                    wallLand = content.Load<SoundEffect>("Sounds/wallland");

                    break;


                case GameObjectList.Vault:
                    keyGet = content.Load<SoundEffect>("Sounds/Key");
                    doorOpen = content.Load<SoundEffect>("Sounds/DoorGrind");
                    break;
                case GameObjectList.StartRoom:
                case GameObjectList.Lobby:
                case GameObjectList.SaveRoom:
                    doorOpen = content.Load<SoundEffect>("Sounds/DoorGrind");
                    break;

                default: break;
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

                case SoundEffects.DoorOpen:
                    doorOpen.Play(volume, zero, zero);
                    break;

                case SoundEffects.KeyGet:
                    keyGet.Play(volume, zero, zero);
                    break;

                default: break;
            }
        }
    }
 
    ///Initiates background music and way of testing
    ///  to see if a change of music is needed.
    public class BGM
    {
        private Song menu;
        private Song main;

        private string currentMusicName;

        public BGM(ContentManager content)
        {
            menu = content.Load<Song>("Sounds/MenuScreen");
            main = content.Load<Song>("Sounds/MainTitle");
            play();
        }
        public void play()
        {
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Play(main);
            MediaPlayer.IsRepeating = true;
            currentMusicName = "Main";
        }

        public void SwitchMusic(string musicName)
        {
            if (currentMusicName != musicName)
            {
                switch (musicName)
                {
                    case "Main":
                        MediaPlayer.Stop();
                        MediaPlayer.Play(main);
                        currentMusicName = "Main";
                        break;
                    case "Menu":
                        MediaPlayer.Stop();
                        MediaPlayer.Play(menu);
                        currentMusicName = "Menu";
                        break;
                    default: break;
                }
            }
        }
    }
}
