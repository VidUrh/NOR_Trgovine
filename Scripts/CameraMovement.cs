using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform CamPos1;
    public Transform CamPos2;
    public Transform CamPos3;
    public Transform CamPos4;
    public Transform CamPos5;
    public Transform CamPos6;
    private Transform CameraPosVar;

    public Transform target; // The target object to rotate towards
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = CamPos3.position;
        CameraPosVar = CamPos3;
    }


    void Update()
    {
        Vector3 targetPosition = target.position;
        Vector3 cameraPosition = transform.position;

        Vector3 direction = targetPosition - cameraPosition;
        Quaternion rotation = Quaternion.LookRotation(direction);

        transform.rotation = rotation;
        transform.position = Vector3.Lerp(transform.position, CameraPosVar.position, 0.1f);


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CameraPosVar = CamPos1;
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            CameraPosVar = CamPos2;
        }

        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            CameraPosVar = CamPos3;
        }

        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            CameraPosVar = CamPos4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CameraPosVar = CamPos5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            CameraPosVar = CamPos6;
        }
    }
}
