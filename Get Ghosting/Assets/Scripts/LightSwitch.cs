using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public bool playerInRange;
    Room room;
    PlayerController player;

    private void Awake()
    {
        room = transform.parent.GetComponent<Room>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (GameManager.instance.inPlay)
        {
            if (playerInRange && player.clickActive)
                room.FlickerLights();
            else
                room.RestoreLights();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && other is CircleCollider2D)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player" && other is CircleCollider2D)
        {
            playerInRange = false;
            room.RestoreLights();
        }
    }
}
