using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("Data")]
    public bool isInventoryOpen;
    public bool IsInventoryOpen
    {
        get { return isInventoryOpen; }
        set
        {
            isInventoryOpen = value;
            //biomesUiGO.SetActive(value);
            //baseBiomesGO.SetActive(value);
            //doubleBiomesGO.SetActive(value);
            //comboBiomesGO.SetActive(value);
        }
    }
    private Vector3 startingPosition;
    [SerializeField] private int speed;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float moveOffset = 5f;
    [SerializeField] private float snapThreshold;

    [Header("References")]
    public GameObject inventoryGO;
    public GameObject biomesUiGO;
    public GameObject diceFacesGO;
    public GameObject baseBiomesGO;
    public GameObject doubleBiomesGO;
    public GameObject comboBiomesGO;
    private void Start()
    {
        startingPosition = inventoryGO.transform.position;
    }
    void Update()
    {
        HandleMoveDiceFaces();
    }

    private void HandleMoveDiceFaces()
    {
        Vector3 offset = new Vector3(-moveOffset, 0, 0);
        targetPosition = isInventoryOpen ? startingPosition + offset : startingPosition;

        inventoryGO.transform.position = Vector3.Lerp(inventoryGO.transform.position, targetPosition, Time.deltaTime * speed);
        if (Vector3.Distance(inventoryGO.transform.position, targetPosition) < snapThreshold)
        {
            inventoryGO.transform.position = targetPosition;
        }
    }
    public void ToggleInventoryUI()
    {
        IsInventoryOpen = !IsInventoryOpen;
    }

}
