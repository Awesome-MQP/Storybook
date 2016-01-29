using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUIHandler : UIHandler {

    [SerializeField]
    private ScrollRect m_playerInventoryPagesRect;

    [SerializeField]
    private ScrollRect m_shopPagesRect;

    [SerializeField]
    private ScrollRect m_playerTradePagesRect;
    private int m_selectedPlayerPages = 0;
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
        if (m_selectedPlayerPages < m_maxPlayerPages)
        {
            buttonPressed.transform.SetParent(m_playerTradePagesRect.content, false);
            buttonPressed.MenuId = m_selectedPlayerPageId;
            m_selectedPages.Add(buttonPressed);
            m_selectedPlayerPages += 1;
        }
    }

    private void _handleSelectedPlayerPagePressed(PageButton buttonPressed)
    {
        buttonPressed.transform.SetParent(m_playerInventoryPagesRect.content, false);
        buttonPressed.MenuId = m_selectedPlayerPageId;
        m_selectedPages.Remove(buttonPressed);
        m_selectedPlayerPages -= 1;
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

    public void PopulateMenu()
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

        // Populate the shop pages scroll rect, number of pages is determined by the level of the room
        DungeonMaster dm = FindObjectOfType<DungeonMaster>();
        for (int i = 0; i < m_shopRoom.RoomPageData.PageLevel + 1; i++)
        {
            PageData shopPageData = dm.GetShopPage(m_shopRoom.RoomPageData);
            Button button = _initializePageButton(shopPageData);
            button.transform.SetParent(m_shopPagesRect.content, false);
            button.GetComponent<PageButton>().MenuId = m_shopInventoryMenuId;
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
        m_shopRoom.OnShopClosed();
        Destroy(gameObject);
    }

    public void SubmitTrade()
    {
        // Only allow the trade if the total level of all the player pages is equal to or greater than the selected shop page
        if (_getTotalSelectedLevel() >= m_selectedShopPageButton.PageLevel)
        {
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

            Page newPlayerPage = dm.SpawnPageWithDataOnNetwork(m_selectedShopPageButton.PageData);
            pi.Add(newPlayerPage, pi.FirstOpenSlot());
            ExitMenu();
        }
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
