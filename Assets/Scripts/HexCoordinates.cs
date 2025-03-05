using System;
using UnityEngine;

public class HexCoordinates : MonoBehaviour
{
    private static float xOffset = 1.732f, yOffset = 1f, zOffset = 1.5f;
    public float displayX;

    [Header("Offset coordinates")]
    [SerializeField] private Vector3Int offsetCoordinates;

    internal Vector3Int GetHexCoords()
    {
        throw new NotImplementedException();
    }

    private void Awake()
    {
        offsetCoordinates = ConvertPositionToOffset(transform.position);
    }

    private Vector3Int ConvertPositionToOffset(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / xOffset);
        displayX = position.x / xOffset;
        int z = Mathf.FloorToInt(position.z / zOffset);
        return new Vector3Int(x, z);
    }

    private void OnMouseOver()
    {
        Debug.Log($"Coordonnées : {offsetCoordinates.x} ; {offsetCoordinates.y} ");
    }
}

