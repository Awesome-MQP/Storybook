using UnityEngine;
using System.Collections;

public class GaussianRandom : System.Random {

    System.Random r;

    public GaussianRandom()
    {
        r = new System.Random();
    }

    public GaussianRandom(int seed)
    {
        r = new System.Random(seed);
    }

    public override double NextDouble()
    {
        float u1, u2, z;
        System.Random r = new System.Random();
        
        u1 = (float) r.NextDouble();
        u2 = (float) r.NextDouble();

        z = Mathf.Sqrt(-2.0f * Mathf.Log(u1, 2.0f)) * Mathf.Sin(2.0f * Mathf.PI * u2);

        return z;
    }
	
}
