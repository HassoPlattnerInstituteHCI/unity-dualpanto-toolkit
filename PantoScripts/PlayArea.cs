using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : PantoBehaviour
{
    public new void Awake()
    {
        base.Awake();
        RegisterPlayArea();
    }

    void RegisterPlayArea()
    {
        GetPantoSync().SetUnityBounds(this.gameObject.transform.position, this.gameObject.transform.lossyScale * 10); //Planes are measured in units of 10
        //GetPantoSync().SetUnityBounds(Vector3.zero, new Vector3(40, 0, 40));
    }
}
