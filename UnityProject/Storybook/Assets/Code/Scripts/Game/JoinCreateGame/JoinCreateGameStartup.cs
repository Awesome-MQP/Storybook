using UnityEngine;
using System.Collections;

public class JoinCreateGameStartup : MonoBehaviour {
    [SerializeField]
    private JoinGameMenuUIHandler m_joinCreateGamePrefab;

	// Use this for initialization
	protected void Awake ()
    {
        JoinGameMenuUIHandler ui = Instantiate(m_joinCreateGamePrefab);
	}

    protected void Start()
    {
        
    }
}
