using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public NetworkVariable<bool> isGameOver;

    public override void OnNetworkSpawn()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        /*if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene("Platforms", LoadSceneMode.Additive);
        }*/
    }

    public void Update()
    {
        if (IsServer)
        {
            ServerUpdate();
        }
    }

    public void ServerUpdate()
    {
        if (isGameOver.Value)
        {
            return;
        }
        
        List<NetworkedFpsController> players = FindObjectsOfType<NetworkedFpsController>().ToList();
        List<NetworkedFpsController> winners = new List<NetworkedFpsController>();

        if (players.Count < 2)
        {
            return;
        }
        
        int alivePlayers = 0;
        foreach (NetworkedFpsController player in players)
        {
            if (!player.isDead.Value)
            {
                alivePlayers++;
                winners.Add(player);
            }
        }
    
        if (alivePlayers == 1)
        {
            winners[0].score.Value += 1;
            winners[0].DieRpc();
            StartCoroutine(GameOverCoroutine());
        }        
    }
    
    public IEnumerator GameOverCoroutine()
    {
        isGameOver.Value = true;
        
        yield return new WaitForSeconds(1.0f);
        
        List<NetworkedFpsController> players = FindObjectsOfType<NetworkedFpsController>().ToList();
        foreach (NetworkedFpsController player in players)
        {
            player.RespawnRpc();
            player.isDead.Value = false;
        }
        
        List<CollapsingPlatform> platforms = FindObjectsOfType<CollapsingPlatform>().ToList();
        foreach (CollapsingPlatform platform in platforms)
        {
            platform.triggered.Value = false;
        }
        
        yield return new WaitForSeconds(0.5f);

        isGameOver.Value = false;
    }
    
}
