using UnityEngine;
using System.Collections;

public class TestGaussianRandom : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("TEST");
        double randomNumber = GaussianRandom.GetRandomNumber();
        Debug.Log("Gaussian Random Number = " + randomNumber);
	}
}
