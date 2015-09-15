using UnityEngine;
using System.Collections;

public struct Location {
    // Location used to give RoomObjects a sense of where they are in the world.
    [SerializeField]
    private int m_x;

    [SerializeField]
    private int m_y;

    public Location(int LocX, int LocY)
    {
        m_x = LocX;
        m_y = LocY;
    }

    public int X
    {
        get { return m_x; }
        set { m_x = value; }
    }

    public int Y
    {
        get { return m_y; }
        set { m_y = value; }
    }

    // Overloads the Subtract function to find the distance between 2 points
    public static float operator -(Location Loc1, Location Loc2)
    {
        float XminusA = Loc1.X - Loc2.X;
        float YminusB = Loc1.Y - Loc2.Y;
        return Mathf.Sqrt((XminusA * XminusA) - (YminusB * YminusB));
    }
}
