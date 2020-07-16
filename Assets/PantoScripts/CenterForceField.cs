
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DualPantoFramework
{
    /// <summary>
    /// Applies a force directed at the center of the field on any object with a "MeHandle" or "ItHandle" tag within its area.
    /// </summary>
    public class CenterForceField : ForceField
    {
        [Tooltip("Positive strength will push the handle towards the center, negative strength towards the edges")]
        [Range(-1, 1)]
        public float strength;

        protected override float GetCurrentStrength(Collider other)
        {
            float dist = (Vector3.Distance(gameObject.transform.position, other.transform.position));
            return strength * dist;
        }

        protected override Vector3 GetCurrentForce(Collider other)
        {
            // try not to oscillate in the center
            if (Vector3.Distance(gameObject.transform.position, other.transform.position) < 0.4) return Vector3.zero;
            return (gameObject.transform.position - other.transform.position).normalized;
        }
    }
}