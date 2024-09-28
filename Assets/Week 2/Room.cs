using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public float CamSize;
    public bool TriggeredOnce = false;
    public Transform RespawnTransform;
    public RoomTrigger roomTrigger;

    private void Update()
    {
        if (roomTrigger.triggered)
        {
            if (Camera.main.GetComponent<CameraMove>().TargetRoom != this && !TriggeredOnce)
            {
                SwitchRoom();
            }
        }
    }

    private void SwitchRoom()
    {
        TriggeredOnce = true;

        foreach (TwoDController player in roomTrigger.triggeredPlayerList)
        {
            player.score += 1;
            StartCoroutine(player.DisableMovement(2f));
        }

        /*if (Camera.main.GetComponent<AudioSource>().clip != audioBG)
        {
            //if (audioBG )
            Camera.main.GetComponent<AudioSource>().clip = audioBG;
            Camera.main.GetComponent<AudioSource>().Play();
        }*/
        
        Camera.main.GetComponent<CameraMove>().TargetRoom = this;
        Debug.Log("CamChange");
    }
}
