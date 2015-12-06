using UnityEngine;
using System.Collections;

public abstract class PageMove : PlayerMove {

    /// <summary>
    /// The enum int value for the genre of the page
    /// </summary>
    [SerializeField]
    private Genre m_pageGenre;

    [SerializeField]
    private MoveType m_pageType;

    public Genre PageGenre
    {
        get { return m_pageGenre; }
        set { m_pageGenre = value; }
    }

    public MoveType PageType
    {
        get { return m_pageType; }
        set { m_pageType = value; }
    }
}
