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
    [SerializeField]
    private AudioClip m_damageSoundEffect;
    [SerializeField]
    private AudioClip m_noDamageHitSoundEffect;
    [SerializeField]
    private AudioClip m_supportSoundEffect;

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
	
    public void PlayDamageSound()
    {
        m_source.clip = m_damageSoundEffect;
        m_source.Play();
    }

    public void PlayHitNoDamageSound()
    {
        m_source.clip = m_noDamageHitSoundEffect;
        m_source.Play();
    }

    public void PlaySupportSound()
    {
        m_source.clip = m_supportSoundEffect;
        m_source.Play();
    }
}
