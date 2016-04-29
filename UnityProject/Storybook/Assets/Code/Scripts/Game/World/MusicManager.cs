using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour, MusicEventDispatcher.IMusicEventListener
{
    private static MusicManager s_instance;
    public static MusicManager Instance
    {
        get { return s_instance; }
    }

    private AudioSource m_musicSource;
    public AudioSource Music
    {
        get { return m_musicSource; }
    }

    // List of Music Tracks. I'm doing one AudioClip per track (instead of an array)
    // because of the short track list
    [SerializeField]
    private AudioClip m_startRoomMusic;
    [SerializeField]
    private AudioClip m_fantasyRoomMusic;
    [SerializeField]
    private AudioClip m_comicRoomMusic;
    [SerializeField]
    private AudioClip m_horrorRoomMusic;
    [SerializeField]
    private AudioClip m_scifiRoomMusic;
    [SerializeField]
    private AudioClip m_shopRoomMusic;
    [SerializeField]
    private AudioClip m_combatMusic;

    // THe last audio clip played, used for switching back after combat.
    [SerializeField]
    private AudioClip m_lastMusicPlayed = null;
    [SerializeField]
    // The current audio clip being played.
    private AudioClip m_currentMusic = null;

    //=======OLD Music Mgr Stuff BELOW============
    [SerializeField]
    private float m_fadeOutThreshold = 0.05f;

    [SerializeField]
    private float m_fadeSpeed = 0.05f;

    [SerializeField]
    private float m_musicVolume = 0.5f;

    private AudioClip m_nextMusicToPlay = null; 
    private float m_nextMusicVolume;
    private bool m_nextMusicLoop;

    private bool m_isMusicPlaying = false;

    private FadeState m_currentFadeState = FadeState.none;

    public enum FadeState { none, fadeIn, fadeOut };

    //public AudioClip testAudio = null;

    //private AudioClip[] m_currentMusicTracks;


    // ====PROPERTIES====
    /*
    public AudioClip[] MusicTracks
    {
        get { return m_currentMusicTracks; }
        set { m_currentMusicTracks = value; }
    }
    */

    // ====METHODS====
    void Awake()
    {
        EventDispatcher.GetDispatcher<MusicEventDispatcher>().RegisterEventListener(this);
        s_instance = this;
        m_musicSource = GetComponent<AudioSource>();
        m_musicSource.volume = 0f;
    }

    
    // Fade in to a music track
    // Fade function is all-in-one, can fade in from no music playing, can fade from one track to another, and can fade out to silence.
    public void Fade(AudioClip clip, float volume, bool loop)
    {
        m_lastMusicPlayed = m_currentMusic;

        if (clip == null || clip == m_currentMusic)
        {
            Debug.Log("no clip/clip is the same as the one we already have");
            return;
        }

        m_currentMusic = clip;
        m_nextMusicVolume = volume;
        m_nextMusicLoop = loop;

        if (m_musicSource.enabled)
        {
            if (m_isMusicPlaying)
            {
                m_currentFadeState = FadeState.fadeOut;
            }
            else
            {
                FadeToNextClip();
            }
        }
        else
        {
            FadeToNextClip();
        }
    }

    // Play music
    public void Play()
    {
        m_currentFadeState = FadeState.fadeIn;
        m_musicSource.Play();
    }

    // Stop music
    public void Stop()
    {
        m_musicSource.Stop();
        m_currentFadeState = FadeState.none;
    }

    // Fade out of a music track
    private void FadeToNextClip()
    {
        m_musicSource.clip = m_currentMusic;
        m_musicSource.loop = m_nextMusicLoop;

        m_currentFadeState = FadeState.fadeIn;

        if (m_musicSource.enabled)
        {
            m_musicSource.Play();
        }
    }

    private void OnDisable()
    {
        m_musicSource.enabled = false;
        this.Stop();
    }

    private void OnEnable()
    {
        m_musicSource.enabled = true;
        this.Play();
    }

    private void Update()
    {
        if (!m_musicSource.enabled)
        {
            return;
        }

        if (m_currentFadeState == FadeState.fadeOut)
        {
            if (m_musicSource.volume > this.m_fadeOutThreshold)
            {
                Debug.Log("Fading out. vol= " + m_musicSource.volume);
                // Fade out current clip.
                m_musicSource.volume -= this.m_fadeSpeed * Time.deltaTime;
            }
            else
            {
                // Start fading in next clip.
                Debug.Log("Fading in...");
                this.FadeToNextClip();
            }
        }
        else if (m_currentFadeState == FadeState.fadeIn)
        {
            if (m_musicSource.volume < m_nextMusicVolume)
            {
                // Fade in next clip.
                m_musicSource.volume += this.m_fadeSpeed * Time.deltaTime;
            }
            else
            {
                // Stop fading in.
                m_currentFadeState = FadeState.none;
            }
        }
    }
    
    /// <summary>
    /// Used for changing music when we enter the Start room.
    /// </summary>
    public void OnStartRoomEntered()
    {
        Fade(m_startRoomMusic, m_musicVolume, true);
    }

    /// <summary>
    /// Changes music based on the room type and genre. This is used for switching to music in any combat room (before combat starts),
    /// as well as when we enter a shop.
    /// </summary>
    /// <param name="roomGenre">Genre of the room. Tells us which track to pick.</param>
    /// <param name="roomType">Type of the room. Primarily used for determining if we are in a shop or not.</param>
    public void OnRoomMusicChange(Genre roomGenre)
    {
        AudioClip musicToPlay = null;
        switch(roomGenre)
        {
            case Genre.Fantasy:
                musicToPlay = m_fantasyRoomMusic;
                break;
            case Genre.SciFi:
                musicToPlay = m_scifiRoomMusic;
                break;
            case Genre.GraphicNovel:
                musicToPlay = m_comicRoomMusic;
                break;
            case Genre.Horror:
                musicToPlay = m_horrorRoomMusic;
                break;
        }
        Fade(musicToPlay, m_musicVolume, true);
    }

    /// <summary>
    /// Signals the MusicMgr when combat starts, so we know to switch to the combat theme.
    /// </summary>
    public void OnCombatStart()
    {
        Fade(m_combatMusic, m_musicVolume, true);
    }

    /// <summary>
    /// Signals the MusicMgr when combat is over, so we know to go back to the overworld music.
    /// </summary>
    public void OnCombatEnd(Genre roomGenre)
    {
        //Fade(m_lastMusicPlayed, m_musicVolume, true);
        OnRoomMusicChange(roomGenre);
    }

    public void OnShopEntered()
    {
        Fade(m_shopRoomMusic, m_musicVolume, true);
    }

    void OnDestroy()
    {
        EventDispatcher.GetDispatcher<MusicEventDispatcher>().RemoveListener(this);
    }
}
