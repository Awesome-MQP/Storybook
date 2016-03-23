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
        // Create the inventory only on the first floor of the game
        if (m_floorNumber <= 1)
        {
            GameObject newInventoryObject = PhotonNetwork.Instantiate("Player/" + m_inventoryPrefab.name, Vector3.zero,
                Quaternion.identity, 0);
            PlayerInventory newInventory = newInventoryObject.GetComponent<PlayerInventory>();
            m_inventory = newInventory;
            //TODO: Inventory spawn code
            PhotonNetwork.Spawn(newInventory.photonView);
        }

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
    private PlayerInventory m_inventoryPrefab;

    private PlayerInventory m_inventory;

    public void UpdateHitPoints(int newHitPoints)
    {
        m_hitPoints = newHitPoints;
    }

    public void SetGenre(Genre playerGenre)
    {
        m_genre = playerGenre;
    }

    public void TransitionFloors(PlayerEntity oldPlayer)
    {
        HitPoints = oldPlayer.HitPoints;
        OurInventory = oldPlayer.OurInventory;
    }
}
