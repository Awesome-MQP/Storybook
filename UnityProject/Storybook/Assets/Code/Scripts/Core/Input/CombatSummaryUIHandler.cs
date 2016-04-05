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

    /// <summary>
    /// Submits the selected page from the drops and adds it to the player's inventory
    /// </summary>
    public void SubmitPressed()
    {
        BaseStorybookGame baseGameManager = GameManager.GetInstance<BaseStorybookGame>();
        GameManager gm = GameManager.GetInstance<GameManager>();
        PlayerInventory playerInv = gm.GetLocalPlayer<PlayerEntity>().OurInventory;

        // Find the first open slot in the player's inventory
        int openSlot = playerInv.FirstOpenSlot();

        // Figure out which page drop was selected
        Page selectedPage = m_pageDrops[m_selectedPage.InventoryId];

        // Destroy the page objects that aren't the selected one
        for (int i = 0; i < m_pageDrops.Count; i++)
        {
            if (i != m_selectedPage.InventoryId)
            {
                Page currentPage = m_pageDrops[i];
                PhotonNetwork.Destroy(currentPage.photonView);
                Destroy(currentPage);
            }
        }

        // Add the page and sort the inventory
        playerInv.Add(selectedPage, openSlot);
        playerInv.SortInventory(baseGameManager.DeckSize, playerInv.DynamicSize);

        EventDispatcher.GetDispatcher<CombatSummaryEventDispatcher>().CombatSummarySubmitted();
    }
}
