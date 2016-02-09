using UnityEngine;
using System.Collections;

public class StartRoom : RoomObject
{
    [SerializeField]
    private AudioClip m_roomMusic;

    [SerializeField]
    private AudioClip[] m_musicTracks; // This array holds all music tracks for a room, in an effort to make it more general. 
                                       // To make accessing tracks from this more easy to follow, use this standard for putting tracks into the array
                                       // INDEX | TRACK
                                       // 0.......RoomMusic
                                       // 1.......FightMusic
                                       // 2+......Miscellaneous

    private MusicManager m_musicManager;

    protected override void Awake()
    {
        m_musicManager = FindObjectOfType<MusicManager>();
        m_musicManager.MusicTracks = m_musicTracks;
        StartCoroutine(m_musicManager.Fade(m_musicTracks[0], 5, true));
        base.Awake();
    }

    protected override void OnRoomEnter(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            return;

        m_musicManager.MusicTracks = m_musicTracks;
        StartCoroutine(m_musicManager.Fade(m_musicTracks[0], 5, true));
    }

    protected override IEnumerable OnRoomEvent(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            yield break;

        //TODO: If this is called for every room then it should be done in the base class. It also should be done by something that is not the UIEventDispatcher as there is no reason this is UI only code.
        EventDispatcher.GetDispatcher<UIEventDispatcher>().OnRoomCleared();
    }

    protected override void OnRoomExit(RoomMover mover)
    {
    }
}