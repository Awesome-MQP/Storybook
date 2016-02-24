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
        m_musicManager.MusicTracks = m_musicTracks;
        m_musicManager.Fade(m_musicTracks[0], 5, true);
        base.Awake();
    }

    protected override void OnRoomEnter(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            return;

        m_musicManager.MusicTracks = m_musicTracks;
        m_musicManager.Fade(m_musicTracks[0], 5, true);
    }

    protected override IEnumerable OnRoomEvent(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            yield break;

        ClearRoom();
    }

    protected override void OnRoomExit(RoomMover mover)
    {
    }
}