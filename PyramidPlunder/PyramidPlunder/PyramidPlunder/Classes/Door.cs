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
        private const float DOOR_ANIMATION_SPEED = 0.2f;

        public enum DoorOrientations : byte
        {
            FacingRight = 0,
            FacingLeft = 1
        }

        private DoorOrientations orientation;
        private Locks lockType;

        private bool isOpen;
        private bool isRoomLoaded;
        private string linkedRoomName;
        private int linkedDoorIndex;

        private Room linkedRoom;
        private System.Threading.Thread roomThread;

        /// <summary>
        /// Constructor call.  Creates a new Door object.
        /// </summary>
        /// <param name="content">The content manager to use when loading assets.</param>
        /// <param name="orient">The orientation of the room (either facing left or facing right).</param>
        /// <param name="roomName">The name of the room that the door leads to.</param>
        /// <param name="connectedDoorIndex">The index of the door that is linked to this one.</param>
        /// <param name="lockT">The type of lock that is on this door.</param>
        public Door(ContentManager content, DoorOrientations orient, string roomName, int connectedDoorIndex, Locks lockT)
            : base(GameObjectList.Door, content)
        {
            Content = content;
            orientation = orient;
            linkedRoom = null;
            linkedRoomName = roomName;
            linkedDoorIndex = connectedDoorIndex;
            lockType = lockT;
            
            isOpen = false;

            if (orientation == DoorOrientations.FacingLeft)
                animationOffset = 1;
            else
                animationOffset = 0;

        }

        /// <summary>
        /// Opens the door, playing the opening animation and causing the door to be enterable.
        /// </summary>
        public void Open()
        {
            isOpen = true;
            animationSpeed[currentAnimation] = DOOR_ANIMATION_SPEED;
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
                linkedRoom = new Room(linkedRoomName, Content, linkedDoorIndex);
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
    }
}
    



