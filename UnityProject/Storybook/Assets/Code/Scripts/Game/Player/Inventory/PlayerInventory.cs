using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : Inventory {

    private int m_playerId;

    public override void OnStartOwner(bool wasSpawn)
    {
        DungeonMaster dm = DungeonMaster.Instance;
        BaseStorybookGame gameManager = GameManager.GetInstance<BaseStorybookGame>();
        for (int i = 0; i < gameManager.StartingPages; i++)
        {
            Page basicPage = dm.GetBasicPage();
            Add(basicPage, i);
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
