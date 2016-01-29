using UnityEngine;
using System.Collections;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class ShopRoom : RoomObject {
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

    [SerializeField]
    private UIHandler m_shopUI;

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
        m_musicManager.MusicTracks = m_musicTracks;
        StartCoroutine(m_musicManager.Fade(m_musicTracks[0], 5, true));
        return;
    }

    // What do we do when all players reach the center of the room?
    // Most likely nothing, but that may change.
    public override void OnRoomEvent()
    {
        // TODO: open the shop UI.
        Debug.Log("Whaddya buyin'?");
        ShopUIHandler shopUI = Instantiate(m_shopUI).GetComponent<ShopUIHandler>();
        shopUI.RegisterShopRoom(this);
        shopUI.PopulateMenu();
        return;
    }

    // What happens when the players leave this room?
    // Hint: Nothing.
    public override void OnRoomExit()
    {
        return;
    }

    public void OnShopClosed()
    {
        FindObjectOfType<StorybookPlayerMover>().OpenDeckManagementMenu();
    }
}
