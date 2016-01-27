using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class CombatMenuUIHandler : UIHandler {

    private Vector3 m_player1InfoPosition = new Vector3(-349, 197, 0);
    private Vector3 m_player2InfoPosition = new Vector3(-131, 197, 0);
    private Vector3 m_player3InfoPosition = new Vector3(59, 197, 0);
    private Vector3 m_player4InfoPosition = new Vector3(255, 197, 0);

    private Vector3 m_page1Pos = new Vector3(-300, -142, 0);
    private Vector3 m_page2Pos = new Vector3(-150, -142, 0);
    private Vector3 m_page3Pos = new Vector3(0, -142, 0);
    private Vector3 m_page4Pos = new Vector3(150, -142, 0);
    private Vector3 m_page5Pos = new Vector3(300, -142, 0);

    // Map of Player IDs to their Info screens, used for when HP is updated.
    private Dictionary<int, Text> m_mapIDtoUI = new Dictionary<int, Text>();

    // List of the pages the player has in their hand
    private Page[] m_pagesInHand = new Page[5];
    private Button[] m_pageButtons = new Button[5];

    private CombatUIEventListener combatUIListener; 

    public EventDispatcher Dispatcher
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    // Use this for initialization
    void Awake () {
        _pollPhotonForPlayerInfo();
        Dispatcher.RegisterEventListener(combatUIListener); 
	}
	
    // Popualtes the menu for the first time upon instantiation
    public void PopulateUI()
    {
        //_pollPhotonForPlayerInfo();
        Debug.Log("Combat UI is active");
    }

    // Gets the player IDs from Photon, then gets player info from the GameManager 
    private void _pollPhotonForPlayerInfo()
    {
        // Get a player ID from Photon and use that to find he corresponding player.
        for(int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            PhotonPlayer player = PhotonNetwork.playerList[i]; // the current player
            int playerID = PhotonNetwork.playerList[i].ID; // player ID
            GameManager gm = FindObjectOfType<GameManager>();
            // PlayerEntity pe = gm.GetPlayerEntity(playerID);
            switch(i)
            {
                case 0:
                    {
                        // Activate the UI elemnts 
                        GameObject.Find("Player1Icon").SetActive(true);
                        GameObject.Find("Player1ID").SetActive(true);
                        GameObject.Find("Player1HP").SetActive(true);
                        // Set the Icon type based on the player's genre
                        Image p1img = GameObject.Find("Player1Icon").GetComponent<Image>();
                        //p1img.sprite = _getImageBasedOnGenre(pe); UNCOMMENT WHEN PLAYER ENTITYS ARE INTEGRATED
                        Text p1ID = GameObject.Find("Player1ID").GetComponent<Text>();
                        p1ID.text = "ID: " + playerID.ToString();
                        Text p1HP = GameObject.Find("Player1HP").GetComponent<Text>();
                        p1HP.text = "HP: " + p1HP.ToString();
                        m_mapIDtoUI.Add(playerID, p1HP);
                        break;
                    }
                case 1:
                    {
                        // Activate the UI elemnts 
                        GameObject.Find("Player2Icon").SetActive(true);
                        GameObject.Find("Player2ID").SetActive(true);
                        GameObject.Find("Player2HP").SetActive(true);
                        // Set the Icon type based on the player's genre
                        Image p2img = GameObject.Find("Player2Icon").GetComponent<Image>();
                        //p2img.sprite = _getImageBasedOnGenre(pe); UNCOMMENT WHEN PLAYER ENTITYS ARE INTEGRATED
                        Text p2ID = GameObject.Find("Player2ID").GetComponent<Text>();
                        p2ID.text = "ID: " + playerID.ToString();
                        Text p2HP = GameObject.Find("Player2HP").GetComponent<Text>();
                        p2HP.text = "HP: " + p2HP.ToString();
                        m_mapIDtoUI.Add(playerID, p2HP);
                        break;
                    }
                case 2:
                    {
                        // Activate the UI elemnts 
                        GameObject.Find("Player3Icon").SetActive(true);
                        GameObject.Find("Player3ID").SetActive(true);
                        GameObject.Find("Player3HP").SetActive(true);
                        // Set the Icon type based on the player's genre
                        Image p3img = GameObject.Find("Player3Icon").GetComponent<Image>();
                        //p3img.sprite = _getImageBasedOnGenre(pe); UNCOMMENT WHEN PLAYER ENTITYS ARE INTEGRATED
                        Text p3ID = GameObject.Find("Player3ID").GetComponent<Text>();
                        p3ID.text = "ID: " + playerID.ToString();
                        Text p3HP = GameObject.Find("Player3HP").GetComponent<Text>();
                        p3HP.text = "HP: " + p3HP.ToString();
                        m_mapIDtoUI.Add(playerID, p3HP);
                        break;
                    }
                case 3:
                    {
                        // Activate the UI elemnts 
                        GameObject.Find("Player4Icon").SetActive(true);
                        GameObject.Find("Player4ID").SetActive(true);
                        GameObject.Find("Player4HP").SetActive(true);
                        // Set the Icon type based on the player's genre
                        Image p4img = GameObject.Find("Player4Icon").GetComponent<Image>();
                        //p4img.sprite = _getImageBasedOnGenre(pe); UNCOMMENT WHEN PLAYER ENTITYS ARE INTEGRATED
                        Text p4ID = GameObject.Find("Player4ID").GetComponent<Text>();
                        p4ID.text = "ID: " + playerID.ToString();
                        Text p4HP = GameObject.Find("Player4HP").GetComponent<Text>();
                        p4HP.text = "HP: " + p4HP.ToString();
                        m_mapIDtoUI.Add(playerID, p4HP);
                        break;
                    }
            }
            
        }
    }

    // Determines what image icon the player will have
    private Image _getImageBasedOnGenre(PlayerEntity pe)
    {
        Image genreIcon = Resources.Load("Sprites/Icon_NoGenre") as Image;

        switch(pe.Genre)
        {
            case Genre.Fantasy:
                {
                    genreIcon = Resources.Load("Sprites/Icon_Fantasy") as Image;
                    break;
                }
            case Genre.GraphicNovel:
                {
                    genreIcon = Resources.Load("Sprites/Icon_Comic") as Image;
                    break;
                }
            case Genre.Horror:
                {
                    genreIcon = Resources.Load("Sprites/Icon_horror") as Image;
                    break;
                }
            case Genre.SciFi:
                {
                    genreIcon = Resources.Load("Sprites/Icon_SciFi") as Image;
                    break;
                }
            default:
                {
                    break;
                }
        }

        return genreIcon;
    }

    // Draws a player's hand on the screen
    // Functionally identical to the similar method from CombatPlayer.
    public void DrawPage(Page page, int counter)
    {
        string pageName = page.PageGenre.ToString();
        string pathToPage = "Pages/" + "PageButton_" + pageName;
        Debug.Log("Card #: " + counter + " is " + pageName);
        Vector3 posToUse = Vector3.zero;
        switch (counter)
        {
            case 0:
                posToUse = m_page1Pos;
                break;
            case 1:
                posToUse = m_page2Pos;
                break;
            case 2:
                posToUse = m_page3Pos;
                break;
            case 3:
                posToUse = m_page4Pos;
                break;
            case 4:
                posToUse = m_page5Pos;
                break;
            default:
                break;
        }
        Button go = Instantiate(Resources.Load(pathToPage), posToUse, Quaternion.identity) as Button;
        // TODO: Add the listener for buttons
        GameObject goLevel = go.transform.GetChild(1).gameObject;
        goLevel.GetComponent<TextMesh>().text = "Level " + page.PageLevel + "\n" + page.PageType.ToString();
        Page pageData = goLevel.GetComponent<Page>(); // Each button has a page script attached to it, which holds page data. The data is assigned at this step.
        m_pagesInHand[counter] = pageData;
    }

    // Removes a page from the screen
    public void DestroyPage(int index)
    {
        Destroy(m_pageButtons[index]);
    }

    // Visually shift pages over to make room for when one is drawn
    public void ShiftPages(int index)
    {
        switch (index)
        {
            case 0:
                m_pageButtons[0] = m_pageButtons[1];
                m_pageButtons[0].transform.position = m_page1Pos;
                m_pageButtons[1] = m_pageButtons[2];
                m_pageButtons[1].transform.position = m_page2Pos;
                m_pageButtons[2] = m_pageButtons[3];
                m_pageButtons[2].transform.position = m_page3Pos;
                m_pageButtons[3] = m_pageButtons[4];
                m_pageButtons[3].transform.position = m_page4Pos;
                m_pageButtons[4] = null;
                break;
            case 1:
                m_pageButtons[1] = m_pageButtons[2];
                m_pageButtons[1].transform.position = m_page2Pos;
                m_pageButtons[2] = m_pageButtons[3];
                m_pageButtons[2].transform.position = m_page3Pos;
                m_pageButtons[3] = m_pageButtons[4];
                m_pageButtons[3].transform.position = m_page4Pos;
                m_pageButtons[4] = null;
                break;
            case 2:
                m_pageButtons[2] = m_pageButtons[3];
                m_pageButtons[2].transform.position = m_page3Pos;
                m_pageButtons[3] = m_pageButtons[4];
                m_pageButtons[3].transform.position = m_page4Pos;
                m_pageButtons[4] = null;
                break;
            case 3:
                m_pageButtons[3] = m_pageButtons[4];
                m_pageButtons[3].transform.position = m_page4Pos;
                m_pageButtons[4] = null;
                break;
            case 4:
                break;
            default:
                break;
        }
    }

    // Modify the player's HP by taking in the PhotonID (so we know what player it is) and the new HP to update it
    public void UpdateHitpointsOfPlayer(PhotonPlayer photonPlayer, float newHP)
    {
        if(m_mapIDtoUI.ContainsKey(photonPlayer.ID))
        {
            m_mapIDtoUI[photonPlayer.ID].text = newHP.ToString();
        }
    }

    public override void PageButtonPressed(PageButton pageButton)
    {
        throw new NotImplementedException();
    }
}
