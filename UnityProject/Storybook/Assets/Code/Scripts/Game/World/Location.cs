using System;
using UnityEngine;
using System.Collections;

public struct Location : INetworkSerializeable
{
    private static Location s_north = new Location(-1, 0);
    private static Location s_east = new Location(0, 1);
    private static Location s_south = new Location(1, 0);
    private static Location s_west = new Location(0, -1);

    public static Location North
    {
        get { return s_north; }
    }

    public static Location East
    {
        get { return s_east; }
    }

    public static Location South
    {
        get { return s_south; }
    }

    public static Location West
    {
        get { return s_west; }
    }

    // Location used to give RoomObjects a sense of where they are in the world.
    [SerializeField]
    private int m_x;

    [SerializeField]
    private int m_y;

    public Location(int locX, int locY)
    {
        m_x = locX;
        m_y = locY;
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
    public static Location operator -(Location Loc1, Location Loc2)
    {
        int DiffX = Loc2.X - Loc1.X;
        int DiffY = Loc2.Y - Loc1.Y;
        return new Location(DiffX, DiffY);
    }

    // Overloads the Add function to find the distance between 2 points
    public static Location operator +(Location Loc1, Location Loc2)
    {
        int DiffX = Loc2.X + Loc1.X;
        int DiffY = Loc2.Y + Loc1.Y;
        return new Location(DiffX, DiffY);
    }

    public static Location operator +(Location loc1, Door.Direction direction)
    {
        switch (direction)
        {
            case Door.Direction.North:
                return loc1 + North;
            case Door.Direction.East:
                return loc1 + East;
            case Door.Direction.South:
                return loc1 + South;
            case Door.Direction.West:
                return loc1 + West;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    // Gets the straight line distance between 2 locations.
    public static float Distance(Location Loc1, Location Loc2)
    {
        float XminusA = Loc1.X - Loc2.X;
        float YminusB = Loc1.Y - Loc2.Y;
        return Mathf.Sqrt((XminusA * XminusA) - (YminusB * YminusB));
    }

    public void OnSerialize(PhotonStream stream)
    {
        stream.SendNext(m_x);
        stream.SendNext(m_y);
    }

    public void OnDeserialize(PhotonStream stream)
    {
        m_x = (int) stream.ReceiveNext();
        m_y = (int) stream.ReceiveNext();
    }
}
