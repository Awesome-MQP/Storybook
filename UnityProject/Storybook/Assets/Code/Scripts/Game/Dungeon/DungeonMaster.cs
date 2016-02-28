using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Random = UnityEngine.Random;

// Dungeon Master class is the handler for all statistics, as well as handling
// the location of all room prefabs.
[Serializable]
public class DungeonMaster : MonoBehaviour
{
    // List of room prefabs in the Dungeon/Rooms folder
    // These are all the rooms that can be spawned
    [SerializeField]
    private RoomObject[] m_rooms;

    [SerializeField]
    private float m_pageSameLevelProbability = 0.7f;

    [SerializeField]
    private float m_pageLevelPlus1Probability = 0.2f;

    [SerializeField]
    private float m_pageLevelPlus2Probability = 0.05f;

    [SerializeField]
    private float m_levelCurveValue = 0.025f;

    [SerializeField]
    private float m_pageSameGenreProbability = 0.75f;

    [SerializeField]
    private float m_rarePageProbability = 0.05f;

    [SerializeField]
    private float m_rarePageLevelIncrease = 0.025f;

    [SerializeField]
    private Page m_pagePrefab;

    [SerializeField]
    private float m_isPageAttackProbability = 0.4f;

    [SerializeField]
    private float m_isPageSupportProbability = 0.3f;

    [SerializeField]
    private float m_isPageStatusProbability = 0.3f;

    [SerializeField]
    private int m_hpIncreasePerLevel = 3;

    [SerializeField]
    private int m_attackIncreasePerLevel = 1;

    [SerializeField]
    private int m_defenseIncreasePerLevel = 1;

    [SerializeField]
    private int m_speedIncreasePerLevel = 1;

    [SerializeField]
    private PageMove m_pageAttack;

    [SerializeField]
    private PageMove m_pageHPBoost;

    [SerializeField]
    private PageMove m_pageAttackBoost;

    [SerializeField]
    private PageMove m_pageDefenseBoost;

    [SerializeField]
    private PageMove m_pageSpeedBoost;

    [SerializeField]
    private int m_startingPageCount = 21;

    private static DungeonMaster s_instance;

    public static DungeonMaster Instance
    {
        get { return s_instance; }
    }

    public void Awake()
    {
        m_rooms = Resources.LoadAll<RoomObject>("RoomPrefabs");
        s_instance = this;
    }

    /// <summary>
    /// Gets a room prefab to place in the world based on certain input criteria.
    /// </summary>
    /// <param name="genre">The specified Genre of the room to search for.</param>
    /// <param name="features">The specified features of the room to search for.</param>
    /// <returns>The room if a match is found, or null if no match is found.</returns>
    RoomObject GetRoomPrefab(Genre genre, params string[] features)
    {
        // List of "good" rooms - ones that match the criteria passed in.
        List<RoomObject> goodRooms = new List<RoomObject>(m_rooms.Length);

        // Check each room to see if there is a match
        foreach (RoomObject r in m_rooms)
        {
            if (true/*r.RoomGenre != genre*/)
            {
                continue;
            }
            else if (r.ContainsFeatures(features))
            {
                continue;
            }
            else
            {
                // Room is good! Now add it to the candidates for rooms to place.
                goodRooms.Add(r);
            }
        }
        // How many matches did we get?
        if (goodRooms.Count == 0)
        {
            return null;
        }

        // Now that we have found all potential matching rooms, choose one to place.
        int roomChooser = Random.Range(0, goodRooms.Count);
        RoomObject roomToBuild = goodRooms[roomChooser];

        return roomToBuild;
    }

    /// <summary>
    /// Generates a basic page: level 1, non-rare, attack for page type and having a random genre
    /// </summary>
    /// <returns>The page object generated</returns>
    public Page GetBasicPage()
    {
        Genre pageGenre = _getRandomGenre();
        Page constructedPage = ConstructPage(1, pageGenre, true);
        return constructedPage;
    }

    public Page ConstructPageFromData(PageData data)
    {
        Page page = _spawnPageOnNetwork(data.PageGenre, data.PageLevel);
        page.Rarity = data.IsRare;
        page.PageType = data.PageMoveType;
        PageMove move = _getMovePrefab(page.PageType, page.PageGenre);
        page.PlayerCombatMove = move;
        move.transform.SetParent(page.transform, false);
        move.MoveLevel = page.PageLevel;
        move.MoveRarity = page.Rarity;
        move.MoveGenre = data.PageGenre;
        move.PageGenre = data.PageGenre;
        if (page.Rarity)
        {
            move.SetNumberOfTargets(4);
        }
        else
        {
            move.SetNumberOfTargets(1);
        }
        return page;
    }

    public Page ConstructBoostPage(int pageLevel, Genre pageGenre, bool isBasicPage)
    {
        Page page = _spawnPageOnNetwork(pageGenre, pageLevel);

        if (!isBasicPage)
        {
            page.Rarity = _getIsPageRare(pageLevel);
        }
        else
        {
            page.Rarity = false;
        }

        page.PageType = MoveType.Boost;
        PageMove move = _getMovePrefab(page.PageType, pageGenre);
        page.PlayerCombatMove = move;
        move.transform.SetParent(page.transform, false);
        move.MoveLevel = pageLevel;
        move.MoveRarity = page.Rarity;
        move.MoveGenre = pageGenre;
        move.PageGenre = pageGenre;
        if (page.Rarity)
        {
            move.SetNumberOfTargets(4);
        }
        else
        {
            move.SetNumberOfTargets(1);
        }
        return page;
    }

    public Page ConstructPage(int pageLevel, Genre pageGenre, bool isBasicPage)
    {
        Page page = _spawnPageOnNetwork(pageGenre, pageLevel);

        if (!isBasicPage)
        {
            page.Rarity = _getIsPageRare(pageLevel);
        }
        else
        {
            page.Rarity = false;
        }

        page.PageType = _getPageMoveType();
        PageMove move = _getMovePrefab(page.PageType, pageGenre);
        page.PlayerCombatMove = move;
        move.transform.SetParent(page.transform, false);
        move.MoveLevel = pageLevel;
        move.MoveRarity = page.Rarity;
        move.MoveGenre = pageGenre;
        move.PageGenre = pageGenre;
        if (page.Rarity)
        {
            move.SetNumberOfTargets(4);
        }
        else
        {
            move.SetNumberOfTargets(1);
        }
        return page;
    }

    private PageMove _getMovePrefab(MoveType pageType, Genre pageGenre)
    {
        string prefabName = "PlayerMoves/";
        if (pageType == MoveType.Attack)
        {
            prefabName += m_pageAttack.name;
            Debug.Log("Prefab name = " + prefabName);
        }
        else if (pageType == MoveType.Boost)
        {
            switch (pageGenre)
            {
                case Genre.SciFi:
                    prefabName += "SciFi/";
                    prefabName += m_pageDefenseBoost.name;
                    break;
                case Genre.Fantasy:
                    prefabName += "Fantasy/";
                    prefabName += m_pageSpeedBoost.name;
                    break;
                case Genre.Horror:
                    prefabName += "Horror/";
                    prefabName += m_pageHPBoost.name;
                    break;
                case Genre.GraphicNovel:
                    prefabName += "ComicBook/";
                    prefabName += m_pageAttackBoost.name;
                    break;
            }
        }

        GameObject pageMoveObject = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity, 0);
        PhotonNetwork.Spawn(pageMoveObject.GetPhotonView());
        return pageMoveObject.GetComponent<PageMove>();
    }

    /// <summary>
    /// Function for getting the random page drop from clearing a combat against regular enemies
    /// TODO: Return a page item once that is created
    /// </summary>
    /// <param name="roomGenre">The genre of the room that was cleared</param>
    /// <param name="roomLevel">The level of the room that was cleared</param>
    public Page GetPageDropFromCombat(Genre roomGenre, int roomLevel)
    {
        Genre pageGenre = _getPageGenre(roomGenre);
        float isPageSameLevel = Random.Range(0.00f, 1.00f);
        if (isPageSameLevel <= (m_pageSameLevelProbability - (m_levelCurveValue * roomLevel)))
        {
            Page page = ConstructPage(roomLevel, pageGenre, false);
            return page;
        }

        float isPageLevelPlus1 = Random.Range(0, 1);
        if (isPageLevelPlus1 <= (m_pageLevelPlus1Probability + (m_levelCurveValue * roomLevel)))
        {
            Page page = ConstructPage(roomLevel + 1, pageGenre, false);
            return page;
        }

        float isPageLevelPlus2 = Random.Range(0, 1);
        if (isPageLevelPlus2 <= (m_pageLevelPlus2Probability + (m_levelCurveValue * roomLevel)))
        {
            Page page = ConstructPage(roomLevel + 2, pageGenre, false);
            return page;
        }

        // If it does not create a page, return null
        return null;
    }

    /// <summary>
    /// Randomly selects a page drop for a boss
    /// </summary>
    /// <param name="bossGenre"></param>
    public void GetPageDropFromBoss(Genre bossGenre, int bossLevel)
    {
        Genre pageGenre = _getPageGenre(bossGenre);
    }

    /// <summary>
    /// Randomly selects a genre for a page drop based on the 'pageSameGenreProbability' value
    /// Should be a higher chance of creating a page of the favored genre
    /// If it is not the favored genre, all other genres have an equal chance of being chosen
    /// </summary>
    /// <param name="favoredGenre">The genre that will be favored and have a higher chance of being selected</param>
    /// <returns>The genre that has been chosen for the page drop</returns>
    private Genre _getPageGenre(Genre favoredGenre)
    {
        Genre pageGenre = favoredGenre;
        float genreRandomizer = Random.Range(0.00f, 1.00f);
        if (genreRandomizer >= m_pageSameGenreProbability)
        {
            Genre[] allGenresArray = { Genre.Fantasy, Genre.GraphicNovel, Genre.Horror, Genre.SciFi };
            List<Genre> allGenres = new List<Genre>(allGenresArray);
            allGenres.Remove(favoredGenre);
            int genreIndex = Random.Range(0, allGenres.Count);
            pageGenre = allGenres[genreIndex];
        }

        return pageGenre;
    }

    private Genre _getRandomGenre()
    {
        Genre[] allGenresArray = { Genre.Fantasy, Genre.GraphicNovel, Genre.Horror, Genre.SciFi };
        int genreIndex = Random.Range(0, allGenresArray.Length);
        return allGenresArray[genreIndex];
    }

    private Page _spawnPageOnNetwork(Genre pageGenre, int pageLevel)
    {
        GameObject pageObject = PhotonNetwork.Instantiate(m_pagePrefab.name, Vector3.zero, Quaternion.identity, 0);
        PhotonNetwork.Spawn(pageObject.GetComponent<PhotonView>());
        Page page = pageObject.GetComponent<Page>();
        page.PageLevel = pageLevel;
        page.PageGenre = pageGenre;
        return page;
    }

    private bool _getIsPageRare(int pageLevel)
    {
        float isPageRareValue = Random.Range(0.00f, 1.00f);
        float rareProbability = m_rarePageProbability + (m_rarePageLevelIncrease * pageLevel);
        bool isPageRare = false;
        if (isPageRareValue <= rareProbability)
        {
            isPageRare = true;
        }
        return isPageRare;
    }

    private MoveType _getPageMoveType()
    {
        MoveType pageMoveType;
        float pageMoveTypeValue = Random.Range(0.00f, 1.00f);
        if (pageMoveTypeValue <= m_isPageAttackProbability)
        {
            pageMoveType = MoveType.Attack;
        }
        else if (pageMoveTypeValue <= m_isPageStatusProbability + m_isPageAttackProbability)
        {
            pageMoveType = MoveType.Status;
        }
        else
        {
            pageMoveType = MoveType.Boost;
        }

        return pageMoveType;
    }

    public void InitializeInventory(Inventory inventoryToInitialize)
    {
        for (int i = 0; i < m_startingPageCount; i++)
        {
            Page basicPage;
            if (m_startingPageCount - i > 3)
            {
                basicPage = GetBasicPage();
            }
            else
            {
                basicPage = ConstructBoostPage(1, Genre.Horror, true);
            }
            inventoryToInitialize.Add(basicPage, i);
        }
        if (inventoryToInitialize is PlayerInventory)
        {
            PlayerInventory playerInv = (PlayerInventory)inventoryToInitialize;
            playerInv.SortInventory(0, 20);
            playerInv.SortInventory(20, playerInv.DynamicSize);
        }
    }

    public PageData GetShopPage(PageData shopRoomPageData)
    {
        Genre pageGenre = _getPageGenre(shopRoomPageData.PageGenre);
        float levelRandomValue = Random.Range(0.00f, 1.00f);
        int pageLevel;
        if (levelRandomValue < 0.75)
        {
            pageLevel = shopRoomPageData.PageLevel + 2;
        }
        else
        {
            pageLevel = shopRoomPageData.PageLevel + 3;
        }
        bool isRare = _getIsPageRare(shopRoomPageData.PageLevel);
        MoveType pageType = _getPageMoveType();
        return new PageData(pageLevel, pageGenre, pageType, isRare);
    }

    public void ScalePawnByLevel(CombatPawn pawnToScale, int roomLevel)
    {
        // Increase HP based on level
        int hpIncrease = m_hpIncreasePerLevel * roomLevel;
        int newMax = hpIncrease + (int)pawnToScale.MaxHealth;
        pawnToScale.SetMaxHealth(newMax);
        pawnToScale.Health = newMax;

        // Increase attack based on level
        int attackIncrease = m_attackIncreasePerLevel * roomLevel;
        int newAttack = attackIncrease + (int)pawnToScale.Attack;
        pawnToScale.Attack = newAttack;

        // Increase defense based on level
        int defenseIncrease = m_defenseIncreasePerLevel * roomLevel;
        int newDefense = defenseIncrease + (int)pawnToScale.Defense;
        pawnToScale.Defense = newDefense;

        // Increase speed based on level
        int speedIncrease = m_speedIncreasePerLevel * roomLevel;
        int newSpeed = speedIncrease + (int)pawnToScale.Speed;
        pawnToScale.Speed = newSpeed;
    }
}
