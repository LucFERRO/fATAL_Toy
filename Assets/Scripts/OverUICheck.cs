using UnityEngine;
using UnityEngine.EventSystems;

public class OverUICheck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ProjectileThrow projectileThrow;
    public void OnPointerEnter(PointerEventData eventData)
    {
        projectileThrow.isOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        projectileThrow.isOverUI = false;
    }
}
