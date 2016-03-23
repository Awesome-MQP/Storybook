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
    Text m_tutorialText;

    List<string> m_tutorialStrings = new List<string>();
    int m_stringIndex = 0;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void PopulateMenu(List<string> tutorialStrings)
    {
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

        // If the last string is already displayed, destroy the menu
        if (m_stringIndex == m_tutorialStrings.Count)
        {
            Destroy(gameObject);
        }
        // Otherwise display the next string in the list
        else
        {
            m_tutorialText.text = m_tutorialStrings[m_stringIndex];

            m_previousButton.interactable = true;
            m_nextButton.interactable = (m_tutorialStrings.Count > m_stringIndex);

            // If we are at the last string in the menu, change the next button text
            if (m_stringIndex == (m_tutorialStrings.Count - 1))
            {
                m_nextButton.GetComponentInChildren<Text>().text = "All Set!";
            }
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
}
