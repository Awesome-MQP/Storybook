using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : Inventory {

    private int m_playerId;

    void OnStartOwner()
    {
        DungeonMaster dm = DungeonMaster.Instance;
        for (int i = 0; i < 21; i++)
        {
            Page basicPage = dm.GetBasicPage();
            Add(basicPage, i);
        }
    }

    void Start()
    {
        /*
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
        */
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

    public void SortInventory(int startingIndex, int endIndex)
    {
        List<int> horrorPageIndices = new List<int>();
        List<int> fantasyPageIndices = new List<int>();
        List<int> comicBookPageIndices = new List<int>();
        List<int> scifiPageIndices = new List<int>();
        int currentIndex = startingIndex;

        currentIndex = _SortByGenre(Genre.Horror, currentIndex, startingIndex, endIndex);
        currentIndex = _SortByGenre(Genre.Fantasy, currentIndex, startingIndex, endIndex);
        currentIndex = _SortByGenre(Genre.SciFi, currentIndex, startingIndex, endIndex);
        currentIndex = _SortByGenre(Genre.GraphicNovel, currentIndex, startingIndex, endIndex);
    }

    private int _SortByGenre(Genre genre, int currentIndex, int startingIndex, int endIndex)
    {
        List<int> genreIndices = _GetIndicesByGenre(genre, startingIndex, endIndex);
        for (int i = 0; i < genreIndices.Count; i++)
        {
            int pageIndex = genreIndices[i];
            if (pageIndex != currentIndex)
            {
                Move(pageIndex, currentIndex);
            }
            currentIndex++;
        }
        return currentIndex;
    }

    private List<int> _GetIndicesByGenre(Genre genre, int startingIndex, int endIndex)
    {
        List<int> indexList = new List<int>();
        for (int i = startingIndex; i < endIndex; i++)
        {
            Page currentPage = (Page)this[i].SlotItem;
            if (currentPage != null)
            {
                if (currentPage.PageGenre == genre)
                {
                    indexList.Add(i);
                }
            }
        }
        return indexList;
    }

    public int PlayerId
    {
        get { return m_playerId; }
        set { m_playerId = value; }
    }
}
