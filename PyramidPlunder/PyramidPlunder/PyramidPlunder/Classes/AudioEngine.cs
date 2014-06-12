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
            EffectInstance = 13,
            Hurt = 14
        }

        private SoundEffect jump1;
        private SoundEffect jump2;
        private SoundEffect jump3;
        private SoundEffect oof1;
        private SoundEffect oof2;
        private SoundEffect oof3;
        private SoundEffect dash;
        private SoundEffect land;
        private SoundEffect wallLand;
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

        private static float volume = 1f;

        //Represents any zero float for the play method.
        private const float zero = 0f;

        // Stores the called object type for reference.
        private string objectName;

        // Used to select a random soundeffect from a list.
        private Random rnd = new Random();

        /// <summary>
        /// this class is the handler of all audio in the game. 
        /// </summary>
        /// <param name="content">Pass the content manager.</param>
        /// <param name="tempObjType">Pass the desired object type.</param>
        public AudioEngine(ContentManager content, string name)
        {
            objectName = name;

            try
            {
                //Depending on what string name is passed, the relevant sound effects are loaded.
                switch (objectName)
                {
                    case "Player":
                        jump1 = content.Load<SoundEffect>("Sounds/hup1");
                        jump2 = content.Load<SoundEffect>("Sounds/hup2");
                        jump3 = content.Load<SoundEffect>("Sounds/hup3");
                        oof1 = content.Load<SoundEffect>("Sounds/oof1");
                        oof2 = content.Load<SoundEffect>("Sounds/oof2");
                        oof3 = content.Load<SoundEffect>("Sounds/oof3");
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
                        break;

                    case "LavaAccess":
                    case "ThePit":
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
                                jump1.Play(volume, zero, zero);
                                break;
                            case 1:
                                jump2.Play(volume, zero, zero);
                                break;
                            case 2:
                                jump3.Play(volume, zero, zero);
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
                                jump1.Play(volume, zero, zero);
                                break;
                            case 1:
                                jump2.Play(volume, zero, zero);
                                break;
                            case 2:
                                jump3.Play(volume, zero, zero);
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

                    case SoundEffects.SaveChime:
                        saveChime.Play(volume, zero, zero);
                        break;

                    case SoundEffects.MenuClick:
                        menuClick.Play(volume, zero, zero);
                        break;

                    case SoundEffects.MenuSelect:
                        menuSelect.Play(volume, zero, zero);
                        break;

                    case SoundEffects.PlatformCrumble:
                        platformCrumble.Play(volume, zero, zero);
                        break;

                    case SoundEffects.Hurt:
                        i = rnd.Next(0, 3);
                        switch (i)
                        {
                            case 0:
                                oof1.Play(volume, zero, zero);
                                break;
                            case 1:
                                oof2.Play(volume, zero, zero);
                                break;
                            case 2:
                                oof3.Play(volume, zero, zero);
                                break;
                            default:
                                break;
                        }
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

        public static float Volume
        {
            get { return volume; }
            set { volume = value; }
        }
    }
 
    ///Initiates background music and way of testing
    ///  to see if a change of music is needed.
    public class BGM
    {
        private Song menu;
        private Song main;
        private Song levelMusicLoop;
        private Song saveMusicLoop;
        private Song deathMusic;

        private string currentMusicName;

        private static float volume = 1f;

        public BGM(ContentManager content)
        {
            menu = content.Load<Song>("Sounds/MenuScreen");
            main = content.Load<Song>("Sounds/MainTitle");
            levelMusicLoop = content.Load<Song>("Sounds/LevelMusicLoopish");
            saveMusicLoop = content.Load<Song>("Sounds/SaveMusicLoop");
            deathMusic = content.Load<Song>("Sounds/DeathMusic");

            play();
        }

        public static void UpdateVolume(float newVolume)
        {
            volume = newVolume;
            MediaPlayer.Volume = volume;
        }

        public void play()
        {
            MediaPlayer.Volume = volume;
            MediaPlayer.IsRepeating = true;
            currentMusicName = "Null";
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
                        MediaPlayer.Volume = volume;
                        currentMusicName = "Main";
                        break;
                    case "Menu":
                        MediaPlayer.Stop();
                        MediaPlayer.Play(menu);
                        MediaPlayer.Volume = volume;
                        currentMusicName = "Menu";
                        break;
                    case "Level":
                        MediaPlayer.Stop();
                        MediaPlayer.Play(levelMusicLoop);
                        MediaPlayer.Volume = volume;
                        currentMusicName = "Level";
                        break;
                    case "Save":
                        MediaPlayer.Stop();
                        MediaPlayer.Play(saveMusicLoop);
                        MediaPlayer.Volume = volume;
                        currentMusicName = "Save";
                        break;
                    case "Death":
                        MediaPlayer.Stop();
                        MediaPlayer.Play(deathMusic);
                        MediaPlayer.Volume = volume;
                        currentMusicName = "Death";
                        break;
                    default: break;
                }
            }
        }

        //called by game manager to handle pausing / unpausing.
        public void PauseMusic()
        {
            MediaPlayer.Pause();
        }

        public void UnpauseMusic()
        {
            MediaPlayer.Resume();
        }

        //Used to set and get the volume in other classes.
        public static float Volume
        {
            get { return volume; }
            set { volume = value; }
        }
    }
}
