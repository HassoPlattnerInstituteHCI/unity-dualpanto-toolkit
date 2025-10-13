using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject plane;      // The Plane GameObject in the scene
    public float cellSize = 1f;

    [HideInInspector] public int gridWidth;
    [HideInInspector] public int gridHeight;

    private bool[,] grid;

    void Awake()
    {
        CalculateGridFromPlane();
        grid = new bool[gridWidth, gridHeight];
    }

    void CalculateGridFromPlane()
    {
        if (plane == null)
        {
            Debug.LogError("Plane not assigned to GridManager.");
            return;
        }

        // Unity's Plane primitive is 10x10 units in size by default
        Vector3 worldScale = plane.transform.localScale;
        float totalWidth = 10f * worldScale.x;
        float totalHeight = 10f * worldScale.z;

        gridWidth = Mathf.FloorToInt(totalWidth / cellSize);
        gridHeight = Mathf.FloorToInt(totalHeight / cellSize);
    }

    public bool IsCellFree(int x, int y)
    {
        return x >= 0 && x < gridWidth &&
               y >= 0 && y < gridHeight &&
               !grid[x, y];
    }

    public bool CanPlaceRectangle(int x, int y, int width, int height)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                if (!IsCellFree(i, j)) return false;
            }
        }
        return true;
    }

    public void OccupyCells(int x, int y, int width, int height)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                grid[i, j] = true;
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        // Align cell centers to the middle of the plane
        float originX = plane.transform.position.x - (gridWidth * cellSize) / 2f + cellSize / 2f;
        float originZ = plane.transform.position.z - (gridHeight * cellSize) / 2f + cellSize / 2f;

        return new Vector3(originX + x * cellSize, 0, originZ + y * cellSize);
    }
}