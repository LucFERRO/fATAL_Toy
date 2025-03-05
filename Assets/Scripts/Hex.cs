using UnityEngine;

public class Hex : MonoBehaviour
{
    private HexCoordinates hexCoordinates;


    public Vector3Int HexCoords()
    {
        return hexCoordinates.GetHexCoords();
    }



}
