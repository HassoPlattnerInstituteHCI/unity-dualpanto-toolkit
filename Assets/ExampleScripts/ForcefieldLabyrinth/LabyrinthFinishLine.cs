using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthFinishLine : MonoBehaviour
{
    // Start is called before the first frame update
    void OnCollisionEnter(Collision col)
    {
        // When rail is hit log time to rail
        if (col.gameObject.name == "Player")
        {
            GameObject gameManager = GameObject.Find("Manager");
            gameManager.GetComponent<LabyrinthGameManager>().LevelCompleted();

        }
    }
}
