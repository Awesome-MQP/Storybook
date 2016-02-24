using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class ShopRoom : RoomObject, ShopEventDispatcher.IShopEventListener {
    [SerializeField]
    private AudioClip m_roomMusic;

    private MusicManager m_musicManager;

    [SerializeField]
    private UIHandler m_shopUI;

    private bool m_hasGeneratedPages = false;

    private List<PageData> m_shopPages = new List<PageData>();

    public EventDispatcher Dispatcher { get { return EventDispatcher.GetDispatcher<ShopEventDispatcher>(); } }

    private bool m_isCurrentRoom;

    // Use this for initialization
    protected override void Awake ()
    {
        m_musicManager = FindObjectOfType<MusicManager>();
        base.Awake();
	}

    void Start()
    {
        EventDispatcher.GetDispatcher<ShopEventDispatcher>().RegisterEventListener(this);
    }

    void OnDestroy()
    {
        EventDispatcher.GetDispatcher<ShopUIEventDispatcher>().RemoveListener(this);
    }

    // On entering the room, do nothing since there is nothing special in this room.
    protected override void OnRoomEnter(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            return;

        Debug.Log("Welcome to the shop!");
        m_isCurrentRoom = true;
        m_musicManager.MusicTracks = m_musicTracks;
        m_musicManager.Fade(m_musicTracks[0], 5, true);
    }

    // What do we do when all players reach the center of the room?
    // Most likely nothing, but that may change.
    protected override IEnumerable OnRoomEvent(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            yield break;

        // TODO: open the shop UI.
        Debug.Log("Whaddya buyin'?");
        if (!(m_hasGeneratedPages && m_shopPages.Count <= 0))
        {
            //TODO: Don't open the UI here, open it in an event handler.
            //TODO: This is not network safe as this event is only called on the world owner (usually the master client).
            ShopUIHandler shopUI = Instantiate(m_shopUI).GetComponent<ShopUIHandler>();
            shopUI.RegisterShopRoom(this);
            shopUI.PopulateMenu(m_shopPages);
        }
        else
        {
            EventDispatcher.GetDispatcher<RoomEventEventDispatcher>().OnRoomCleared();
        }
    }

    // What happens when the players leave this room?
    // Hint: Nothing.
    protected override void OnRoomExit(RoomMover mover)
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
