using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Rendering.FilterWindow;
using UnityEngine.UI;
using System.Collections.Generic;

public class OverUICheck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ProjectileThrow projectileThrow;
    public bool isInventory;
    //[SerializeField] private GraphicRaycaster raycaster;
    //[SerializeField] private EventSystem eventSystem;

    //private void Update()
    //{
    //    if (!isInventory)
    //    {
    //        return;
    //    }
    //    if (IsPointerOverSpecificElement())
    //    {
    //        Debug.Log("Cursor is over the specified UI element!");
    //    }
    //}

    //private bool IsPointerOverSpecificElement()
    //{
    //    PointerEventData pointerData = new PointerEventData(eventSystem)
    //    {
    //        position = Input.mousePosition
    //    };
    //    List<RaycastResult> results = new List<RaycastResult>();
    //    raycaster.Raycast(pointerData, results);
    //    foreach (RaycastResult raycastResult in results)
    //    {
    //        if (raycastResult.gameObject.CompareTag("DiceSlot"))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("enter ui");
        //Debug.Log(eventData.rawPointerPress.name);
        projectileThrow.isOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("exit ui");
        projectileThrow.isOverUI = false;
    }
}
