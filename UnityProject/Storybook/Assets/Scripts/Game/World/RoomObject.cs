using UnityEngine;
using System.Collections;

public class RoomObject {

    public Door[] RoomDoors;
                         // Ordering for indices should be clockwise, starting from the north.
                         // In a standard 1x1 room, it would be like:
                         // 0 - North, 1 - East, 2 - South, 3 - West.
                         // In a larger room, it would probably be more like 0-N, 1-N, 2-E, 3-S, 4-S, and so on.
                         // If a door does not exist here, just use "null"
    public Location RoomLocation;
    public int RoomSize;
    public int RoomSizeConstraint; // Can be x1, x2, x4.
                                   // Did I handle this right? I'm not really sure what the difference is meant to be between
                                   // room size and room size constraint.

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
