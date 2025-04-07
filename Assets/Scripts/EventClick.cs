using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private PhysicalDiceSpawner diceSpawner;
    private Camera cam;
    private GridCoordinates gridCoordinates;




    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (gameManager == null)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        if (diceSpawner == null)
        {
            diceSpawner = GameObject.FindGameObjectWithTag("DiceSpawner").GetComponent<PhysicalDiceSpawner>();
        }
        cam = Camera.main;
        gridCoordinates = GetComponent<GridCoordinates>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            diceSpawner.SpawnDice(transform.position - cam.transform.position, cam.transform);
        }
        if (Input.GetMouseButtonDown(1))
        {
            ToggleLockTile();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateHex();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(GridCoordinates.currentLockedTiles);
        }
    }

    private void ToggleLockTile()
    {
        if (GridCoordinates.currentLockedTiles == gameManager.maxLockedTiles && !gridCoordinates.IsLocked)
        {
            return;
        }
        gridCoordinates.IsLocked = !gridCoordinates.IsLocked;
        GetComponent<GlowingHexes>().glowMaterial = GetComponent<GlowingHexes>().lockedGlowMaterial;
    }

    private void UpdateHex()
    {
        Vector3Int hexPosition = gridCoordinates.cellPosition;
        GameObject newHex = Instantiate(gameManager.chosenPrefab, transform.parent);
        newHex.transform.position = transform.position;

        GridCoordinates newGridCoordinates = newHex.GetComponent<GridCoordinates>();
        newGridCoordinates.tiletype = gameManager.chosenTileType;
        newGridCoordinates.cellPosition = hexPosition;

        Destroy(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3Int hexPosition = gridCoordinates.cellPosition;
        //Debug.Log("Hex Coordinates:" + hexPosition);

        gridCoordinates.tiletype = gameManager.chosenTileType;
        gridCoordinates.currentPrefab = gameManager.chosenPrefab;
    }

    public void OnPointerUp(PointerEventData eventData) { }
    public void OnPointerClick(PointerEventData eventData) { }
    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData) { }
}
