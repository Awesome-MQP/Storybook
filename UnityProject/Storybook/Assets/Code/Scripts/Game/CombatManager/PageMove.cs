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

    // Constructor for PageMove, used for wrapper.
    public void construct(Page page)
    {
        this.PageGenre = page.PageGenre;
        this.PageType = page.PageType;
        this.MoveLevel = page.PageLevel;
        this.SetMoveOwner(page.PageOwner);
        // If page is rare, set targets to all pawns on the enemy team
        if (page.Rarity)
        {
            this.SetNumberOfTargets(FindObjectOfType<GameManager>().EnemyTeamForCombat.ActivePawnsOnTeam.Count);
        }
        else // else it's common, only 1 target
        {
            this.SetNumberOfTargets(1);
        }
    }
}
