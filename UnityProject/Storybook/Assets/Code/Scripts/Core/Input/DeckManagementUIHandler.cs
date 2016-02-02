using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class DeckManagementUIHandler : UIHandler
{
    private float m_buttonHeight;
    private float m_buttonWidth;

    [SerializeField]
    private float m_gridXPadding;

    [SerializeField]
    private float m_gridYPadding;

    private PageButton m_selectedDeckPage = null;
    private PageButton m_selectedInventoryPage = null;

    private ScrollRect m_deckScrollRect;
    private ScrollRect m_inventoryScrollRect;

    public void Awake()
    {
        // Get the dimensions of the button
        m_buttonHeight = m_greenPageButton.GetComponent<RectTransform>().rect.height;
        m_buttonWidth = m_greenPageButton.GetComponent<RectTransform>().rect.width;

        // Set the values in the GridLayoutGroup for each of the scroll rects based on the page sizes
        ScrollRect[] allScrollRects = GetComponentsInChildren<ScrollRect>();
        foreach (ScrollRect scrollRect in allScrollRects)
        {
            GridLayoutGroup gridGroup = scrollRect.GetComponentInChildren<GridLayoutGroup>();
            gridGroup.cellSize = new Vector2(m_buttonWidth, m_buttonHeight);
            gridGroup.spacing = new Vector2(m_gridXPadding, m_gridYPadding);
            if (scrollRect.name == "PagesInDeck")
            {
                m_deckScrollRect = scrollRect;
            }
            if (scrollRect.name == "PagesInInventory")
            {
                m_inventoryScrollRect = scrollRect;
            }
        }
    }

    public override void PageButtonPressed(PageButton buttonPressed)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        int deckSize = 0;//gameManager.DeckSize;
        PageData pageData = buttonPressed.PageData;

        if (pageData.InventoryId < deckSize)
        {
            m_selectedDeckPage = buttonPressed;
        }
        else
        {
            m_selectedInventoryPage = buttonPressed;
        }
        _checkForSwap();
    }

    private void _checkForSwap()
    {
        if (m_selectedDeckPage != null && m_selectedInventoryPage != null)
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            PlayerInventory localInventory = null;//gameManager.GetLocalPlayerInventory();
            localInventory.Move(m_selectedDeckPage.PageData.InventoryId, m_selectedInventoryPage.PageData.InventoryId);
            _swapPagesInMenu();
            m_selectedInventoryPage = null;
            m_selectedDeckPage = null;
        }
    }

    private void _swapPagesInMenu()
    {
        int previousDeckPageId = m_selectedDeckPage.InventoryId;
        int previousInventoryPageId = m_selectedInventoryPage.InventoryId;
        m_selectedDeckPage.transform.SetParent(m_inventoryScrollRect.content, false);
        m_selectedInventoryPage.transform.SetParent(m_deckScrollRect.content, false);
        m_selectedDeckPage.InventoryId = previousInventoryPageId;
        m_selectedInventoryPage.InventoryId = previousDeckPageId;
    }

    public void PopulateMenu()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        PlayerInventory pi = null;//gameManager.GetLocalPlayerInventory();

        ScrollRect[] allScrollRects = GetComponentsInChildren<ScrollRect>();

        RectTransform deckContent = m_deckScrollRect.content;
        RectTransform outOfDeckContent = m_inventoryScrollRect.content;

        for (int i = 0; i < 0 /*gameManager.DeckSize*/; i++)
        {
            Inventory.Slot currentSlot = pi[i];
            if (!currentSlot.IsEmpty)
            {
                Item currentItem = currentSlot.SlotItem;
                Page currentPage = (Page)currentItem;
                PageData currentPageData = currentPage.GetPageData();
                currentPageData.InventoryId = i;
                Button pageButton = _initializePageButton(currentPageData);
                pageButton.transform.SetParent(deckContent, false);
            }
        }

        for (int i = 0 /*gameManager.DeckSize*/; i < pi.DynamicSize; i++)
        {
            Inventory.Slot currentSlot = pi[i];
            if (!currentSlot.IsEmpty)
            {
                Item currentItem = currentSlot.SlotItem;
                Page currentPage = (Page)currentItem;
                PageData currentPageData = currentPage.GetPageData();
                currentPageData.InventoryId = i;
                Button pageButton = _initializePageButton(currentPageData);
                pageButton.transform.SetParent(outOfDeckContent, false);
            }
        }
    }

    public void RegisterPlayerMover(StorybookPlayerMover playerMover)
    {
        
    }

    public void FinishedClicked()
    {
        Destroy(this.gameObject);
    }
}
