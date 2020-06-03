using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The main DualPanto class. Takes care of the communication with the Panto.
/// </summary>
public class DualPantoSync : MonoBehaviour
{
    public delegate void SyncDelegate(ulong handle);
    public delegate void HeartbeatDelegate(ulong handle);
    public delegate void LoggingDelegate(IntPtr msg);
    public delegate void PositionDelegate(ulong handle, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8, SizeConst = 10)] double[] positions);

    [Header("When Debug is enabled, the emulator mode will be used. You do not need to be connected to a Panto for this mode.")]
    public bool debug = false;
    public float debugRotationSpeed = 10.0f;
    public KeyCode toggleVisionKey = KeyCode.B;
    protected ulong Handle;
    private static LowerHandle lowerHandle;
    private static UpperHandle upperHandle;

    // bounds are defined by center and extent
    //private static Vector2[] pantoBounds = { new Vector2(0, -110), new Vector2(320, 160) }; // for version D
    private static Vector2[] pantoBounds = { new Vector2(20, -80), new Vector2(180, 140) }; // ember
    private static Vector2[] unityBounds;
    public static Vector3 handleDefaultPosition = new Vector3(0f, 0f, 0f);//new Vector3(0f, 0f, 14.5f);
    private static Vector3 upperHandlePos = handleDefaultPosition;
    private static Vector3 lowerHandlePos = handleDefaultPosition;
    private static Vector3 upperGodObject = handleDefaultPosition;
    private static Vector3 lowerGodObject = handleDefaultPosition;
    private static float lowerHandleRot = 0f;
    private static float upperHandleRot = 0f;

    private bool isBlindModeOn = false;
    private ushort currentObstacleId = 0;
    private GameObject debugLowerObject;
    private GameObject debugUpperObject;

    #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    private const string plugin = "serial";
    #else
    private const string plugin = "libserial";
    #endif

    [DllImport(plugin)]
    private static extern uint GetRevision();
    [DllImport(plugin)]
    private static extern void SetSyncHandler(SyncDelegate func);
    [DllImport(plugin)]
    private static extern void SetHeartbeatHandler(HeartbeatDelegate func);
    [DllImport(plugin)]
    private static extern void SetLoggingHandler(LoggingDelegate func);
    [DllImport(plugin)]
    private static extern ulong Open(IntPtr port);
    [DllImport(plugin)]
    private static extern void Close(ulong handle);
    [DllImport(plugin)]
    private static extern void Poll(ulong handle);
    [DllImport(plugin)]
    private static extern void SendSyncAck(ulong handle);
    [DllImport(plugin)]
    private static extern void SendHeartbeatAck(ulong handle);
    [DllImport(plugin)]
    private static extern void SendMotor(ulong handle, byte controlMethod, byte pantoIndex, float positionX, float positionY, float rotation);
    [DllImport(plugin)]
    private static extern void FreeMotor(ulong handle, byte controlMethod, byte pantoIndex);
    [DllImport(plugin)]
    private static extern void SetPositionHandler(PositionDelegate func);
    [DllImport(plugin)]
    private static extern void CreateObstacle(ulong handle, byte pantoIndex, ushort obstacleId, float vector1x, float vector1y, float vector2x, float vector2y);
    [DllImport(plugin)]
    private static extern void AddToObstacle(ulong handle, byte pantoIndex, ushort obstacleId, float vector1x, float vector1y, float vector2x, float vector2y);
    [DllImport(plugin)]
    private static extern void RemoveObstacle(ulong handle, byte pantoIndex, ushort obstacleId);
    [DllImport(plugin)]
    private static extern void EnableObstacle(ulong handle, byte pantoIndex, ushort obstacleId);
    [DllImport(plugin)]
    private static extern void DisableObstacle(ulong handle, byte pantoIndex, ushort obstacleId);

    void Start(){
        Application.targetFrameRate = 60;
    }
    private static void SyncHandler(ulong handle)
    {
        Debug.Log("[DualPanto] Received sync");
        SendSyncAck(handle);
    }

    private static void HeartbeatHandler(ulong handle)
    {
        Debug.Log("[DualPanto] Received heartbeat");
        SendHeartbeatAck(handle);
    }

    private static void LogHandler(IntPtr msg)
    {
        String message = Marshal.PtrToStringAnsi(msg);
        if (message.Contains("Free heap") || message.Contains("Task \"Physics\"") || message.Contains("Task \"I/O\""))
        {
            return;
        }
        else if (message.Contains("failed"))
        {
            Debug.LogError("[DualPanto] " +  message);
        }
        else
        {
            Debug.Log("[DualPanto] " + message);
        }
    }

    private static void PositionHandler(ulong handle, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8, SizeConst = 10)] double[] positions)
    {
        //Debug.Log("Received positions: (" + positions[0] + "|" + positions[1] + "rot:" + positions[2] + ")");
        Vector2 unityPosUpper = pantoToUnity(new Vector2((float)positions[0], (float)positions[1]));
        Vector2 unityGodUpper = pantoToUnity(new Vector2((float)positions[3], (float)positions[4]));
        upperHandlePos = new Vector3(unityPosUpper.x, 0, unityPosUpper.y);
        upperHandleRot = PantoToUnityRotation(positions[2]);
        upperGodObject = new Vector3(unityGodUpper.x, 0, unityGodUpper.y);
        upperHandle.SetPositions(upperHandlePos, upperHandleRot, upperGodObject);

        Vector2 unityPosLower = pantoToUnity(new Vector2((float)positions[5], (float)positions[6]));
        Vector2 unityGodLower = pantoToUnity(new Vector2((float)positions[8], (float)positions[9]));
        lowerHandlePos = new Vector3(unityPosLower.x, 0, unityPosLower.y);
        lowerHandleRot = PantoToUnityRotation(positions[7]);
        lowerGodObject = new Vector3(unityGodLower.x, 0, unityGodLower.y);
        lowerHandle.SetPositions(lowerHandlePos, lowerHandleRot, lowerGodObject);
    }

    private static ulong OpenPort(string port)
    {
        return Open(Marshal.StringToHGlobalAnsi(port));
    }

    public GameObject getDebugObject(bool isUpper)
    {
        if (isUpper)
        {
            return debugUpperObject;
        }
        else
        {
            return debugLowerObject;
        }
    }

    void Awake()
    {
        if (!debug)
        {
            Debug.Log("[DualPanto] Serial protocol revision: " + GetRevision());
            SetLoggingHandler(LogHandler);
            SetSyncHandler(SyncHandler);
            SetHeartbeatHandler(HeartbeatHandler);
            SetPositionHandler(PositionHandler);
            // should be discovered automatically
            Handle = OpenPort("/dev/cu.SLAB_USBtoUART");
            //Handle = OpenPort("//.//COM3");
            if(Handle == (ulong)0){ // if device not found then switch to debug mode.
                debug = true; 
            }
        }
        if (debug)
        {
            CreateDebugObjects();
        }
    }

    private void CreateDebugObjects()
    {
        UnityEngine.Object prefab = Resources.Load("ItHandlePrefab");
        debugLowerObject = Instantiate(prefab) as GameObject;
        debugLowerObject.transform.position = new Vector3(0f, 0.5f, 13.0f);

        prefab = Resources.Load("MeHandlePrefab");
        debugUpperObject = Instantiate(prefab) as GameObject;
        debugUpperObject.transform.position = new Vector3(0f, 0.5f, 13.0f);
    }

    void OnDestroy()
    {
        FreeHandle(true);
        FreeHandle(false);
        Close(Handle);
    }

    void Update()
    {
        if (!debug)
        {
            Poll(Handle);
        }
        else
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float mouseRotation = Input.GetAxis("Horizontal") * debugRotationSpeed;
            Vector3 position = new Vector3(mousePosition.x, 0.0f, mousePosition.z);
            float r = debugUpperObject.transform.eulerAngles.y + mouseRotation;
            upperHandlePos = position;
            upperHandle.SetPositions(upperHandlePos, r, null);
            
            lowerHandleRot = debugLowerObject.transform.eulerAngles.y + mouseRotation;
            lowerHandlePos = position;
            lowerHandle.SetPositions(lowerHandlePos, r, null);
        }

        if (Input.GetKeyDown(toggleVisionKey))
        {
            toggleBlindMode();
        }
    }

    private void toggleBlindMode()
    {
        if (!debug)
        {
            if (isBlindModeOn)
            {
                Camera.main.farClipPlane = 1;
            }
            else
            {
                Camera.main.farClipPlane = 1000;
            }
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                light.enabled = !light.enabled;
            }
        }
        isBlindModeOn = !isBlindModeOn;
    }

    public void FreeHandle(bool isUpper)
    {
        if (!debug)
        {
            SendMotor(Handle, (byte) 0, isUpper ? (byte)0 : (byte)1, float.NaN, float.NaN, float.NaN);
        }
    }

    private Vector3 GetPositionWithObstacles(Vector3 currentPosition, Vector3 wantedPosition)
    {
        return wantedPosition;
    }

    public void ApplyForce(bool isUpper, Vector3 direction) {
        direction = direction.normalized;
        SendMotor(Handle, (byte)1, isUpper ? (byte)0 : (byte)1, direction.x, direction.z, 0);
    }

    public void UpdateHandlePosition(Vector3 position, float? rotation, bool isUpper)
    {
        if (debug)
        {
            GameObject debugObject = getDebugObject(isUpper);
            //TODO make it so position can be null
            if (position != null) debugObject.transform.position = GetPositionWithObstacles(debugObject.transform.position, (Vector3)position);
            if (rotation != null) debugObject.transform.eulerAngles = new Vector3(debugObject.transform.eulerAngles.x, (float)rotation, debugObject.transform.eulerAngles.z);
            return;
        }
        Vector2 pantoPoint = unityToPanto(new Vector2(position.x, position.z));
        if (IsInBounds(pantoPoint))
        {
            Vector2 currentPantoPoint = unityToPanto(new Vector2(lowerHandlePos.x, lowerHandlePos.z));
            if (Vector2.Distance(currentPantoPoint, pantoPoint) > 100f) {
                Debug.LogWarning("[DualPanto] Handle moving too fast: " +  Vector3.Distance(currentPantoPoint, pantoPoint));
                return;
            }
            float pantoRotation = rotation != null ? UnityToPantoRotation((float)rotation) : 0; 
            SendMotor(Handle, (byte)0, isUpper ? (byte)0 : (byte)1, pantoPoint.x, pantoPoint.y, pantoRotation);
        }
        else
        {
            Debug.LogWarning("[DualPanto] Position not in bounds: " + pantoPoint);
        }
    }

    public void SetDebugObjects(bool isUpper, Vector3? position, float? rotation)
    {
        GameObject debugObject = getDebugObject(isUpper);
        if (position != null) debugObject.transform.position = (Vector3)position;
        if (rotation != null) debugObject.transform.eulerAngles = new Vector3(0, (float)rotation, transform.eulerAngles.z);
    }

    private static float UnityToPantoRotation(float rotation)
    {
        return (-rotation) / (180f / Mathf.PI);
    }

    private static float PantoToUnityRotation(double pantoDegrees)
    {
        return (float)((180f / Mathf.PI) * -pantoDegrees);
    }

    private static Vector2 unityToPanto(Vector2 point)
    {
        if (unityBounds == null) {
            Debug.LogError("[DualPanto] Unity Bounds are null, did you forget to create a Play Area?");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        float newX = (point.x - unityBounds[0].x) * pantoBounds[1].x / unityBounds[1].x + pantoBounds[0].x;
        float newY = (point.y - unityBounds[0].y) * pantoBounds[1].y / unityBounds[1].y + pantoBounds[0].y;
        return new Vector2(newX, newY);
    }
    private static Vector2 pantoToUnity(Vector2 point)
    {
        if (unityBounds == null) {
            Debug.LogError("[DualPanto] Unity Bounds are null, did you forget to create a Play Area?");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        float newX = (point.x - pantoBounds[0].x) * unityBounds[1].x / pantoBounds[1].x + unityBounds[0].x;
        float newY = (point.y - pantoBounds[0].y) * unityBounds[1].y / pantoBounds[1].y + unityBounds[0].y;
        return new Vector2(newX, newY);
    }

    private void testConversion()
    {
        Assert.IsTrue(unityToPanto(new Vector2(25.0f, -25.0f)) == new Vector2(160.0f, -190.0f));
        Assert.IsTrue(unityToPanto(new Vector2(0.0f, 0.0f)) == new Vector2(0.0f, -110.0f));
        Assert.IsTrue(unityToPanto(new Vector2(0.0f, -25.0f)) == new Vector2(0.0f, -190.0f));

        Assert.IsTrue(new Vector2(25.0f, -25.0f) == pantoToUnity(new Vector2(160.0f, -190.0f)));
        Assert.IsTrue(new Vector2(0.0f, 0.0f) == pantoToUnity(new Vector2(0.0f, -110.0f)));
        Assert.IsTrue(new Vector2(0.0f, -25.0f) == pantoToUnity(new Vector2(0.0f, -190.0f)));
    }

    private bool IsInBounds(Vector2 point)
    {
        //should be betwwen -160 and 160
        bool hortCorrect = point.x >= (pantoBounds[0].x - pantoBounds[1].x * 0.5) && point.x <= (pantoBounds[0].x + pantoBounds[1].x * 0.5);
        //should be betwwen -30 and -190
        bool vertCorrect = point.y >= (pantoBounds[0].y - pantoBounds[1].y * 0.5) && point.y <= (pantoBounds[0].y + pantoBounds[1].y * 0.5);
        return hortCorrect && vertCorrect;
    }

    public void SetUnityBounds(Vector3 origin, Vector3 extent)
    {
        Debug.Log("[DualPanto] Play Area setting bounds");
        unityBounds = new Vector2[] { new Vector2(origin.x, origin.z), new Vector2(extent.x, extent.z) };
        //unityBounds = new Vector2[] { new Vector2(origin.x - extent.x / 4, origin.z), new Vector2(extent.x / 2, extent.z / 2) };
    }

    public void RegisterUpperHandle(UpperHandle newHandle)
    {
        upperHandle = newHandle;
    }

    public void RegisterLowerHandle(LowerHandle newHandle)
    {
        lowerHandle = newHandle;
    }

    public ushort GetNextObstacleId()
    {
        return ++currentObstacleId;
    }

    public void CreateObstacle(byte pantoIndex, ushort obstacleId, Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 pantoStartPoint = unityToPanto(startPoint);
        Vector2 pantoEndPoint = unityToPanto(endPoint);
        CreateObstacle(Handle, pantoIndex, obstacleId, pantoStartPoint.x, pantoStartPoint.y, pantoEndPoint.x, pantoEndPoint.y);
    }

    public void AddToObstacle(byte pantoIndex, ushort obstacleId, Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 pantoStartPoint = unityToPanto(startPoint);
        Vector2 pantoEndPoint = unityToPanto(endPoint);
        AddToObstacle(Handle, pantoIndex, obstacleId, pantoStartPoint.x, pantoStartPoint.y, pantoEndPoint.x, pantoEndPoint.y);
    }

    public void EnableObstacle(byte pantoIndex, ushort obstacleId)
    {
        EnableObstacle(Handle, pantoIndex, obstacleId);
    }

    public void DisableObstacle(byte pantoIndex, ushort obstacleId)
    {
        DisableObstacle(Handle, pantoIndex, obstacleId);
    }

    public void RemoveObstacle(byte pantoIndex, ushort obstacleId)
    {
        RemoveObstacle(Handle, pantoIndex, obstacleId);
    }
}
