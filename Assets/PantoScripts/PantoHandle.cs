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
            GameObject debugObject = pantoSync.GetDebugObject(isUpper);
            return debugObject.transform.position;
        }
        else
        {
            return position;
        }
    }

    public Vector3 HandlePosition(Vector3 currentPosition)
    {
        //TODO only consider enabled obstacles
        if (!pantoSync.debug) return GetPosition();
        Vector3 desiredPosition = GetPosition();
        Vector3 direction = desiredPosition - currentPosition;
        Ray ray = new Ray(currentPosition, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, direction.magnitude))
        {
            return hit.point - (direction.normalized * 0.01f);
        }
        else
        {
            return desiredPosition;
        }
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
        if (cornerObjects.Count == 0) {
            Debug.LogWarning("[DualPanto] Can't trace shape if object has no children");
            return;
        }
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
