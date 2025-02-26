using System;
using UnityEngine;

public class HexCoordinates : MonoBehaviour
{
    private static float xOffset = 0.866f, yOffset = 1f, zOffset = 1.5f;
    public float displayX;

    [Header("Offset coordinates")]
    [SerializeField] private Vector3Int offsetCoordinates;

    private void Awake()
    {
        offsetCoordinates = ConvertPositionToOffset(transform.position);
    }

    private Vector3Int ConvertPositionToOffset(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / xOffset);
        displayX = position.x / xOffset;
        int z = Mathf.FloorToInt(position.z / zOffset);
        return new Vector3Int(x, z);
    }

    private void OnMouseOver()
    {
        Debug.Log($"Coordonnées : {offsetCoordinates.x} ; {offsetCoordinates.y} ");
    }
}

