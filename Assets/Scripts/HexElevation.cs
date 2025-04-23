using UnityEngine;

public class HexElevation : MonoBehaviour
{
    public float elevationStep = 3f; // difference between highs and lows
    public float terrainScale = 5f; // level of details
    [Range(0,20)] public int elevationRoundNumber;
    private float hexHeight;

    public Transform[] gridCells;
    public NeighbourTileProcessor[] gridCellsCoords;
    public GameObject[] gridObjects;

    private float perlinOffsetX;
    private float perlinOffsetY;

    void Start()
    {
        perlinOffsetX = Random.Range(0f, 1000f);
        perlinOffsetY = Random.Range(0f, 1000f);

        int childCount = transform.childCount;
        gridObjects = new GameObject[childCount];
        gridCellsCoords = new NeighbourTileProcessor[childCount];
        gridCells = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            gridObjects[i] = transform.GetChild(i).gameObject;
            gridCellsCoords[i] = gridObjects[i].transform.GetChild(0).GetComponent<NeighbourTileProcessor>();
            gridCells[i] = gridObjects[i].transform;
        }

        for (int i = 0; i < gridCells.Length; i++)
        {
            float perlinX = (gridCellsCoords[i].cellPosition.x / terrainScale) + perlinOffsetX;
            float perlinY = (gridCellsCoords[i].cellPosition.z / terrainScale) + perlinOffsetY;


            if (elevationRoundNumber > 0)
            {
                hexHeight = Mathf.PerlinNoise(perlinX, perlinY);
                hexHeight = RoundedElevation(hexHeight);
            }
            else
            {
                hexHeight = Mathf.PerlinNoise(perlinX, perlinY) * elevationStep;
            }

            gridCells[i].position = new Vector3(gridCells[i].position.x, hexHeight, gridCells[i].position.z);
        }
    }
    private float[] CreateRoundArray(int partNumber)
    {
        float[] roundArray = new float[partNumber+1];
        for (int i = 0; i < partNumber+1; i++)
        {
            roundArray[i] = (float)i / partNumber;
        }
        return roundArray;
    }
    private float RoundedElevation(float elevation)
    {
        float[] roundArray = CreateRoundArray(elevationRoundNumber);

        for (int i = 0; i < elevationRoundNumber; i++)
        {
            if (elevation > roundArray[i] && elevation < roundArray[i + 1])
            {
                elevation = roundArray[i];
            }
        }
        return elevation;
    }
}
