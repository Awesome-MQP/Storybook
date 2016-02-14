using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUIHandler : PageUIHandler {

    [SerializeField]
    private ScrollRect m_playerInventoryPagesRect;

    [SerializeField]
    private ScrollRect m_shopPagesRect;

    [SerializeField]
    private ScrollRect m_playerTradePagesRect;
    private int m_maxPlayerPages;

    [SerializeField]
    private RectTransform m_selectedShopPageRect;
    private PageButton m_selectedShopPageButton;
    private bool m_shopPageSelected = false;

    private ShopRoom m_shopRoom;

    private const int m_playerInventoryMenuId = 0;
    private const int m_shopInventoryMenuId = 1;
    private const int m_selectedShopPageId = 2;
    private const int m_selectedPlayerPageId = 3;

    private List<PageButton> m_selectedPages = new List<PageButton>();

    private List<PageData> m_shopPages = new List<PageData>();

    // Use this for initialization
    void Awake ()
    {
        float pageButtonWidth = m_greenPageButton.GetComponent<RectTransform>().rect.width;
        float pageButtonHeight = m_greenPageButton.GetComponent<RectTransform>().rect.height;

        ScrollRect[] allScrollRects = new ScrollRect[] { m_playerInventoryPagesRect, m_shopPagesRect, m_playerTradePagesRect};
        foreach(ScrollRect rect in allScrollRects)
        {
            RectTransform content = rect.content;
            GridLayoutGroup gridGroup = content.GetComponent<GridLayoutGroup>();
            gridGroup.cellSize = new Vector2(pageButtonWidth, pageButtonHeight);
        }

        m_selectedShopPageRect.GetComponent<GridLayoutGroup>().cellSize = new Vector2(pageButtonWidth, pageButtonHeight);
	}

    public override void PageButtonPressed(PageButton pageButton)
    {
        switch (pageButton.MenuId)
        {
            case m_playerInventoryMenuId:
                _handlePlayerPagePressed(pageButton);
                break;
            case m_shopInventoryMenuId:
                _handleShopPagePressed(pageButton);
                break;
            case m_selectedPlayerPageId:
                _handleSelectedPlayerPagePressed(pageButton);
                break;
            case m_selectedShopPageId:
                _handleSelectedShopPagePressed(pageButton);
                break;
        }
    }

    private void _handlePlayerPagePressed(PageButton buttonPressed)
    {
        if (m_selectedPages.Count < m_maxPlayerPages)
        {
            buttonPressed.transform.SetParent(m_playerTradePagesRect.content, false);
            buttonPressed.MenuId = m_selectedPlayerPageId;
            m_selectedPages.Add(buttonPressed);
        }
    }

    private void _handleSelectedPlayerPagePressed(PageButton buttonPressed)
    {
        buttonPressed.transform.SetParent(m_playerInventoryPagesRect.content, false);
        buttonPressed.MenuId = m_selectedPlayerPageId;
        m_selectedPages.Remove(buttonPressed);
    }

    private void _handleShopPagePressed(PageButton buttonPressed)
    {
        if (!m_shopPageSelected)
        {
            buttonPressed.transform.SetParent(m_selectedShopPageRect, false);
            buttonPressed.MenuId = m_selectedShopPageId;
            m_shopPageSelected = true;
            m_selectedShopPageButton = buttonPressed;
        }
    }

    private void _handleSelectedShopPagePressed(PageButton buttonPressed)
    {
        buttonPressed.transform.SetParent(m_shopPagesRect.content, false);
        buttonPressed.MenuId = m_shopInventoryMenuId;
        m_shopPageSelected = false;
        m_selectedShopPageButton = null;
    }

    public void PopulateMenu(List<PageData> shopPages)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        PlayerInventory pi = gameManager.GetLocalPlayerInventory();

        // Populate the player inventory scroll rect
        for (int i = 0; i < pi.DynamicSize; i++)
        {
            Inventory.Slot currentSlot = pi[i];
            if (!currentSlot.IsEmpty)
            {
                Item currentItem = currentSlot.SlotItem;
                Page currentPage = (Page)currentItem;
                PageData currentPageData = currentPage.GetPageData();
                currentPageData.InventoryId = i;
                Button button = _initializePageButton(currentPageData);
                button.transform.SetParent(m_playerInventoryPagesRect.content, false);
                button.GetComponent<PageButton>().MenuId = m_playerInventoryMenuId;
            }
        }

        // If pages have not been generated for this shop, generate pages using the DungeonMaster
        if (shopPages.Count <= 0)
        {
            // Populate the shop pages scroll rect, number of pages is determined by the level of the room
            DungeonMaster dm = FindObjectOfType<DungeonMaster>();
            for (int i = 0; i < m_shopRoom.RoomPageData.PageLevel + 1; i++)
            {
                PageData shopPageData = dm.GetShopPage(m_shopRoom.RoomPageData);
                Button button = _initializePageButton(shopPageData);
                button.transform.SetParent(m_shopPagesRect.content, false);
                button.GetComponent<PageButton>().MenuId = m_shopInventoryMenuId;
                m_shopPages.Add(shopPageData);
            }
            EventDispatcher.GetDispatcher<UIEventDispatcher>().PagesGenerated(m_shopPages.ToArray());
        }

        // If the pages have already been generated for the shop, use the list passed in
        else
        {
            m_shopPages = shopPages;
            foreach (PageData data in m_shopPages)
            {
                Button button = _initializePageButton(data);
                button.transform.SetParent(m_shopPagesRect.content, false);
                button.GetComponent<PageButton>().MenuId = m_shopInventoryMenuId;
            }
        }
    }

    public void RegisterShopRoom(ShopRoom shopRoom)
    {
        m_shopRoom = shopRoom;
        if (shopRoom.RoomPageData.PageLevel == 1 || shopRoom.RoomPageData.PageLevel == 2)
        {
            m_maxPlayerPages = 3;
        }
        else
        {
            m_maxPlayerPages = 3;
        }
    }

    public void ExitMenu()
    {
        PlayClickSound();
        EventDispatcher.GetDispatcher<UIEventDispatcher>().OnRoomCleared();
        Destroy(gameObject);
    }

    public void SubmitTrade()
    {
        // If no shop page has been selected, just return
        if (m_selectedShopPageButton == null)
        {
            return;
        }

        // Only allow the trade if the total level of all the player pages is equal to or greater than the selected shop page
        if (_getTotalSelectedLevel() >= m_selectedShopPageButton.PageLevel)
        {
            PlayClickSound();
            GameManager gameManager = FindObjectOfType<GameManager>();
            DungeonMaster dm = FindObjectOfType<DungeonMaster>();
            PlayerInventory pi = gameManager.GetLocalPlayerInventory();

            /*
            foreach (PageButton pageButton in m_selectedPages)
            {
                pi.Drop(pageButton.InventoryId);
                pi.Add(dm.GetBasicPage(), pageButton.InventoryId);
            }
            */

            PageData selectedPageData = m_selectedShopPageButton.PageData;
            Page newPlayerPage = dm.ConstructPageFromData(selectedPageData);
            pi.Add(newPlayerPage, pi.FirstOpenSlot());
            m_shopPages.Remove(m_selectedShopPageButton.PageData);

            // Send out a page traded event
            EventDispatcher.GetDispatcher<UIEventDispatcher>().PageTraded(m_selectedShopPageButton.PageData);
            _clearSelected();

            // Exit the menu if all the pages have been traded for
            if (m_shopPages.Count <= 0)
            {
                ExitMenu();
            }
        }
    }

    private void _clearSelected()
    {
        foreach(PageButton b in m_selectedPages)
        {
            Destroy(b.gameObject);
        }
        m_selectedPages = new List<PageButton>();
        m_selectedShopPageButton.transform.SetParent(m_playerInventoryPagesRect.content, false);
        m_shopPageSelected = false;
    }

    private int _getTotalSelectedLevel()
    {
        int totalLevel = 0;
        foreach(PageButton p in m_selectedPages)
        {
            totalLevel += p.PageLevel;
        }
        return totalLevel;
    }
}
