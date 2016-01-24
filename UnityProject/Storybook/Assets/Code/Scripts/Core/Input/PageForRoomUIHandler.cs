using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PageForRoomUIHandler : UIHandler {

    [SerializeField]
    private float m_gridXPadding;

    [SerializeField]
    private float m_gridYPadding;

    private float m_buttonHeight;
    private float m_buttonWidth;

    private float m_contentWidth;
    private float m_contentHeight;

    private PageButton m_selectedPageButton;
    private Button m_selectedButton;
    private Button m_submitPageButton;

    private StorybookPlayerMover m_playerMover;

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
    }

    public void PopulateMenu()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        PlayerInventory pi = gameManager.GetLocalPlayerInventory();

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
	
    public override void PageButtonPressed(PageButton buttonPressed)
    {
        Debug.Log("Page button pressed");

        Button selectedButton = _initializePageButton(buttonPressed.PageData);
        selectedButton.enabled = false;
        RectTransform[] AllRects = GetComponentsInChildren<RectTransform>();
        RectTransform selectedPageRect = null;

        foreach(RectTransform rectT in AllRects)
        {
            if (rectT.name == "SelectedPage")
            {
                selectedPageRect = rectT;
                break;
            }
        }

        if (m_selectedButton != null)
        {
            Debug.Log("Destroying previous page");
            Destroy(selectedPageRect.GetChild(0).gameObject);
        }

        GridLayoutGroup grid = selectedPageRect.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(m_buttonWidth, m_buttonHeight);

        selectedButton.transform.SetParent(selectedPageRect.transform, false);
        m_selectedButton = selectedButton;
        m_selectedPageButton = selectedButton.GetComponent<PageButton>();
        m_submitPageButton.enabled = true;
    }

    public void SubmitPage()
    {
        Debug.Log("Page submitted");
        //_dropAndReplaceSelectedPage();
        m_playerMover.SubmitPageForRoom(m_selectedPageButton.PageData);
    }

    public void RegisterPlayerMover(StorybookPlayerMover playerMover)
    {
        m_playerMover = playerMover;
    }

    private void _dropAndReplaceSelectedPage()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        DungeonMaster dm = FindObjectOfType<DungeonMaster>();
        PlayerInventory currentPlayerInventory = gameManager.GetLocalPlayerInventory();
        currentPlayerInventory.Drop(m_selectedPageButton.PageData.InventoryId);
        currentPlayerInventory.Add(dm.GetBasicPage(), m_selectedPageButton.PageData.InventoryId);
    }
}
