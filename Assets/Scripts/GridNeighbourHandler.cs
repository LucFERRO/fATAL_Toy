using UnityEngine;

public class GridNeighbourHandler : MonoBehaviour
{
    public NeighbourTileProcessor[] neighbourTileProcessors;
    public GameObject[] neighbourTileGOs;
    //private
    void Start()
    {
        neighbourTileProcessors = new NeighbourTileProcessor[neighbourTileGOs.Length];
        for (int i = 0; i < neighbourTileGOs.Length; i++)
        {
            neighbourTileProcessors[i] = neighbourTileGOs[i].transform.GetChild(0).GetComponent<NeighbourTileProcessor>();
        }
    }

    // Update is called once per frame
    //void OnMouseOver()
    //{
    //    if(Input.GetKeyDown(KeyCode.M)) {
    //        foreach (var item in neighbourTileProcessors)
    //        {
    //            item.GetComponent<MeshRenderer>().material.color = Color.red;
    //        }
    //    }
    //}
}
