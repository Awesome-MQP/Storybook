using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class UIHandler : MonoBehaviour {

    [SerializeField]
    private AudioClip m_clickSoundEffect;

    public void PlayClickSound()
    {
        AudioSource clickSource = GetComponent<AudioSource>();
        if (clickSource == null)
        {
            clickSource = gameObject.AddComponent<AudioSource>();
        }
        clickSource.clip = m_clickSoundEffect;
        clickSource.Play();
    }

}
