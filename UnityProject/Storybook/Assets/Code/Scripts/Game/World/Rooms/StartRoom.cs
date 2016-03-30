using UnityEngine;
using System.Collections;

public class StartRoom : RoomObject
{
    [SerializeField]
    private AudioClip m_roomMusic;
    
    private MusicManager m_musicManager;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        m_musicManager = MusicManager.Instance;
        /*
        m_musicManager.MusicTracks = m_musicTracks;
        m_musicManager.Fade(m_musicTracks[0], 5, true);
        */
        EventDispatcher.GetDispatcher<MusicEventDispatcher>().OnStartRoomEntered();
    }

    protected override void OnRoomEnter(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            return;
        /*
        m_musicManager.MusicTracks = m_musicTracks;
        m_musicManager.Fade(m_musicTracks[0], 5, true);
        */
        EventDispatcher.GetDispatcher<MusicEventDispatcher>().OnStartRoomEntered();
    }

    protected override IEnumerable OnRoomEvent(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            yield break;

        EventDispatcher.GetDispatcher<RoomEventEventDispatcher>().OnRoomCleared();
    }

    protected override void OnRoomExit(RoomMover mover)
    {
    }
}