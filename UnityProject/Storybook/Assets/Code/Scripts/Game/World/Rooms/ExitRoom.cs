using UnityEngine;
using System.Collections;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class ExitRoom : RoomObject
{
    [SerializeField]
    private AudioClip m_roomMusic;

    private MusicManager m_musicManager;

    // Use this for initialization
    protected override void Awake ()
    {
        m_musicManager = FindObjectOfType<MusicManager>();
        base.Awake();
	}

    // On entering the room, do nothing since there is nothing special in this room.
    protected override void OnRoomEnter(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            return;
        /*
        m_musicManager.Fade(m_musicTracks[0], 5, true);
        m_musicManager.MusicTracks = m_musicTracks;
        */
        EventDispatcher.GetDispatcher<MusicEventDispatcher>().OnRoomMusicChange(RoomPageData.PageGenre);
    }

    // What do we do when all players reach the center of the room?
    // Most likely nothing, but that may change.
    protected override IEnumerable OnRoomEvent(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            yield break;

        //EventDispatcher.GetDispatcher<UIEventDispatcher>().OnRoomCleared();
        SceneFading fader = SceneFading.Instance();
        fader.LoadScene("DemoCompleteScene");
    }

    // What happens when the players leave this room?
    // Hint: Nothing.
    protected override void OnRoomExit(RoomMover mover)
    {
        // TODO: Load next level; or clear current level, generate new start position, and move players there.
    }
}
