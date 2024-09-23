using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestructiveTrigger : MonoBehaviour
{
    public bool triggered = false;
    public float speed = 5f;
    public float timer = 1f;


    public void Update()
    {
        if (triggered)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, timer);
            }
            else
            {
                GetComponent<Collider2D>().enabled = false;
                triggered = false;
                timer = 1f;
            }
        }
        else
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                GetComponent<Collider2D>().enabled = true;
                GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 1);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!triggered && timer <= 0)
            {
                triggered = true;
                timer = 0.5f;
            }
        }
    }
}
