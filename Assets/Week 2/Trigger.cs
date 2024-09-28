using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public bool triggered;
    public int requiredPlayerCount = 2;
    public List<TwoDController> triggeredPlayerList = new List<TwoDController>();
    
    public Transform buttonTransform;
    public Transform buttonStart;
    public Transform buttonEnd;
    public float buttonSpeed = 5f;
    
    public TMP_Text text;

    public void Start()
    {
        text.text = $"{triggeredPlayerList.Count} / {requiredPlayerCount}";
        /*if (requiredPlayerCount > 1)
        {
            text.text = $"{triggeredPlayerList.Count} / {requiredPlayerCount}";
        }
        else
        {
            text.text = "";
        }*/
    }

    public void Update()
    {
        Vector3 targetPosition = Vector3.Lerp(buttonStart.position, buttonEnd.position, (float)triggeredPlayerList.Count / requiredPlayerCount);
        buttonTransform.position = Vector3.Lerp(buttonTransform.position, targetPosition, Time.deltaTime * buttonSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            triggeredPlayerList.Add(collision.gameObject.GetComponent<TwoDController>());
            
            text.text = $"{triggeredPlayerList.Count} / {requiredPlayerCount}";
            /*if (requiredPlayerCount > 1)
            {
                text.text = $"{triggeredPlayerList.Count} / {requiredPlayerCount}";
            }*/

            if (triggeredPlayerList.Count >= requiredPlayerCount)
            {
                triggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            triggeredPlayerList.Remove(collision.gameObject.GetComponent<TwoDController>());
            
            text.text = $"{triggeredPlayerList.Count} / {requiredPlayerCount}";
            /*if (requiredPlayerCount > 1)
            {
                text.text = $"{triggeredPlayerList.Count} / {requiredPlayerCount}";
            }*/
            
            if (triggeredPlayerList.Count < requiredPlayerCount)
            {
                triggered = false;
            }
        }
    }
}
