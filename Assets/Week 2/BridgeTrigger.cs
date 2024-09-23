using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeTrigger : MonoBehaviour
{
    public Transform bridgeTransform;
    public Transform bridgeStart;
    public Transform bridgeEnd;
    
    public Transform buttonTransform;
    public Transform buttonStart;
    public Transform buttonEnd;
    public bool triggered = false;
    public float speed = 5f;

    private void Update()
    {
        Vector3 targetPosition1 = triggered ? bridgeEnd.position : bridgeStart.position;
        bridgeTransform.position = Vector3.Lerp(bridgeTransform.position, targetPosition1, Time.deltaTime * speed);
        
        Vector3 targetPosition2 = triggered ? buttonEnd.position : buttonStart.position;
        buttonTransform.position = Vector3.Lerp(buttonTransform.position, targetPosition2, Time.deltaTime * speed);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            triggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            triggered = false;
        }
    }
}
