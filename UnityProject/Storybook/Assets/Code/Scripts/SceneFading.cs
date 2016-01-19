using UnityEngine;
using System.Collections;

public class SceneFading : Photon.PunBehaviour {

    [SerializeField]
    private Texture2D m_fadeOutTexture = null;

    [SerializeField]
    private float m_fadeSpeed = 0.8f;

    private int m_drawDepth = -1000;
    private float m_alpha = 0.0f;
    private int m_fadeDir = -1;

    private bool m_isFading = false;
    private bool m_shouldFlipDirection = false;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
	}

    public void OnGUI()
    {
        if (m_isFading)
        {
            m_alpha += m_fadeDir * m_fadeSpeed * Time.deltaTime;
            m_alpha = Mathf.Clamp01(m_alpha);

            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, m_alpha);
            GUI.depth = m_drawDepth;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_fadeOutTexture);
            
            if ((m_alpha == 0 || m_alpha == 1) && m_shouldFlipDirection)
            {
                BeginFadeOverNetwork(-1);
            }
        }
    }

    public float BeginFade(int direction)
    {
        photonView.RPC("BeginFadeOverNetwork", PhotonTargets.All, direction);
        return m_fadeSpeed;
    }

    public void LevelWasLoaded()
    {
        Debug.Log("Level was loaded");
        if (PhotonNetwork.isMasterClient)
        {
            BeginFade(-1);
        }
    }

    [PunRPC]
    private float BeginFadeOverNetwork(int direction)
    {
        if (!(m_alpha == 0 || m_alpha == 1))
        {
            m_shouldFlipDirection = true;
        }
        else
        {
            m_fadeDir = direction;
            m_isFading = true;
            m_shouldFlipDirection = false;
        }
        return m_fadeSpeed;
    }
}
