using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(MapManager))]
[RequireComponent(typeof(DungeonMaster))]
public class BaseStorybookGame : GameManager
{
    private DungeonMaster m_dungeonMaster;

    [SerializeField]
    private ResourceAsset m_playerMoverPrefab = new ResourceAsset(typeof(StorybookPlayerMover));

    [SerializeField]
    private ResourceAsset m_defaultCombatManager = new ResourceAsset(typeof(CombatManager));

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

    protected override void Awake()
    {
        m_dungeonMaster = GetComponent<DungeonMaster>();

        base.Awake();
    }

    public override void OnStartOwner(bool wasSpawn)
    {
        base.OnStartOwner(wasSpawn);

        //Startup the map manager
        m_mapManager = GetComponent<MapManager>();
        m_mapManager.GenerateMap();

        //Spawn the player mover on the map
        BasePlayerMover mover = PhotonNetwork.Instantiate<BasePlayerMover>(m_playerMoverPrefab,
            Vector3.zero, Quaternion.identity, 0);
        mover.Construct(m_mapManager.StartRoom);
        Mover = mover;
        PhotonNetwork.Spawn(mover.photonView);
    }

    public void StartCombat(CombatInstance combatInstance)
    {
        Assert.IsTrue(IsMine);

        CombatManager newManager = PhotonNetwork.Instantiate<CombatManager>(m_defaultCombatManager, Vector3.zero,
            Quaternion.identity, 0);
        StartCombat(newManager, combatInstance);
    }

    public void StartCombat(CombatManager combatManager, CombatInstance combatInstance)
    {
        Assert.IsTrue(IsMine);

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
        const string resourcePathTemplate = "Resources/Player/WorldPawn/{0}WorldPawn";
        return new ResourceAsset(string.Format(resourcePathTemplate, genre), typeof(WorldPawn));
    }
}
