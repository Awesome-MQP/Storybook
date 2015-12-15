using UnityEngine;
using System.Collections;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class EmptyRoom : RoomObject {

	// Use this for initialization
	protected override void Awake ()
    {
        base.Awake();
	}

    // On entering the room, do nothing since there is nothing special in this room.
    public override void OnRoomEnter()
    {
        Debug.Log("Room entered");
        return;
    }

    // What do we do when all players reach the center of the room?
    // Most likely nothing, but that may change.
    public override void OnRoomEvent()
    {
        Debug.Log("Room event");
        return;
    }

    // What happens when the players leave this room?
    // Hint: Nothing.
    public override void OnRoomExit()
    {
        Debug.Log("Room exited");
        return;
    }
}
