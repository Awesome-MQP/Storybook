﻿using UnityEngine;
using System.Collections;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class CombatRoom : RoomObject {

	// Use this for initialization
	protected override void Start ()
    {
        base.Start();
	}

    // On entering the room, do nothing since there is nothing special in this room.
    protected override void OnRoomEnter()
    {
        // TODO: Spawn monsters
        return;
    }

    // What do we do when all players reach the center of the room?
    // Most likely nothing, but that may change.
    protected override void OnRoomEvent()
    {
        // TODO: Transition into Combat.
        return;
    }

    // What happens when the players leave this room?
    // Hint: Nothing.
    protected override void OnRoomExit()
    {
        return;
    }
}
