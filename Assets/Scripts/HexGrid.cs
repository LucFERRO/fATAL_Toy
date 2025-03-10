using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{

    Dictionary<Vector3Int, GridCoordinates> hexTileDict;
    Dictionary<Vector3Int, List<Vector3Int>> hexTileNeighbourDict;

    private void Start()
    {
        hexTileDict = new Dictionary<Vector3Int, GridCoordinates>();
        new Dictionary<Vector3Int, List<Vector3Int>>();
        foreach (GridCoordinates hex in FindObjectsByType<GridCoordinates>(FindObjectsSortMode.None))
        {
            hexTileDict[hex.HexCoords()] = hex;
        }

        List<Vector3Int> neighbours = GetNeighboursFor(new Vector3Int(0, 0, 0));
        Debug.Log("Neighbours for (0,0,0) are:");
        foreach (Vector3Int neighboursPos in neighbours)
        {
            Debug.Log(neighboursPos);
        }
    }

    public GridCoordinates GetTileAt(Vector3Int hexCoords)
    {
        GridCoordinates result = null;
        hexTileDict.TryGetValue(hexCoords, out result);
        return result;
    }

    public List<Vector3Int> GetNeighboursFor(Vector3Int hexCoords)
    {
        if (hexTileDict.ContainsKey(hexCoords) == false)
        {
            return new List<Vector3Int>();
        };

        if (hexTileDict.ContainsKey(hexCoords))
        {
            return hexTileNeighbourDict[hexCoords];
        };

        hexTileNeighbourDict.Add(hexCoords, new List<Vector3Int>());

        foreach (Vector3Int direction in Direction.GetDirectionList(hexCoords.z))
        {
            if (hexTileDict.ContainsKey(hexCoords + direction))
            {
                hexTileNeighbourDict[hexCoords].Add(hexCoords + direction);
            }
        }

        return hexTileNeighbourDict[hexCoords];
    }

    public static class Direction
    {
        public static Vector3Int[] directionOffsetOdd = new Vector3Int[] {
            new Vector3Int(-1, 0, 1), //N1
            new Vector3Int(0, 0, 1), //N2
            new Vector3Int(1, 0, 0), //E
            new Vector3Int(0, 0, -1), //S2
            new Vector3Int(-1, 0, -1), //S1
            new Vector3Int(-1, 0, 0), //W
        };

        public static Vector3Int[] directionOffsetEven = new Vector3Int[] {
            new Vector3Int(0, 0, 1), //N1
            new Vector3Int(1, 0, 1), //N2
            new Vector3Int(1, 0, 0), //E
            new Vector3Int(1, 0, -1), //S2
            new Vector3Int(0, 0, -1), //S1
            new Vector3Int(-1, 0, 0), //W
        };

        public static Vector3Int[] GetDirectionList(int z)
        {
            return z % 2 == 0 ? directionOffsetEven : directionOffsetOdd;
        }
    }
}
