using UnityEngine;
using System.Collections;
using System.Net.Security;

public struct PageData : INetworkSerializeable
{
    private int m_pageLevel;
    private int m_inventoryId;
    private Genre m_pageGenre;
    private MoveType m_pageType;
    private bool m_isRare;

    public PageData(int pageLevel, Genre pageGenre)
    {
        m_pageLevel = pageLevel;
        m_pageGenre = pageGenre;
        m_inventoryId = -1;
        m_pageType = MoveType.None;
        m_isRare = false;
    }

    public PageData(int pageLevel, Genre pageGenre, int inventoryId)
    {
        m_pageLevel = pageLevel;
        m_pageGenre = pageGenre;
        m_inventoryId = inventoryId;
        m_pageType = MoveType.None;
        m_isRare = false;
    }

    public PageData(int pageLevel, Genre pageGenre, MoveType pageType)
    {
        m_pageLevel = pageLevel;
        m_pageGenre = pageGenre;
        m_inventoryId = -1;
        m_pageType = pageType;
        m_isRare = false;
    }

    public PageData(int pageLevel, Genre pageGenre, MoveType pageType, bool isRare)
    {
        m_pageLevel = pageLevel;
        m_pageGenre = pageGenre;
        m_pageType = pageType;
        m_isRare = isRare;
        m_inventoryId = -1;
    }

    /// <summary>
    /// The level of the page
    /// </summary>
    public int PageLevel
    {
        get { return m_pageLevel; }
        set { m_pageLevel = value; }
    }

    /// <summary>
    /// The inventory index of the page
    /// </summary>
    public int InventoryId
    {
        get { return m_inventoryId; }
        set { m_inventoryId = value; }
    }

    /// <summary>
    /// The genre of the page
    /// </summary>
    public Genre PageGenre
    {
        get { return m_pageGenre; }
        set { m_pageGenre = value; }
    }

    /// <summary>
    /// The move type of the page
    /// </summary>
    public MoveType PageMoveType
    {
        get { return m_pageType; }
        set { m_pageType = value; }
    }

    /// <summary>
    /// The rarity of the page; true if it is rare, false otherwise
    /// </summary>
    public bool IsRare
    {
        get { return m_isRare; }
        set { m_isRare = value; }
    }

    public void OnSerialize(PhotonStream stream)
    {
        stream.SendNext(m_pageLevel);
        stream.SendNext(m_inventoryId);
        stream.SendNext(m_isRare);
        stream.SendNext((int)m_pageGenre);
        stream.SendNext((int)m_pageType);
    }

    public void OnDeserialize(PhotonStream stream)
    {
        m_pageLevel = (int) stream.ReceiveNext();
        m_inventoryId = (int) stream.ReceiveNext();
        m_isRare = (bool) stream.ReceiveNext();
        m_pageGenre = (Genre) stream.ReceiveNext();
        m_pageType = (MoveType) stream.ReceiveNext();
    }
}
