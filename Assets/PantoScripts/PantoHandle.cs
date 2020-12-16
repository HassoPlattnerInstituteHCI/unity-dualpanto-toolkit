using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DualPantoFramework
{
    /// <summary>
    /// Representation of a panto handle.
    /// </summary>
    public class PantoHandle : PantoBehaviour
    {
        protected bool isUpper = true;
        private GameObject handledGameObject;
        private float speed = 5.0f;
        private bool inTransition = false;
        private float rotation;
        static Vector3 handleDefaultPosition = new Vector3(0f, 0f, 14.5f);
        private Vector3 position = handleDefaultPosition;
        private Vector3? godObjectPosition;
        protected bool userControlledPosition = true; //for debug only
        protected bool userControlledRotation = true;

        /// <summary>
        /// Moves the handle to the given position at the given speed. The handle will then be freed.
        /// </summary>
<<<<<<< HEAD
        async public Task MoveToPosition(Vector3 position, float newSpeed = 10.0f, bool shouldFreeHandle = true)
||||||| merged common ancestors
        async public Task MoveToPosition(Vector3 position, float newSpeed, bool shouldFreeHandle = true)
=======
        async public Task MoveToPosition(Vector3 position, float newSpeed = 5.0f, bool shouldFreeHandle = true)
>>>>>>> e209fef0bb1e9821f28aa0bc641a1622d8b1803c
        {
            GameObject go = new GameObject();
            go.transform.position = position;
            await SwitchTo(go, newSpeed);
            handledGameObject = null;
            Destroy(go);
            if (shouldFreeHandle)
            {
                Free();
            }
            else
            {
                Freeze();
            }
        }

        /// <summary>
        /// Moves the handle to the given GameObject at the given speed. The handle will follow this object, until Free() is called or the handle is switched to another object.
        /// </summary>
<<<<<<< HEAD
        async public Task SwitchTo(GameObject newHandle, float newSpeed = 3.0f)
||||||| merged common ancestors
        async public Task SwitchTo(GameObject newHandle, float newSpeed)
=======
        async public Task SwitchTo(GameObject newHandle, float newSpeed = 5.0f)
>>>>>>> e209fef0bb1e9821f28aa0bc641a1622d8b1803c
        {
            int time = 0;
            userControlledPosition = false;
            userControlledRotation = false;
            if (inTransition)
            {
                Debug.LogWarning("[DualPanto] Discarding not yet reached gameObject: " + handledGameObject.name);
            }
            Debug.Log("[DualPanto] Switching to: " + newHandle.name);
            handledGameObject = newHandle;
            if (!pantoSync.debug)
            {
                pantoSync.SetSpeed(isUpper, Mathf.Min(newSpeed, MaxMovementSpeed()));
                GetPantoSync().UpdateHandlePosition(handledGameObject.transform.position, handledGameObject.transform.eulerAngles.y, isUpper);
            }
            inTransition = true;

            while (inTransition)
            {
                if (time > 3000)
                {
                    Debug.Log("Abandoning gameobject that couldn't be reached: " + handledGameObject.name);
                    return;
                }
                await Task.Delay(10);
                time += 10;
            }
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

        /// <summary>
        /// Get the current position (taking obstacles into account) of the handle in Unity coordinates.
        /// </summary>
        public Vector3 HandlePosition(Vector3 currentPosition)
        {
            //TODO only consider enabled obstacles
            if (!pantoSync.debug) return GetPosition();
            Vector3 desiredPosition = GetPosition();
            Vector3 direction = desiredPosition - currentPosition;
            Ray ray = new Ray(currentPosition, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, direction.magnitude, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
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
            userControlledRotation = true;
        }

        /// <summary>
        /// Apply a force to the handle.
        /// </summary>
        public void ApplyForce(Vector3 direction, float strength)
        {
            pantoSync.ApplyForce(isUpper, direction, strength);
        }

        public void ApplyForce(Vector3 direction)
        {
            float defaultForce = 0.5f;
            pantoSync.ApplyForce(isUpper, direction, defaultForce);
        }

        /// <summary>
        /// Cancel forces applied to a handle.
        /// </summary>
        public void StopApplyingForce()
        {
            pantoSync.ApplyForce(isUpper, new Vector3(0, 0, 0), 0f);
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

        /// <summary>
        /// Freezes the position of the handle to the current position.
        /// </summary>
        public void Freeze()
        {
            if (pantoSync.debug)
            {
                userControlledPosition = false;
                userControlledRotation = false;
            }
            else
            {
                pantoSync.FreezeHandle(isUpper);
            }
        }

        float MaxMovementSpeed()
        {
            return 20f;
        }

        public void SetPositions(Vector3 newPosition, float newRotation, Vector3? newGodObjectPosition)
        {
            if (pantoSync.uiManager.blindPanelEnabled)
            {
                float y = GameObject.Find("Blind Panel GO").transform.position.y;
                newPosition.y = y + 1;
                position.y = y + 1;
            }
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
                GameObject debugGodObject = pantoSync.GetDebugGodObject(isUpper);
                if (newGodObjectPosition != null)
                {
                    debugGodObject.transform.position = newGodObjectPosition.Value;
                }
            }
            position = newPosition;
            rotation = newRotation;
            godObjectPosition = newGodObjectPosition;
        }

        async public Task TraceObjectByPoints(List<GameObject> cornerObjects, float speed)
        {
            if (cornerObjects.Count == 0)
            {
                Debug.LogWarning("[DualPanto] Can't trace shape if object has no children");
                return;
            }
            for (int i = 0; i < cornerObjects.Count; i++)
            {
                await SwitchTo(cornerObjects[i], speed);
            }
            await SwitchTo(cornerObjects[0], speed);
        }

        protected void FixedUpdate()
        {
            if (pantoSync.debug && handledGameObject != null && Vector3.Distance(handledGameObject.transform.position, position) < 0.1f)
            {
                inTransition = false;
            }
            if (handledGameObject != null && !inTransition)// reached gameobject initially 
            {
                GetPantoSync().UpdateHandlePosition(handledGameObject.transform.position, null, isUpper);
            }
        }

        public void TweeningEnded()
        {
            inTransition = false;
        }
    }
}
