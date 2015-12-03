using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Dungeon Master class is the handler for all statistics, as well as handling
// the location of all room prefabs.
public class DungeonMaster : MonoBehaviour{
    
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
    private float m_pageSameGenreProbability = 0.75f;

	// When the DungeonMaster is spawned in the world, have it immediately get all the room prefabs.
	void Awake () {
        m_rooms = Resources.LoadAll<RoomObject>("RoomPrefabs");
    }

    /// <summary>
    /// Gets a room prefab to place in the world based on certain input criteria.
    /// </summary>
    /// <param name="size">The specified size of the room to search for.</param>
    /// <param name="genre">The specified Genre of the room to search for.</param>
    /// <param name="feature">The specified Feature of the room to search for.</param>
    /// <returns>The room if a match is found, or null if no match is found.</returns>
    RoomObject GetRoomPrefab(int size, Genre genre, string feature)
    {
        // List of "good" rooms - ones that match the criteria passed in.
        List<RoomObject> goodRooms = new List<RoomObject>();

        // Check each room to see if there is a match
        foreach(RoomObject r in m_rooms)
        {
            if(r.RoomSize != size)
            {
                continue;
            }
            else if(r.RoomGenre != genre)
            {
                continue;
            }
            else if(r.RoomFeature != feature)
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
        if(goodRooms.Count == 0)
        {
            return null;
        }

        // Now that we have found all potential matching rooms, choose one to place.
        int roomChooser = Random.Range(0, goodRooms.Count);
        RoomObject roomToBuild = goodRooms[roomChooser];

        // TODO: Send to WorldManager.placeRoom()?
        return roomToBuild;
    }

    /// <summary>
    /// Tests the GetRoomPrefab function.
    /// </summary>
    void TestGetRoomPrefab()
    {
        GetRoomPrefab(1, Genre.SciFi, "Curse");
        GetRoomPrefab(4, Genre.Horror, "Shop");
        GetRoomPrefab(2, Genre.Fantasy, "Treasure");
    }

    /// <summary>
    /// Function for getting the random page drop from clearing a combat against regular enemies
    /// TODO: Return a page item once that is created
    /// </summary>
    /// <param name="roomGenre">The genre of the room that was cleared</param>
    /// <param name="roomLevel">The level of the room that was cleared</param>
    public void GetPageDropFromCombat(Genre roomGenre, int roomLevel)
    {
        Genre pageGenre = _getPageGenre(roomGenre);
        float isPageSameLevel = Random.Range(0.00f, 1.00f);
        if (isPageSameLevel <= m_pageSameLevelProbability)
        {
            //TODO: Create a page of the same level and return it
            return;
        }

        float isPageLevelPlus1 = Random.Range(0, 1);
        if (isPageLevelPlus1 <= m_pageLevelPlus1Probability)
        {
            //TODO: Create a page with one level higher than the given room level
            return;
        }

        float isPageLevelPlus2 = Random.Range(0, 1);
        if (isPageLevelPlus2 <= m_pageLevelPlus2Probability)
        {
            //TODO: Create a page with two levels higher than the given room level
            return;
        }
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
}
