using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex) 
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell positiojn {pos}");
            placedObjects[pos] = data;
        }
    }


    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize) 
    {
        // Debug.Log(gridPosition);
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        List<Vector3Int> positionOfNeighbors = GetPositionOfNeighbors(gridPosition, objectSize);
        foreach (var pos in positionOfNeighbors) 
        {
            if (placedObjects.ContainsKey(pos))
                return true;
        }
        return false;
    }

    private List<Vector3Int> GetPositionOfNeighbors(Vector3Int gridPosition, Vector2Int objectSize) 
    {
        List<Vector3Int> returnVal = new();
        if (IsInRange(gridPosition + new Vector3Int(0, 1, 0)))
            returnVal.Add(gridPosition + new Vector3Int(0, 1, 0));

        if (IsInRange(gridPosition + new Vector3Int(0, -1, 0)))
            returnVal.Add(gridPosition + new Vector3Int(0, -1, 0));

        if (IsInRange(gridPosition + new Vector3Int(1, 0, 0)))
            returnVal.Add(gridPosition + new Vector3Int(1, 0, 0));

        if (IsInRange(gridPosition + new Vector3Int(-1, 0, 0)))
            returnVal.Add(gridPosition + new Vector3Int(-1, 0, 0));
        return returnVal;
    }
    bool IsInRange(Vector3Int vector)
    {
        return vector.x >= -2 && vector.x <= 2 && vector.y >= -2 && vector.y <= 2;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}
