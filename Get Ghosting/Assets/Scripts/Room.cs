using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    Animator lightAnimator;
    public bool lightOn;
    public bool flickering;
    public bool ghostInRoom;
    bool roomIsScary;
    public List<Victim> victimsInRoom = new List<Victim>();
    float randomPlacementRange = 0.5f;
    public ScareTactic activeTactic;
    PlayerController player;
    public SpriteRenderer icon1;
    public SpriteRenderer icon2;
    public GameObject plusIcon;
    public GameObject xIcon;
    public SpriteRenderer iconTray;
    public Color defaultTrayColor;
    public Color highlightedTrayColor;
    public AudioSource lightsAudio;
    public AudioSource buzzAudio;
    public AudioClip buzzClip;
    bool canBuzz = true;


    private void Awake()
    {
        lightAnimator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    private void Start()
    {
        UpdateRoomTactic();
    }

    private void Update()
    {
        if (GameManager.instance.inPlay)
            UpdateScariness();
    }

    void UpdateScariness()
    {
        roomIsScary = false;

        if (victimsInRoom.Count > 0)
        {
            // set room to scary if active tactic is fullfilled
            switch (activeTactic)
            {
                case ScareTactic.boo:
                {
                    if (ghostInRoom && player.booActive && !flickering)
                        roomIsScary = true;
                    break;
                }

                case ScareTactic.lights:
                {
                    if (flickering && !player.booActive)
                        roomIsScary = true;
                    break;
                }

                case ScareTactic.boolights:
                {
                    if (ghostInRoom && player.booActive && flickering)
                        roomIsScary = true;
                    break;
                }

                case ScareTactic.suspense:
                {
                    if (!(player.booActive && ghostInRoom) && !flickering)
                        roomIsScary = true;
                    break;
                }
            }

            // set each occupant's scared state to match the room status
            foreach (Victim victim in victimsInRoom)
            {
                victim.activeScare = roomIsScary;
            }
        }

        if (roomIsScary)
            iconTray.color = highlightedTrayColor;
        else
        {
            iconTray.color = defaultTrayColor;

            if (victimsInRoom.Count > 0 && ((ghostInRoom && player.booActive) || flickering) && canBuzz)
            {
                buzzAudio.PlayOneShot(buzzClip);
                canBuzz = false;
                StartCoroutine(BuzzWait());
            }
        }
    }

    IEnumerator BuzzWait()
    {
        yield return new WaitForSeconds(1.5f);
        canBuzz = true;
    }

    void ClearIcons()
    {
        // clear icons
        icon1.sprite = null;
        icon2.sprite = null;
        plusIcon.SetActive(false);
        xIcon.SetActive(false);
    }

    public void UpdateRoomTactic()
    {
        ClearIcons();

        // if no victims in this room, clear icons
        if (victimsInRoom.Count == 0)
            return;

        bool confirmed = false;
        while (!confirmed)
        {
            int randomTactic = Random.Range(0, System.Enum.GetValues(typeof(ScareTactic)).Length);
            activeTactic = (ScareTactic)randomTactic;
            confirmed = true;

            if (activeTactic == ScareTactic.suspense)
            {
                foreach (Victim victim in victimsInRoom)
                {
                    if (victim.panic < 3)
                        confirmed = false;
                }
            }
        }

        switch (activeTactic)
        {
            case ScareTactic.boo:
            {
                icon1.sprite = GameManager.instance.booSprite;
                icon1.color = Color.white;
                break;
            }

            case ScareTactic.lights:
            {
                icon1.sprite = GameManager.instance.lightSprite;
                icon1.color = Color.yellow;
                break;
            }

            case ScareTactic.boolights:
            {
                icon1.sprite = GameManager.instance.booSprite;
                icon1.color = Color.white;
                icon2.sprite = GameManager.instance.lightSprite;
                icon2.color = Color.yellow;
                plusIcon.SetActive(true);
                break;
            }

            case ScareTactic.suspense:
            {
                xIcon.SetActive(true);
                break;
            }
        }
    }

    public void FlickerLights()
    {
        if (!flickering)
        {
            flickering = true;
            lightAnimator.SetBool("flickering", true);
            lightsAudio.Play();
        }
    }

    public void RestoreLights()
    {
        if (flickering)
        {
            flickering = false;
            lightAnimator.SetBool("flickering", false);
            lightsAudio.Stop();
        }
    }

    void LightsOn()
    {
        lightOn = true;
        lightAnimator.SetBool("lighton", true);
    }

    void LightsOff()
    {
        lightOn = false;
        lightAnimator.SetBool("lighton", false);
    }

    public void ResetRoom()
    {
        ClearIcons();
        iconTray.color = defaultTrayColor;
        ghostInRoom = false;
        roomIsScary = false;
        canBuzz = true;
        victimsInRoom.Clear();
        RestoreLights();
        LightsOff();
    }

    public void AddVictimToRoom(GameObject victimGO)
    {
        // place the victim at a random spot near the center of the room
        Vector2 randomDirection = new Vector2(Random.Range(0,1f), Random.Range(0,1f)).normalized;
        victimGO.transform.position = transform.position + (Vector3)(randomDirection * randomPlacementRange);

        // add the victim to the list of victims in the room
        Victim victim = victimGO.GetComponent<Victim>();
        victimsInRoom.Add(victim);

        LightsOn();
    }
}
