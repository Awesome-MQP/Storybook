using UnityEngine;
using System.Collections;

public class SceneFading : MonoBehaviour {

    [SerializeField]
    private Texture2D m_fadeOutTexture;

    [SerializeField]
    private float m_fadeSpeed = 0.8f;

    private int m_drawDepth = -1000;
    private float m_alpha = 0.0f;
    private int m_fadeDir = -1;

    private bool m_isFading = false;

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
        }
    }

    public float BeginFade(int direction)
    {
        m_fadeDir = direction;
        m_isFading = true;
        return m_fadeSpeed;
    }

    void OnLevelWasLoaded()
    {
        BeginFade(-1);
    }
}
