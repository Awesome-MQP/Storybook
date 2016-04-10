using UnityEngine;
using System.Collections;

public class MainMenuPhoton : Photon.PunBehaviour {

    [SerializeField]
    private MainMenuUIHandler m_mainMenu;

    /// <summary>
    /// When the player joins a room, check to see if they are the first player in the room
    /// </summary>
    public override void OnJoinedRoom()
    {
        if (!m_mainMenu.IsTutorial)
        {
            SceneFading fader = SceneFading.Instance();
            fader.LoadScene("CharacterSelect");
        }
        else
        {
            SceneFading fader = SceneFading.Instance();
            fader.LoadScene("TutorialCharacterSelect");
        }
    }

}
