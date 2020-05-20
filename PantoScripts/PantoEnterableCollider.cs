using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PantoEnterableCollider : PantoBehaviour
{
    [Range(0, 5)]
    public float enterStrength;
    [Range(0, 5)]
    public float leaveStrength;

    void Start()
    {
        //PantoCollider pantoCollider = GetComponent<PantoCollider>();
        //TODO get bounds, create Trigger that is enterStrength bigger (and smaller)

        //Get God Object Position
        //When god object enters the inner one?? disable shortly so both can move inside

    }
}
