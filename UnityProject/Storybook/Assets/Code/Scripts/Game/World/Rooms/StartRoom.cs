using UnityEngine;
using System.Collections;

public class StartRoom : RoomObject
{
    [SerializeField]
    private AudioClip m_roomMusic;

    private MusicManager m_musicManager;

    protected override void Awake()
    {
        m_musicManager = FindObjectOfType<MusicManager>();
        m_musicManager.Fade(m_roomMusic, 5, true);
        base.Awake();
    }

    public override void OnRoomEnter()
    {
        m_musicManager.Fade(m_roomMusic, 5, true);
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