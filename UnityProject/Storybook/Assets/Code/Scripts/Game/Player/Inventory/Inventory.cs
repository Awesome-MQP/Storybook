using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;
using Photon;
using UnityEngine.Assertions;

/// <summary>
/// A generic inventory system. The inventory system uses a git type sytem.
/// 
/// For non-owners we have to keep in mind that delay will be a thing. So for non owners we will be looking at an inventory that is several milliseconds behind.
/// This can allow for conflict to happen and for the inventory to go out of sync. Sending the entire inventory each change would solve the conflict problem
/// however it would also greatly increase the amount of data sent. Because of this we need someway to incremently modify the inventory across the network
/// while resolving conflicts and letting players know of conflicts so that they can be corrected. My solution is to use a system simular to how a source
/// repository works. For this to work a few things must be assumed. First the owner is always correct. Although an inventory in our game can operate as
/// a peer to peer box, we cannot handle data transfer this way. If data transfer were handled this way then conflict resolution would be impossible. Thus
/// we assume that the owner is always correct and this allows us to resolve conflicts easily. Second we use a first come first serve system relative to the owner.
/// Because of this we must assume changes made by the owner happen first, even if they didn't.
/// 
/// Changes are kept in a timeline known as commits. This timeline is divided into the locked timeline and floating timeline. The locked timeline is considered valid,
/// all commits on this timeline have been checked by the owner. The floating timeline holds commits which have not been validated by the owner. They are considered floating
/// because commits can be inserted at the end of the locked timeline when validated, thus causing the floating timeline to move around. Everytime a new commit is put on the
/// locked timeline it forces the floting timeline to go through a validation process. If a floating commit is found to be invalid then it, along with all floating commits
/// after it, will be popped and there effects will be undone.
/// 
/// For the details of networking owner clients only ever send out info on the locked timeline, not the floting. When a commit arrives the owner tries to take the commit
/// and lock it to the end of the locked timeline. If it fails then it throws it away, if it passes it is sent to all clients as being locked. Errors are not sent
/// as the only time a failure will happen is if there is a conflict with an earlier commit. On non-owner clients these commits will be recieved and locked in
/// which will cause an error with the floating timeline thus popping it as planned.
/// </summary>
/// <typeparam name="T"></typeparam>
[RequireComponent(typeof(PhotonView))]
public abstract class Inventory : PunBehaviour
{
    private static Dictionary<byte, InventoryCommit> commitTypeTable = new Dictionary<byte, InventoryCommit>
    {
        {0, new InventoryAddCommit()},
        {1, new InventoryRemoveCommit()},
        {2, new InventroyMoveCommit()}
    };

    private static Dictionary<Type, byte> commitTypeIdTable = new Dictionary<Type, byte>
    {
        {typeof(InventoryAddCommit), 0},
        {typeof(InventoryRemoveCommit), 1},
        {typeof(InventroyMoveCommit), 2}
    };

    //A list of all needed commits
    private LinkedList<InventoryCommit> m_commits = new LinkedList<InventoryCommit>();
    private LinkedListNode<InventoryCommit> m_head; 

    //An array of all items in this inventory
    private Item[] m_items;
    private Slot[] m_slots;
    private HashSet<Item> m_itemsSet;

    private short m_commitCounter;

    [SerializeField]
    [Tooltip("The size of the array to allocate in the backend for the inventory.")]
    private int m_staticSize = 20;

    [SerializeField]
    [Tooltip("The size of memory to use for the inventory. This size can grow and shrink.")]
    private int m_dynamicSize = 20;

    public int StaticSize
    {
        get { return m_staticSize; }
    }

    public int DynamicSize
    {
        get { return m_dynamicSize; }
        set { throw new NotImplementedException(); }
    }

    public Slot this[int index]
    {
        get { return m_slots[index]; }
    }

    protected override void Awake()
    {
        m_items = new Item[m_staticSize];
        m_itemsSet = new HashSet<Item>();

        m_slots = new Slot[m_staticSize];
        for (int i = 0; i < m_staticSize; i++)
        {
            m_slots[i] = new Slot(this, i);
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
    /// Delegate for when an inventory slot changes
    /// </summary>
    /// <param name="slot">The slot that was changed.</param>
   // public delegate void SlotChangedDelegate(Slot slot);

    /// <summary>
    /// Represents a slot in the iventory.
    /// This slot can be monitored for changes.
    /// </summary>
    public class Slot
    {
        private Inventory m_owner;
        private int m_index;

        //TODO: Implement in future version
        /*
        public event SlotChangedDelegate OnItemAdded;
        public event SlotChangedDelegate OnItemRemoved;
        public event SlotChangedDelegate OnItemMoved;
        */

        public Inventory Owner
        {
            get { return m_owner; }
        }

        public int Index
        {
            get { return m_index; }
        }

        public Item SlotItem
        {
            get { return m_owner.m_items[m_index]; }
        }
        
        public bool IsEmpty
        {
            get { return m_owner.m_items[m_index] == null; }
        }

        public Slot(Inventory owner, int index)
        {
            this.m_owner = owner;
            this.m_index = index;
        }

        public void Insert(Item item)
        {
            m_owner.Add(item, m_index);
        }

        public void Drop()
        {
            m_owner.Drop(m_index);
        }
    }

    #region CommitTypes

    /// <summary>
    /// A single commit in a timeline of inventory changes.
    /// </summary>
    private abstract class InventoryCommit
    {
        public short CommitNumber
        {
            get { return m_commitNumber; }
        }

        public short Index
        {
            get { return m_index; }
        }

        public Item Data
        {
            get { return m_data; }
        }

        public PhotonPlayer Owner
        {
            get { return m_owner; }
        }

        protected InventoryCommit(short commitNumber, short index, Item data)
        {
            m_commitNumber = commitNumber;
            m_index = index;
            m_data = data;
            m_owner = PhotonNetwork.player;
        }

        protected InventoryCommit()
        {
            m_owner = PhotonNetwork.player;
            m_commitNumber = -1;
            m_index = -1;
            m_data = null;
        }

        /// <summary>
        /// Applies the commit to the inventory.
        /// </summary>
        /// <param name="target">The target inventory to apply to.</param>
        /// <returns></returns>
        public abstract bool Apply(Inventory target);

        /// <summary>
        /// Reverts this commit so that the inventory no longer has the changes this
        /// </summary>
        /// <param name="target"></param>
        /// <param name="commitNode"></param>
        /// <returns></returns>
        public abstract bool Revert(Inventory target);

        /// <summary>
        /// Locks this commit into the timeline.
        /// </summary>
        /// <param name="target">The target commit.</param>
        /// <param name="commitNode">The commit node that this commit is at.</param>
        /// <returns>True when when after the lock this commit should be made the head.</returns>
        public abstract bool Lock(Inventory target, LinkedListNode<InventoryCommit> commitNode);

        public virtual bool IsDataRelevantTo(Item data)
        {
            return data == Data;
        }

        public virtual bool IsIndexRelevantTo(short index)
        {
            return index == Index;
        }

        public virtual bool Serialize(PhotonStream stream)
        {
            stream.SendNext(m_commitNumber);
            stream.SendNext(m_index);
            stream.SendNext(m_data ? m_data.photonView : null);

            return true;
        }

        public virtual bool Deserialize(PhotonStream stream, PhotonMessageInfo info)
        {
            m_commitNumber = (short) stream.ReceiveNext();
            m_index = (short) stream.ReceiveNext();

            PhotonView view = stream.ReceiveNext() as PhotonView;
            if (view)
                m_data = view.GetComponent<Item>();

            m_owner = info.sender;

            return true;
        }

        private short m_commitNumber;

        private short m_index;

        private Item m_data;

        private PhotonPlayer m_owner;
    }

    /// <summary>
    /// Adds an item to the inventory at a location with no item.
    /// </summary>
    private class InventoryAddCommit : InventoryCommit
    {
        public InventoryAddCommit(short commitNumber, short index, Item data) : base(commitNumber, index, data)
        {

        }

        public InventoryAddCommit() : base()
        {
        }

        public override bool Apply(Inventory target)
        {
            Item data = Data;
            int index = Index;
            if (!data || index < 0 || index >= target.m_items.Length || target.m_items[Index] != null || target.m_itemsSet.Contains(data))
                return false;

            target.m_items[Index] = data;
            target.m_itemsSet.Add(data);
            data.transform.parent = target.transform;
            return true;
        }

        public override bool Revert(Inventory target)
        {
            target.m_items[Index] = null;
            target.m_itemsSet.Remove(Data);
            Data.transform.parent = target.transform.parent;

            return true;
        }

        public override bool Lock(Inventory target, LinkedListNode<InventoryCommit> commitNode)
        {
            return true;
        }

        public bool CanMergeWith (InventoryAddCommit floatingCommit)
        {
            //An add done later is invalid if its on 
            return floatingCommit.Index != Index;
        }

        public bool CanMergeWith(InventoryRemoveCommit floatingCommit)
        {
            return true;
        }

        public bool CanMergeWith(InventroyMoveCommit floatingCommit)
        {
            return floatingCommit.FromIndex == Index || floatingCommit.ToIndex == Index;
        }
    }

    private class InventoryRemoveCommit : InventoryCommit
    {
        public InventoryRemoveCommit()
        {
            
        }

        public InventoryRemoveCommit(short commitNumber, short index, Item data) : base(commitNumber, index, data)
        {

        }

        public override bool Apply(Inventory target)
        {
            Item data = Data;
            int index = Index;

            if (index < 0 || index >= target.m_items.Length || !data || target.m_items[index] != data)
                return false;

            target.m_items[index] = null;
            target.m_itemsSet.Remove(data);
            data.transform.parent = target.transform.parent;

            data.DropedFromCurrent();

            return true;
        }

        public override bool Revert(Inventory target)
        {
            Item data = Data;
            target.m_items[Index] = data;
            target.m_itemsSet.Add(data);
            Data.transform.parent = target.transform;

            Data.PickedupWithInventory(target, Index);

            return true;
        }

        public override bool Lock(Inventory target, LinkedListNode<InventoryCommit> commitNode)
        {
            for (LinkedListNode<InventoryCommit> node = target.m_head; node != null;)
            {
                InventoryCommit commit = node.Value;
                if (!commit.IsDataRelevantTo(Data))
                    continue;

                //Rewind the head if we are removing the head
                if (node == target.m_head)
                    target.m_head = node.Previous;

                LinkedListNode<InventoryCommit> removingNode = node;
                node = node.Previous;

                target.m_commits.Remove(removingNode);
            }

            return false;//never keep a remove commit, it is not needed long term to rebuild the current state
        }

        public bool CanMergeWith(InventoryCommit commit)
        {
            return commit.Index == Index;
        }
    }

    private class InventroyMoveCommit : InventoryCommit
    {
        public short ToIndex
        {
            get { return m_toIndex; }
        }

        public Item ToData
        {
            get { return m_toData; }
        }

        public short FromIndex
        {
            get { return Index; }
        }

        public Item FromData
        {
            get { return Data; }
        }

        public InventroyMoveCommit()
        {
            
        }

        public InventroyMoveCommit(short commitNumber, short fromIndex, Item fromData, short toIndex, Item toData) : base(commitNumber, fromIndex, fromData)
        {
            m_toIndex = toIndex;
            m_toData = toData;
        }

        public override bool Apply(Inventory target)
        {
            short fromIndex = FromIndex;
            Item fromData = FromData;
            short toIndex = ToIndex;
            Item toData = ToData;

            //Out of bounds error
            if (fromIndex < 0 || fromIndex >= target.m_items.Length)
                return false;

            //Out of bounds error
            if (toIndex < 0 || toIndex >= target.m_items.Length)
                return false;

            //Validate data, error out if invalid
            if (fromData != target.m_items[fromIndex] || toData != target.m_items[toIndex])
                return false;

            target.m_items[fromIndex] = toData;
            target.m_items[toIndex] = fromData;

            if(toData)
                toData.MovedInInventory(fromIndex);
            if(fromData)
                fromData.MovedInInventory(toIndex);

            return true;
        }

        public override bool Revert(Inventory target)
        {
            Item fromData = FromData;
            Item toData = ToData;

            target.m_items[FromIndex] = fromData;
            target.m_items[ToIndex] = toData;

            if(fromData)
                fromData.MovedInInventory(FromIndex);
            if(toData)
                toData.MovedInInventory(ToIndex);

            return true;
        }

        public override bool IsDataRelevantTo(Item data)
        {
            return data == FromData || data == ToData;
        }

        public override bool IsIndexRelevantTo(short index)
        {
            return index == FromIndex || index == ToIndex;
        }

        public override bool Lock(Inventory target, LinkedListNode<InventoryCommit> commitNode)
        {
            //TODO: Delta compress loops
            return true;
        }

        public override bool Serialize(PhotonStream stream)
        {
            stream.SendNext(m_toIndex);
            stream.SendNext(m_toData ? m_toData.photonView : null);

            return base.Serialize(stream);
        }

        public override bool Deserialize(PhotonStream stream, PhotonMessageInfo info)
        {
            m_toIndex = (short) stream.ReceiveNext();

            PhotonView view = stream.ReceiveNext() as PhotonView;
            if (view)
                m_toData = view.GetComponent<Item>();

            return base.Deserialize(stream, info);
        }

        private short m_toIndex;
        private Item m_toData;
    }

    #endregion

    public override void OnSerializeReliable(PhotonStream stream, PhotonMessageInfo info, bool isInit)
    {
        base.OnSerializeReliable(stream, info, isInit);
    }

    public override void OnDeserializeReliable(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnDeserializeReliable(stream, info);
    }

    /// <summary>
    /// Adds an item to the inventory.
    /// </summary>
    /// <param name="item">The item to add to the inventory.</param>
    /// <param name="index">The index to put the item in.</param>
    /// <returns>True if the item could be added, otherwise false.</returns>
    public virtual bool Add(Item item, int index)
    {
        if (index < 0 || index >= m_items.Length || m_items[index] != null || !CanAddItem(item, index))
            return false;

        InventoryAddCommit addCommit = new InventoryAddCommit(m_commitCounter++, (short)index, item);
        bool wasAdded = _PushCommit(addCommit);
        if (wasAdded)
        {
            item.PickedupWithInventory(this, index);
        }
        return wasAdded;
    }

    /// <summary>
    /// Drops an item from the inventory.
    /// </summary>
    /// <param name="index">The index to drop from.</param>
    /// <returns>True if the item was dropped, otherwise false.</returns>
    public virtual bool Drop(int index)
    {
        if (index < 0 || index >= m_items.Length)
            return false;

        Item item = m_items[index];
        if (item == null || !CanRemoveItem(index))
            return false;

        InventoryRemoveCommit removeCommit = new InventoryRemoveCommit(m_commitCounter++, (short) index, item);
        return _PushCommit(removeCommit);
    }

    public virtual bool Move(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= m_items.Length || toIndex < 0 || toIndex >= m_items.Length)
            return false;

        Item fromItem = m_items[fromIndex];
        Item toItem = m_items[toIndex];

        if (fromItem == null && toItem == null)
            return true;//This is technically a valid move, just no data changes so there is no need to update anything.

        InventroyMoveCommit moveCommit = new InventroyMoveCommit(m_commitCounter++, (short) fromIndex, fromItem,
            (short) toIndex, toItem);

        return _PushCommit(moveCommit);
    }

    public bool ContainsItem(Item item)
    {
        return m_itemsSet.Contains(item);
    }

    /// <summary>
    /// Checks to see if all of the slots in the inventory are filled with an item
    /// </summary>
    /// <returns>True if the inventory is full, false otherwise</returns>
    public bool IsInventoryFull()
    {
        foreach (Slot slot in m_slots)
        {
            if (slot.IsEmpty)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Finds the first empty slot in the inventory
    /// </summary>
    /// <returns>The index of the first empty slot</returns>
    public int FirstOpenSlot()
    {
        int i = 0;
        foreach(Slot slot in m_slots)
        {
            if (slot.IsEmpty)
            {
                return i;
            }
            i++;
        }
        return -1;
    }

    /// <summary>
    /// Checks to see if the local player can add the item to this inventory.
    /// </summary>
    /// <param name="item">The item to try and add.</param>
    /// <param name="index">The index in the inventory to add to.</param>
    /// <returns>True if the item can be</returns>
    protected abstract bool CanAddItem(Item item, int index);

    /// <summary>
    /// Checks to see if the local player can remove the item from this inventory.
    /// </summary>
    /// <param name="index">The index of the item to remove.</param>
    /// <returns>True if the item could be removed otherwise false.</returns>
    protected abstract bool CanRemoveItem(int index);

    protected abstract bool CanMoveItem(int fromIndex, int toIndex);

    [PunRPC]
    public void _RequestCommit(PhotonStream data, PhotonMessageInfo messageInfo)
    {
        if (!IsMine)
            return;

        InventoryCommit commit = _DeserializeCommitFromStream(data, messageInfo);
        if (_InsertOnLocked(commit))
        {
            photonView.RPC("_ApproveCommit", messageInfo.sender, commit.CommitNumber);
            photonView.RpcOthersExcept("_RecieveCommit", messageInfo.sender, data);
        }
        else
        {
            photonView.RPC("_RejectCommit", messageInfo.sender, commit.CommitNumber);
        }
    }

    [PunRPC]
    public void _RecieveCommit(PhotonStream data, PhotonMessageInfo messageInfo)
    {
        if (messageInfo.sender != Owner)
            return;

        InventoryCommit newCommit = _DeserializeCommitFromStream(data, messageInfo);

        _InsertOnLocked(newCommit);
    }
    
    [PunRPC]
    public void _ApproveCommit(short commitId, PhotonMessageInfo messageInfo)
    {
        if (messageInfo.sender != Owner)
            return;

        _MoveToLocked(commitId);
    }

    [PunRPC]
    public void _RejectCommit(short commitId, PhotonMessageInfo messageInfo)
    {
        if (messageInfo.sender != Owner)
            return;

        LinkedListNode<InventoryCommit> node = _GetCommitFromFloating(commitId);
        if(node != null)
            _RemoveFromFloating(node.Value);
    }

    private bool _PushCommit(InventoryCommit commit)
    {
        bool isMine = IsMine;
        if (isMine)
        {
            return _PushCommitAsOwner(commit);
        }
        else
        {
            return _PushCommitAsPeer(commit);
        }
    }

    /// <summary>
    /// Pushes the commit onto the locked timeline and to all other peers.
    /// </summary>
    /// <param name="commit">The commit to push.</param>
    /// <returns>True if the commit could be pushed, false otherwise.</returns>
    private bool _PushCommitAsOwner(InventoryCommit commit)
    {
        Assert.IsTrue(IsMine);

        if (_InsertOnLocked(commit))
        {
            PhotonStream data = new PhotonStream(true, null);
            _SerializeCommitToStream(data, commit);

            photonView.RPC("_RecieveCommit", PhotonTargets.Others, data);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Pushes the commit onto the floating timeline and to the owner.
    /// </summary>
    /// <param name="commit">The commit to push.</param>
    /// <returns>True if the commit could be pushed, false otherwise.</returns>
    private bool _PushCommitAsPeer(InventoryCommit commit)
    {
        Assert.IsFalse(IsMine);

        if (!_CommitOnFloating(commit))
            return false;

        PhotonStream data = new PhotonStream(false, null);
        _SerializeCommitToStream(data, commit);

        photonView.RpcOwner("_RequestCommit", data);

        return true;
    }

    private bool _CommitOnFloating(InventoryCommit commit)
    {
        if (commit.Apply(this))
            return false;

        m_commits.AddLast(commit);

        return true;
    }

    //Inserts a commit between float and locked without moving it from float to locked first

    private bool _InsertOnLocked(InventoryCommit commit)
    {
        commit.Data.PickedupWithInventory(this, commit.Index);

        //Revert back to the head
        LinkedListNode<InventoryCommit> node;
        for (node = m_commits.Last;
            node != null && node != m_head;
            node = node.Previous)
        {
            node.Value.Revert(this);
        }

        //Apply the new commit onto the head.
        if (commit.Apply(this))
        {
            LinkedListNode<InventoryCommit> newHead;
            if (m_head != null)
                newHead = m_commits.AddAfter(m_head, commit);
            else
                newHead = m_commits.AddFirst(commit);

            if (commit.Lock(this, newHead))
            {
                m_head = newHead;
            }
            else
            {
                m_commits.Remove(newHead);
            }
        }
        else
        {
            commit.Revert(this);
            return false;
        }

        //Push floating commits back onto inventory.
        for (node = _GetFloatingTale(); node != null;)
        {
            InventoryCommit floatingCommit = node.Value;
            if (!floatingCommit.Apply(this))
            {
                LinkedListNode<InventoryCommit> removingNode = node;
                node = node.Next;
                m_commits.Remove(removingNode);
            }
            else
            {
                node = node.Next;
            }
        }

        return true;
    }

    private bool _RemoveFromFloating(InventoryCommit commit)
    {
        LinkedListNode<InventoryCommit> commitNode = _GetCommitFromFloating(commit.CommitNumber);

        if (commitNode == null)
            return false;

        for (LinkedListNode<InventoryCommit> node = m_commits.Last; node != commitNode; node = node.Previous)
        {
            node.Value.Revert(this);
        }

        commitNode.Value.Revert(this);

        LinkedListNode<InventoryCommit> removingNode = commitNode;
        commitNode = commitNode.Next;
        m_commits.Remove(removingNode);

        while (commitNode != null)
        {
            InventoryCommit floatingCommit = commitNode.Value;
            if (!floatingCommit.Apply(this))
            {
                removingNode = commitNode;
                commitNode = commitNode.Next;
                m_commits.Remove(removingNode);
            }
            else
            {
                commitNode = commitNode.Next;
            }
        }

        return true;
    }

    [Obsolete]
    private bool _CheckMergeWithHead(InventoryCommit floatingCommit)
    {
        if (m_head == null)
            return true;

        InventoryCommit lockedCommit = m_head.Value;

        MethodInfo method = m_head.Value.GetType().GetMethod("CanMergeWith", new Type[] { floatingCommit.GetType() });
        Assert.IsNotNull(method);

        bool canMerge = (bool)method.Invoke(m_head.Value, new object[] { floatingCommit });
        return canMerge;
    }

    //Commit a commit on floating to the locked timeline

    private bool _MoveToLocked(InventoryCommit commit)
    {
        LinkedListNode<InventoryCommit> floatingNodeTail = _GetFloatingTale();

        if (floatingNodeTail == null || floatingNodeTail.Value != commit)
        {
            m_commits.Remove(floatingNodeTail);
            return false;
        }

        if (!commit.Lock(this, floatingNodeTail))
            return false;

        m_head = floatingNodeTail;

        return true;
    }

    private bool _MoveToLocked(short commitId)
    {
        LinkedListNode<InventoryCommit> floatingNodeTail = _GetFloatingTale();

        if (floatingNodeTail == null || floatingNodeTail.Value.CommitNumber != commitId)
            return false;

        if (!floatingNodeTail.Value.Lock(this, floatingNodeTail))
        {
            m_commits.Remove(floatingNodeTail);
            return false;
        }

        m_head = floatingNodeTail;

        return true;
    }

    private bool _IsFloating(InventoryCommit commit)
    {
        for (LinkedListNode<InventoryCommit> node = _GetFloatingTale(); node != null; node = node.Next)
        {
            if (node.Value == commit)
                return true;
        }

        return false;
    }

    private bool _IsLocked(InventoryCommit commit)
    {
        if (m_head == null)
            return false;

        foreach (InventoryCommit inventoryCommit in m_commits)
        {
            if (commit == inventoryCommit)
                return true;
            if (inventoryCommit == m_head.Value)
                return false;
        }

        return false;
    }

    private LinkedListNode<InventoryCommit> _GetFloatingTale()
    {
        return m_head == null ? m_commits.First : m_head.Next;
    }

    private LinkedListNode<InventoryCommit> _GetCommitFromFloating(short commitId)
    {
        LinkedListNode<InventoryCommit> floatingTale = _GetFloatingTale();

        for (LinkedListNode<InventoryCommit> node = floatingTale; node != null; node = node.Next)
        {
            if (node.Value.CommitNumber == commitId)
                return node;
        }

        return null;
    }

    private LinkedListNode<InventoryCommit> _GetCommitFromLocked(short commitId, PhotonPlayer owner)
    {
        for (LinkedListNode<InventoryCommit> node = m_head; node != null; node = node.Next)
        {
            if (node.Value.CommitNumber == commitId && node.Value.Owner == owner)
                return node;
        }

        return null;
    }

    /// <summary>
    /// Re-builds the array with the items that are children to this object
    /// </summary>
    private void _CacheCurrentInventory()
    {
        m_items = GetComponentsInChildren<Item>().ToArray();
        m_commits.Clear();
        m_head = null;

        Debug.LogWarning("Inventory cache cleared, this may cause sync errors.");
    }

    private void _SerializeCommitToStream(PhotonStream stream, InventoryCommit commit)
    {
        byte commitTypeId = commitTypeIdTable[commit.GetType()];

        stream.SendNext(commitTypeId);
        commit.Serialize(stream);
    }

    private InventoryCommit _DeserializeCommitFromStream(PhotonStream stream, PhotonMessageInfo info)
    {
        byte commitTypeId = (byte) stream.ReceiveNext();

        InventoryCommit commit = commitTypeTable[commitTypeId];
        commit.Deserialize(stream, info);

        return commit;
    }
}