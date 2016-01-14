using UnityEngine;
using System.Collections;

public class GaussianRandom : System.Random {

    public static double GetRandomNumber()
    {
        float u1, u2, z;
        System.Random r = new System.Random();
        
        u1 = float.Parse(r.NextDouble().ToString());
        u2 = float.Parse(r.NextDouble().ToString());

        z = Mathf.Sqrt(-2 * Mathf.Log(u1, 2)) * Mathf.Sin(2 * Mathf.PI * u2);

        return z;
    }
	
}
