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
        PersistentMusicPlayer[] musicPlayers = FindObjectsOfType<PersistentMusicPlayer>();
        PersistentMusicPlayer previousPlayer = null;

        foreach(PersistentMusicPlayer player in musicPlayers)
        {
            if (player.gameObject != this.gameObject)
            {
                previousPlayer = player;
                break; 
            }
        }

        if (previousPlayer != null)
        {
            Destroy(previousPlayer.gameObject);
        }

        s_instance = this;
        DontDestroyOnLoad(this);
        m_source.clip = m_titleTheme;
        m_source.Play();
	}

}
