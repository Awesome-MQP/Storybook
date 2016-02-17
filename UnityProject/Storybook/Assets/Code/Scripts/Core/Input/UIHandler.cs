using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class UIHandler : MonoBehaviour {

    public void PlayClickSound()
    {
        SoundEffectsManager.Instance.PlayClickSound();
    }

}
