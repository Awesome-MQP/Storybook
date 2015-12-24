using UnityEngine;
using System.Collections;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class ExitRoom : RoomObject {
    [SerializeField]
    private AudioClip m_roomMusic;

    // Use this for initialization
    protected override void Awake ()
    {
        base.Awake();
	}

    // On entering the room, do nothing since there is nothing special in this room.
    public override void OnRoomEnter()
    {
        return;
    }

    // What do we do when all players reach the center of the room?
    // Most likely nothing, but that may change.
    public override void OnRoomEvent()
    {
        return;
    }

    // What happens when the players leave this room?
    // Hint: Nothing.
    public override void OnRoomExit()
    {
        // TODO: Load next level; or clear current level, generate new start position, and move players there.
        return;
    }
}
