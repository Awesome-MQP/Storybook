using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PageButton : MonoBehaviour {

    private PageData m_pageData;

    /// <summary>
    /// Called when the page button is pressed
    /// Calls the PageButtonPressed function on the current UIHandler in the scene
    /// </summary>
    public void OnClick()
    {
        UIHandler currentUIHandler = FindObjectOfType<UIHandler>();
        currentUIHandler.PageButtonPressed(this);
    }

    /// <summary>
    /// The level of the page that is tied to this button
    /// </summary>
    public int PageLevel
    {
        get { return m_pageData.PageLevel; }
        set
        {
            m_pageData.PageLevel = value;
            _updateButtonText();
        }
    }

    /// <summary>
    /// The inventory index of the page that is tied to this button
    /// </summary>
    public int InventoryId
    {
        get { return m_pageData.InventoryId; }
        set
        {
            m_pageData.InventoryId = value;
        }
    }
    
    /// <summary>
    /// The genre of the page that is tied to this button
    /// </summary>
    public Genre PageGenre
    {
        get { return m_pageData.PageGenre; }
        set
        {
            m_pageData.PageGenre = value;
            _updateButtonText();
        }
    }

    /// <summary>
    /// The page data that is tied to this button
    /// </summary>
    public PageData PageData
    {
        get { return m_pageData; }
        set
        {
            m_pageData = value;
            _updateButtonText();
        }
    }

    /// <summary>
    /// Called when changes are maded to the page data of the button
    /// Updates the text fields of the page based on the changes
    /// </summary>
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
