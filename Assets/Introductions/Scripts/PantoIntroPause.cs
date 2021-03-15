using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PantoIntroPause : PantoIntroBase
{
    public float durationInSeconds = 1f;

    public override async Task Introduce()
    {
        await Task.Delay((int)durationInSeconds * 1000);
    }
}
