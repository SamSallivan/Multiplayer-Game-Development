using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHitbox : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.TryGetComponent<TwoDController>(out var controller))
        {
            controller.StartCoroutine(controller.Kill());
        }
    }
    
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.TryGetComponent<TwoDController>(out var controller))
        {
            controller.StartCoroutine(controller.Kill());
        }
    }
}
