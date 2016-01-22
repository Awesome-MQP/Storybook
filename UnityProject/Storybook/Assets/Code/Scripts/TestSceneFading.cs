﻿using UnityEngine;
using System.Collections;

public class TestSceneFading : MonoBehaviour {

    [SerializeField]
    SceneFading sceneFader;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
        StartCoroutine(loadNextScene());
	}

    private IEnumerator loadNextScene()
    {
        GameObject faderObject = PhotonNetwork.Instantiate("UIPrefabs/" + sceneFader.name, Vector3.zero, Quaternion.identity, 0);
        PhotonNetwork.Spawn(faderObject.GetPhotonView());
        SceneFading fader = faderObject.GetComponent<SceneFading>();
        yield return new WaitForSeconds(2.0f);
        float fadeTime = fader.BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        PhotonNetwork.LoadLevel("FaderScene2");
        fader.LevelWasLoaded();
        yield return new WaitForSeconds(fadeTime);
        Destroy(fader.gameObject);
        Debug.Log("Fader destroyed");
    }
}