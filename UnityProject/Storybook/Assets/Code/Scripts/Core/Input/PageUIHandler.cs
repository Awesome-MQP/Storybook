using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class PageUIHandler : UIHandler {

    [SerializeField]
    protected Button m_greenPageButton;

    [SerializeField]
    protected Button m_redPageButton;

    [SerializeField]
    protected Button m_yellowPageButton;

    [SerializeField]
    protected Button m_bluePageButton;

    /// <summary>
    /// Function that is called by a page button that is in the current UI
    /// </summary>
    /// <param name="pageButton">The page button that was pressed</param>
    public abstract void PageButtonPressed(PageButton pageButton);

    /// <summary>
    /// Generates a page button based on the given page data
    /// </summary>
    /// <param name="pageData">The data to use for the button</param>
    /// <returns>The page button that was generated</returns>
    protected Button _initializePageButton(PageData pageData)
    {
        Button prefabToUse = null;
        switch (pageData.PageGenre)
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
        pageButton.PageData = pageData;
        return button;
    }
}
