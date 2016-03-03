using UnityEngine;
using System.Collections;

public class CharacterSelectStartup : MonoBehaviour {

    [SerializeField]
    private ResourceAsset m_characterSelectPrefab;

	// Use this for initialization
	void Awake () {
        // Spawn the UI if we are the host
        if (PhotonNetwork.isMasterClient)
        {
            CharacterSelectUIHandler ui = PhotonNetwork.Instantiate<CharacterSelectUIHandler>(m_characterSelectPrefab, Vector3.zero, Quaternion.identity, 1);
            ui.FantasyModel = GameObject.Find("FantasyCharacter").GetComponent<Animator>();
            ui.HorrorModel = GameObject.Find("HorrorCharacter").GetComponent<Animator>();
            ui.SciFiModel = GameObject.Find("SciFiCharacter").GetComponent<Animator>();
            ui.ComicModel = GameObject.Find("ComicCharacter").GetComponent<Animator>();
            PhotonNetwork.Spawn(ui.photonView);
        }
	}
}
