using UnityEngine;

public class GridCoordinates : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3Int cellPosition;
    
    void Start()
    {
        Grid grid = transform.parent.GetComponent<Grid>();
        cellPosition = grid.WorldToCell(transform.position);
        //transform.localPosition = grid.GetCellCenterLocal(cellPosition);
    }

}
