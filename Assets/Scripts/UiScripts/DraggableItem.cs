using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Image image;
    CanvasGroup group;
    //public Item item;
    public Transform parentAfterDrag;
    public int biomeId;

    private void Start()
    {
        image = GetComponent<Image>();
        group = GetComponent<CanvasGroup>();
        //item = GetComponent<Item>();
        //image.sprite = item.sprite;

        Material potentialMaterialItem = GetComponent<Material>();
        //Consumable potentialConsumableItem = GetComponent<Consumable>();

        //if (potentialMaterialItem != null || potentialConsumableItem != null)
        //{
        //    HandleStackableItems();
        //}

        //if (item.name == "Material" || item.name == "Consumable")
        //{
        //}
    }

    public void HandleStackableItems()
    {
        //transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{item.currentStack} / {item.maxStack}";
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        group.alpha = .5f;
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        group.alpha = 1f;
        image.raycastTarget = true;
    }
}