using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PantoHandle : PantoBehaviour
{
    protected bool isUpper = true;
    private GameObject handledGameObject;
    private MeHandle meObject;
    private float speed;
    private bool inTransition = false;
    private float rotation;
    private Vector3 position;
    private Vector3? godObjectPosition;
    protected bool userControlledPosition = true; //for debug only
    protected bool userControlledRotation = true; //for debug only

    public IEnumerator MoveToPosition(Vector3 position, float newSpeed) {
        //TODO
        userControlledPosition = false;
        userControlledRotation = false;
        if (inTransition)
        {
            Debug.LogWarning("Discarding not yet reached gameObject" + gameObject);
        }
        Debug.Log("Switching to:" + position);
        //handledGameObject = newHandle;
        speed = newSpeed;
        inTransition = true;

        while (inTransition)
        {
            yield return new WaitForSeconds(.01f);
        }
        Free();
    }

    public IEnumerator SwitchTo(GameObject newHandle, float newSpeed)
    {
        userControlledPosition = false;
        userControlledRotation = false;
        if (inTransition)
        {
            //Debug.LogWarning("Discarding not yet reached gameObject: " + handledGameObject.name);
        }
        Debug.Log("Switching to:" + newHandle.name);
        handledGameObject = newHandle;
        speed = newSpeed;
        inTransition = true;

        while (inTransition)
        {
            yield return new WaitForSeconds(.01f);
        }
    }

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

    public Vector3 getPosition() {
        if (pantoSync.debug) {
            if (userControlledPosition) {
                return position;
            } else {
                GameObject debugObject = pantoSync.getDebugObject(isUpper);
                return debugObject.transform.position;
            }
        } else {
            return position;
        }
    }
    
    public Vector3? getGodObjectPosition() {
        return godObjectPosition;
    }

    public void FreeRotation() {
        if (pantoSync.debug) {
            userControlledRotation = true;
        } else {
            //TODO
        }
    }

    public void applyForce(Vector3 direction)
    {
        pantoSync.ApplyForce(isUpper, direction);
    }

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
        return float.PositiveInfinity;
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

        if (inTransition)
        {
            Vector3 currentPos = getPosition();
            Vector3 goalPos = handledGameObject.transform.position;
            Vector3 distance = goalPos - currentPos;
            Vector3 movement = (distance).normalized * speed;
            if (distance.magnitude <= movement.magnitude)
            {
                Debug.Log("Reached: " + handledGameObject.name);
                inTransition = false;
            }
            else
            {
                GetPantoSync().UpdateHandlePosition(currentPos + movement, null, isUpper);
                //GetPantoSync().UpdateHandlePosition(currentPos + movement, handledGameObject.transform.eulerAngles.y, isUpper);
            }
        }
        if (!inTransition)
        {
            GetPantoSync().UpdateHandlePosition(handledGameObject.transform.position, null, isUpper);
            //GetPantoSync().UpdateHandlePosition(handledGameObject.transform.position, handledGameObject.transform.eulerAngles.y, isUpper);
        }
    }
}
