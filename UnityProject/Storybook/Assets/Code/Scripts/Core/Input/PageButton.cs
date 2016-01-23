using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PageButton : MonoBehaviour {

    private PageData m_pageData;

    public void OnClick()
    {
        UIHandler currentUIHandler = FindObjectOfType<UIHandler>();
        currentUIHandler.PageButtonPressed(m_pageData);
    }

    public int PageLevel
    {
        get { return m_pageData.PageLevel; }
        set
        {
            m_pageData.PageLevel = value;
            _updateButtonText();
        }
    }

    public Genre PageGenre
    {
        get { return m_pageData.PageGenre; }
        set
        {
            m_pageData.PageGenre = value;
            _updateButtonText();
        }
    }

    public PageData PageData
    {
        get { return m_pageData; }
        set
        {
            m_pageData = value;
            _updateButtonText();
        }
    }

    private void _updateButtonText()
    {
        Text[] allButtonTexts = GetComponentsInChildren<Text>();
        foreach(Text t in allButtonTexts)
        {
            if (t.name == "Level")
            {
                t.text = "Level " + PageLevel.ToString();
            }
        }
    }
}
