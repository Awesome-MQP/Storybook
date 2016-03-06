﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(MapManager))]
[RequireComponent(typeof(DungeonMaster))]
public class BaseStorybookGame : GameManager
{
    private DungeonMaster m_dungeonMaster;

    [SerializeField]
    private StorybookPlayerMover m_playerMoverPrefab;

    [SerializeField]
    private ResourceAsset m_defaultCombatManager = new ResourceAsset(typeof(CombatManager));

    [SerializeField]
    private ResourceAsset m_defaultPlayerTeam = new ResourceAsset(typeof(PlayerTeam));

    [SerializeField]
    private ResourceAsset m_comicBookPlayerObject = new ResourceAsset(typeof(PlayerObject));

    [SerializeField]
    private ResourceAsset m_sciFiPlayerObject = new ResourceAsset(typeof(PlayerObject));

    [SerializeField]
    private ResourceAsset m_horrorPlayerObject = new ResourceAsset(typeof(PlayerObject));

    [SerializeField]
    private ResourceAsset m_fantasyPlayerObject = new ResourceAsset(typeof(PlayerObject));

    private MapManager m_mapManager;

    private BasePlayerMover m_mover;

    /// <summary>
    /// The games dungeon manager
    /// </summary>
    public DungeonMaster DM
    {
        get { return m_dungeonMaster; }
    }

    /// <summary>
    /// The player mover being used by the game.
    /// </summary>
    [SyncProperty]
    public BasePlayerMover Mover
    {
        get { return m_mover; }
        set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_mover = value;
            PropertyChanged();
        }
    }

    public ResourceAsset DefaultPlayerTeam
    {
        get { return m_defaultPlayerTeam; }
    }

    protected override void Awake()
    {
        m_dungeonMaster = GetComponent<DungeonMaster>();

        base.Awake();
    }

    public override void OnStartOwner(bool wasSpawn)
    {
        //Startup the map manager
        m_mapManager = GetComponent<MapManager>();
        m_mapManager.GenerateMap();
        RoomObject startRoom = m_mapManager.PlaceStartRoom();

        //Spawn the player mover on the map
        GameObject moverObject = PhotonNetwork.Instantiate(m_playerMoverPrefab.name, Vector3.zero, Quaternion.identity, 0);
        BasePlayerMover mover = moverObject.GetComponent<BasePlayerMover>();
        m_mover = mover;
        m_mover.Construct(startRoom);
        PhotonNetwork.Spawn(mover.photonView);

        InitializeCamera(startRoom.CameraNode.position, startRoom.CameraNode.rotation);
        photonView.RPC(nameof(InitializeCamera), PhotonTargets.Others, startRoom.CameraNode.position, startRoom.CameraNode.rotation);

        base.OnStartOwner(wasSpawn);
    }

    [PunRPC]
    public void InitializeCamera(Vector3 position, Quaternion rotation)
    {
        Camera.main.transform.position = position;
        Camera.main.transform.rotation = rotation;
    }

    public override void OnSerializeReliable(PhotonStream stream, PhotonMessageInfo info, bool isInit)
    {
        base.OnSerializeReliable(stream, info, isInit);
    }

    public CombatManager StartCombat(CombatInstance combatInstance)
    {
        Assert.IsTrue(IsMine);

        CombatManager newManager = PhotonNetwork.Instantiate<CombatManager>(m_defaultCombatManager, Vector3.zero,
            Quaternion.identity, 0);
        StartCombat(newManager, combatInstance);

        return newManager;
    }

    public void StartCombat(CombatManager combatManager, CombatInstance combatInstance)
    {
        Assert.IsTrue(IsMine);

        AudioClip prevMusic = combatInstance.GetPreviousMusic(); //FindObjectOfType<GameManager>().GetComponent<MusicManager>().Music.clip;
        AudioClip fightMusic = combatInstance.GetCombatMusic();

        combatManager.Construct(combatInstance);
        PhotonNetwork.Spawn(combatManager.photonView);
    }

    /// <summary>
    /// Gets the resource asset to use for a certain genre.
    /// </summary>
    /// <param name="genre">The genre of the pawn to get.</param>
    /// <returns>A resource asset for the world pawn asset ot use for this genre.</returns>
    public virtual ResourceAsset GetWorldPawnForGenre(Genre genre)
    {
        //Generate the world pawn string from the data as a default implementation.
        const string resourcePathTemplate = "Player/WorldPawn/{0}WorldPawn";
        return new ResourceAsset(string.Format(resourcePathTemplate, genre), typeof(WorldPawn));
    }

    protected override PlayerObject CreatePlayerObject(PlayerObject playerObject)
    {
        PlayerEntity lastPlayerEntity = playerObject as PlayerEntity;
        CharacterSelectPlayerEntity playerSelectEntity = playerObject as CharacterSelectPlayerEntity;
        if (lastPlayerEntity)
        {
            ResourceAsset prefabForGenre = _GetEntityByGenre(lastPlayerEntity.Genre);
            return PhotonNetwork.Instantiate<PlayerEntity>(prefabForGenre, Vector3.zero, Quaternion.identity, 0);
        }
        else if (playerSelectEntity)
        {
            ResourceAsset prefabForGenre = _GetEntityByGenre(playerSelectEntity.PlayerGenre);
            return PhotonNetwork.Instantiate<PlayerEntity>(prefabForGenre, Vector3.zero, Quaternion.identity, 0);
        }
        else
        {
            return base.CreatePlayerObject(playerObject);
        }
    }

    private ResourceAsset _GetEntityByGenre(Genre oldGenre)
    {
        switch (oldGenre)
        {
            case Genre.Fantasy:
                return m_fantasyPlayerObject;
            case Genre.Horror:
                return m_horrorPlayerObject;
            case Genre.SciFi:
                return m_sciFiPlayerObject;
            case Genre.GraphicNovel:
                return m_comicBookPlayerObject;
        }

        return null;
    }
}
