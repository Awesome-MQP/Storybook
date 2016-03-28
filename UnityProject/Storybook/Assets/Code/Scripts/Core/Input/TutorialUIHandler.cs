using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialUIHandler : UIHandler {

    [SerializeField]
    Button m_nextButton;

    [SerializeField]
    Button m_previousButton;

    [SerializeField]
    Button m_finishButton;

    [SerializeField]
    Text m_tutorialText;

    [SerializeField]
    Text m_titleText;

    List<string> m_tutorialStrings = new List<string>();
    int m_stringIndex = 0;

    /// <summary>
    /// Sets the title in the tutorial menu and displays the first string in the given list of tutorial strings
    /// </summary>
    /// <param name="tutorialTitle">The title of the current tutorial</param>
    /// <param name="tutorialStrings">The list of tutorial messages</param>
    public void PopulateMenu(string tutorialTitle, List<string> tutorialStrings)
    {
        m_titleText.text = tutorialTitle;

        m_tutorialStrings = tutorialStrings;
        m_tutorialText.text = tutorialStrings[m_stringIndex];

        m_previousButton.interactable = false;
        m_nextButton.interactable = (m_tutorialStrings.Count > 1);
    }

    /// <summary>
    /// Display the next string in the current tutorial text
    /// </summary>
    public void NextButtonPressed()
    {
        m_stringIndex++;

        m_tutorialText.text = m_tutorialStrings[m_stringIndex];

        m_previousButton.interactable = true;

        // If we are at the last string in the menu, change the next button text
        if (m_stringIndex == (m_tutorialStrings.Count - 1))
        {
            m_nextButton.interactable = false;
        }
    }

    /// <summary>
    /// Display the previous string in the current tutorial text
    /// </summary>
    public void PreviousButtonPressed()
    {
        m_stringIndex--;
        m_tutorialText.text = m_tutorialStrings[m_stringIndex];

        m_previousButton.interactable = (m_stringIndex > 0);
        m_nextButton.interactable = true;
        m_nextButton.GetComponentInChildren<Text>().text = "Next";
    }

    public void FinishButtonPressed()
    {
        Destroy(gameObject);
    }

    public void changeFinishButtonOnClick()
    {
        Debug.Log("Changing on click listener");
        m_finishButton.onClick.RemoveAllListeners();
        m_finishButton.onClick.AddListener(delegate () { returnToMainMenu(); });
    }

    protected void returnToMainMenu()
    {
        Debug.Log("Returing to main menu");
        PhotonNetwork.LeaveRoom();
        SceneFading fader = SceneFading.Instance();
        fader.LoadScene("MainMenu");
    }
}
