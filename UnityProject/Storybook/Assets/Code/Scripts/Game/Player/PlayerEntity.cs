using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The representation of the real world player and there stats.
/// </summary>
public class PlayerEntity : PlayerObject
{
    [SyncProperty]
    public int HitPoints
    {
        get { return m_hitPoints; }
        protected set
        {
            Assert.IsTrue(ShouldBeChanging);

            m_hitPoints = Mathf.Clamp(value, 0, m_maxHitPoints);
            PropertyChanged();
        }
    }

    [SyncProperty]
    public Genre Genre
    {
        get { return m_genre; }
        protected set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_genre = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public int MaxHitPoints
    {
        get { return m_maxHitPoints; }
        protected set
        {
            Assert.IsTrue(ShouldBeChanging);

            m_maxHitPoints = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public int Attack
    {
        get { return m_attack; }
        protected set
        {
            Assert.IsTrue(ShouldBeChanging);

            m_attack = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public int Defense
    {
        get { return m_defense; }
        protected set
        {
            Assert.IsTrue(ShouldBeChanging);

            m_defense = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public int Speed
    {
        get { return m_speed; }
        protected set
        {
            Assert.IsTrue(ShouldBeChanging);

            m_speed = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public PlayerInventory OurInventory
    {
        get { return m_inventory;}
        protected set
        {
            Assert.IsTrue(ShouldBeChanging);

            m_inventory = value;
            PropertyChanged();
        }
    }

    public override void OnStartOwner(bool wasSpawn)
    {
        PlayerInventory newInventory = PhotonNetwork.Instantiate<PlayerInventory>(m_inventoryPrefab, Vector3.zero,
            Quaternion.identity, 0);
        //TODO: Inventory spawn code
        PhotonNetwork.Spawn(newInventory.photonView);

        GameManager.GetInstance<BaseStorybookGame>().Mover.RegisterPlayer(this);
    }

    [SerializeField]
    private int m_hitPoints = 10;

    [SerializeField]
    private int m_maxHitPoints = 10;

    [SerializeField]
    private int m_attack = 1;

    [SerializeField]
    private int m_defense = 1;

    [SerializeField]
    private int m_speed = 1;

    [SerializeField]
    private int m_luck = 1;

    [SerializeField]
    private Genre m_genre = Genre.None;

    [SerializeField]
    private ResourceAsset m_inventoryPrefab = new ResourceAsset(typeof(PlayerInventory));

    private PlayerInventory m_inventory;

    public void UpdateHitPoints(int newHitPoints)
    {
        m_hitPoints = newHitPoints;
    }

    public void SetGenre(Genre playerGenre)
    {
        m_genre = playerGenre;
    }
}
