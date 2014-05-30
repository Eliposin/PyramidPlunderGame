using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Pyramid_Plunder.Classes
{
    public class Door : GameObject
    {
        private const float DOOR_ANIMATION_SPEED = 0.5f;

        public enum DoorOrientations : byte
        {
            FacingRight = 0,
            FacingLeft = 1
        }

        private enum Animations : byte
        {
            Unlocked = 0,
            RedLock = 2
        }

        private DoorOrientations orientation;
        private Locks lockType;
        private Animations animation;

        private bool isOpen;
        private bool isRoomLoaded;
        private string linkedRoomName;
        private int linkedDoorIndex;

        private Room linkedRoom;
        private System.Threading.Thread roomThread;

        private AudioEngine soundEngine;

        /// <summary>
        /// Constructor call.  Creates a new Door object.
        /// </summary>
        /// <param name="content">The content manager to use when loading assets.</param>
        /// <param name="orient">The orientation of the room (either facing left or facing right).</param>
        /// <param name="roomName">The name of the room that the door leads to.</param>
        /// <param name="connectedDoorIndex">The index of the door that is linked to this one.</param>
        /// <param name="lockT">The type of lock that is on this door.</param>
        public Door(ContentManager content, AudioEngine audioEngine, DoorOrientations orient, string roomName, int connectedDoorIndex, Locks lockT)
            : base("Door", content)
        {
            Content = content;
            soundEngine = audioEngine;
            orientation = orient;
            linkedRoom = null;
            linkedRoomName = roomName;
            linkedDoorIndex = connectedDoorIndex;
            lockType = lockT;
            
            isOpen = false;
            isSolid = true;

            switch (lockType)
            {
                case Locks.Unlocked:
                    animation = Animations.Unlocked;
                    break;
                case Locks.Red:
                    animation = Animations.RedLock;
                    break;
                default:
                    animation = Animations.Unlocked;
                    break;
            }

            currentAnimation = (int)animation + (int)orientation;

        }

        /// <summary>
        /// Opens the door, playing the opening animation and causing the door to be enterable.
        /// </summary>
        public void Open()
        {
            isOpen = true;
            animationSpeed[currentAnimation] = DOOR_ANIMATION_SPEED;

            soundEngine.Play(AudioEngine.SoundEffects.DoorOpen);

            looping = false;
            isSolid = false;

            roomThread = new System.Threading.Thread(LoadRoom);
            roomThread.Start();
        }

        /// <summary>
        /// Loads the linked room in a seperate thread
        /// </summary>
        private void LoadRoom()
        {
            try
            {
                linkedRoom = new Room(linkedRoomName, linkedDoorIndex);
                isRoomLoaded = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("There was an error loading the room: " + e.Message);
            }

        }

        /// <summary>
        /// Lock Type property
        /// </summary>
        public Locks LockType
        {
            get { return lockType; }
            set { lockType = value; }
        }

        /// <summary>
        /// A representation of which direction the door is facing
        /// </summary>
        public DoorOrientations Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        /// <summary>
        /// The room linked with this door.  Returns null if the room is not loaded.
        /// </summary>
        public Room LinkedRoom
        {
            get { return linkedRoom; }
        }

        /// <summary>
        /// Whether or not the door is currently open.
        /// </summary>
        public bool IsOpen
        {
            get { return isOpen; }
        }

        /// <summary>
        /// Whether or not the linked room is loaded in memory.
        /// </summary>
        public bool IsRoomLoaded
        {
            get { return isRoomLoaded; }
        }

        /// <summary>
        /// Overriding Public To String that returns the door object in the form necessary to read it in from a file.
        /// </summary>
        /// <returns></returns>
        public string toString()
        {
            //Not sure if I have these in the right order but this is how I think it should go.
            String outputString = "";
            outputString = this.linkedRoomName;
            outputString += "\n" + this.lockType;
            outputString += "\n" + this.linkedDoorIndex;
            outputString += "\n" + this.coordinates.X;
            outputString += "\n" + this.coordinates.Y;

            return outputString;
        }

        public override Rectangle HitBox
        {
            get 
            {
                if (isSolid)
                    return base.HitBox;
                else
                {
                    int XLocation;
                    if (orientation == DoorOrientations.FacingLeft)
                        XLocation = (int)(position.X + (animationDimensions[currentAnimation].X * 0.3));
                    else
                        XLocation = (int)position.X;

                    return new Rectangle(XLocation, (int)position.Y,
                        (int)(animationDimensions[currentAnimation].X * 0.7),
                        (int)animationDimensions[currentAnimation].Y);
                }
            }
        }
    }
}
    



