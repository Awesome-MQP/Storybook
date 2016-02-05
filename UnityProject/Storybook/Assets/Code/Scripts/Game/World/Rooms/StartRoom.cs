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

    public override void OnRoomEnter()
    {
        m_musicManager.MusicTracks = m_musicTracks;
        StartCoroutine(m_musicManager.Fade(m_musicTracks[0], 5, true));
        return;
    }

    public override void OnRoomEvent()
    {
        EventDispatcher.GetDispatcher<UIEventDispatcher>().OnRoomCleared();
        return;
    }

    public override void OnRoomExit()
    {
        // TODO: Load next level; or clear current level, generate new start position, and move players there.
        return;
    }
}