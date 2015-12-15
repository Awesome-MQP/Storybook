using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : Inventory {

    [SerializeField]
    private Page m_testPage;

    void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            GameObject pageObject = PhotonNetwork.Instantiate(m_testPage.name, Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(pageObject.GetComponent<PhotonView>());
            Page testPage = pageObject.GetComponent<Page>();
            bool wasItemAdded = Add(testPage, 0);
            Debug.Log(ContainsItem(testPage));
            Debug.Log(this[0].SlotItem.Owner);
            Move(0, 2);
            Debug.Log(this[2].SlotItem);
            Drop(0);
            Debug.Log(ContainsItem(testPage));
        }
    }

    protected override bool CanAddItem(Item item, int index)
    {
        return true;
    }

    protected override bool CanRemoveItem(int index)
    {
        return true;
    }

    protected override bool CanMoveItem(int fromIndex, int toIndex)
    {
        return true;
    }
}
