using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DualPantoToolkit
{
    public class GridForceField : ForceField
    {
        [Tooltip("Positive strength will pull towards the center, negative strength will repel")]
        [Range(-1f, 1f)]
        public float strength = 0.8f;

        public bool isAttractive = true;

        [Tooltip("Max force applied at center or edge depending on attraction")]
        public float maxForce = 1.2f;

        [Tooltip("Minimum distance to avoid oscillation near the center")]
        public float deadZoneRadius = 0.3f;

        [Tooltip("Radius where the force starts to fall off near the edges")]
        public float falloffRadius = 0.4f;

        protected override float GetCurrentStrength(Collider other)
        {
            float dist = Vector3.Distance(transform.position, other.transform.position);
            float radius = transform.localScale.x / 2;

            // Falloff zone: linear interpolation for smoother transition
            float falloffFactor = 1f;
            if (dist > radius - falloffRadius)
            {
                falloffFactor = Mathf.Clamp01((radius - dist) / falloffRadius);
            }

            float effectiveStrength = strength * dist * falloffFactor;
            return isAttractive ? effectiveStrength : -effectiveStrength;
        }

        protected override Vector3 GetCurrentForce(Collider other)
        {
            Vector3 direction = transform.position - other.transform.position;
            float distance = direction.magnitude;

            // Avoid oscillation when too close to the center
            if (isAttractive && distance < deadZoneRadius)
                return Vector3.zero;

            Vector3 forceDir = direction.normalized;

            // Optional: smoother force fade-out near the edge
            float radius = transform.localScale.x / 2;
            float falloffFactor = 1f;
            if (distance > radius - falloffRadius)
            {
                falloffFactor = Mathf.Clamp01((radius - distance) / falloffRadius);
            }

            return forceDir * maxForce * falloffFactor * (isAttractive ? 1 : -1);
        }
    }
}
