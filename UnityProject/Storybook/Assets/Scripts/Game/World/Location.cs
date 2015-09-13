using UnityEngine;
using System.Collections;

public class Location {
    // Location used to give RoomObjects a sense of where they are in the world.
    public int LocX;
    public int LocY;

    public Location(int x, int y) {
        LocX = x;
        LocY = y;
    }

    // get x-pos
    public int getX() {
        return LocX;
    }

    // Get y-pos
    public int getY()
    {
        return LocY;
    }

}
