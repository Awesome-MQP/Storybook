using UnityEngine;
using System.Collections;

public class CharacterSelectStartup : GameManager
{
    [SerializeField]
    private ResourceAsset m_characterSelectPrefab = new ResourceAsset(typeof(CharacterSelectUIHandler));

    [SerializeField]
    private bool m_isTutorial = false;

	// Use this for initialization
    protected override void Awake ()
    {
        base.Awake();

        // Spawn the UI if we are the host
        if (IsMine)
        {
            CharacterSelectUIHandler ui = PhotonNetwork.Instantiate<CharacterSelectUIHandler>(m_characterSelectPrefab, Vector3.zero, Quaternion.identity, 1);
            ui.IsTutorial = m_isTutorial;
            ui.FantasyModel = GameObject.Find("FantasyCharacter").GetComponent<Animator>();
            ui.HorrorModel = GameObject.Find("HorrorCharacter").GetComponent<Animator>();
            ui.SciFiModel = GameObject.Find("SciFiCharacter").GetComponent<Animator>();
            ui.ComicModel = GameObject.Find("ComicCharacter").GetComponent<Animator>();
            PhotonNetwork.Spawn(ui.photonView);
        }
	}
}
