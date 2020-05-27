using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representation of a panto handle.
/// </summary>
public class PantoHandle : PantoBehaviour
{
    protected bool isUpper = true;
    private GameObject handledGameObject;
    private float speed;
    private bool inTransition = false;
    private float rotation;
    static Vector3 handleDefaultPosition = new Vector3(0f, 0f, 14.5f);
    private Vector3 position = handleDefaultPosition;
    private Vector3? godObjectPosition;
    protected bool userControlledPosition = true; //for debug only
    protected bool userControlledRotation = true; //for debug only

    /// <summary>
    /// Moves the handle to the given position at the given speed. The handle will then be freed.
    /// </summary>
    public IEnumerator MoveToPosition(Vector3 position, float newSpeed) {
        //TODO
        userControlledPosition = false;
        userControlledRotation = false;
        if (inTransition)
        {
            Debug.LogWarning("[DualPanto] Discarding not yet reached gameObject" + gameObject);
        }
        Debug.Log("[DualPanto] Switching to:" + position);
        //handledGameObject = newHandle;
        speed = newSpeed;
        inTransition = true;

        while (inTransition)
        {
            yield return new WaitForSeconds(.01f);
        }
        Free();
    }

    /// <summary>
    /// Moves the handle to the given GameObject at the given speed. The handle will follow this object, until Free() is called or the handle is switched to another object.
    /// </summary>
    public IEnumerator SwitchTo(GameObject newHandle, float newSpeed)
    {
        userControlledPosition = false;
        userControlledRotation = false;
        if (inTransition)
        {
            Debug.LogWarning("[DualPanto] Discarding not yet reached gameObject: " + handledGameObject.name);
        }
        Debug.Log("[DualPanto] Switching to: " + newHandle.name);
        handledGameObject = newHandle;
        speed = newSpeed;
        inTransition = true;

        while (inTransition)
        {
            yield return new WaitForSeconds(.01f);
        }
    }

    /// <summary>
    /// Get the current rotation of the handle, use this as the y axis in Unity.
    /// </summary>
    public float getRotation() {
        if (pantoSync.debug) {
            if (userControlledRotation) {
                return rotation;
            } else {
                GameObject debugObject = pantoSync.getDebugObject(isUpper);
                return debugObject.transform.eulerAngles.y;
            }
        } else {
            return rotation;
        }
    }

    /// <summary>
    /// Get the current position of the handle in Unity coordinates.
    /// </summary>
    public Vector3 getPosition() {
        if (pantoSync.debug) {
            if (userControlledPosition) {
                return GetPositionOutsideObstacles(position);
            } else {
                GameObject debugObject = pantoSync.getDebugObject(isUpper);
                return debugObject.transform.position;
            }
        } else {
            return position;
        }
    }
    
    private Vector3 GetPositionOutsideObstacles(Vector3 newPosition)
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

    public Vector3? getGodObjectPosition() {
        return godObjectPosition;
    }

    /// <summary>
    /// Get the current position of the handle in Unity coordinates.
    /// </summary>
    public void FreeRotation() {
        if (pantoSync.debug) {
            userControlledRotation = true;
        } else {
            //TODO
        }
    }

    /// <summary>
    /// Get the current position of the handle in Unity coordinates.
    /// </summary>
    public void applyForce(Vector3 direction)
    {
        pantoSync.ApplyForce(isUpper, direction);
    }

    /// <summary>
    /// Free both position and rotation, meaning giving the control back to the user.
    /// </summary>
    public void Free()
    {
        handledGameObject = null;
        if (pantoSync.debug) {
            userControlledPosition = true;
            userControlledRotation = true;
        } else {
            pantoSync.FreeHandle(isUpper);
        }
    }

    float MaxMovementSpeed()
    {
        return 0.5f;
    }

    public void OverlayScriptedMotion(ScriptedMotion motion)
    {

    }

    public void SetPositions(Vector3 newPosition, float newRotation, Vector3? newGodObjectPosition)
    {
        if (pantoSync.debug && userControlledRotation) {
            GameObject debugObject = pantoSync.getDebugObject(isUpper);
            debugObject.transform.eulerAngles = new Vector3(debugObject.transform.eulerAngles.x, newRotation, debugObject.transform.eulerAngles.z);
        }
        if (pantoSync.debug && userControlledPosition) {
            GameObject debugObject = pantoSync.getDebugObject(isUpper);
            debugObject.transform.position = position;
        }
        position = newPosition;
        rotation = newRotation;
        godObjectPosition = newGodObjectPosition;
    }

    public IEnumerator TraceObjectByPoints(List<GameObject> cornerObjects, float speed)
    {
        for (int i = 0; i < cornerObjects.Count; i++)
        {
            yield return SwitchTo(cornerObjects[i], speed);
        }
        yield return SwitchTo(cornerObjects[0], speed);
    }

    protected void Update()
    {
        if (handledGameObject == null)
        {
            inTransition = false;
            return;
        }
        float movementSpeed = Mathf.Min(MaxMovementSpeed(), speed);
        Vector3 currentPos = getPosition();
        Vector3 goalPos = handledGameObject.transform.position;

        if (Vector3.Distance(currentPos, goalPos) > movementSpeed)
        {
            Vector3 movement = (goalPos - currentPos).normalized * movementSpeed;
            GetPantoSync().UpdateHandlePosition(currentPos + movement, handledGameObject.transform.eulerAngles.y, isUpper);
        }
        else
        {
            if (inTransition) {
                Debug.Log("[DualPanto] Reached: " + handledGameObject.name);
                inTransition = false;
            }
            GetPantoSync().UpdateHandlePosition(goalPos, handledGameObject.transform.eulerAngles.y, isUpper);
        }
    }
}
