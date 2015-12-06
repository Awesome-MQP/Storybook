using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CombatPlayer : CombatPawn {

    [SerializeField]
    private int m_handSize = 5;

    private List<Page> m_playerHand = new List<Page>();

    private CombatDeck m_playerDeck;

    [SerializeField]
    private PlayerMove[] m_testHand;

    public PlayerMove[] TestHand
    {
        get { return m_testHand; }
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
        set { m_playerDeck = value; }
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
}
