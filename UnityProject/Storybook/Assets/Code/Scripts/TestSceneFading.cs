using UnityEngine;
using System.Collections;

public class TestSceneFading : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
        StartCoroutine(loadNextScene());
	}

    private IEnumerator loadNextScene()
    {
        yield return new WaitForSeconds(5.0f);
        float fadeTime = GameObject.Find("Fader").GetComponent<SceneFading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        Application.LoadLevel("FaderScene2");
    }
}
