using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpening : MonoBehaviour
{
    public Transform levoKrilo;
    public Transform desnoKrilo;

    public float openSpeed = 1.0f;
    public float closeSpeed = 2.0f;

    private Vector3 levoKriloOpenPos;
    private Vector3 levoKriloClosedPos;
    private Vector3 desnoKriloOpenPos;
    private Vector3 desnoKriloClosedPos;

    void Start()
    {
        // Record the open and closed positions of the door wings
        levoKriloOpenPos = levoKrilo.position + levoKrilo.right * 150f;
        levoKriloClosedPos = levoKrilo.position;
        desnoKriloOpenPos = desnoKrilo.position + desnoKrilo.right * -      150f;
        desnoKriloClosedPos = desnoKrilo.position;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "MobilnaPlatforma")
        {
            // Open the doors
            StopAllCoroutines();
            StartCoroutine(OpenDoors());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "MobilnaPlatforma")
        {

            // Close the doors
            StopAllCoroutines();
            StartCoroutine(CloseDoors());
        }
    }

    private IEnumerator OpenDoors()
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * openSpeed;
            levoKrilo.position = Vector3.Lerp(levoKriloClosedPos, levoKriloOpenPos, t);
            desnoKrilo.position = Vector3.Lerp(desnoKriloClosedPos, desnoKriloOpenPos, t);
            yield return null;
        }
    }

    private IEnumerator CloseDoors()
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * closeSpeed;
            levoKrilo.position = Vector3.Lerp(levoKriloOpenPos, levoKriloClosedPos, t);
            desnoKrilo.position = Vector3.Lerp(desnoKriloOpenPos, desnoKriloClosedPos, t);
            yield return null;
        }
    }

}
