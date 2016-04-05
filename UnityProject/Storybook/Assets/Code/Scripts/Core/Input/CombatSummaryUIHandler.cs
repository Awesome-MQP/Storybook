using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class CombatSummaryUIHandler : PageUIHandler {

    [SerializeField]
    private int m_defaultPageDrops = 2;

    [SerializeField]
    private ScrollRect m_pageScrollRect;

    [SerializeField]
    private Button m_submitButton;

    private PageButton m_selectedPage = null;
    private List<Page> m_pageDrops = new List<Page>();
    private Genre m_combatGenre;
    private int m_combatLevel;

	// Use this for initialization
	void Start ()
    {
        m_submitButton.interactable = false;
	}

    public void PopulateMenu(int combatLevel, Genre combatGenre)
    {
        m_combatLevel = combatLevel;
        m_combatGenre = combatGenre;
        DungeonMaster dm = FindObjectOfType<DungeonMaster>();
        for (int i = 0; i < m_defaultPageDrops; i++)
        {
            Page currentPage = dm.GetPageDropFromCombat(m_combatGenre, m_combatLevel);
            PageData currentPageData = currentPage.GetPageData();
            m_pageDrops.Add(currentPage);
            Button button = _initializePageButton(currentPage.GetPageData());
            button.transform.SetParent(m_pageScrollRect.content, false);
            button.GetComponent<PageButton>().InventoryId = i;
        }
    }

    public override void PageButtonPressed(PageButton pageButton)
    {
        if (m_selectedPage != null)
        {
            m_selectedPage.DisplaySelectedImage(false);
        }

        if (pageButton == m_selectedPage)
        {
            m_selectedPage = null;
            m_submitButton.interactable = false;
        }
        else
        {
            m_selectedPage = pageButton;
            pageButton.DisplaySelectedImage(true);
            m_submitButton.interactable = true;
        }  
    }

    public void SubmitPressed()
    {
        GameManager gm = GameManager.GetInstance<GameManager>();
        PlayerInventory playerInv = gm.GetLocalPlayer<PlayerEntity>().OurInventory;
        int openSlot = playerInv.FirstOpenSlot();
        Page selectedPage = m_pageDrops[m_selectedPage.InventoryId];
        for (int i = 0; i < m_pageDrops.Count; i++)
        {
            if (i != m_selectedPage.InventoryId)
            {
                Page currentPage = m_pageDrops[i];
                PhotonNetwork.Destroy(currentPage.photonView);
                Destroy(currentPage);
            }
        }
        playerInv.Add(selectedPage, openSlot);
        EventDispatcher.GetDispatcher<CombatSummaryEventDispatcher>().CombatSummarySubmitted();
    }
}
