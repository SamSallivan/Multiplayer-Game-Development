using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 origin;
    public Transform targetTransform;
    public Vector3 target;
    public float speed = 2;

    public void Start()
    {
        origin = transform.position;
        target = targetTransform.position + new Vector3(0.01f, 0.01f, 0.01f);
    }

    void Update()
    {
        float x = Mathf.PingPong(Time.time * speed, target.x - origin.x) + origin.x;
        float y = Mathf.PingPong(Time.time * speed, target.y - origin.y) + origin.y;
        float z = Mathf.PingPong(Time.time * speed, target.z - origin.z) + origin.z;
        transform.position = new Vector3(x, y, z);
    }
}
