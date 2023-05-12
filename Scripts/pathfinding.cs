using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class pathfinding : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Transform> points;
    public NavMeshAgent platforma;
    public Transform pointsParent;
    public Transform vrhRoke;
    public Transform nadPlatformo;
    public float turningSpeed;

    public bool moving = true;

    private float startSpeed;
    private float startAcceleration;

    public int whichPoint = -1;
    void Start()
    {
        // Get point array
        for (int i = 0; i < pointsParent.transform.childCount; i++)
        {
            points.Add(pointsParent.transform.GetChild(i));
        }
        startSpeed = platforma.speed;
        startAcceleration = platforma.acceleration;
    }

    private void Update()
    {
        platforma.updateRotation = false;
        if (moving)
        {
            vrhRoke.position = nadPlatformo.position;
            vrhRoke.localRotation = nadPlatformo.rotation;
        }
        
        if(whichPoint >= 0)
        {
            platforma.SetDestination(points[whichPoint].position);
        }
        FaceTarget();
    }
    void FaceTarget()
    {
        var turnTowardNavSteeringTarget = platforma.steeringTarget;
        Vector3 direction = (turnTowardNavSteeringTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        lookRotation *= Quaternion.Euler(0, 90, 0);

        Quaternion difference = transform.rotation * Quaternion.Inverse(lookRotation);

        float angleDiff = difference.eulerAngles.y;


        if (angleDiff > 10 && angleDiff < 350)
        {
            platforma.speed = 0;
            platforma.acceleration = 10000;
        }
        else
        {
            platforma.speed = startSpeed;
            platforma.acceleration = startAcceleration;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turningSpeed);
    }
}