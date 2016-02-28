using UnityEngine;
using System.Collections;

public class SoundEffectsManager : MonoBehaviour
{

    private static SoundEffectsManager s_instance;

    public static SoundEffectsManager Instance
    {
        get { return s_instance; }
    }

    [SerializeField]
    private AudioSource m_source;

    [SerializeField]
    private AudioClip m_clickSoundEffect;

	// Use this for initialization
	protected void Awake ()
    {
        s_instance = this;
	}
	
    public void PlayClickSound()
    {
        m_source.clip = m_clickSoundEffect;
        m_source.Play();
    }
	
}
