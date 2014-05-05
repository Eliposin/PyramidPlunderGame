using System;

public class Room
{
    //An Array of Door objects representing all possible paths a player
    //could take to exit the room.
    public Door[] DoorArray
    {
        get { return DoorArray; }
        //TODO Not a good setter, need to fix to account for different sized arrays.
        set { DoorArray = value; }
    }

    //An Array of Enemies representing all enemies that exist in the room.
    public Enemy[] EnemyArray{
     get { return EnemyArray;}
        //TODO Not a good setter, need to fix to account for different sized arrays.
        set {EnemyArray = value;}
    }


    public boolean IsPersistant
    {
        get { return IsPersistant; }
        //Not sure if Setter is needed here.
        set { IsPersistant = value; }

    }
    /// <summary>
    ///  Loads the room into memory. 
    ///  If IsPersistant is set to true, loads up the previously saved State file
    /// </summary>
    /// <param name="doorIndex">  represents which door the player is entering from </param>
    public void Load(int doorIndex)
    {

    }

    /// <summary>
    /// Clears all memory assets related to the room.  Saves to a saved state 
    /// file if IsPersistant is set to true.
    /// </summary>
    public void Dispose()
    {

    }


    /// <summary>
    /// Saves room state to file when room IsPersistant.
    /// </summary>
    private void saveToFile()
    {

    }
}
