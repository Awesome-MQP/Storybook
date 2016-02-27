using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

    [SerializeField]
    private Button m_comicBookButton;

    [SerializeField]
    private Button m_horrorButton;

    [SerializeField]
    private Button m_scifiButton;

    [SerializeField]
    private Button m_fantasyButton;

    private Button m_selectedButton = null;

    public void SelectComicBook()
    {
        StartCoroutine(_AnimateComicBook());
        if (m_selectedButton != null)
        {
            m_selectedButton.interactable = true;
        }
        m_comicBookButton.interactable = false;
        m_selectedButton = m_comicBookButton;
        m_selectedGenre = Genre.GraphicNovel;
        m_selectedEntity = m_comicBookEntity;
    }

    public void SelectHorror()
    {
        StartCoroutine(_AnimateHorror());
        if (m_selectedButton != null)
        {
            m_selectedButton.interactable = true;
        }
        m_horrorButton.interactable = false;
        m_selectedButton = m_horrorButton;
        m_selectedGenre = Genre.Horror;
        m_selectedEntity = m_horrorEntity;
    }

    public void SelectSciFi()
    {
        StartCoroutine(_AnimateSciFi());
        if (m_selectedButton != null)
        {
            m_selectedButton.interactable = true;
        }
        m_scifiButton.interactable = false;
        m_selectedButton = m_scifiButton;
        m_selectedGenre = Genre.SciFi;
        m_selectedEntity = m_scifiEntity;
    }

    public void SelectFantasy()
    {
        StartCoroutine(_AnimateFantasy());
        if (m_selectedButton != null)
        {
            m_selectedButton.interactable = true;
        }
        m_fantasyButton.interactable = false;
        m_selectedButton = m_fantasyButton;
        m_selectedGenre = Genre.Fantasy;
        m_selectedEntity = m_fantasyEntity;
    }

    public void SubmitCharacter()
    {
        GameObject entityObject = PhotonNetwork.Instantiate("PlayerEntity/" + m_selectedEntity.name, Vector3.zero, Quaternion.identity, 0);
        DontDestroyOnLoad(entityObject);
        PlayerEntity createdEntity = entityObject.GetComponent<PlayerEntity>();
        createdEntity.Construct(PhotonNetwork.player);
        PhotonNetwork.Spawn(entityObject.GetPhotonView());
        SceneFading.Instance().LoadScene("WorldMovementTest");
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
