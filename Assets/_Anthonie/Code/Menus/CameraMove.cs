using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;
    public GameObject cameraObj;
    public float smoothAmount;
    public float rotateSmoothAmount;
    bool moving = false;
    public float distanceToStop;


    private void Update()
    {
        if (moving)
        {
            cameraObj.transform.position = Vector3.Lerp(cameraObj.transform.position, target.position, smoothAmount * Time.deltaTime);
            cameraObj.transform.rotation = Quaternion.Lerp(cameraObj.transform.rotation, target.rotation, rotateSmoothAmount * Time.deltaTime);
            if(Vector3.Distance(cameraObj.transform.position, target.transform.position) < distanceToStop)
            {
                moving = false;
            }
        }
    }

    public void MoveCameraOnButtonclick()
    {
        moving = true;
    }
}
