using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class CombatMenuUIHandler : UIHandler, ICombatEventListener {

    private Vector3 m_player1InfoPosition = new Vector3(-349, 197, 0);
    private Vector3 m_player2InfoPosition = new Vector3(-131, 197, 0);
    private Vector3 m_player3InfoPosition = new Vector3(59, 197, 0);
    private Vector3 m_player4InfoPosition = new Vector3(255, 197, 0);

    // Map of Player IDs to their Info screens, used for when HP is updated.
    private Dictionary<int, Text> m_mapIDtoUI = new Dictionary<int, Text>();

    // List of the pages the player has in their hand
    //private PageButton[] m_pageButtons = new PageButton[5];
    private List<PageButton> m_pageButtonList = new List<PageButton>();

    [SerializeField]
    private float m_gridXPadding;

    [SerializeField]
    private float m_gridYPadding;

    [SerializeField]
    private float m_buttonHeight;
    [SerializeField]
    private float m_buttonWidth;

    private bool m_isThinking = false;

    public EventDispatcher Dispatcher { get { return EventDispatcher.GetDispatcher<CombatEventDispatcher>(); } }

    // Use this for initialization
    void Awake () {
        _pollPhotonForPlayerInfo();
        EventDispatcher.GetDispatcher<CombatEventDispatcher>().RegisterEventListener(this); 

        // Set the values in the GridLayoutGroup for each of the scroll rects based on the page sizes
        ScrollRect scrollRect = GetComponentInChildren<ScrollRect>();
        GridLayoutGroup gridGroup = scrollRect.GetComponentInChildren<GridLayoutGroup>();
        gridGroup.cellSize = new Vector2(m_buttonWidth, m_buttonHeight);
        gridGroup.spacing = new Vector2(m_gridXPadding, m_gridYPadding);
    }
	
    // Popualtes the menu for the first time upon instantiation
    public void PopulateUI()
    {

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
                        this.transform.GetChild(0).gameObject.SetActive(true);
                        this.transform.GetChild(1).gameObject.SetActive(true);
                        this.transform.GetChild(2).gameObject.SetActive(true);
                        // Set the Icon type based on the player's genre
                        Image p1img = GameObject.Find("Player1Icon").GetComponent<Image>();
                        //p1img.sprite = _getImageBasedOnGenre(pe); //UNCOMMENT WHEN PLAYER ENTITYS ARE INTEGRATED
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
                        this.transform.GetChild(3).gameObject.SetActive(true);
                        this.transform.GetChild(4).gameObject.SetActive(true);
                        this.transform.GetChild(5).gameObject.SetActive(true);
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
                        this.transform.GetChild(6).gameObject.SetActive(true);
                        this.transform.GetChild(7).gameObject.SetActive(true);
                        this.transform.GetChild(8).gameObject.SetActive(true);
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
                        this.transform.GetChild(9).gameObject.SetActive(true);
                        this.transform.GetChild(10).gameObject.SetActive(true);
                        this.transform.GetChild(11).gameObject.SetActive(true);
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
    public void _drawPage(Page page, int counter)
    {
        string pageName = page.PageGenre.ToString();
        string pageNameAsColor = "";
        switch (pageName)
        {
            case "Fantasy":
                pageNameAsColor = "Green";
                break;
            case "GraphicNovel":
                pageNameAsColor = "Yellow";
                break;
            case "Horror":
                pageNameAsColor = "Red";
                break;
            case "SciFi":
                pageNameAsColor = "Blue";
                break;
            default:
                pageNameAsColor = "NULL";
                break;
        }
        string pathToPage = pageNameAsColor + "PageButton";
        Debug.Log("Card #: " + counter + " is " + pageName);
        Debug.Log(pathToPage);
        pathToPage = "UIPrefabs/" + pathToPage;

        /*
        Button go = Instantiate(Resources.Load(pathToPage), posToUse, Quaternion.identity) as Button;
        // TODO: Add the listener for buttons
        GameObject goLevel = go.transform.GetChild(0).gameObject;
        goLevel.GetComponent<TextMesh>().text = "Level " + page.PageLevel + "\n" + page.PageType.ToString();
        */
        ScrollRect pageButtonArea = GetComponentInChildren<ScrollRect>();
        RectTransform content = pageButtonArea.content;

        PageData currentPageData = page.GetPageData();
        currentPageData.InventoryId = counter;
        Button pageButton = _initializePageButton(currentPageData);
        pageButton.transform.SetParent(content, false);
        m_pageButtonList.Add(pageButton.GetComponent<PageButton>());
    }

    // Removes a page from the screen
    public void DestroyPage(int index)
    {
        PageButton pageButton = m_pageButtonList[index];
        m_pageButtonList.RemoveAt(index);
        Destroy(pageButton.gameObject);
    }    

    // Modify the player's HP by taking in the PhotonID (so we know what player it is) and the new HP to update it
    private void _updateHitpointsOfPlayer(PhotonPlayer photonPlayer, int newHP)
    {
        if(m_mapIDtoUI.ContainsKey(photonPlayer.ID))
        {
            m_mapIDtoUI[photonPlayer.ID].text = newHP.ToString();
        }
    }

    // Send the page data to the combat system which will process the move, but only if the player is thinking
    public override void PageButtonPressed(PageButton pageButton)
    {
        // Send PageData to the combat system
        if(m_isThinking)
        {
            int handId = m_pageButtonList.IndexOf(pageButton);
            EventDispatcher.GetDispatcher<CombatEventDispatcher>().OnCombatMoveChosen(handId);
            Debug.Log("Sending a combatMoveChosen event, index: " + handId);
            // Delete the page and shift pages
            Destroy(pageButton.gameObject);
            m_isThinking = false;
        }
        return;
    }

    // Know to draw a page on the screen when the player draws one
    public void OnReceivePage(Page playerPage, int counter)
    {
        if(this != null)
        {
            Debug.Log("Counter = " + counter);
            _drawPage(playerPage, counter);
            m_isThinking = true; // Since we are getting our hand back, we are back in thinking.
        }
    }

    // Know to update the UI when a player takes damage
    public void OnPawnTakesDamage(PhotonPlayer thePlayer, int currentHealth)
    {
        _updateHitpointsOfPlayer(thePlayer, currentHealth);
    }

    // Don't do anything - this method is for players only.
    public void OnCombatMoveChosen(int handNumber)
    {
        return;
    }
}
