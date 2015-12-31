using UnityEngine;
using System.Collections;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class ShopRoom : RoomObject {
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
    public override void OnRoomEnter()
    {
        // TODO: spawn shopkeeper
        Debug.Log("Welcome to the shop!");
        m_musicManager.Fade(m_roomMusic, 5, true);
        return;
    }

    // What do we do when all players reach the center of the room?
    // Most likely nothing, but that may change.
    public override void OnRoomEvent()
    {
        // TODO: open the shop UI.
        Debug.Log("Whaddya buyin'?");
        return;
    }

    // What happens when the players leave this room?
    // Hint: Nothing.
    public override void OnRoomExit()
    {
        return;
    }
}
