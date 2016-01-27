using UnityEngine;
using System.Collections;

public class NetWinState : NetworkState {

    private bool m_exitCombat = false;
    private bool m_isClientReady = false;
    private int m_playersReady = 0;
    private int m_trigger = 0;

    override protected void Awake()
    {
        SetCombatManager(FindObjectOfType<CombatManager>());
    }

    void Update()
    {
        if (!m_isClientReady)
        {
            m_trigger += 1;
            if (m_trigger > 20)
            {
                m_isClientReady = true;
                _getPageDrop();
                GetComponent<PhotonView>().RPC("IncrementPlayersReady", PhotonTargets.All);
            }
        }
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
        BaseStorybookGame dm = GameManager.GetInstance<BaseStorybookGame>();
        Page pageDrop = dm.GetPageDropFromCombat(Genre.GraphicNovel, 1);

        GameManager gm = FindObjectOfType<GameManager>();
        PlayerInventory localPlayerInventory = null;//gm.GetLocalPlayerInventory();

        // TODO: Use the number of items in the inventory to figure out the position to add to
        if (!localPlayerInventory.IsInventoryFull()) {
            localPlayerInventory.Add(pageDrop, localPlayerInventory.FirstOpenSlot());
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
}
