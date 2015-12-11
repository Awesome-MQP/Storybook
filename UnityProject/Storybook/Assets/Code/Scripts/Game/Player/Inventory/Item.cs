using System;
using UnityEngine;
using System.Collections;
using Photon;

[RequireComponent(typeof(PhotonView))]
public abstract class Item : PunBehaviour
{
    private Inventory m_inventory;
    private int m_index;

    /// <summary>
    /// The index this item is at in the inventory.
    /// </summary>
    public int Index
    {
        get { return m_index; }
    }

    public Inventory Owner
    {
        get { return m_inventory; }
    }

    public Inventory.Slot Slot
    {
        get
        {
            if (!m_inventory)
                return null;

            return m_inventory[m_index];
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        PhotonView view = photonView;
        if (!view.ObservedComponents.Contains(this))
            view.ObservedComponents.Add(this);
    }
#endif

    /// <summary>
    /// Called when an inventory picks up this item.
    /// </summary>
    /// <param name="inventory">The inventory that picked up this item.</param>
    /// <param name="index">The index in the inventory where we were placed at.</param>
    public void PickedupWithInventory(Inventory inventory, int index)
    {
        if(m_inventory)
            throw new InvalidOperationException("Cannot be picked up by two inventories at once.");

        m_inventory = inventory;

        Renderer renderComponent = GetComponent<Renderer>();
        if (renderComponent != null)
            renderComponent.enabled = false;

        OnPickup();
    }

    /// <summary>
    /// Called when an inventory moves an item to a new index.
    /// </summary>
    /// <param name="newIndex">The new index of the item.</param>
    public void MovedInInventory(int newIndex)
    {
        if (!m_inventory)
            throw new InvalidOperationException("Cannot move item that does not belong to an inventory.");

        m_index = newIndex;

        OnMoved();
    }

    /// <summary>
    /// Called when dropped from the current inventory.
    /// </summary>
    public void DropedFromCurrent()
    {
        m_inventory = null;
        m_index = -1;

        Renderer renderComponent = GetComponent<Renderer>();
        if (renderComponent != null)
            renderComponent.enabled = true;

        OnDrop();
    }

    protected abstract void OnPickup();

    protected abstract void OnDrop();

    protected abstract void OnMoved();
}
