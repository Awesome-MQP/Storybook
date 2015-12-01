using UnityEngine;
using System.Collections;

public struct RoomData {

    private bool m_isNorthDoorActive;
    private bool m_isEastDoorActive;
    private bool m_isSouthDoorActive;
    private bool m_isWestDoorActive;
    private MapManager.RoomType m_roomType;
    private float m_costToHere;
    private int m_parentX;
    private int m_parentY;

    public RoomData(bool isNorthDoorActive, bool isEastDoorActive, bool isSouthDoorActive, bool isWestDoorActive, MapManager.RoomType roomType)
    {
        m_isNorthDoorActive = isNorthDoorActive;
        m_isEastDoorActive = isEastDoorActive;
        m_isSouthDoorActive = isSouthDoorActive;
        m_isWestDoorActive = isWestDoorActive;
        m_roomType = roomType;
        m_costToHere = 0;
        m_parentX = -1;
        m_parentY = -1;
    }

    public MapManager.RoomType RoomType
    {
        get { return m_roomType; }
        set { m_roomType = value; }
    }

    public bool IsNorthDoorActive
    {
        get { return m_isNorthDoorActive; }
        set { m_isNorthDoorActive = value; }
    }

    public bool IsEastDoorActive
    {
        get { return m_isEastDoorActive; }
        set { m_isEastDoorActive = value; }
    }

    public bool IsSouthDoorActive
    {
        get { return m_isSouthDoorActive; }
        set { m_isSouthDoorActive = value; }
    }

    public bool IsWestDoorActive
    {
        get { return m_isWestDoorActive; }
        set { m_isWestDoorActive = value; }
    }

    public float CostToHere
    {
        get { return m_costToHere; }
        set { m_costToHere = value; }
    }

    public int ParentX
    {
        get { return m_parentX; }
        set { m_parentX = value; }
    }

    public int ParentY
    {
        get { return m_parentY; }
        set { m_parentY = value; }
    }

    /// <summary>
    /// Resets all of the AStar data
    /// </summary>
    public void ResetAStarData()
    {
        m_costToHere = 0;
        m_parentX = -1;
        m_parentY = -1;
    }
}
