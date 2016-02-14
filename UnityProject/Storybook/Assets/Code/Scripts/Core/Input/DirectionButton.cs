using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DirectionButton : MonoBehaviour {

    [SerializeField]
    private Door.Direction m_buttonDirection;

    public void OnClick()
    {
        UIHandler currentUIHandler = FindObjectOfType<UIHandler>();
        currentUIHandler.PlayClickSound();
        if (currentUIHandler is OverworldUIHandler)
        {
            OverworldUIHandler overworldUIHandler = (OverworldUIHandler)currentUIHandler;
            overworldUIHandler.DirectionButtonPressed(m_buttonDirection);
        }
    }
}
