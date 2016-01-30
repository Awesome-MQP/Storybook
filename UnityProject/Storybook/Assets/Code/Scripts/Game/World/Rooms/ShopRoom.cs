using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class ShopRoom : RoomObject, UIEventDispatcher.IShopEventListener {
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

    private bool m_hasGeneratedPages = false;

    private List<PageData> m_shopPages = new List<PageData>();

    public EventDispatcher Dispatcher { get { return EventDispatcher.GetDispatcher<UIEventDispatcher>(); } }

    private bool m_isCurrentRoom;

    // Use this for initialization
    protected override void Awake ()
    {
        m_musicManager = FindObjectOfType<MusicManager>();
        base.Awake();
	}

    void Start()
    {
        EventDispatcher.GetDispatcher<UIEventDispatcher>().RegisterEventListener(this);
    }

    // On entering the room, do nothing since there is nothing special in this room.
    public override void OnRoomEnter()
    {
        Debug.Log("Welcome to the shop!");
        m_isCurrentRoom = true;
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
        if (!(m_hasGeneratedPages && m_shopPages.Count <= 0))
        {
            ShopUIHandler shopUI = Instantiate(m_shopUI).GetComponent<ShopUIHandler>();
            shopUI.RegisterShopRoom(this);
            shopUI.PopulateMenu(m_shopPages);
            return;
        }
        else
        {
            EventDispatcher.GetDispatcher<UIEventDispatcher>().OnRoomCleared();
        }
    }

    // What happens when the players leave this room?
    // Hint: Nothing.
    public override void OnRoomExit()
    {
        m_isCurrentRoom = false;
        return;
    }

    public void OnShopClosed()
    {

    }

    public void PagesGenerated(PageData[] pagesGenerated)
    {
        if (m_isCurrentRoom)
        {
            Debug.Log("Shop - PagesGenerated");
            m_shopPages = new List<PageData>(pagesGenerated);
            m_hasGeneratedPages = true;
        }
    }

    public void PageTraded(PageData pageTraded)
    {
        if (m_isCurrentRoom)
        {
            Debug.Log("Shop - PageTraded");
            m_shopPages.Remove(pageTraded);
        }
    }
}
