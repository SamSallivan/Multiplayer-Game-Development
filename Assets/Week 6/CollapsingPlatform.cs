using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CollapsingPlatform : NetworkBehaviour
{
    public NetworkVariable<bool> triggered;
    public NetworkVariable<float> timer = new (1f);
    public float defaultTime = 1f;

    public void Update()
    {
        if (IsServer)
        {
            if (triggered.Value)
            {
                if (timer.Value > 0)
                {
                    timer.Value -= Time.deltaTime;
                }
                else
                {
                    ToggleColliderRpc(false);
                }
            }
            else
            {
                timer.Value = defaultTime;
                ToggleColliderRpc(true);
            }
        }
        
        GetComponent<Renderer>().material.color = Color.Lerp(new Color(Color.black.r, Color.black.g, Color.black.b, timer.Value), Color.white, timer.Value);
        
    }
    
    [Rpc(SendTo.Everyone)]
    public void ToggleColliderRpc(bool value)
    {
        GetComponent<Collider>().enabled = value;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<NetworkedFpsController>().NetworkObject.IsOwner)
            {
                if (!triggered.Value)
                {
                    TriggerRpc();
                }
            }
        }
    }
    
    [Rpc((SendTo.Server))]
    public void TriggerRpc()
    {
        triggered.Value = true;
    }
}