using UnityEngine;
using System.Collections;

public class NetWinState : NetworkState, CombatSummaryEventDispatcher.ICombatSummaryListener {

    private bool m_exitCombat = false;
    private bool m_isClientReady = false;
    private int m_playersReady = 0;
    private GameObject m_combatSummaryUI;

    public EventDispatcher Dispatcher { get { return EventDispatcher.GetDispatcher<CombatSummaryEventDispatcher>(); } }

    override protected void Awake()
    {
        SetCombatManager(FindObjectOfType<CombatManager>());
    }

    void Start()
    {
        _openCombatSummaryMenu();
        EventDispatcher.GetDispatcher<CombatSummaryEventDispatcher>().RegisterEventListener(this);
    }

    void OnDestroy()
    {
        EventDispatcher.GetDispatcher<CombatSummaryEventDispatcher>().RemoveListener(this);
    }

    void Update()
    {
        if (m_playersReady >= PhotonNetwork.playerList.Length)
        {
            m_exitCombat = true;
        }
    }

    public bool ExitCombat
    {
        get { return m_exitCombat; }
    }

    // Ends the current combat
    public void DeleteCombat()
    {
        CManager.EndCurrentCombat();
    }

    /// <summary>
    /// Increments the number of players ready on all clients
    /// </summary>
    [PunRPC]
    private void IncrementPlayersReady()
    {
        Debug.Log("Another player is ready");
        m_playersReady += 1;
    }

    private void _getPageDrop()
    {
        DungeonMaster dm = GameManager.GetInstance<BaseStorybookGame>().DM;
        Page pageDrop = dm.GetPageDropFromCombat(CManager.CombatGenre, CManager.CombatLevel);

        GameManager gm = FindObjectOfType<GameManager>();
        PlayerInventory localPlayerInventory = null;//gm.GetLocalPlayerInventory();

        // TODO: Use the number of items in the inventory to figure out the position to add to
        if (!localPlayerInventory.IsInventoryFull()) {
            localPlayerInventory.Add(pageDrop, localPlayerInventory.FirstOpenSlot());
            localPlayerInventory.SortInventory(20, localPlayerInventory.DynamicSize);
        }

        if (pageDrop != null)
        {
            Debug.Log("Got a page drop!");
            Debug.Log("Page genre = " + pageDrop.PageGenre);
            Debug.Log("Page level = " + pageDrop.PageLevel);
            Debug.Log("Page type = " + pageDrop.PageType);
            Debug.Log("Is the page rare = " + pageDrop.Rarity);
        }
        else
        {
            Debug.Log("No page was dropped");
        }
    }

    private void _openCombatSummaryMenu()
    {
        CombatMenuUIHandler combatMenu = FindObjectOfType<CombatMenuUIHandler>();
        Destroy(combatMenu.gameObject);
        GameObject uiGameObject = Resources.Load("UI/CombatSummaryUI") as GameObject;
        m_combatSummaryUI = Instantiate(uiGameObject);
        CombatSummaryUIHandler uiHandler = m_combatSummaryUI.GetComponent<CombatSummaryUIHandler>();
        uiHandler.PopulateMenu(CManager.CombatLevel, CManager.CombatGenre);

        // Send out a tutorial event
        EventDispatcher.GetDispatcher<TutorialEventDispatcher>().OnCombatCleared();
    }

    public void CombatSummarySubmitted()
    {
        Destroy(m_combatSummaryUI);
        GetComponent<PhotonView>().RPC("IncrementPlayersReady", PhotonTargets.All);
    }
}
