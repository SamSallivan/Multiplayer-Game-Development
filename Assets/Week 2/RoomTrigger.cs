using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public bool triggered = false;
    public TwoDController triggeredPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            triggered = true;
            triggeredPlayer = collision.gameObject.GetComponent<TwoDController>();
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
