using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class DeckManagementUIHandler : PageUIHandler
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

    /// <summary>
    /// When a page button is pressed, check to see if it is in the deck or outside of the deck
    /// If the page is in the deck, save it as the selected deck page
    /// If it is outside of the deck, save it as the out of deck page
    /// </summary>
    /// <param name="buttonPressed"></param>
    public override void PageButtonPressed(PageButton buttonPressed)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        int deckSize = gameManager.DeckSize;
        PageData pageData = buttonPressed.PageData;

        if (pageData.InventoryId < deckSize)
        {
            if (m_selectedDeckPage != null)
            {
                m_selectedDeckPage.DisplaySelectedImage(false);
            }

            if (m_selectedDeckPage == buttonPressed)
            {
                m_selectedDeckPage.DisplaySelectedImage(false);
                m_selectedDeckPage = null;
            }
            else
            {
                m_selectedDeckPage = buttonPressed;
                m_selectedDeckPage.DisplaySelectedImage(true);
            }          
        }
        else
        {
            if (m_selectedInventoryPage != null)
            {
                m_selectedInventoryPage.DisplaySelectedImage(false);
            }

            if (m_selectedInventoryPage == buttonPressed)
            {
                m_selectedInventoryPage.DisplaySelectedImage(false);
                m_selectedInventoryPage = null;
            }
            else
            {
                m_selectedInventoryPage = buttonPressed;
                m_selectedInventoryPage.DisplaySelectedImage(true);
            }
        }
        _checkForSwap();
    }

    /// <summary>
    /// Checks to see if both a deck page and an out of deck page has been selected
    /// If both have been selected, swap their position's in the player's inventory and swap them in the menu
    /// </summary>
    private void _checkForSwap()
    {
        if (m_selectedDeckPage != null && m_selectedInventoryPage != null)
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            PlayerInventory localInventory = gameManager.GetLocalPlayerInventory();
            localInventory.Move(m_selectedDeckPage.PageData.InventoryId, m_selectedInventoryPage.PageData.InventoryId);
            _swapPagesInMenu();
            m_selectedInventoryPage = null;
            m_selectedDeckPage = null;
        }
    }

    /// <summary>
    /// Swaps the two selected pages' positions in the menu
    /// </summary>
    private void _swapPagesInMenu()
    {
        int previousDeckPageId = m_selectedDeckPage.InventoryId;
        int previousInventoryPageId = m_selectedInventoryPage.InventoryId;
        m_selectedDeckPage.transform.SetParent(m_inventoryScrollRect.content, false);
        m_selectedInventoryPage.transform.SetParent(m_deckScrollRect.content, false);
        m_selectedDeckPage.InventoryId = previousInventoryPageId;
        m_selectedInventoryPage.InventoryId = previousDeckPageId;
        _removeAllPageButtons();
        PopulateMenu();
    }

    /// <summary>
    /// Removes all the page buttons in both of the scroll rects
    /// Used to refresh the menus
    /// </summary>
    private void _removeAllPageButtons()
    {
        PageButton[] inventoryPages = m_inventoryScrollRect.content.GetComponentsInChildren<PageButton>();
        foreach(PageButton pb in inventoryPages)
        {
            Destroy(pb.gameObject);
        }

        PageButton[] deckPages = m_deckScrollRect.content.GetComponentsInChildren<PageButton>();
        foreach(PageButton pb in deckPages)
        {
            Destroy(pb.gameObject);
        }
    }

    /// <summary>
    /// Populates the menu with page buttons representing the pages in the player's inventory
    /// Puts all the pages that are in the player's deck in the left scroll rect and the rest of the pages in the right scroll rect
    /// </summary>
    public void PopulateMenu()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        PlayerInventory pi = gameManager.GetLocalPlayerInventory();

        ScrollRect[] allScrollRects = GetComponentsInChildren<ScrollRect>();

        RectTransform deckContent = m_deckScrollRect.content;
        RectTransform outOfDeckContent = m_inventoryScrollRect.content;

        // Check all the positions in the inventory that are a part of the player's deck
        for (int i = 0; i < gameManager.DeckSize; i++)
        {
            Inventory.Slot currentSlot = pi[i];

            // If the slot is not empty, generate a page button and place it in the deck side of the UI
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

        // Check all of the positions in the inventory that are not a part of the player's deck
        for (int i = gameManager.DeckSize; i < pi.DynamicSize; i++)
        {
            Inventory.Slot currentSlot = pi[i];

            // If the slot is not empty, generate a page button and place it in the inventory side of the UI
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

    /// <summary>
    /// Called when the finish button is clicked, this function destroys the deck management UI
    /// </summary>
    public void FinishedClicked()
    {
        EventDispatcher.GetDispatcher<UIEventDispatcher>().OnDeckManagementClosed();
        Debug.Log("Destroying deck management menu");
        Destroy(gameObject);
    }
}
