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
            KeyGet = 7,
            ItemGet = 8,
            MenuSelect = 9,
            MenuClick = 10,
            SaveChime = 11,
            PlatformCrumble = 12,
            EffectInstance = 13
        }

        private SoundEffect jump1;
        private SoundEffect jump2;
        private SoundEffect jump3;
        private SoundEffect dash;
        private SoundEffect land;
        private SoundEffect wallLand;
        //private SoundEffect wallJump;
        private SoundEffect doorOpen;
        private SoundEffect keyGet;
        private SoundEffect itemGet;
        private SoundEffect menuSelect;
        private SoundEffect menuClick;
        private SoundEffect saveChime;
        private SoundEffect platformCrumble;
        private SoundEffect torch;
        private SoundEffect lavaLoop;
        private SoundEffectInstance soundInstance;

        private float volume = 1f; //TODO: Call menu's volume parameter to get the user defined varaible.
        private const float zero = 0f;

        // Stores the called object type for reference.
        private string objectName;

        // Used to select a random soundeffect from a list.
        private Random rnd = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content">Pass the content manager.</param>
        /// <param name="tempObjType">Pass the desired object type.</param>
        public AudioEngine(ContentManager content, string name)
        {
            objectName = name;

            try
            {
                switch (objectName)
                {
                    case "Player":
                        jump1 = content.Load<SoundEffect>("Sounds/hup1");
                        jump2 = content.Load<SoundEffect>("Sounds/hup2");
                        jump3 = content.Load<SoundEffect>("Sounds/hup3");
                        dash = content.Load<SoundEffect>("Sounds/Dash");
                        land = content.Load<SoundEffect>("Sounds/land");
                        wallLand = content.Load<SoundEffect>("Sounds/wallland");
                        keyGet = content.Load<SoundEffect>("Sounds/Key");
                        itemGet = content.Load<SoundEffect>("Sounds/Item Jingle");
                        platformCrumble = content.Load<SoundEffect>("Sounds/PlatformCrumble");
                        break;


                    case "SaveRoomA":
                        doorOpen = content.Load<SoundEffect>("Sounds/DoorGrind");
                        saveChime = content.Load<SoundEffect>("Sounds/SaveChime");
                        break;

                    case "Vault":
                        doorOpen = content.Load<SoundEffect>("Sounds/DoorGrind");
                        torch = content.Load<SoundEffect>("Sounds/Torch");
                        soundInstance = torch.CreateInstance();
                        soundInstance.Volume = volume;
                        soundInstance.IsLooped = true;
                        //soundInstance.Play();
                        break;


                    case "LavaPassageA":
                    case "LavaPassageB":
                    case "StairwayToHell":
                    case "DashRoom":
                        doorOpen = content.Load<SoundEffect>("Sounds/DoorGrind");
                        platformCrumble = content.Load<SoundEffect>("Sounds/PlatformCrumble");
                        lavaLoop = content.Load<SoundEffect>("Sounds/LavaLoop");
                        soundInstance = lavaLoop.CreateInstance();
                        soundInstance.Volume = volume;
                        soundInstance.IsLooped = true;
                        //soundInstance.Play();
                        break;

                    case "LavaAccess":
                        doorOpen = content.Load<SoundEffect>("Sounds/DoorGrind");
                        platformCrumble = content.Load<SoundEffect>("Sounds/PlatformCrumble");
                        break;

                    case "StartRoom":
                    case "Lobby":
                        doorOpen = content.Load<SoundEffect>("Sounds/DoorGrind");
                        break;

                    case "Menu":
                        menuClick = content.Load<SoundEffect>("Sounds/MenuClick");
                        menuSelect = content.Load<SoundEffect>("Sounds/MenuSelect");
                        break;

                    default: break;
                }
            }
            catch(NullReferenceException e)
            {

                System.Diagnostics.Debug.WriteLine("Error loading audio clip(s) for object: " + objectName +
                    "\n" + e.Message);
            }

        }


        /// <summary>
        /// Plays the called soundeffect. Returns debug line if sound effect is 
        /// not loaded.
        /// </summary>
        /// <param name="effect">Pass desired soundeffect.</param>
        public void Play(SoundEffects effect)
        {
            try
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

                    //Called by room switch.
                    case SoundEffects.EffectInstance:
                        //If current room has a looping sound, play that sound.
                        switch (objectName)
                        {
                            case "LavaPassageA":
                            case "LavaPassageB":
                            case "StairwayToHell":
                            case "DashRoom":
                            case "Vault":
                                soundInstance.Play();
                                break;
                            default: break;
                        }
                        break;
                    
                    case SoundEffects.ItemGet:
                        itemGet.Play(volume, zero, zero);
                        break;

                    default: break;
                }
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.WriteLine("The audio clip was not loaded for object: " + objectName +
                    "\n" + e.Message);
            }
        }
    }
 
    ///Initiates background music and way of testing
    ///  to see if a change of music is needed.
    public class BGM
    {
        private Song menu;
        private Song main;
        private Song levelMusicIntro;
        private Song levelMusicLoop;
        private Song SaveMusicLoop;


        private string currentMusicName;

        public BGM(ContentManager content)
        {
            menu = content.Load<Song>("Sounds/MenuScreen");
            main = content.Load<Song>("Sounds/MainTitle");
            play();
        }
        public void play()
        {
            MediaPlayer.Volume = 1f;
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
