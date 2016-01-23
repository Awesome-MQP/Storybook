using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PageForRoomUIHandler : UIHandler {

    private GameObject m_canvas;

    [SerializeField]
    private Button m_greenPageButton;

    [SerializeField]
    private Button m_redPageButton;

    [SerializeField]
    private Button m_yellowPageButton;

    [SerializeField]
    private Button m_bluePageButton;

    [SerializeField]
    private float m_gridXPadding;

    [SerializeField]
    private float m_gridYPadding;

    private float m_buttonHeight;
    private float m_buttonWidth;

    private float m_contentWidth;
    private float m_contentHeight;

    private Button m_selectedPageButton;

    public void Awake()
    {
        Object loadedObject = Resources.Load("UIPrefabs/ChoosePageForRoomCanvas");
        m_canvas = (GameObject) Instantiate(loadedObject);

        // Get the dimensions of the button
        m_buttonHeight = m_greenPageButton.GetComponent<RectTransform>().rect.height;
        m_buttonWidth = m_greenPageButton.GetComponent<RectTransform>().rect.width;

        // Get the dimensions of the scrollview content
        ScrollRect scrollView = m_canvas.GetComponentInChildren<ScrollRect>();
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

        ScrollRect scrollView = m_canvas.GetComponentInChildren<ScrollRect>();
        RectTransform scrollContent = scrollView.content;

        for(int i = 0; i < pi.DynamicSize; i++)
        {
            Inventory.Slot currentSlot = pi[i];
            if (!currentSlot.IsEmpty)
            {
                Item currentItem = currentSlot.SlotItem;
                Page currentPage = (Page)currentItem;
                Button pageButton = _initializePageButton(currentPage.PageLevel, currentPage.PageGenre);
                pageButton.transform.SetParent(scrollContent, false);
            }
        }
    }
	
    public override void PageButtonPressed(int pageLevel, Genre pageGenre)
    {
        Debug.Log("Page button pressed");

        Button selectedButton = _initializePageButton(pageLevel, pageGenre);
        selectedButton.enabled = false;
        RectTransform[] AllRects = m_canvas.GetComponentsInChildren<RectTransform>();
        RectTransform selectedPageRect = null;

        foreach(RectTransform rectT in AllRects)
        {
            if (rectT.name == "SelectedPage")
            {
                selectedPageRect = rectT;
                break;
            }
        }

        if (m_selectedPageButton != null)
        {
            Debug.Log("Destroying previous page");
            Destroy(selectedPageRect.GetChild(0).gameObject);
        }

        GridLayoutGroup grid = selectedPageRect.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(m_buttonWidth, m_buttonHeight);

        selectedButton.transform.SetParent(selectedPageRect.transform, false);
        m_selectedPageButton = selectedButton;
    }

    private Button _initializePageButton(int pageLevel, Genre pageGenre)
    {
        Button prefabToUse = null;
        switch (pageGenre)
        {
            case Genre.Fantasy:
                prefabToUse = m_greenPageButton;
                break;
            case Genre.GraphicNovel:
                prefabToUse = m_yellowPageButton;
                break;
            case Genre.Horror:
                prefabToUse = m_redPageButton;
                break;
            case Genre.SciFi:
                prefabToUse = m_bluePageButton;
                break;
        }
        Button button = Instantiate(prefabToUse);
        PageButton pageButton = button.GetComponent<PageButton>();
        pageButton.PageLevel = pageLevel;
        pageButton.PageGenre = pageGenre;
        return button;
    }
}
