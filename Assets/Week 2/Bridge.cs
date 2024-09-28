using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public Transform bridgeTransform;
    public Transform bridgeStart;
    public Transform bridgeEnd;
    public float bridgeSpeed = 5f;

    public Trigger trigger;
    public bool linearInterpolation;

    private void Update()
    {
        Vector3 targetPosition1 = trigger.triggered ? bridgeEnd.position : bridgeStart.position;
        if (linearInterpolation)
        {
            bridgeTransform.position = Vector3.MoveTowards(bridgeTransform.position, targetPosition1, Time.deltaTime * bridgeSpeed);
        }
        else
        {
            bridgeTransform.position = Vector3.Lerp(bridgeTransform.position, targetPosition1, Time.deltaTime * bridgeSpeed);
        }
    }
}
