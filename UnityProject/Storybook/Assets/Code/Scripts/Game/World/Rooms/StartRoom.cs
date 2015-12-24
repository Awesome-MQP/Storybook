using UnityEngine;
using System.Collections;

public class StartRoom : RoomObject
{
    [SerializeField]
    private AudioClip m_roomMusic;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnRoomEnter()
    {
        return;
    }

    public override void OnRoomEvent()
    {
        return;
    }

    public override void OnRoomExit()
    {
        // TODO: Load next level; or clear current level, generate new start position, and move players there.
        return;
    }
}