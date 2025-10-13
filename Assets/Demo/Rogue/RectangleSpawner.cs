using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleSpawner : MonoBehaviour
{
    public GridManager gridManager;
    public GameObject rectanglePrefab;

    public int minRectSize = 1;
    public int maxRectSize = 4;
    public int spawnCount = 2;

    void Start()
    {
        if (gridManager == null || rectanglePrefab == null)
        {
            Debug.LogError("Missing GridManager or RectanglePrefab.");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnRectangle();
        }
    }

    void SpawnRectangle()
    {
        int maxAttempts = 100;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Clamp rectangle size to available grid dimensions
            int maxWidth = Mathf.Min(maxRectSize, gridManager.gridWidth);
            int maxHeight = Mathf.Min(maxRectSize, gridManager.gridHeight);

            int width = Random.Range(minRectSize, maxWidth + 1);
            int height = Random.Range(minRectSize, maxHeight + 1);

            int x = Random.Range(0, gridManager.gridWidth - width + 1);
            int y = Random.Range(0, gridManager.gridHeight - height + 1);
    
            if (gridManager.CanPlaceRectangle(x, y, width, height))
            {
                gridManager.OccupyCells(x, y, width, height);

                Vector3 worldPos = gridManager.GetWorldPosition(x, y);
                Vector3 centerOffset = new Vector3(width * gridManager.cellSize, 0, height * gridManager.cellSize) * 0.5f;
                Vector3 spawnPos = worldPos + centerOffset;

                GameObject rect = Instantiate(rectanglePrefab, spawnPos, Quaternion.identity);
                rect.transform.localScale = new Vector3(width * gridManager.cellSize, 1f, height * gridManager.cellSize);

                return;
            }
        }

        Debug.LogWarning("Failed to place rectangle after many attempts.");
    }
}