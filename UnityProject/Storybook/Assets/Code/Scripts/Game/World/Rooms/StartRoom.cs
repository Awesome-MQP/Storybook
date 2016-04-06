using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class StartRoom : 
    PlayerEventRoom, 
    DeckManagementEventDispatcher.IDeckManagementEventListener
{
    [SerializeField]
    private AudioClip m_roomMusic;
    
    private MusicManager m_musicManager;

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
        //TODO: Remove this
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

    protected override void OnRoomExit(RoomMover mover)
    {
    }
    
    public void OnDeckManagementClosed()
    {
        LocalPlayerFinished();
    }

    protected override void OnNetworkEvent()
    {
        UnityEngine.Object loadedObject = Resources.Load("UI/DeckManagementCanvas");
        GameObject canvas = (GameObject)Instantiate(loadedObject);
        DeckManagementUIHandler uiHandler = canvas.GetComponent<DeckManagementUIHandler>();
        uiHandler.PopulateMenu();
    }
}