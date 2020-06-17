using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private Vector3 startPosition; //tweening
    private Vector3? godObjectPosition;
    protected bool userControlledPosition = true; //for debug only
    protected bool userControlledRotation = true; //for debug only

    public float tweenValue = 0.0f; //tweening
    /// <summary>
    /// Moves the handle to the given position at the given speed. The handle will then be freed.
    /// </summary>
    async public Task MoveToPosition(Vector3 position, float newSpeed, bool shouldFreeHandle = true)
    {
        userControlledPosition = false;
        userControlledRotation = false;
        if (inTransition)
        {
            Debug.LogWarning("[DualPanto] Discarding not yet reached gameObject" + gameObject);
        }
        Debug.Log("[DualPanto] Switching to:" + position);

        GameObject go = new GameObject();
        go.transform.position = position;
        handledGameObject = go;

        speed = newSpeed;
        inTransition = true;

        while (inTransition)
        {
            await Task.Delay(10);
        }
        if (shouldFreeHandle)
        {
            Free();
        }
        Destroy(go);
    }

    /// <summary>
    /// Moves the handle to the given GameObject at the given speed. The handle will follow this object, until Free() is called or the handle is switched to another object.
    /// </summary>
    async public Task SwitchTo(GameObject newHandle, float newSpeed)
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

        tweenValue = 0;
        startPosition = GetPosition();

        while (inTransition)
        {
            await Task.Delay(10);
        }
        //tweenValue = 0;
        //startPosition = getPosition();
        //Debug.Log("startPosition" + startPosition);
    }

    /// <summary>
    /// Get the current rotation of the handle, use this as the y axis in Unity.
    /// </summary>
    public float GetRotation()
    {
        if (pantoSync.debug)
        {
            if (userControlledRotation)
            {
                return rotation;
            }
            else
            {
                GameObject debugObject = pantoSync.GetDebugObject(isUpper);
                return debugObject.transform.eulerAngles.y;
            }
        }
        else
        {
            return rotation;
        }
    }

    /// <summary>
    /// Get the current position of the handle in Unity coordinates.
    /// </summary>
    public Vector3 GetPosition()
    {
        if (pantoSync.debug)
        {
            if (userControlledPosition)
            {
                return GetPositionOutsideObstacles(position);
            }
            else
            {
                GameObject debugObject = pantoSync.GetDebugObject(isUpper);
                return debugObject.transform.position;
            }
        }
        else
        {
            return position;
        }
    }

    private Vector3 GetPositionOutsideObstacles(Vector3 newPosition)
    {
        var colliders = FindObjectsOfType<PantoCollider>();
        foreach (PantoCollider collider in colliders)
        {
            if (collider.GetComponent<Collider>() != null && collider.GetComponent<Collider>().bounds.Contains(newPosition) && collider.enabled)
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
                Vector3 correctedPosition = new Vector3(x, transform.position.y, z);
                Debug.DrawLine(newPosition, correctedPosition, Color.red, 0f, false);
                return correctedPosition;
            }
        }
        return newPosition;
    }

    /// <summary>
    /// Get the current position of the handle in Unity coordinates.
    /// </summary>
    public void FreeRotation()
    {
        if (pantoSync.debug)
        {
            userControlledRotation = true;
        }
        else
        {
            //TODO
        }
    }

    /// <summary>
    /// Get the current position of the handle in Unity coordinates.
    /// </summary>
    public void ApplyForce(Vector3 direction)
    {
        pantoSync.ApplyForce(isUpper, direction);
    }

    /// <summary>
    /// Free both position and rotation, meaning giving the control back to the user.
    /// </summary>
    public void Free()
    {
        handledGameObject = null;
        if (pantoSync.debug)
        {
            userControlledPosition = true;
            userControlledRotation = true;
        }
        else
        {
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
        if (pantoSync.debug && userControlledRotation)
        {
            GameObject debugObject = pantoSync.GetDebugObject(isUpper);
            debugObject.transform.eulerAngles = new Vector3(debugObject.transform.eulerAngles.x, newRotation, debugObject.transform.eulerAngles.z);
        }
        if (pantoSync.debug && userControlledPosition)
        {
            GameObject debugObject = pantoSync.GetDebugObject(isUpper);
            debugObject.transform.position = position;
        }
        if (!pantoSync.debug)
        {
            GameObject debugObject = pantoSync.GetDebugObject(isUpper);
            debugObject.transform.eulerAngles = new Vector3(debugObject.transform.eulerAngles.x, newRotation, debugObject.transform.eulerAngles.z);
            debugObject.transform.position = position;
        }
        position = newPosition;
        rotation = newRotation;
        godObjectPosition = newGodObjectPosition;
    }

    async public Task TraceObjectByPoints(List<GameObject> cornerObjects, float speed)
    {
        for (int i = 0; i < cornerObjects.Count; i++)
        {
            await SwitchTo(cornerObjects[i], speed);
        }
        await SwitchTo(cornerObjects[0], speed);
    }

    protected void Update()
    {
        tweenValue = Mathf.Min(1.0f, tweenValue + 0.04f);
        if (handledGameObject == null)
        {
            inTransition = false;
            return;
        }
        float movementSpeed = Mathf.Min(MaxMovementSpeed(), speed);
        Vector3 currentPos = GetPosition();
        Vector3 goalPos = handledGameObject.transform.position;

        if (Vector3.Distance(currentPos, goalPos) > movementSpeed)
        {
            Vector3 movement = startPosition + (goalPos - startPosition) * tweenValue;
            GetPantoSync().UpdateHandlePosition(movement, handledGameObject.transform.eulerAngles.y, isUpper);
        }
        else
        {
            if (inTransition)
            {
                Debug.Log("[DualPanto] Reached: " + handledGameObject.name);
                inTransition = false;
            }
            GetPantoSync().UpdateHandlePosition(goalPos, handledGameObject.transform.eulerAngles.y, isUpper);
        }
    }
}
