using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class ShopRoom : 
    PlayerEventRoom, 
    ShopEventDispatcher.IShopEventListener
{
    [SerializeField]
    private AudioClip m_roomMusic;

    private MusicManager m_musicManager;

    [SerializeField]
    private UIHandler m_shopUI;

    private bool m_hasGeneratedPages = false;

    private List<PageData> m_shopPages = new List<PageData>();

    public EventDispatcher Dispatcher
    {
        get { return EventDispatcher.GetDispatcher<ShopEventDispatcher>(); }
    }

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
        EventDispatcher.GetDispatcher<ShopEventDispatcher>().RemoveListener(this);
    }

    // On entering the room, do nothing since there is nothing special in this room.
    protected override void OnRoomEnter(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            return;

        Debug.Log("Welcome to the shop!");
        /*
        m_musicManager.MusicTracks = m_musicTracks;
        m_musicManager.Fade(m_musicTracks[0], 5, true);
        */
        EventDispatcher.GetDispatcher<MusicEventDispatcher>().OnShopEntered();
    }

    protected override void OnNetworkEvent()
    {
        if (!(m_hasGeneratedPages && m_shopPages.Count <= 0))
        {
            ShopUIHandler shopUI = Instantiate(m_shopUI).GetComponent<ShopUIHandler>();
            shopUI.RegisterShopRoom(this);
            shopUI.PopulateMenu(m_shopPages);
        }
        else
        {
            OnShopClosed();
        }
    }

    // What happens when the players leave this room?
    // Hint: Nothing.
    protected override void OnRoomExit(RoomMover mover)
    {
    }

    public void OnShopClosed()
    {
        LocalPlayerFinished();
    }

    public void PagesGenerated(PageData[] pagesGenerated)
    {
        m_shopPages = new List<PageData>(pagesGenerated);
        m_hasGeneratedPages = true;
    }

    public void PageTraded(PageData pageTraded)
    {
        m_shopPages.Remove(pageTraded);
    }
}
