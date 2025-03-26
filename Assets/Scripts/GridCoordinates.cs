using UnityEngine;

public class GridCoordinates : MonoBehaviour
{
    public Vector3Int cellPosition;
    public string tiletype;
    public GameObject currentPrefab;
    private Grid grid;

    void Start()
    {
        grid = transform.parent.GetComponent<Grid>();
        cellPosition = grid.WorldToCell(transform.position);

    }

    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GetNeighbourTiles(cellPosition.x == 0, cellPosition.x == 8, cellPosition.z == 0, cellPosition.z == 8);
        }
    }

    private void GetNeighbourTiles(bool isLeft, bool isRight, bool isBot, bool isTop)
    {
        for (int i = (isLeft ? 1 : -1); i < (isRight ? 1 : 2); i += 2)
        {
            for (int j = (isBot ? 0 : -1); j < (isTop ? 1 : 2); j++)
            {
                GetTileAtCoordinates(new Vector3Int(cellPosition.x + i, cellPosition.y, cellPosition.z + j));
            }
        }
    }

    private void GetTileAtCoordinates(Vector3Int cellCoordinates)
    {
        Vector3 coordinates = grid.CellToWorld(cellCoordinates);
        coordinates.y += 10;
        GameObject foundObject;
        Ray ray = new Ray(coordinates, Vector3.down);
        RaycastHit hit;

        Physics.Raycast(ray, out hit);
        //if (hit.collider == null)
        //{
        //    return;
        //}
        foundObject = hit.collider.gameObject;
        Debug.Log(foundObject.name);
        return;
        //return foundObject;
    }
}