using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

    [SerializeField]
    private AudioClip m_currentMusicTrack = null;

    public float FadeOutThreshold = 0.05f;
    public float FadeSpeed = 0.05f;

    [SerializeField]
    private AudioSource m_musicSource = null;

    [SerializeField]
    private float m_musicVolume = 5.0f;

    private AudioClip m_nextMusicToPlay = null; 
    private float m_nextMusicVolume;
    private bool m_nextMusicLoop;

    private bool m_isMusicPlaying = false;

    private FadeState m_currentFadeState = FadeState.none;

    public enum FadeState { none, fadeIn, fadeOut };

    public AudioClip testAudio = null;

    void Awake()
    {
        m_musicSource = GetComponent<AudioSource>();
        m_musicSource.volume = 0f;
    }

    // GUI to test the music
    /*
    void OnGUI()
    {
        GUI.Box(new Rect(10, 500, 150, 100), "Music Test Menu");

        // Button to test FadeIn
        if (GUI.Button(new Rect(20, 520, 130, 20), "Fade In"))
        {
            Fade(m_currentMusicTrack, m_musicVolume, true);
        }

        // Button to test FadeOut
        if (GUI.Button(new Rect(20, 550, 130, 20), "Fade Out"))
        {
            Fade(testAudio, m_musicVolume, true);
        }
    }
    */
    
    // Fade in to a music track
    // Fade function is all-in-one, can fade in from no music playing, can fade from one track to another, and can fade out to silence.
    public void Fade(AudioClip clip, float volume, bool loop)
    {
        if (clip == null || clip == this.m_musicSource.clip)
        {
            Debug.Log("no clip/clip is the same as the one we already have");
            return;
        }

        m_nextMusicToPlay = clip;
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
        m_musicSource.clip = m_nextMusicToPlay;
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
            if (m_musicSource.volume > this.FadeOutThreshold)
            {
                // Fade out current clip.
                m_musicSource.volume -= this.FadeSpeed * Time.deltaTime;
            }
            else
            {
                // Start fading in next clip.
                this.FadeToNextClip();
            }
        }
        else if (m_currentFadeState == FadeState.fadeIn)
        {
            if (m_musicSource.volume < m_nextMusicVolume)
            {
                // Fade in next clip.
                m_musicSource.volume += this.FadeSpeed * Time.deltaTime;
            }
            else
            {
                // Stop fading in.
                m_currentFadeState = FadeState.none;
            }
        }
    }
}
