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

        // Send out a tutorial event
        EventDispatcher.GetDispatcher<TutorialEventDispatcher>().OnDeckManagementOpened();
    }

    public override void PageButtonPressed(PageButton buttonPressed)
    {
        BaseStorybookGame gameManager = GameManager.GetInstance<BaseStorybookGame>();
        PageData pageData = buttonPressed.PageData;

        if (pageData.InventoryId < gameManager.DeckSize)
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

    private void _checkForSwap()
    {
        if (m_selectedDeckPage != null && m_selectedInventoryPage != null)
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            PlayerInventory localInventory = gameManager.GetLocalPlayer<PlayerEntity>().OurInventory;
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

    public void PopulateMenu()
    {
        StartCoroutine(_waitForInventory());
    }


    /// <summary>
    /// Called when the finish button is clicked, this function destroys the deck management UI
    /// </summary>
    public void FinishedClicked()
    {
        PlayClickSound();
        EventDispatcher.GetDispatcher<DeckManagementEventDispatcher>().OnDeckManagementClosed();
        Debug.Log("Destroying deck management menu");
        Destroy(gameObject);
    }

    private IEnumerator _waitForInventory()
    {
        BaseStorybookGame gameManager = GameManager.GetInstance<BaseStorybookGame>();
        PlayerEntity localPlayer = gameManager.GetLocalPlayer<PlayerEntity>();

        while (!localPlayer)
        {
            yield return null;
            localPlayer = gameManager.GetLocalPlayer<PlayerEntity>();
        }

        PlayerInventory pi = localPlayer.OurInventory;

        ScrollRect[] allScrollRects = GetComponentsInChildren<ScrollRect>();

        RectTransform deckContent = m_deckScrollRect.content;
        RectTransform outOfDeckContent = m_inventoryScrollRect.content;

        //TODO: Store the deck size in inventory
        for (int i = 0; i < gameManager.DeckSize; i++)
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

        for (int i = gameManager.DeckSize; i < pi.DynamicSize; i++)
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
}
