using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PageForRoomUIHandler : PageUIHandler {

    [SerializeField]
    private float m_gridXPadding;

    [SerializeField]
    private float m_gridYPadding;

    private float m_buttonHeight;
    private float m_buttonWidth;

    private float m_contentWidth;
    private float m_contentHeight;

    private PageButton m_selectedPageButton;
    private PageButton m_pageButtonInScroll = null;
    private Button m_selectedButton;
    private Button m_submitPageButton;

    public void Awake()
    {
        m_submitPageButton = GetComponentInChildren<Button>();
        m_submitPageButton.enabled = false;

        // Get the dimensions of the button
        m_buttonHeight = m_greenPageButton.GetComponent<RectTransform>().rect.height;
        m_buttonWidth = m_greenPageButton.GetComponent<RectTransform>().rect.width;

        // Get the dimensions of the scrollview content
        ScrollRect scrollView = GetComponentInChildren<ScrollRect>();
        RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
        RectTransform scrollContent = scrollView.content;
        m_contentWidth = scrollRect.rect.xMax - scrollRect.rect.xMin;
        m_contentHeight = scrollContent.rect.yMax - scrollContent.rect.yMin;

        // Initialize the grid cell size and spacing
        GridLayoutGroup gridGroup = scrollView.GetComponentInChildren<GridLayoutGroup>();
        gridGroup.cellSize = new Vector2(m_buttonWidth, m_buttonHeight);
        gridGroup.spacing = new Vector2(m_gridXPadding, m_gridYPadding);

        // Send out a tutorial event
        EventDispatcher.GetDispatcher<TutorialEventDispatcher>().OnPageForRoomUIOpened();
    }

    /// <summary>
    /// Fills the menu with all of the pages currently in the player's inventory
    /// Creates the pages in the UI as page buttons so that they can be clicked
    /// </summary>
    public void PopulateMenu()
    {
        PlayerInventory pi = GameManager.GetInstance<GameManager>().GetLocalPlayer<PlayerEntity>().OurInventory;//gameManager.GetLocalPlayerInventory();

        ScrollRect scrollView = GetComponentInChildren<ScrollRect>();
        RectTransform scrollContent = scrollView.content;

        for(int i = 0; i < pi.DynamicSize; i++)
        {
            Inventory.Slot currentSlot = pi[i];
            if (!currentSlot.IsEmpty)
            {
                Item currentItem = currentSlot.SlotItem;
                Page currentPage = (Page)currentItem;
                PageData currentPageData = currentPage.GetPageData();
                currentPageData.InventoryId = i;
                Button pageButton = _initializePageButton(currentPageData);
                pageButton.transform.SetParent(scrollContent, false);
            }
        }
    }
	
    /// <summary>
    /// When a page button is pressed in this menu, that page becomes the current selected page and is displayed
    /// underneath the 'selected page' label
    /// </summary>
    /// <param name="buttonPressed">The page button that was pressed</param>
    public override void PageButtonPressed(PageButton buttonPressed)
    {
        if (m_pageButtonInScroll != null)
        {
            m_pageButtonInScroll.DisplaySelectedImage(false);
        }
        m_pageButtonInScroll = buttonPressed;
        buttonPressed.DisplaySelectedImage(true);
        Button selectedButton = _initializePageButton(buttonPressed.PageData);
        selectedButton.enabled = false;
        RectTransform[] AllRects = GetComponentsInChildren<RectTransform>();
        RectTransform selectedPageRect = null;

        // Find the selected page rect
        foreach(RectTransform rectT in AllRects)
        {
            if (rectT.name == "SelectedPage")
            {
                selectedPageRect = rectT;
                break;
            }
        }

        // If there was a previously selected page, destroy it in the UI
        if (m_selectedButton != null)
        {
            Destroy(selectedPageRect.GetChild(0).gameObject);
        }

        GridLayoutGroup grid = selectedPageRect.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(m_buttonWidth, m_buttonHeight);

        selectedButton.transform.SetParent(selectedPageRect.transform, false);
        m_selectedButton = selectedButton;
        m_selectedPageButton = selectedButton.GetComponent<PageButton>();
        m_submitPageButton.enabled = true;
    }

    /// <summary>
    /// Called when the submit button is pressed in the UI
    /// Submits the selected page as the one to use for the next room
    /// </summary>
    public void SubmitPage()
    {
        PlayClickSound();
        //_dropAndReplaceSelectedPage();
        Destroy(gameObject);
        EventDispatcher.GetDispatcher<PageForRoomEventDispatcher>().SubmitPageForRoom(m_selectedPageButton.PageData);
    }

    /// <summary>
    /// Removes the selected page from the player's inventory and replaces it with a default page
    /// </summary>
    private void _dropAndReplaceSelectedPage()
    {
        DungeonMaster dm = DungeonMaster.Instance;
        PlayerInventory currentPlayerInventory =
            GameManager.GetInstance<GameManager>().GetLocalPlayer<PlayerEntity>().OurInventory;
        currentPlayerInventory.Drop(m_selectedPageButton.PageData.InventoryId);
        currentPlayerInventory.Add(dm.GetBasicPage(), m_selectedPageButton.PageData.InventoryId);
    }
}
