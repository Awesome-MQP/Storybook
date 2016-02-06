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
    private Dictionary<int, Text> m_mapIDtoUI;

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

    [SerializeField]
    private Sprite m_comicSprite;

    [SerializeField]
    private Sprite m_scifiSprite;

    [SerializeField]
    private Sprite m_fantasySprite;

    [SerializeField]
    private Sprite m_horrorSprite;

    [SerializeField]
    private RectTransform m_playerTargetButtons;

    [SerializeField]
    private RectTransform m_enemyTargetButtons;

    private bool m_isThinking = false;

    private int m_handId;
    private List<int> m_targets = new List<int>();
    private List<int> m_activePlayers = new List<int>();
    private List<int> m_activeEnemies = new List<int>();
    private int m_selectedMoveTargets;

    public EventDispatcher Dispatcher { get { return EventDispatcher.GetDispatcher<CombatEventDispatcher>(); } }

    // Use this for initialization
    void Awake () {
        m_mapIDtoUI = new Dictionary<int, Text>();
        _pollPhotonForPlayerInfo();
        EventDispatcher.GetDispatcher<CombatEventDispatcher>().RegisterEventListener(this); 

        // Set the values in the GridLayoutGroup for each of the scroll rects based on the page sizes
        ScrollRect scrollRect = GetComponentInChildren<ScrollRect>();
        GridLayoutGroup gridGroup = scrollRect.GetComponentInChildren<GridLayoutGroup>();
        gridGroup.cellSize = new Vector2(m_buttonWidth, m_buttonHeight);
        gridGroup.spacing = new Vector2(m_gridXPadding, m_gridYPadding);
    }

    void OnDestroy()
    {
        EventDispatcher.GetDispatcher<CombatEventDispatcher>().RemoveListener(this);
        Debug.Log("Menu was destroyed");
    }

    // Popualtes the menu for the first time upon instantiation
    public void PopulateUI()
    {

    }

    // Gets the player IDs from Photon, then gets player info from the GameManager 
    private void _pollPhotonForPlayerInfo()
    {
        // Get a player ID from Photon and use that to find he corresponding player.
        for (int i = 1; i <= PhotonNetwork.playerList.Length; i++)
        {
            PhotonPlayer player = PhotonNetwork.playerList[i - 1]; // the current player
            int playerID = PhotonNetwork.playerList[i - 1].ID; // player ID
            GameManager gm = FindObjectOfType<GameManager>();
            PlayerEntity pe = gm.FindPlayerEntity(playerID);

            Debug.Log("Children count = " + gameObject.GetComponentsInChildren<Image>().Length);

            // Set and enable the corresponding player icon
            Image playerImage = null;
            string iconName = "Player" + i + "Icon";
            foreach(Image image in GetComponentsInChildren<Image>())
            {
                Debug.Log("Image name = " + image.name);
                if (image.name == iconName)
                {
                    playerImage = image;
                }
            }
            playerImage.enabled = true;
            playerImage.sprite = _getImageBasedOnGenre(pe);

            // Set and enable the corresponding player ID text
            Text playerIdText = null;
            string idName = "Player" + i + "ID";
            foreach (Text text in GetComponentsInChildren<Text>())
            {
                if (text.name == idName)
                {
                    playerIdText = text;
                }
            }
            playerIdText.enabled = true;
            playerIdText.text = "ID: " + playerID.ToString();

            // Set and enable the corresponding player HP text
            Text playerHP = null;
            string hpName = "Player" + i + "HP";
            foreach (Text text in GetComponentsInChildren<Text>())
            {
                if (text.name == hpName)
                {
                    playerHP = text;
                }
            }
            playerHP.enabled = true;
            playerHP.text = "HP: " + pe.HitPoints + "/" + pe.MaxHitPoints;
            m_mapIDtoUI.Add(playerID, playerHP);
         }          
     }

    // Determines what image icon the player will have
    private Sprite _getImageBasedOnGenre(PlayerEntity pe)
    {
        Sprite genreIcon = Resources.Load("Sprites/Icon_NoGenre") as Sprite;

        switch(pe.Genre)
        {
            case Genre.Fantasy:
                {
                    genreIcon = m_fantasySprite;
                    break;
                }
            case Genre.GraphicNovel:
                {
                    genreIcon = m_comicSprite;
                    break;
                }
            case Genre.Horror:
                {
                    genreIcon = m_horrorSprite;
                    break;
                }
            case Genre.SciFi:
                {
                    genreIcon = m_scifiSprite;
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
    private void _updateHitpointsOfPlayer(PhotonPlayer photonPlayer, int newHP, int maxHP)
    {
        if (m_mapIDtoUI.ContainsKey(photonPlayer.ID))
        {
            m_mapIDtoUI[photonPlayer.ID].text = "HP: " + newHP.ToString() + "/" + maxHP.ToString();
        }
    }

    // Send the page data to the combat system which will process the move, but only if the player is thinking
    public override void PageButtonPressed(PageButton pageButton)
    {
        // Send PageData to the combat system
        if (m_isThinking)
        {
            m_handId = m_pageButtonList.IndexOf(pageButton);
            _displayTargetButtons(pageButton);
            if (pageButton.PageData.IsRare)
            {
                m_selectedMoveTargets = 4;
            }
            else
            {
                m_selectedMoveTargets = 1;
            }
            /*
            int handId = m_pageButtonList.IndexOf(pageButton);
            EventDispatcher.GetDispatcher<CombatEventDispatcher>().OnCombatMoveChosen(handId);
            Debug.Log("Sending a combatMoveChosen event, index: " + handId);
            // Delete the page and shift pages
            Destroy(pageButton.gameObject);
            m_isThinking = false;
            */
            foreach (PageButton pb in m_pageButtonList)
            {
                pb.GetComponent<Button>().interactable = false;
            }
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
            foreach (PageButton pb in m_pageButtonList)
            {
                pb.GetComponent<Button>().interactable = true;
            }
        }
    }

    // Know to update the UI when a player takes damage
    public void OnPawnTakesDamage(PhotonPlayer thePlayer, int currentHealth, int maxHealth)
    {
        _updateHitpointsOfPlayer(thePlayer, currentHealth, maxHealth);
    }

    // Don't do anything - this method is for players only.
    public void OnCombatMoveChosen(int pawnId, int handIndex, int[] targets)
    {
        return;
    }

    private void _displayTargetButtons(PageButton buttonPressed)
    {
        switch (buttonPressed.PageMoveType)
        {
            case MoveType.Attack:
                _displayEnemyTargetButtons();
                break;
            case MoveType.Boost:
                _displayPlayerTargetButtons();
                break;
            case MoveType.Status:
                _displayEnemyTargetButtons();
                break;
        }
    }

    private void _displayPlayerTargetButtons()
    {
        PlayerTeam playerTeam = FindObjectOfType<PlayerTeam>();
        foreach(CombatPawn pawn in playerTeam.ActivePawnsOnTeam)
        {
            m_activePlayers.Add(pawn.PawnId);
        }

        TargetButton[] playerTargetButtons = m_playerTargetButtons.GetComponentsInChildren<TargetButton>();

        foreach(TargetButton b in playerTargetButtons)
        {
            if (m_activePlayers.Contains(b.TargetId))
            {
                b.enabled = true;
                b.GetComponent<Button>().enabled = true;
                b.GetComponent<Image>().enabled = true;
                b.GetComponentInChildren<Text>().enabled = true;
            }
        }
    }

    private void _displayEnemyTargetButtons()
    {
        EnemyTeam enemyTeam = FindObjectOfType<EnemyTeam>();
        foreach (CombatPawn pawn in enemyTeam.ActivePawnsOnTeam)
        {
            m_activeEnemies.Add(pawn.PawnId);
        }

        TargetButton[] enemyTargetButtons = m_enemyTargetButtons.GetComponentsInChildren<TargetButton>();
        foreach (TargetButton b in enemyTargetButtons)
        {
            if (m_activeEnemies.Contains(b.TargetId))
            {
                b.enabled = true;
                b.GetComponent<Button>().enabled = true;
                b.GetComponent<Image>().enabled = true;
                b.GetComponentInChildren<Text>().enabled = true;
            }
        }
    }

    private void _hidePlayerTargetButtons()
    {
        TargetButton[] playerTargetButtons = m_playerTargetButtons.GetComponentsInChildren<TargetButton>();

        foreach (TargetButton b in playerTargetButtons)
        {
            b.enabled = false;
            b.GetComponent<Button>().enabled = false;
            b.GetComponent<Image>().enabled = false;
            b.GetComponentInChildren<Text>().enabled = false;
        }
    }

    private void _hideEnemyTargetButtons()
    {
        TargetButton[] enemyTargetButtons = m_enemyTargetButtons.GetComponentsInChildren<TargetButton>();
        foreach (TargetButton b in enemyTargetButtons)
        {
            b.enabled = false;
            b.GetComponent<Button>().enabled = false;
            b.GetComponent<Image>().enabled = false;
            b.GetComponentInChildren<Text>().enabled = false;
        }
    }

    public void TargetSelected(int targetId)
    {
        Debug.Log("Button pressed");
        _hideEnemyTargetButtons();
        _hidePlayerTargetButtons();
        m_targets.Add(targetId);
        if (m_targets.Count >= m_selectedMoveTargets)
        {
            EventDispatcher.GetDispatcher<CombatEventDispatcher>().OnCombatMoveChosen(PhotonNetwork.player.ID, m_handId, m_targets.ToArray());
            m_activePlayers = new List<int>();
            m_activeEnemies = new List<int>();
            m_targets = new List<int>();
        }
    }
}
