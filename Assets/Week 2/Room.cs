using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public float CamSize;
    public bool Disable = true;
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
        
        if (Camera.main.GetComponent<CameraMove>().Lerp == 10)
        {
            StopCoroutine(LerpBack());
            StartCoroutine(LerpBack());
        }
    }

    private void SwitchRoom()
    {
        TriggeredOnce = true;
        roomTrigger.triggeredPlayer.score += 1;
        
        /*if (Camera.main.GetComponent<AudioSource>().clip != audioBG)
        {
            //if (audioBG )
            Camera.main.GetComponent<AudioSource>().clip = audioBG;
            Camera.main.GetComponent<AudioSource>().Play();
        }*/
        Camera.main.GetComponent<CameraMove>().TargetRoom = this;
        Camera.main.GetComponent<CameraMove>().Lerp = 10;
        StopCoroutine(LerpBack());
        Camera.main.GetComponent<CameraMove>().Lerp = 10;
        Debug.Log("CamChange");

        if (Disable)
        {
            StopCoroutine(roomTrigger.triggeredPlayer.DisableMovement(0));
            StartCoroutine(roomTrigger.triggeredPlayer.DisableMovement(2f));
        }
    }

    IEnumerator LerpBack()
    {
        yield return new WaitForSeconds(0.5f);

        if (Camera.main.GetComponent<CameraMove>().TargetRoom == this)
        {
            Camera.main.GetComponent<CameraMove>().Lerp = 3000;
        }
    }
}
