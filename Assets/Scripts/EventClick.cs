using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private GameManager gameManager;
    private PhysicalDiceSpawner diceSpawner;
    private Camera camera;
    private GridCoordinates gridCoordinates;
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        diceSpawner = GameObject.FindGameObjectWithTag("DiceSpawner").GetComponent<PhysicalDiceSpawner>();
        camera = Camera.main;
        gridCoordinates = GetComponent<GridCoordinates>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(transform.position);
            diceSpawner.SpawnDice(transform.position - camera.transform.position, camera.transform);
        }        
        if (Input.GetMouseButtonDown(1))
        {
            UpdateHex();
        }
    }

    private void UpdateHex()
    {
        Vector3Int hexPosition = this.gameObject.GetComponent<GridCoordinates>().cellPosition;
        //Debug.Log("Hex Coordinates:" + hexPosition);
        GameObject newHex = Instantiate(gameManager.chosenPrefab, transform.parent);
        newHex.transform.position = transform.position;
        GridCoordinates newGridCoordinates = newHex.GetComponent<GridCoordinates>();
        newGridCoordinates.tiletype = gameManager.chosenTileType;
        newGridCoordinates.cellPosition = hexPosition;
        //newGridCoordinates.currentPrefab = gameManager.chosenPrefab;
        Destroy(gameObject);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3Int hexPosition = this.gameObject.GetComponent<GridCoordinates>().cellPosition;
        Debug.Log("Hex Coordinates:" + hexPosition);
        gridCoordinates.tiletype = gameManager.chosenTileType;
        gridCoordinates.currentPrefab = gameManager.chosenPrefab;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
    public void OnPointerClick(PointerEventData eventData)
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerExit(PointerEventData eventData)
    {

    }


}
