using UnityEngine;

namespace DualPantoFramework
{
    public enum SpeedControlOption
    {
        PENALIZATION,
        EXPLORATION,
        LEASH
    }
    public class Settings : PantoBehaviour
    {
        public float tetherFactor;
        public float innerRadius;
        public float outerRadius;
        public SpeedControlOption speedControlOption;
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
        void SetSpeedControlOption(SpeedControlOption option)
        {
            speedControlOption = option;
            UpdateSettings();
        }

        void UpdateSettings()
        {
            if (!pantoSync.debug)
            {
                //SyncSettings(tetherFactor, innerRadius, outerRadius, speedControlOption);
            }
        }
    }
}