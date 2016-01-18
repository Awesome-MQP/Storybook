using UnityEngine;
using System.Collections;

public class TestGaussianRandom : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GaussianRandom gr = new GaussianRandom();
        double randomNumber = gr.NextDouble();
        Debug.Log("Gaussian Random Number = " + randomNumber);
	}
}
