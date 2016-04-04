using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class ExitRoom : RoomObject
{
    [SerializeField]
    private AudioClip m_roomMusic;

    private MusicManager m_musicManager;

    [SerializeField]
    private List<Transform> m_enemyPosList = new List<Transform>();

    private List<GameObject> m_enemyWorldPawns = new List<GameObject>();

    private EnemyTeam m_roomEnemies;
    private string m_roomEnemiesPrefabLoc;

    private bool m_wonCombat = false;

    // Use this for initialization
    protected override void Awake ()
    {
        m_musicManager = FindObjectOfType<MusicManager>();
        base.Awake();
	}

    // On entering the room, do nothing since there is nothing special in this room.
    protected override void OnRoomEnter(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            return;
        
        if (!m_wonCombat)
        {
            _chooseEnemyTeam();

            int i = 0;

            foreach (CombatPawn pawn in m_roomEnemies.PawnsToSpawn)
            {
                Vector3 currentEnemyPos = m_enemyPosList[i].position;
                Quaternion currentEnemyRot = m_enemyPosList[i].rotation;
                GameObject pawnGameObject = PhotonNetwork.Instantiate("Enemies/EnemyTypes/" + pawn.PawnGenre + "/Bosses/" + pawn.name, currentEnemyPos, currentEnemyRot, 0);
                pawnGameObject.GetComponent<CombatPawn>().enabled = false;
                PhotonNetwork.Spawn(pawnGameObject.GetComponent<PhotonView>());
                m_enemyWorldPawns.Add(pawnGameObject);
                i++;
            }
        }

        m_musicManager.Fade(m_musicTracks[0], 5, true);
        //m_musicManager.MusicTracks = m_musicTracks;
        
        EventDispatcher.GetDispatcher<MusicEventDispatcher>().OnRoomMusicChange(RoomPageData.PageGenre);
    }

    // What do we do when all players reach the center of the room?
    // Most likely nothing, but that may change.
    protected override IEnumerable OnRoomEvent(RoomMover mover)
    {

        //TODO: Integrate with new game manager code to start combat
        if (!(mover is BasePlayerMover))
            yield break;

        if (!m_wonCombat)
        {
            //TODO: This code can be moved into the combat manager, seeing as it is the combats music.
            ResourceAsset playerTeam = GameManager.GetInstance<BaseStorybookGame>().DefaultPlayerTeam;
            Debug.Log("Team = " + m_roomEnemiesPrefabLoc + m_roomEnemies.gameObject.name);
            ResourceAsset enemyTeam = new ResourceAsset(m_roomEnemiesPrefabLoc + m_roomEnemies.gameObject.name, typeof(EnemyTeam));

            //m_musicManager.Fade(m_musicTracks[1], 5, true);
            CombatManager cm = GameManager.GetInstance<BaseStorybookGame>().StartCombat(new StandardCombatInstance(playerTeam, enemyTeam, RoomPageData.PageGenre, RoomPageData.PageLevel));

            // Send out a tutorial event
            EventDispatcher.GetDispatcher<TutorialEventDispatcher>().OnBossFightStarted();

            while (cm.IsRunning)
            {
                yield return null;
            }

            photonView.RPC(nameof(_resetCameraAfterCombat), PhotonTargets.All);

            DestroyEnemyWorldPawns();

            // Send out a tutorial event, notifying that the demo is completed
            EventDispatcher.GetDispatcher<TutorialEventDispatcher>().OnDemoCompleted();

            // Check to see if this is the final floor
            GameManager.GetInstance<BaseStorybookGame>().CheckIfGameIsWon();
        }
        else
        {
            m_musicManager.Fade(m_musicTracks[0], 5, true);
            ClearRoom();
        }
    }

    // What happens when the players leave this room?
    // Hint: Nothing.
    protected override void OnRoomExit(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            return;

        m_wonCombat = true;
    }

    [PunRPC]
    protected void _resetCameraAfterCombat()
    {
        Camera.main.transform.position = CameraNode.position;
        Camera.main.transform.rotation = CameraNode.rotation;
    }

    private void _chooseEnemyTeam()
    {
        Object[] teams = null;
        switch (RoomPageData.PageGenre)
        {
            case Genre.Fantasy:
                m_roomEnemiesPrefabLoc = "Enemies/BossTeams/Fantasy/";
                teams = Resources.LoadAll("Enemies/BossTeams/Fantasy");
                break;
            case Genre.GraphicNovel:
                m_roomEnemiesPrefabLoc = "Enemies/BossTeams/Comic/";
                teams = Resources.LoadAll("Enemies/BossTeams/Comic");
                break;
            case Genre.Horror:
                m_roomEnemiesPrefabLoc = "Enemies/BossTeams/Horror/";
                teams = Resources.LoadAll("Enemies/BossTeams/Horror");
                break;
            case Genre.SciFi:
                m_roomEnemiesPrefabLoc = "Enemies/BossTeams/SciFi/";
                teams = Resources.LoadAll("Enemies/BossTeams/SciFi");
                break;
        }

        if (teams != null)
        {
            GameObject enemyTeam = (GameObject)teams[Random.Range(0, teams.Length)];
            m_roomEnemies = enemyTeam.GetComponent<EnemyTeam>();
        }
    }

    public void DestroyEnemyWorldPawns()
    {
        foreach (GameObject go in m_enemyWorldPawns)
        {
            PhotonNetwork.Destroy(go);
            Destroy(go);
        }
    }

    public void MoveToNextFloor()
    {
        photonView.RPC(nameof(MoveToNextFloorMaster), PhotonTargets.MasterClient);
    }

    [PunRPC]
    protected void MoveToNextFloorMaster()
    {
        SceneFading fader = SceneFading.Instance();
        fader.LoadScene("TestingLevel");
    }
}
