using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatDeck {

    List<Page> m_deckContents;
    List<Page> m_deckList;
    List<Page> m_graveyardContents = new List<Page>();

	public CombatDeck(List<Page> deckContents)
    {
        m_deckContents = deckContents;
        m_deckList = deckContents;
    }

    public void ShuffleDeck()
    {
        List<Page> newDeckOrder = new List<Page>();
        while (m_deckContents.Count > 0)
        {
            int pageIndex = Random.Range(0, m_deckContents.Count);
            Page currentPage = m_deckContents[pageIndex];
            m_deckContents.Remove(currentPage);
            newDeckOrder.Add(currentPage);
        }
        m_deckContents = newDeckOrder;
        Debug.Log("Deck shuffled");
    }

    public Page GetNextPage()
    {
        if (m_deckContents.Count > 0)
        {
            Page nextPage = m_deckContents[0];
            m_deckContents.Remove(nextPage);
            return nextPage;
        }
        else
        {
            Debug.Log("Rebuilding deck from graveyard");
            _rebuildDeckFromGraveyard();
            return GetNextPage();
        }
    }

    private void _rebuildDeckFromGraveyard()
    {
        m_deckContents = new List<Page>(m_graveyardContents);
        m_graveyardContents = new List<Page>();
        ShuffleDeck();
    }

    public void AddPageToGraveyard(Page pageToAdd)
    {
        m_graveyardContents.Add(pageToAdd);
    }

    public Page[] DeckContents
    {
        get { return m_deckContents.ToArray(); }
    }

    public int[] GetPageViewIds()
    {
        int[] pageViewIds = new int[m_deckContents.Count];
        int i = 0;
        foreach (Page page in m_deckContents)
        {
            int pageId = page.GetComponent<PhotonView>().viewID;
            pageViewIds[i] = pageId;
            i++;
        }
        return pageViewIds;
    }
}
