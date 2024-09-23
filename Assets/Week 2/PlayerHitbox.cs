using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public TwoDController _controller;
    public float dashHitForce = 10f;
    public float dashStunTime = 1f;
    public float dashZeroGravTime = 0.25f;


    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.TryGetComponent<TwoDController>(out var controller)  && controller != _controller)
        {
            StartCoroutine(Hit(controller));
        }
    }


    IEnumerator Hit(TwoDController controller)
    {
        Vector3 direction = - transform.position + controller.transform.position;
        controller.StartCoroutine(controller.DisableMovement(dashStunTime));
        controller._rigidbody.gravityScale = 0;
        controller._rigidbody.velocity = (direction + Vector3.up) * dashHitForce;
        
        
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(dashZeroGravTime);
        controller._rigidbody.gravityScale = controller.gravityScaler;
    }
}
