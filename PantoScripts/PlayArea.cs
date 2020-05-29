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
        //GetPantoSync().SetUnityBounds(this.gameObject.transform.position, this.gameObject.transform.lossyScale * 10); //Planes are measured in units of 10
        GetPantoSync().SetUnityBounds(new Vector3(0, 0, -11), new Vector3(42, 0, 22)); //Planes are measured in units of 10
    }
}
