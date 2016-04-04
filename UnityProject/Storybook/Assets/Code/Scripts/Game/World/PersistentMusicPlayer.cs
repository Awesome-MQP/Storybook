using UnityEngine;
using System.Collections;

// A class that is meant to play music across multiple scenes (i.e. the main theme)
public class PersistentMusicPlayer : MonoBehaviour {

    [SerializeField]
    private AudioSource m_source;
    [SerializeField]
    private AudioClip m_titleTheme;

    private static PersistentMusicPlayer s_instance;

    public static PersistentMusicPlayer Instance
    {
        get { return s_instance; }
    }

    // Use this for initialization
    protected void Awake () {
        s_instance = this;
        DontDestroyOnLoad(this);
        m_source.clip = m_titleTheme;
        m_source.Play();
	}

    void OnLevelWasLoaded(int level)
    {
        switch(level)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                break;
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
                Destroy(this.gameObject);
                break;
        }
    }
}
