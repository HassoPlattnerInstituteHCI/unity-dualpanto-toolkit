using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeHandle : HighLevelHandle
{

    public Vector3 UpdateMePosition(Vector3 newPosition, float newRotation)
    {
        if (getIsActive())
        {
            if (pantoSync.debug)
            {
                newPosition = CheckForCollisions(newPosition);
            }
            gameObject.transform.position = newPosition;
            gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, newRotation, transform.eulerAngles.z);
        }
        return newPosition;
    }

    Vector3 CheckForCollisions(Vector3 newPosition)
    {
        var colliders = FindObjectsOfType<PantoCollider>();
        foreach (PantoCollider collider in colliders)
        {
            if (collider.GetComponent<Collider>() != null && collider.GetComponent<Collider>().bounds.Contains(newPosition))
            {
                Bounds bounds = collider.GetComponent<Collider>().bounds;
                //which side am I closer to?
                float x = newPosition.x;
                float z = newPosition.z;
                float maxXDistance = Mathf.Max(Mathf.Abs(newPosition.x - bounds.min.x), Mathf.Abs(newPosition.x - bounds.max.x));
                float maxZDistance = Mathf.Max(Mathf.Abs(newPosition.z - bounds.min.z), Mathf.Abs(newPosition.z - bounds.max.z));
                if (maxXDistance > maxZDistance)
                {
                    x = newPosition.x < bounds.center.x ? bounds.min.x : bounds.max.x;
                }
                else
                {
                    z = newPosition.z < bounds.center.z ? bounds.min.z : bounds.max.z;
                }
                return new Vector3(x, transform.position.y, z);
            }
        }
        return newPosition;
        //return transform.position = new Vector3(0, 0, 0);
    }
}
