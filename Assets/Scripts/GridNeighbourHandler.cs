using UnityEngine;

public class GridNeighbourHandler : MonoBehaviour
{
    public NeighbourTileProcessor[] neighbourTileProcessors;
    public GameObject[] neighbourTileGOs;
    //private
    void Start()
    {
        neighbourTileProcessors = new NeighbourTileProcessor[neighbourTileGOs.Length];
        UpdateNeighbourTiles();
    }

    public void UpdateNeighbourTiles()
    {
        for (int i = 0; i < neighbourTileGOs.Length; i++)
        {
            neighbourTileProcessors[i] = neighbourTileGOs[i].transform.GetChild(0).GetComponent<NeighbourTileProcessor>();
        }
    }
}