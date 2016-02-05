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

    public override void OnStartOwner(bool wasSpawn)
    {
        m_inventory = GetComponentInChildren<Inventory>();
        Assert.IsNotNull(m_inventory);

        m_inventory.photonView.TransferController(Player);
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

    private Inventory m_inventory;

    public void UpdateHitPoints(int newHitPoints)
    {
        m_hitPoints = newHitPoints;
    }

    public void SetGenre(Genre playerGenre)
    {
        m_genre = playerGenre;
    }
}
