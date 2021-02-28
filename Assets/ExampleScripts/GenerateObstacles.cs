using UnityEngine;
using System.Threading.Tasks;
using DualPantoFramework;

public class GenerateObstacles : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i <= 30; i++)
        {
            float height = -8f;
            if (i < 20) height = -9f;
            if (i < 10) height = -10f;
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            float x = Random.Range(0.3f, 1.5f);
            float z = Random.Range(0.3f, 1.5f);
            go.transform.localScale = new Vector3(x, 1, z);
            go.transform.position = new Vector3(
                -4f + (i % 10) * 0.8f,
                0f,
                height
            );
            PantoBoxCollider collider = go.AddComponent<PantoBoxCollider>();
            //collider.CreateObstacle();
            //collider.Enable();
        }
    }
}
