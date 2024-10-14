using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Lava : NetworkBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
        {
            return;
        }

        if (GameManager.instance.isGameOver.Value)
        {
            return;
        }
        
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log(other.gameObject.name);
            other.gameObject.GetComponent<NetworkedFpsController>().DieRpc();
        }
    }
    
    public void OnTriggerStay(Collider other)
    {
        if (!IsServer)
        {
            return;
        }

        if (GameManager.instance.isGameOver.Value)
        {
            return;
        }
        
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<NetworkedFpsController>().DieRpc();
        }
    }
}
