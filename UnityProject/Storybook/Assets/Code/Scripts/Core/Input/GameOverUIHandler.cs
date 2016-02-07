using UnityEngine;
using System.Collections;
using System;

public class GameOverUIHandler : UIHandler
{

    public override void PageButtonPressed(PageButton pageButton)
    {

    }

    public void StartNewGame()
    {
        GameObject faderObject = PhotonNetwork.Instantiate("UIPrefabs/Fader", Vector3.zero, Quaternion.identity, 0);
        PhotonNetwork.Spawn(faderObject.GetPhotonView());
        SceneFading fader = faderObject.GetComponent<SceneFading>();
        fader.LoadScene("WorldMovementTest");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
