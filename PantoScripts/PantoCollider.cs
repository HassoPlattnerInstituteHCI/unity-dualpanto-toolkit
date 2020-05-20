using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PantoCollider : PantoBehaviour
{
    protected ushort id;
    protected bool pantoEnabled = false;
    public bool onUpper = true;
    public bool onLower = true;

    public new void Awake()
    {
        base.Awake();
        id = pantoSync.GetNextObstacleId();
    }
    protected byte getPantoIndex()
    {
        if (onUpper && onLower) return 0xff;
        if (onUpper) return 0;
        if (onLower) return 1;
        return 2;
    }

    protected Vector2[] CornersFromBounds(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;
        Vector2 topRight = new Vector2(center.x + size.x / 2, center.z + size.z / 2);
        Vector2 bottomRight = new Vector2(center.x + size.x / 2, center.z - size.z / 2);
        Vector2 bottomLeft = new Vector2(center.x - size.x / 2, center.z - size.z / 2);
        Vector2 topLeft = new Vector2(center.x - size.x / 2, center.z + size.z / 2);
        return new Vector2[] { topRight, bottomRight, bottomLeft, topLeft };
    }

    public abstract void CreateObstacle();

    public void CreateBoxObstacle()
    {
        Bounds bounds = GetComponent<Collider>().bounds;
        CreateFromCorners(CornersFromBounds(bounds));
    }

    public void CreateCircularCollider(int numberOfCorners)
    {
        Vector3 center = GetComponent<SphereCollider>().center + transform.position;
        Vector3 radius = GetComponent<SphereCollider>().radius * transform.lossyScale;

        Vector2[] corners = new Vector2[numberOfCorners];
        for (var i = 0; i < numberOfCorners; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfCorners;
            float x = Mathf.Cos(angle) * radius.x;
            float z = Mathf.Sin(angle) * radius.z;
            Debug.DrawRay(center, new Vector3(x, 0, z), Color.red, 20);
            corners[i] = new Vector2(x + transform.position.x, z + transform.position.z);
        }
        CreateFromCorners(corners);
    }

    public void CreateFromCorners(Vector2[] corners)
    {
        byte index = getPantoIndex();
        if (index == 2)
        {
            Debug.LogWarning("Skipping creation for object with no handles");
        }
        pantoSync.CreateObstacle(index, id, corners[0], corners[1]);
        for (int i = 1; i < corners.Length - 1; i++)
        {
            pantoSync.AddToObstacle(index, id, corners[i], corners[i + 1]);
        }
        pantoSync.AddToObstacle(index, id, corners[corners.Length - 1], corners[0]);
    }

    public void DisableSelf()
    {
        if (!pantoEnabled)
        {
            Debug.Log("Already disabled");
            return;
        }
        pantoEnabled = false;
        GetPantoSync().DisableObstacle(getPantoIndex(), id);
    }

    public void RemoveSelf()
    {
        GetPantoSync().RemoveObstacle(getPantoIndex(), id);
    }

    public void EnableSelf()
    {
        if (pantoEnabled)
        {
            Debug.Log("Already enabled");
            return;
        }
        pantoEnabled = true;
        GetPantoSync().EnableObstacle(getPantoIndex(), id);
    }
}