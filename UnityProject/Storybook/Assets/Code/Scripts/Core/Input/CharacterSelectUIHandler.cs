using UnityEngine;
using System.Collections;

public class CharacterSelectUIHandler : UIHandler {

    private Genre m_selectedGenre;
    private PlayerEntity m_selectedEntity;

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

    public void SelectComicBook()
    {
        m_selectedGenre = Genre.GraphicNovel;
        m_selectedEntity = m_comicBookEntity;
    }

    public void SelectHorror()
    {
        m_selectedGenre = Genre.Horror;
        m_selectedEntity = m_horrorEntity;
    }

    public void SelectSciFi()
    {
        m_selectedGenre = Genre.SciFi;
        m_selectedEntity = m_scifiEntity;
    }

    public void SelectFantasy()
    {
        m_selectedGenre = Genre.Fantasy;
        m_selectedEntity = m_fantasyEntity;
    }

    public void SubmitCharacter()
    {
        GameObject entityObject = PhotonNetwork.Instantiate(m_selectedEntity.name, Vector3.zero, Quaternion.identity, 0);
        DontDestroyOnLoad(entityObject);
        PlayerEntity createdEntity = entityObject.GetComponent<PlayerEntity>();
        createdEntity.Construct(PhotonNetwork.player);
        PhotonNetwork.Spawn(entityObject.GetPhotonView());
        SceneFading.Instance().LoadScene("WorldMovementTest");
    }
	
}
