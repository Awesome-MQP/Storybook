using UnityEngine;
using Photon;
using System.Collections;

public class JoinCreateGameStartup : Photon.MonoBehaviour
{
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
