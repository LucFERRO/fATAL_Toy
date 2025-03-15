using UnityEngine;

public class GridCoordinates : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3Int cellPosition;
    private HexCoordinates hexCoordinates;
    public string tiletype;
    public GameObject currentPrefab;

    void Start()
    {
        
        Grid grid = transform.parent.GetComponent<Grid>();
        cellPosition = grid.WorldToCell(transform.position);
    }


    public Vector3Int HexCoords()
    {
        return hexCoordinates.GetHexCoords();
    }

}
