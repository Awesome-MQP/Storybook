using UnityEngine;
using System.Collections;

public class PersistentMusicRemover : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        GameObject titleThemePlayer = GameObject.Find("PersistentMusicPlayer");
        GameObject.Destroy(titleThemePlayer);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
