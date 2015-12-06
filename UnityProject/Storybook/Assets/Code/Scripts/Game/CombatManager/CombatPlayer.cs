using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CombatPlayer : CombatPawn {

    [SerializeField]
    private static int m_handSize = 5;

    [SerializeField]
    private Page m_pageToUse;

    private List<Page> m_playerHand = new List<Page>();

    private CombatDeck m_playerDeck;

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
        }
    }

    public void RemovePageFromHand(Page pageToRemove)
    {
        Debug.Log("Removing page from hand");
        m_playerHand.Remove(pageToRemove);
        m_playerDeck.AddPageToGraveyard(pageToRemove);
    }

    public void DrawPageForTurn()
    {
        Page currentPage = m_playerDeck.GetNextPage();
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
            GameObject pageObject = PhotonNetwork.Instantiate(m_pageToUse.name, Vector3.zero, Quaternion.identity, 0);
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
}
