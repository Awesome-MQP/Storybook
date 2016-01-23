using UnityEngine;
using System.Collections;

public class PageButton : MonoBehaviour {

    [SerializeField]
    private int m_pageLevel;

    [SerializeField]
    private Genre m_pageGenre;

    public void OnClick()
    {
        UIHandler currentUIHandler = FindObjectOfType<UIHandler>();
        currentUIHandler.PageButtonPressed(m_pageLevel, m_pageGenre);
    }

    public int PageLevel
    {
        get { return m_pageLevel; }
        set { m_pageLevel = value; }
    }

    public Genre PageGenre
    {
        get { return m_pageGenre; }
        set { m_pageGenre = value; }
    }
}
