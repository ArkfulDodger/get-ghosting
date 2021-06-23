using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostInRoom : MonoBehaviour
{
    Room room;

    private void Awake() {
        room = transform.parent.GetComponent<Room>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && other is CapsuleCollider2D)
        {
            room.ghostInRoom = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player" && other is CapsuleCollider2D)
        {
            room.ghostInRoom = false;
        }
    }
}
