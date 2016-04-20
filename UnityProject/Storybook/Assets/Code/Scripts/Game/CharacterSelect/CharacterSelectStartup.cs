using UnityEngine;
using System.Collections;

public class CharacterSelectStartup : GameManager
{
    [SerializeField]
    private ResourceAsset m_characterSelectPrefab = new ResourceAsset(typeof(CharacterSelectUIHandler));

    [SerializeField]
    private bool m_isTutorial = false;

    CharacterSelectUIHandler m_characterSelectUI;

	// Use this for initialization
    protected override void Awake ()
    {
        base.Awake();

        // Spawn the UI if we are the host
        if (IsMine)
        {
            m_characterSelectUI = PhotonNetwork.Instantiate<CharacterSelectUIHandler>(m_characterSelectPrefab, Vector3.zero, Quaternion.identity, 1);
            m_characterSelectUI.IsTutorial = m_isTutorial;

            PhotonNetwork.Spawn(m_characterSelectUI.photonView);

            photonView.RPC(nameof(InitializeAnimators), PhotonTargets.All, m_characterSelectUI.photonView);
        }
	}

    [PunRPC]
    protected void InitializeAnimators(PhotonView photonView)
    {
        CharacterSelectUIHandler ui = photonView.GetComponent<CharacterSelectUIHandler>();
        ui.FantasyModel = GameObject.Find("FantasyCharacter").GetComponent<Animator>();
        ui.HorrorModel = GameObject.Find("HorrorCharacter").GetComponent<Animator>();
        ui.SciFiModel = GameObject.Find("SciFiCharacter").GetComponent<Animator>();
        ui.ComicModel = GameObject.Find("ComicCharacter").GetComponent<Animator>();
    }
}
