using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CombatPlayer : CombatPawn {

    [SerializeField]
    private static int m_handSize = 4;

    [SerializeField]
    private Page m_pageToUse;

    [SerializeField]
    private Page m_otherPageToUse;

    private List<Page> m_playerHand = new List<Page>();

    private CombatDeck m_playerDeck;

    private bool m_canSelectMove = false;

    private Vector3 m_page1Pos = new Vector3(1586, 1185, 1044);
    private Vector3 m_page2Pos = new Vector3(1591, 1185, 1044);
    private Vector3 m_page3Pos = new Vector3(1596, 1185, 1044);
    private Vector3 m_page4Pos = new Vector3(1601, 1185, 1044);
    private Vector3 m_page5Pos = new Vector3(1606, 1185, 1044);

    private GameObject[] m_displayedPages = new GameObject[5];

    private int m_selectedPageIndex;

    public int SelectedPageIndex
    {
        get { return m_selectedPageIndex; }
        set { m_selectedPageIndex = value; }
    }

    public void Start()
    {
        base.Start();
        if (PhotonNetwork.isMasterClient)
        {
            _createDeck();
        }
    }
    
    public Page[] PlayerHand
    {
        get { return m_playerHand.ToArray(); }
    }

    public void DrawStartingHand()
    {
        for (int i = 0; i < m_handSize; i++)
        {
            Page currentPage = m_playerDeck.GetNextPage();
            m_playerHand.Add(currentPage);
            Debug.Log("Draw a card");
            
            if(PhotonNetwork.player.ID == PawnId)
            {
                //DrawPageOnScreen(currentPage, i);
            }
        }
    }
    
    public void DrawPageOnScreen(Page thePage, int pageCounter)
    {
        string pageName = thePage.PageGenre.ToString();
        string pathToPage = "Pages/" + pageName + "Page";
        Debug.Log("Card #: " + pageCounter + " is " + pageName);
        Vector3 posToUse = Vector3.zero;
        switch (pageCounter)
        {
            case 0:
                posToUse = m_page1Pos;
                break;
            case 1:
                posToUse = m_page2Pos;
                break;
            case 2:
                posToUse = m_page3Pos;
                break;
            case 3:
                posToUse = m_page4Pos;
                break;
            case 4:
                posToUse = m_page5Pos;
                break;
            default:
                break;
        }
        GameObject go = Instantiate(Resources.Load(pathToPage), posToUse, Quaternion.identity) as GameObject;
        GameObject goLevel = go.transform.GetChild(1).gameObject;
        goLevel.GetComponent<TextMesh>().text = "Level " + thePage.PageLevel + "\n" + thePage.PageType.ToString();
        m_displayedPages[pageCounter] = go;
    }

    public void RemovePageFromHand(Page pageToRemove)
    {
        m_playerHand.Remove(pageToRemove);

        /*
        if (PhotonNetwork.player.ID == PawnId)
        {
            Destroy(m_displayedPages[SelectedPageIndex]);
            ShiftPages(SelectedPageIndex);
        }
        */

        m_playerDeck.AddPageToGraveyard(pageToRemove);
    }

    public void ShiftPages(int removedSlot)
    {
        switch (removedSlot)
        {
            case 0:
                m_displayedPages[0] = m_displayedPages[1];
                m_displayedPages[0].transform.position = m_page1Pos;
                m_displayedPages[1] = m_displayedPages[2];
                m_displayedPages[1].transform.position = m_page2Pos;
                m_displayedPages[2] = m_displayedPages[3];
                m_displayedPages[2].transform.position = m_page3Pos;
                m_displayedPages[3] = m_displayedPages[4];
                m_displayedPages[3].transform.position = m_page4Pos;
                m_displayedPages[4] = null;
                break;
            case 1:
                m_displayedPages[1] = m_displayedPages[2];
                m_displayedPages[1].transform.position = m_page2Pos;
                m_displayedPages[2] = m_displayedPages[3];
                m_displayedPages[2].transform.position = m_page3Pos;
                m_displayedPages[3] = m_displayedPages[4];
                m_displayedPages[3].transform.position = m_page4Pos;
                m_displayedPages[4] = null;
                break;
            case 2:
                m_displayedPages[2] = m_displayedPages[3];
                m_displayedPages[2].transform.position = m_page3Pos;
                m_displayedPages[3] = m_displayedPages[4];
                m_displayedPages[3].transform.position = m_page4Pos;
                m_displayedPages[4] = null;
                break;
            case 3:
                m_displayedPages[3] = m_displayedPages[4];
                m_displayedPages[3].transform.position = m_page4Pos;
                m_displayedPages[4] = null;
                break;
            case 4:
                break;
            default:
                break;
        }
    }

    public void DrawPageForTurn()
    {
        Page currentPage = m_playerDeck.GetNextPage();
        m_playerHand.Add(currentPage);
        if (PhotonNetwork.player.ID == PawnId)
        {
            //DrawPageOnScreen(currentPage, 4);
        }
    }

    public CombatDeck PlayerDeck
    {
        get { return m_playerDeck; }
    }

    public void SendDeckPageViewIds(int[] viewIds)
    {
        GetComponent<PhotonView>().RPC("RPCSendDeckIds", PhotonTargets.Others, viewIds);
    }

    [PunRPC]
    public void RPCSendDeckIds(int[] viewIds)
    {
        List<Page> deckList = new List<Page>();
        foreach(int i in viewIds)
        {
            PhotonView pagePhotonView = PhotonView.Find(i);
            Page page = pagePhotonView.GetComponent<Page>();
            deckList.Add(page);
        }
        m_playerDeck = new CombatDeck(deckList);
        DrawStartingHand();
    }

    //TODO: Get the player inventory from the given PlayerEntity
    private CombatDeck _initializePlayerDeck(/*PlayerEntity playerToCreateFor*/)
    {
        List<Page> deckPages = new List<Page>();
        for (int i = 0; i < 20; i++)
        {
            GameObject pageObject;
            if (i < 10)
            {
                pageObject = PhotonNetwork.Instantiate(m_pageToUse.name, Vector3.zero, Quaternion.identity, 0);
            }
            else
            {
                pageObject = PhotonNetwork.Instantiate(m_otherPageToUse.name, Vector3.zero, Quaternion.identity, 0);
            }
            PhotonNetwork.Spawn(pageObject.GetComponent<PhotonView>());
            Page page = pageObject.GetComponent<Page>();
            int pageViewId = pageObject.GetComponent<PhotonView>().viewID;
            deckPages.Add(page);
        }
        CombatDeck playerDeck = new CombatDeck(deckPages);
        playerDeck.ShuffleDeck();
        return playerDeck;
    }

    private void _createDeck()
    {
        CombatDeck pawnDeck = _initializePlayerDeck();
        int[] pageViewIds = pawnDeck.GetPageViewIds();
        m_playerDeck = pawnDeck;
        DrawStartingHand();
        SendDeckPageViewIds(pageViewIds);
    }

    /// <summary>
    /// Sends the player move over network to the corresponding pawn in all clients
    /// </summary>
    /// <param name="playerId">The PawnID of the player submitting the move</param>
    /// <param name="targetIds">The PawnID of the targets of the selected move</param>
    /// <param name="moveIndex">The index of the move in the player's hand of moves</param>
    [PunRPC]
    protected void SendPlayerMoveOverNetwork(int playerId, int[] targetIds, int moveIndex)
    {
        Debug.Log("Other player submitted move");

        Page chosenPage = PlayerHand[moveIndex];
        PlayerMove chosenMove = chosenPage.PlayerCombatMove;

        RemovePageFromHand(chosenPage);

        List<CombatPawn> targets = new List<CombatPawn>();

        // Determine the targets of the move based on the list of target ids
        CombatPawn[] possibleTargetList = null;

        // If the move is an attack, the possible targets are the enemy list
        if (chosenMove.IsMoveAttack)
        {
            Debug.Log("Move is an attack");
            possibleTargetList = GetPawnsOpposing();
        }

        // If the move is a support move, the possible targets are the player list
        else
        {
            Debug.Log("Move is not an attack");
            possibleTargetList = GetPawnsOnTeam();
        }

        // Iterate through the possible targets and find the targets based on the targetIds
        foreach (CombatPawn pawn in possibleTargetList)
        {
            if (targetIds.Contains(pawn.PawnId))
            {
                targets.Add(pawn);
            }
        }

        chosenMove.SetMoveOwner(this);
        chosenMove.SetMoveTargets(targets);
        chosenMove.InitializeMove();
        SetMoveForTurn(chosenMove);
        SetHasSubmittedMove(true);
    }

    // Set the stat mods from pages in a player's hand to their stats
    public void CalculateStatMods()
    {
        foreach(Page p in m_playerHand)
        {
            switch(p.PageGenre)
            {
                case Genre.Horror:
                    AttackMod = (AttackMod + p.PageLevel);
                    break;
                case Genre.SciFi:
                    DefenseMod = (DefenseMod + p.PageLevel);
                    break;
                case Genre.GraphicNovel:
                    SpeedMod = (SpeedMod + p.PageLevel);
                    break;
                case Genre.Fantasy:
                    HitpointsMod = (HitpointsMod + p.PageLevel);
                    break;
                default:
                    break;
            }
        }
    }

    public void PrintPlayerHand()
    {
        string move = "";
        for (int i = 0; i < m_playerHand.Count; i++)
        {
            Page p = m_playerHand[i];
            if (p.PageType == MoveType.Attack)
            {
                move = "Attack";
            }
            else if (p.PageType == MoveType.Boost)
            {
                move = "Boost";
            }
            Debug.Log("Page " + i + " = " + move);
        }
    }
}
