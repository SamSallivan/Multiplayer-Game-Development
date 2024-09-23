using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float Lerp = 10f;
    /*public Vector2 TargetRangeX;
    public Vector2 TargetRangeY;*/
    public Room TargetRoom;

    private void Update()
    {
        if (TargetRoom){
            Vector3 target = TargetRoom.transform.position;
            target = new Vector3(target.x, target.y, -10);
            //Vector3 target = new Vector3(Player.transform.position.x, Player.transform.position.y, -10);
            //transform.position = Vector3.Lerp(transform.position, target, Lerp * Time.deltaTime);
            /*target.x = Mathf.Clamp(target.x, TargetRangeX.x + 16, TargetRangeX.y - 16);
            target.y = Mathf.Clamp(target.y, TargetRangeY.x + 10, TargetRangeY.y - 10);*/
            transform.position = Vector3.Lerp(transform.position, target, Lerp * Time.deltaTime);

            GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, TargetRoom.CamSize, 1000 * Time.deltaTime);
        }
    }
}