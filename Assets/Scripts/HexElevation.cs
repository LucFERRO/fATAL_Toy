using UnityEngine;

public class HexElevation : MonoBehaviour
{
    public float elevationStep = 10f;  // Amplitude des hauteurs
    public float terrainScale = 1f; // Facteur de normalisation du Perlin Noise
    public Transform[] gridCells;
    public GridCoordinates[] gridCellsCoords;
    public GameObject[] gridObjects;
    private float perlinOffsetX;
    private float perlinOffsetY;

    void Start()
    {
        perlinOffsetX = Random.Range(0f, 1000f);
        perlinOffsetY = Random.Range(0f, 1000f);

        int childCount = transform.childCount;
        gridObjects = new GameObject[childCount];
        gridCellsCoords = new GridCoordinates[childCount];
        gridCells = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            gridObjects[i] = transform.GetChild(i).gameObject;
            gridCellsCoords[i] = gridObjects[i].GetComponent<GridCoordinates>();
            gridCells[i] = gridObjects[i].transform;
        }

        // Génération des hauteurs avec Perlin Noise
        for (int i = 0; i < gridCells.Length; i++)
        {
            float perlinX = (gridCellsCoords[i].cellPosition.x / terrainScale) + perlinOffsetX;
            float perlinY = (gridCellsCoords[i].cellPosition.z / terrainScale) + perlinOffsetY;

            float hexHeight = Mathf.PerlinNoise(perlinX, perlinY) * elevationStep;  // Hauteur basée sur Perlin Noise

            gridCells[i].position = new Vector3(gridCells[i].position.x, hexHeight, gridCells[i].position.z);
        }
    }
}
