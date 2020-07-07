using UnityEngine;
using DualPantoFramework;

public class MovingIt : MonoBehaviour
{
    PantoHandle itHandle;
    Vector3 direction = Vector3.right;
    bool movementStarted = false;
    public float speed = 0.2f;
    async void Start()
    {
        itHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        await itHandle.SwitchTo(gameObject, 0.4f);
        movementStarted = true;
    }

    void Update()
    {
        if (!movementStarted) return;
        if (transform.position.x > 12 || transform.position.x < -12)
        {
            direction *= -1;
        }
        transform.position += direction * speed;
    }
}
