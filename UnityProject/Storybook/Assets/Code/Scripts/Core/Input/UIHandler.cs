using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class UIHandler : MonoBehaviour {

    [SerializeField]
    protected Button m_greenPageButton;

    [SerializeField]
    protected Button m_redPageButton;

    [SerializeField]
    protected Button m_yellowPageButton;

    [SerializeField]
    protected Button m_bluePageButton;

    public abstract void PageButtonPressed(PageButton pageButton);

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
