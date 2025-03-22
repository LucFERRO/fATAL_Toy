using UnityEngine;

public class GridCoordinates : MonoBehaviour
{
    public Vector3Int cellPosition;
    public string tiletype;
    public GameObject currentPrefab;

    void Start()
    {
        Grid grid = transform.parent.GetComponent<Grid>();
        cellPosition = grid.WorldToCell(transform.position);
    }
}
