using UnityEngine;
using System.Collections;

public class RoomData : MonoBehaviour {

    private bool m_isNorthDoorActive;
    private bool m_isEastDoorActive;
    private bool m_isSouthDoorActive;
    private bool m_isWestDoorActive;
    private MapManager.RoomType m_roomType;

    public RoomData(bool isNorthDoorActive, bool isEastDoorActive, bool isSouthDoorActive, bool isWestDoorActive)
    {
        m_isNorthDoorActive = isNorthDoorActive;
        m_isEastDoorActive = isEastDoorActive;
        m_isSouthDoorActive = isSouthDoorActive;
        m_isWestDoorActive = isWestDoorActive;
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
}
