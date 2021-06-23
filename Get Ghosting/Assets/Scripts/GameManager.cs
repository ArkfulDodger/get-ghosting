using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum ScareTactic {boo, lights, boolights, suspense};
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool inPlay;
    public List<Room> rooms = new List<Room>();
    public List<Room> roomsWithOccupants = new List<Room>();
    public List<Victim> victims = new List<Victim>();
    public Sprite booSprite;
    public Sprite lightSprite;
    public Image scoreBarFill;
    public TMP_Text timerText;
    public TMP_Text levelText;
    public RectTransform scareBar;
    public RectTransform goalIndicator;
    public GameObject infoPanel;
    public GameObject successScreen;
    public TMP_Text successScore;
    public TMP_Text successQuota;
    public TMP_Text failureScore;
    public TMP_Text failureQuota;
    public TMP_Text earlyWinCountdownText;
    public GameObject failureScreen;
    PlayerController player;
    int totalRooms;
    float startingChangeTimer = 4f;
    float minChangeTimer = 1f;
    float startingTargetScore = 0.5f;
    float maxTargetScore = 0.9f;
    float startingStableTime = 3f;
    float minStableTime = 1.5f;
    float score;
    public int earlyWinTime = 3;
    public bool checkingEarlyWin;
    public int level = 0;

    // Level Components
    [SerializeField] int timer = 15;
    [SerializeField] int roomsOccupied;
    [SerializeField] int maxDoubleRooms;
    [SerializeField] int minDoubleRooms;
    [SerializeField] int minRoomsToChange;
    [SerializeField] int maxRoomsToChange;
    [SerializeField] float changeTimer;
    [SerializeField] float targetScore;
    public float stableTime;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        totalRooms = rooms.Count;
        timerText.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if (inPlay)
            UpdateScore();
    }

    public void StartPlay()
    {
        level++;
        inPlay = true;
        GenerateLevel();
        player.gameObject.transform.position = player.playPosition;
        StartCoroutine("Countdown");
        StartCoroutine("UpdateRooms");
    }

    public void RedoLevel()
    {
        inPlay = true;
        GenerateLevel();
        player.gameObject.transform.position = player.playPosition;
        StartCoroutine("Countdown");
        StartCoroutine("UpdateRooms");
    }

    void LevelComplete()
    {
        StopAllCoroutines();
        ClearRooms();
        player.ResetPlayer();
        earlyWinCountdownText.gameObject.SetActive(false);
        inPlay = false;

        if (score > targetScore)
        {
            successScore.text = Mathf.RoundToInt(score * 100).ToString() + " / 100";
            successQuota.text = "Quota: " + Mathf.RoundToInt(targetScore * 100).ToString();
            successScreen.SetActive(true);
        }
        else
        {
            failureScore.text = Mathf.RoundToInt(score * 100).ToString() + " / 100";
            failureQuota.text = "Quota: " + Mathf.RoundToInt(targetScore * 100).ToString();
            failureScreen.SetActive(true);
        }
        infoPanel.SetActive(true);
    }

    void UpdateScore()
    {
        float totalscore = 0;
        foreach (var victim in victims)
        {
            totalscore += victim.score;
        }

        score = totalscore / victims.Count;

        scoreBarFill.fillAmount = score;
        scoreBarFill.color = score > targetScore ? Color.green : Color.red;

        if (score > 0.995f && !checkingEarlyWin)
        {
            checkingEarlyWin = true;
            StartCoroutine("EarlyWin");
        }
        else if (score < 0.995f && checkingEarlyWin)
        {
            checkingEarlyWin = false;
        }
    }

    IEnumerator Countdown()
    {
        while (timer > 0)
        {
            yield return new WaitForSecondsRealtime(1.2f);

            timer--;
            timerText.text = timer.ToString();
            if (timer < 6)
                timerText.color = Color.red;
        }

        LevelComplete();
    }

    IEnumerator UpdateRooms()
    {
        while (inPlay)
        {
            yield return new WaitForSeconds(changeTimer);

            int roomsToChange = Random.Range(minRoomsToChange, maxRoomsToChange + 1);

            List<Room> tmpList = new List<Room>();
            foreach (var room in roomsWithOccupants)
            {
                tmpList.Add(room);
            }

            for (int i = 0; i < roomsToChange; i++)
            {
                int index = Random.Range(0, tmpList.Count);
                tmpList[index].UpdateRoomTactic();
                tmpList.Remove(tmpList[index]);
            }
        }
    }

    IEnumerator EarlyWin()
    {
        earlyWinCountdownText.gameObject.SetActive(true);
        int count = earlyWinTime;
        earlyWinCountdownText.text = count.ToString();

        while (checkingEarlyWin && count > 0)
        {
            yield return new WaitForSeconds(1);
            count--;
            earlyWinCountdownText.text = count.ToString();
        }

        if (checkingEarlyWin)
        {
            checkingEarlyWin = false;
            LevelComplete();
        }
        else
        {
            earlyWinCountdownText.gameObject.SetActive(false);
        }
    }

    void GenerateLevel()
    {
        GenerateLevelValues();
        
        //update level text
        levelText.text = "NIGHT " + level.ToString();

        //reset timer
        timerText.text = timer.ToString();
        timerText.color = Color.white;

        //position goal indicator
        goalIndicator.anchoredPosition = new Vector3(goalIndicator.anchoredPosition.x, scareBar.rect.height * targetScore);

        PopulateRooms();
    }

    void GenerateLevelValues()
    {
        // timer is always 60
        timer = 60;

        // rooms occupied starts at 1, is 2 for level two, and increases every other level up to max
        roomsOccupied = Mathf.Min(totalRooms, Mathf.RoundToInt( ((float)level + 1f) / 2f));

        // maxdouble rooms start at 0, become 1 at level 5, and increase every three levels up to max
        maxDoubleRooms = Mathf.Clamp(Mathf.RoundToInt( ((float)level - 3f) / 3f), 0, totalRooms);

        // min double rooms starts at 0, becomes 1 at level 10, and increases every other level until half the max rooms
        minDoubleRooms = Mathf.Clamp(Mathf.RoundToInt( ((float)level - 9f) / 2f), 0, Mathf.RoundToInt((float)totalRooms / 2));

        // min # rooms to update at once starts at 1, and increases every 8 levels up to half the max rooms
        minRoomsToChange = Mathf.Min(Mathf.RoundToInt((float)totalRooms / 2f), Mathf.RoundToInt( ((float)level + 4f) / 8f));

        // max # of rooms to update at once increases every three levels up to max
        maxRoomsToChange = Mathf.Min(totalRooms, Mathf.RoundToInt( ((float)level + 2f) / 3f));

        // change frequency starts at starting timer and decreases until min timer at level 15
        changeTimer = Mathf.Max(minChangeTimer, Mathf.Lerp(startingChangeTimer, minChangeTimer, ((float)level - 1f) / 14f));

        // target score starts at 0.5 and increases to 0.9 at level 15
        targetScore = Mathf.Min(maxTargetScore, Mathf.Lerp(startingTargetScore, maxTargetScore, ((float)level - 1f) / 14f));

        // change frequency starts at starting timer and decreases until min timer at level 15
        stableTime = Mathf.Max(minStableTime, Mathf.Lerp(startingStableTime, minStableTime, ((float)level - 1f) / 14f));
    }

    void PopulateRooms()
    {
        // Copy List of rooms to new list of candidate rooms
        List<Room> roomsToFill = new List<Room>();
        foreach (var room in rooms)
        {
            roomsToFill.Add(room);
        }

        // set variables for number of total rooms to fill and number of double rooms
        int doubleRoomsToFill = Random.Range(minDoubleRooms, maxDoubleRooms + 1);
        int leftToFill = roomsOccupied;

        // add victims to rooms until no rooms left to fill
        while (leftToFill > 0)
        {
            // get random room index from remaining selection
            int tmp = Random.Range(0, roomsToFill.Count);

            // generate a pooled victim and add them to the room and victim list
            GameObject victimGO = GenerateVictim();
            if (victimGO != null)
            {
                roomsToFill[tmp].AddVictimToRoom(victimGO);
                victims.Add(victimGO.GetComponent<Victim>());
            }

            // if double rooms remain to be filled, add generate a second victim
            if (doubleRoomsToFill > 0)
            {
                victimGO = GenerateVictim();
                if (victimGO != null)
                {
                    roomsToFill[tmp].AddVictimToRoom(victimGO);
                    victims.Add(victimGO.GetComponent<Victim>());
                }
                doubleRoomsToFill--;
            }

            leftToFill--;
            roomsToFill[tmp].UpdateRoomTactic();
            roomsWithOccupants.Add(roomsToFill[tmp]);
            roomsToFill.Remove(roomsToFill[tmp]);
        }
    }

    GameObject GenerateVictim()
    {
        GameObject victimGO = ObjectPool.SharedInstance.GetPooledObject();
        if (victimGO != null)
        {
            victimGO.GetComponent<Victim>().RefreshLook();
            victimGO.SetActive(true);
            return victimGO;
        }
        return null;
    }

    void ClearRooms()
    {
        foreach (var room in rooms)
        {
            room.ResetRoom();
        }

        foreach (var victim in victims)
        {
            victim.ResetVictim();
            victim.gameObject.SetActive(false);
        }

        roomsWithOccupants.Clear();
        victims.Clear();
    }
}
