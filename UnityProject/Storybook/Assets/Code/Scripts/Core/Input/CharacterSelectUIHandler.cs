using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterSelectUIHandler : Photon.PunBehaviour {

    private Dictionary<PhotonPlayer, Genre> m_playerToCharacter = new Dictionary<PhotonPlayer, Genre>();
    private int m_playersReady = 0;

    [SerializeField]
    private Animator m_comicBookModel;

    [SerializeField]
    private Animator m_horrorModel;

    [SerializeField]
    private Animator m_scifiModel;

    [SerializeField]
    private Animator m_fantasyModel;

    [SerializeField]
    private PlayerEntity m_comicBookEntity;

    [SerializeField]
    private PlayerEntity m_horrorEntity;

    [SerializeField]
    private PlayerEntity m_scifiEntity;

    [SerializeField]
    private PlayerEntity m_fantasyEntity;

    [SerializeField]
    private Button m_comicBookButton;

    [SerializeField]
    private Button m_horrorButton;

    [SerializeField]
    private Button m_scifiButton;

    [SerializeField]
    private Button m_fantasyButton;

    [SerializeField]
    private Button m_submitButton;

    protected override void Awake()
    {
        photonView.RPC("InitializeDictionary", PhotonTargets.All, PhotonNetwork.player);
        m_submitButton.interactable = false;
        MainMenuUIHandler mainMenu = FindObjectOfType<MainMenuUIHandler>();
        Destroy(mainMenu);
    }

    public void SelectComicBook()
    {
        StartCoroutine(_AnimateComicBook());
        photonView.RPC("SelectCharacter", PhotonTargets.All, PhotonNetwork.player, Genre.GraphicNovel);
    }

    public void SelectHorror()
    {
        StartCoroutine(_AnimateHorror());
        photonView.RPC("SelectCharacter", PhotonTargets.All, PhotonNetwork.player, Genre.Horror);
    }

    public void SelectSciFi()
    {
        StartCoroutine(_AnimateSciFi());
        photonView.RPC("SelectCharacter", PhotonTargets.All, PhotonNetwork.player, Genre.SciFi);
    }

    public void SelectFantasy()
    {
        StartCoroutine(_AnimateFantasy());
        photonView.RPC("SelectCharacter", PhotonTargets.All, PhotonNetwork.player, Genre.Fantasy);
    }

    public void SubmitCharacter()
    {
        foreach(PhotonPlayer player in m_playerToCharacter.Keys)
        {
            Genre playerGenre = m_playerToCharacter[player];
            PlayerEntity playerCharacterEntity = _GenreToEntity(playerGenre);
            GameObject entityObject = PhotonNetwork.Instantiate("PlayerEntity/" + playerCharacterEntity.name, Vector3.zero, Quaternion.identity, 0);
            DontDestroyOnLoad(entityObject);
            PlayerEntity createdEntity = entityObject.GetComponent<PlayerEntity>();
            createdEntity.Construct(PhotonNetwork.player);
            PhotonNetwork.Spawn(entityObject.GetPhotonView());
            SceneFading.Instance().LoadScene("WorldMovementTest");
        }
    }

    private Button _GenreToButton(Genre characterGenre)
    {
        switch (characterGenre)
        {
            case Genre.Fantasy:
                return m_fantasyButton;
            case Genre.GraphicNovel:
                return m_comicBookButton;
            case Genre.Horror:
                return m_horrorButton;
            case Genre.SciFi:
                return m_scifiButton;
        }
        return null;
    }

    private PlayerEntity _GenreToEntity(Genre characterGenre)
    {
        switch (characterGenre)
        {
            case Genre.Fantasy:
                return m_fantasyEntity;
            case Genre.GraphicNovel:
                return m_comicBookEntity;
            case Genre.Horror:
                return m_horrorEntity;
            case Genre.SciFi:
                return m_scifiEntity;
        }
        return null;
    }

    [PunRPC]
    public void SelectCharacter(PhotonPlayer player, int selectedGenre)
    {
        Debug.Log("Character selected");
        Genre playerGenre = m_playerToCharacter[player];
        if (playerGenre != Genre.None)
        {
            Button genreButton = _GenreToButton(playerGenre);
            genreButton.interactable = true;
        }
        m_playerToCharacter.Remove(player);

        Button selectedButton = _GenreToButton((Genre)selectedGenre);
        selectedButton.interactable = false;
        m_playerToCharacter.Add(player, (Genre)selectedGenre);

        m_playersReady += 1;

        if (m_playersReady == PhotonNetwork.playerList.Length && IsMasterClient)
        {
            m_submitButton.interactable = true;
        }
    }

    [PunRPC]
    public void InitializeDictionary(PhotonPlayer player)
    {
        m_playerToCharacter.Add(player, Genre.None);
    }

    private IEnumerator _AnimateComicBook()
    {
        m_comicBookModel.SetBool("IdleToIdle", false);
        m_comicBookModel.SetBool("AttackToIdle", false);
        m_comicBookModel.SetBool("IdleToAttack", true);
        yield return new WaitForSeconds(0.3f);
        float animationLength = m_comicBookModel.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(animationLength);
        m_comicBookModel.SetBool("IdleToIdle", true);
        m_comicBookModel.SetBool("IdleToAttack", false);
        m_comicBookModel.SetBool("AttackToIdle", true);
    }

    private IEnumerator _AnimateHorror()
    {
        m_horrorModel.SetBool("IdleToIdle", false);
        m_horrorModel.SetBool("AttackToIdle", false);
        m_horrorModel.SetBool("IdleToAttack", true);
        yield return new WaitForSeconds(0.3f);
        float animationLength = m_horrorModel.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(animationLength);
        m_horrorModel.SetBool("IdleToIdle", true);
        m_horrorModel.SetBool("IdleToAttack", false);
        m_horrorModel.SetBool("AttackToIdle", true);
    }

    private IEnumerator _AnimateSciFi()
    {
        m_scifiModel.SetBool("IdleToIdle", false);
        m_scifiModel.SetBool("AttackToIdle", false);
        m_scifiModel.SetBool("IdleToAttack", true);
        yield return new WaitForSeconds(0.3f);
        float animationLength = m_scifiModel.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(animationLength);
        m_scifiModel.SetBool("IdleToIdle", true);
        m_scifiModel.SetBool("IdleToAttack", false);
        m_scifiModel.SetBool("AttackToIdle", true);
    }

    private IEnumerator _AnimateFantasy()
    {
        m_fantasyModel.SetBool("IdleToIdle", false);
        m_fantasyModel.SetBool("AttackToIdle", false);
        m_fantasyModel.SetBool("IdleToAttack", true);
        yield return new WaitForSeconds(0.3f);
        float animationLength = m_fantasyModel.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(animationLength - 0.2f);
        m_fantasyModel.SetBool("IdleToIdle", true);
        m_fantasyModel.SetBool("IdleToAttack", false);
        m_fantasyModel.SetBool("AttackToIdle", true);
    }
}
