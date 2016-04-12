using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

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

    protected List<PageButton> _SortByGenre(List<PageButton> listToSort)
    {
        List<PageButton> sortedList = new List<PageButton>();

        sortedList.AddRange(_GetPagesOfGenre(listToSort, Genre.Fantasy));
        sortedList.AddRange(_GetPagesOfGenre(listToSort, Genre.GraphicNovel));
        sortedList.AddRange(_GetPagesOfGenre(listToSort, Genre.Horror));
        sortedList.AddRange(_GetPagesOfGenre(listToSort, Genre.SciFi));

        return sortedList;
    }

    protected List<PageButton> _GetPagesOfGenre(List<PageButton> buttonList, Genre genreToGet)
    {
        List<PageButton> buttonsOfGenre = new List<PageButton>();
        foreach(PageButton pb in buttonList)
        {
            if (pb.PageGenre == genreToGet)
            {
                buttonsOfGenre.Add(pb);
            }
        }

        return buttonsOfGenre;
    }
}
