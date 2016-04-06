using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class StartRoom : 
    RoomObject, 
    DeckManagementEventDispatcher.IDeckManagementEventListener
{
    [SerializeField]
    private AudioClip m_roomMusic;
    
    private MusicManager m_musicManager;

    private HashSet<PhotonPlayer> m_readyPlayers = new HashSet<PhotonPlayer>();

    private bool m_inRoomEvent;

    [SyncProperty]
    protected bool IsInRoomEvent
    {
        get { return m_inRoomEvent; }
        set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_inRoomEvent = value;

            if(value)
                OnNetworkEvent();

            PropertyChanged();
        }
    }

    protected override void Awake()
    {
        EventDispatcher.GetDispatcher<DeckManagementEventDispatcher>().RegisterEventListener(this);

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

    void OnDestroy()
    {
        EventDispatcher.GetDispatcher<DeckManagementEventDispatcher>().RemoveListener(this);
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

        IsInRoomEvent = true;

        while (m_readyPlayers.Count < PhotonNetwork.countOfPlayersInRooms)
        {
            yield return null;
        }

        IsInRoomEvent = false;
    }

    protected override void OnRoomExit(RoomMover mover)
    {
    }

    // protected, rename as more generic
    public void OnDeckManagementClosed()
    {
        photonView.RPC(nameof(_playerFinished), Owner, PhotonNetwork.player);
    }

    [PunRPC(AllowFullCommunication = true)]
    protected void _playerFinished(PhotonPlayer player)
    {
        m_readyPlayers.Add(player);
    }

    // protected abstract
    private void OnNetworkEvent()
    {
        UnityEngine.Object loadedObject = Resources.Load("UI/DeckManagementCanvas");
        GameObject canvas = (GameObject)Instantiate(loadedObject);
        DeckManagementUIHandler uiHandler = canvas.GetComponent<DeckManagementUIHandler>();
        uiHandler.PopulateMenu();
    }
}