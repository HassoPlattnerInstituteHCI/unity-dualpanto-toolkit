using UnityEngine;

namespace DualPantoFramework
{
    public enum SpeedControlStrategy
    {
        MAX_SPEED,
        EXPLORATION,
        LEASH
    }
    public class Settings : PantoBehaviour
    {
        public bool tetheringEnabled;
        public float tetherFactor;
        public float innerRadius;
        public float outerRadius;
        public bool pockEnabled;
        public SpeedControlStrategy speedControlOption;
        void SetTetherFactor(float newTetherFactor)
        {
            if (newTetherFactor > 1.0f || newTetherFactor < 0.0f)
            {
                Debug.LogWarning("[DualPanto] Tether factor should be in range (0, 1)");
            }
            tetherFactor = Mathf.Max(Mathf.Min(newTetherFactor, 1.0f), 0.0f);
            UpdateSettings();
        }
        void SetInnerRadius(float newRadius)
        {
            innerRadius = newRadius;
            UpdateSettings();
        }
        void SetOuterRadius(float newRadius)
        {
            innerRadius = newRadius;
            UpdateSettings();
        }
        void SetSpeedControlOption(SpeedControlStrategy option)
        {
            speedControlOption = option;
            UpdateSettings();
        }

        void UpdateSettings()
        {
            if (!pantoSync.debug)
            {
                Debug.Log("updating settings");
                pantoSync.SetSpeedControl(tetheringEnabled, tetherFactor, innerRadius, outerRadius, speedControlOption, pockEnabled);
                //SyncSettings(tetherFactor, innerRadius, outerRadius, speedControlOption);
            }
        }

        void Start()
        {
            UpdateSettings();
        }
    }
}