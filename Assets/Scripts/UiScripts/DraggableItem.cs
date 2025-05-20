using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    public GameObject childBackground;
    public CanvasGroup group;
    public Transform originalParent; // The original parent (inventory slot)
    public bool isDraggable = true; // Determines if the item can be dragged
    public bool isAvailable;
    public int biomeId;

    private GameObject placeholderClone;

    private void Start()
    {
        image = GetComponent<Image>();
        group = GetComponent<CanvasGroup>();
        childBackground = transform.GetChild(0).gameObject;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        originalParent = transform.parent;

        placeholderClone = Instantiate(gameObject, originalParent);

        transform.SetParent(transform.root); // Temporarily move to the root for dragging
        transform.SetAsLastSibling(); // Ensure it's rendered on top
        group.alpha = 0.6f; // Make the item semi-transparent while dragging
        image.raycastTarget = false; // Disable raycast to avoid blocking drop detection
        childBackground.SetActive(true);
        childBackground.GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        transform.position = Input.mousePosition; // Follow the mouse position
        childBackground.SetActive(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        transform.SetParent(originalParent); // Return to the original parent if no valid drop
        transform.localPosition = Vector3.zero; // Reset position

        if (placeholderClone != null)
        {
            Destroy(placeholderClone);
        }
        group.alpha = 1f; // Restore full opacity
        image.raycastTarget = true; // Re-enable raycast
        childBackground.SetActive(false);
    }
}