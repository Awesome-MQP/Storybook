using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(MapManager))]
public class BaseStorybookGame : GameManager
{
    [SerializeField]
    private DungeonMaster m_dungeonMaster = new DungeonMaster();

    [SerializeField]
    private ResourceAsset m_playerMoverPrefab = new ResourceAsset(typeof(StorybookPlayerMover));

    private ResourceAsset m_defaultCombatManager = new ResourceAsset(typeof(CombatManager));

    private MapManager m_mapManager;

    /// <summary>
    /// The games dungeon manager
    /// </summary>
    public DungeonMaster DM
    {
        get { return m_dungeonMaster; }
    }

    public override void OnStartOwner(bool wasSpawn)
    {
        m_mapManager = GetComponent<MapManager>();
        m_mapManager.GenerateMap();

        //Spawn the player mover on the map
        BasePlayerMover mover = PhotonNetwork.Instantiate<BasePlayerMover>(m_playerMoverPrefab,
            Vector3.zero, Quaternion.identity, 0);
        mover.Construct(m_mapManager.StartRoom);
        PhotonNetwork.Spawn(mover.photonView);

        base.OnStartOwner(wasSpawn);
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
}
