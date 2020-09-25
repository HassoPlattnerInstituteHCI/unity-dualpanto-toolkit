using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DualPantoFramework
{
    /// <summary>
    /// The main DualPanto class. Takes care of the communication with the Panto.
    /// </summary>
    public class DualPantoSync : MonoBehaviour
    {
        public delegate void SyncDelegate(ulong handle);
        public delegate void HeartbeatDelegate(ulong handle);
        public delegate void LoggingDelegate(IntPtr msg);
        public delegate void PositionDelegate(ulong handle, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8, SizeConst = 10)] double[] positions);
        public UIManager uiManager;
        public string portName = "//.//COM3";
        [Header("When Debug is enabled, the emulator mode will be used. You do not need to be connected to a Panto for this mode.")]
        public bool debug = false;
        public float debugRotationSpeed = 10.0f;
        public KeyCode toggleVisionKey = KeyCode.B;
        public bool showRawValues = true;
        protected ulong Handle;
        private static LowerHandle lowerHandle;
        private static UpperHandle upperHandle;

        // bounds are defined by center and extent
        //private static Vector2[] pantoBounds = { new Vector2(0, -110), new Vector2(320, 160) }; // for version D
        private static Vector2[] pantoBounds = { new Vector2(0, -100), new Vector2(360, 210) }; // ember
        private static Vector2[] unityBounds;
        private Vector3 upperHandlePos;
        private Vector3 lowerHandlePos;
        private Vector3 upperGodObject;
        private Vector3 lowerGodObject;
        private float lowerHandleRot = 0f;
        private float upperHandleRot = 0f;

        private bool isBlindModeOn = false;
        private ushort currentObstacleId = 0;
        private GameObject debugLowerHandle;
        private GameObject debugUpperHandle;
        private GameObject debugLowerGodObject;
        private GameObject debugUpperGodObject;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        private const string plugin = "serial";
#else
        private const string plugin = "libserial";
#endif

        private static bool connected = false;

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
        private static extern void SendSpeed(ulong handle, byte pantoIndex, float speed);
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

        void Start()
        {
            Application.targetFrameRate = 60;
        }
        private static void SyncHandler(ulong handle)
        {
            Debug.Log("[DualPanto] Received sync");
            connected = true;
            SendSyncAck(handle);
        }

        private void HeartbeatHandler(ulong handle)
        {
            Debug.Log("[DualPanto] Received heartbeat");
            if (showRawValues) uiManager.UpdateHeartbeat();
            SendHeartbeatAck(handle);
        }

        private static void LogHandler(IntPtr msg)
        {
            String message = Marshal.PtrToStringAnsi(msg);
            if (message.Contains("Free heap") || message.Contains("Task \"Physics\"") || message.Contains("Task \"I/O\"") || message.Contains("Encoder") || message.Contains("SPI"))
            {
                return;
            }
            else if (message.Contains("disconnected"))
            {
                Debug.LogError("[DualPanto] " + message);
            }
            else
            {
                Debug.Log("[DualPanto] " + message);
            }
        }

        private void PositionHandler(ulong handle, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8, SizeConst = 10)] double[] positions)
        {
            Vector2 unityPosUpper = PantoToUnity(new Vector2((float)positions[0], (float)positions[1]));
            Vector2 unityGodUpper = PantoToUnity(new Vector2((float)positions[3], (float)positions[4]));
            upperHandlePos = new Vector3(unityPosUpper.x, 0, unityPosUpper.y);
            upperHandleRot = PantoToUnityRotation(positions[2]);
            upperGodObject = new Vector3(unityGodUpper.x, 0, unityGodUpper.y);
            upperHandle.SetPositions(upperHandlePos, upperHandleRot, upperGodObject);

            Vector2 unityPosLower = PantoToUnity(new Vector2((float)positions[5], (float)positions[6]));
            Vector2 unityGodLower = PantoToUnity(new Vector2((float)positions[8], (float)positions[9]));
            lowerHandlePos = new Vector3(unityPosLower.x, 0, unityPosLower.y);
            lowerHandleRot = PantoToUnityRotation(positions[7]);
            lowerGodObject = new Vector3(unityGodLower.x, 0, unityGodLower.y);
            lowerHandle.SetPositions(lowerHandlePos, lowerHandleRot, lowerGodObject);

            Quaternion lower = Quaternion.Euler(0, lowerHandleRot, 0);
            Quaternion upper = Quaternion.Euler(0, upperHandleRot, 0);
            Debug.DrawLine(lowerHandlePos + lower * Vector3.back * 0.5f, lowerHandlePos + lower * Vector3.forward, Color.black);
            Debug.DrawLine(lowerHandlePos + lower * Vector3.left * 0.5f, lowerHandlePos + lower * Vector3.right * 0.5f, Color.black);

            Debug.DrawLine(upperHandlePos + upper * Vector3.back * 0.5f, upperHandlePos + upper * Vector3.forward, Color.black);
            Debug.DrawLine(upperHandlePos + upper * Vector3.left * 0.5f, upperHandlePos + upper * Vector3.right * 0.5f, Color.black);
            if (showRawValues) uiManager.UpdateValues(positions);
        }

        private static ulong OpenPort(string port)
        {
            return Open(Marshal.StringToHGlobalAnsi(port));
        }

        public GameObject GetDebugObject(bool isUpper)
        {
            if (isUpper)
            {
                return debugUpperHandle;
            }
            else
            {
                return debugLowerHandle;
            }
        }

        public GameObject GetDebugGodObject(bool isUpper)
        {
            if (isUpper)
            {
                return debugUpperGodObject;
            }
            else
            {
                return debugLowerGodObject;
            }
            
        }

        public void StartInDebug()
        {
            debug = true;
            uiManager.ShowPortWindow(false);
        }

        public void SetPort(string name)
        {
            portName = name;
            uiManager.UpdatePort(portName);
            uiManager.ShowPortWindow(false);
            Handle = OpenPort(portName);
            if (Handle == (ulong)0)
            {
                uiManager.ShowPortWindow(true);
            }
        }

        void ParseCommandLineArguments()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            for (int i = 0; i < arguments.Length; i++)
            {
                string arg = arguments[i];
                if (arg == "--debug" || arg == "-d")
                {
                    debug = true;
                }
                if (arg == "--portName")
                {
                    if (arguments.Length < i + 1)
                    {
                        Debug.LogError("Not enough arguments");
                    }
                    else
                    {
                        portName = arguments[i + 1];
                    }
                }
            }
        }

        void Awake()
        {
            ParseCommandLineArguments();

            Vector3 handleDefaultPosition = transform.position + new Vector3(0, 0, 3);
            upperHandlePos = handleDefaultPosition;
            lowerHandlePos = handleDefaultPosition;
            CreateDebugObjects(handleDefaultPosition);
            if (!debug)
            {
                if (showRawValues) SetUpDebugValuesWindow();
                globalSync = this;
                SetLoggingHandler(LogHandler);
                SetSyncHandler(SyncHandler);
                SetHeartbeatHandler(StaticHeartbeatHandler);
                SetPositionHandler(StaticPositionHandler);
                SetPort(portName);

                // keep polling until we receive the first SYNC (which we ACK in the handler and set connected)
                // only then everyone else can start sending their own stuff
                while (!connected)
                {
                    Poll(Handle);
                }
            }
            else
            {
                StartInDebug();
            }
        }

        void SetUpDebugValuesWindow()
        {
            uiManager.UpdateRevisionID((int)GetRevision());
            uiManager.UpdatePort(portName);
            uiManager.ShowDebugValuesWindow();
        }

        static DualPantoSync globalSync;
        static void StaticPositionHandler(ulong handle, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8, SizeConst = 10)] double[] positions)
        {
            globalSync.PositionHandler(handle, positions);
        }

        static void StaticHeartbeatHandler(ulong handle)
        {
            globalSync.HeartbeatHandler(handle);
        }

        private void CreateDebugObjects(Vector3 position)
        {
            UnityEngine.Object prefab = Resources.Load("ItHandlePrefab");
            debugLowerHandle = Instantiate(prefab) as GameObject;
            debugLowerHandle.transform.position = position;

            prefab = Resources.Load("MeHandlePrefab");
            debugUpperHandle = Instantiate(prefab) as GameObject;
            debugUpperHandle.transform.position = position;

            debugUpperGodObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugUpperGodObject.transform.position = position;
            debugUpperGodObject.transform.localScale = new Vector3(1, 1, 1);
            debugLowerGodObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugLowerGodObject.transform.position = position;
            debugLowerGodObject.transform.localScale = new Vector3(1, 1, 1);
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
                float mouseRotation = Input.GetAxis("Horizontal") * debugRotationSpeed * Time.deltaTime * 60f;
                Vector3 position = new Vector3(mousePosition.x, 0.0f, mousePosition.z);
                float r = debugUpperHandle.transform.eulerAngles.y + mouseRotation;
                upperHandlePos = position;
                upperHandle.SetPositions(upperHandlePos, r, null);

                lowerHandleRot = debugLowerHandle.transform.eulerAngles.y + mouseRotation;
                lowerHandlePos = position;
                lowerHandle.SetPositions(lowerHandlePos, r, null);
            }

            if (Input.GetKeyDown(toggleVisionKey))
            {
                ToggleBlindMode();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();
            }
        }

        private void ToggleBlindMode()
        {
            if (!debug)
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
                SendMotor(Handle, (byte)0, isUpper ? (byte)0 : (byte)1, float.NaN, float.NaN, float.NaN);
            }
        }

        private Vector3 GetPositionWithObstacles(Vector3 currentPosition, Vector3 wantedPosition)
        {
            return wantedPosition;
        }

        public void ApplyForce(bool isUpper, Vector3 direction, float strength)
        {
            strength = Mathf.Min(strength, 1);
            direction = direction.normalized * strength;
            if (!debug)
            {
                SendMotor(Handle, (byte)1, isUpper ? (byte)0 : (byte)1, direction.x, direction.z, 0);
            }
        }

        public void UpdateHandlePosition(Vector3 position, float? rotation, bool isUpper)
        {
            if (debug)
            {
                GameObject debugObject = GetDebugObject(isUpper);
                //TODO: make it so position can be null

                //TODO: also update the GodObject
                if (position != null) debugObject.transform.position = GetPositionWithObstacles(debugObject.transform.position, (Vector3)position);
                if (rotation != null) debugObject.transform.eulerAngles = new Vector3(debugObject.transform.eulerAngles.x, (float)rotation, debugObject.transform.eulerAngles.z);
                return;
            }
            Vector2 pantoPoint = UnityToPanto(new Vector2(position.x, position.z));
            if (IsInBounds(pantoPoint))
            {
                Vector2 currentPantoPoint = new Vector2();
                if (isUpper) currentPantoPoint = UnityToPanto(new Vector2(upperHandlePos.x, upperHandlePos.z));
                else currentPantoPoint = UnityToPanto(new Vector2(lowerHandlePos.x, lowerHandlePos.z));
                float pantoRotation = rotation != null ? UnityToPantoRotation((float)rotation) : 0;
                SendMotor(Handle, (byte)0, isUpper ? (byte)0 : (byte)1, pantoPoint.x, pantoPoint.y, pantoRotation);
            }
            else
            {
                Debug.LogWarning("[DualPanto] Position not in bounds: " + pantoPoint);
            }
        }

        public void SetSpeed(bool isUpper, float speed)
        {
            SendSpeed(Handle, isUpper ? (byte)0 : (byte)1, speed);
        }

        public void SetDebugObjects(bool isUpper, Vector3? position, float? rotation)
        {
            GameObject debugObject = GetDebugObject(isUpper);
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

        private Vector2 UnityToPanto(Vector2 point)
        {
            float x = (point.x - transform.position.x) / transform.localScale.x * 10;
            float y = (point.y - transform.position.z) / transform.localScale.z * 10;
            return new Vector2(x, y);
        }

        private Vector2 PantoToUnity(Vector2 point)
        {
            float x = (point.x / 10) * transform.localScale.x + transform.position.x;
            float y = (point.y / 10) * transform.localScale.z + transform.position.z;
            return new Vector2(x, y);
        }

        private bool IsInBounds(Vector2 point)
        {
            bool hortCorrect = point.x >= (pantoBounds[0].x - pantoBounds[1].x * 0.5) && point.x <= (pantoBounds[0].x + pantoBounds[1].x * 0.5);
            bool vertCorrect = point.y >= (pantoBounds[0].y - pantoBounds[1].y * 0.5) && point.y <= (pantoBounds[0].y + pantoBounds[1].y * 0.5);
            return hortCorrect && vertCorrect;
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
            if (!debug)
            {
                Vector2 pantoStartPoint = UnityToPanto(startPoint);
                Vector2 pantoEndPoint = UnityToPanto(endPoint);
                CreateObstacle(Handle, pantoIndex, obstacleId, pantoStartPoint.x, pantoStartPoint.y, pantoEndPoint.x, pantoEndPoint.y);
            }
        }

        public void AddToObstacle(byte pantoIndex, ushort obstacleId, Vector2 startPoint, Vector2 endPoint)
        {
            if (!debug)
            {
                Vector2 pantoStartPoint = UnityToPanto(startPoint);
                Vector2 pantoEndPoint = UnityToPanto(endPoint);
                AddToObstacle(Handle, pantoIndex, obstacleId, pantoStartPoint.x, pantoStartPoint.y, pantoEndPoint.x, pantoEndPoint.y);
            }
        }

        public void EnableObstacle(byte pantoIndex, ushort obstacleId)
        {
            if (!debug)
            {
                EnableObstacle(Handle, pantoIndex, obstacleId);
            }
        }

        public void DisableObstacle(byte pantoIndex, ushort obstacleId)
        {
            if (!debug)
            {
                DisableObstacle(Handle, pantoIndex, obstacleId);
            }
        }

        public void RemoveObstacle(byte pantoIndex, ushort obstacleId)
        {
            if (!debug)
            {
                RemoveObstacle(Handle, pantoIndex, obstacleId);
            }
        }
    }
}