using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskTransition : MonoBehaviour
{
    public enum Sides
    {
        Left,
        Right,
        Top,
        Bottom
    }
    public Image[] SideRects;
    [SerializeField] Image MaskImage;

    void OnEnable()
    {
        for (int i = 0; i < SideRects.Length; i++)
        {
            if (SideRects[i] != null) continue;
            Image sideRectImg = SideRects[i].GetComponent<Image>();
            //sideRectImg.enabled = false;
            SetRectSizePosBySide(SideRects[i].rectTransform, (Sides)i);
        }
    }
    void Update()
    {
        for (int i = 0; i < SideRects.Length; i++)
        {
            //SideRects[i].enabled = MaskImage.rectTransform.anchoredPosition.x < -1000;
            if (SideRects[i] != null)
            {
                SetRectSizePosBySide(SideRects[i].rectTransform, (Sides)i);
            }
        }
    }

    private void SetRectSizePosBySide(RectTransform sideRect, Sides side)
    {
        Vector2 MaskPos = MaskImage.rectTransform.anchoredPosition;
        /* RENDERED SIZE */
        Vector2 MaskSize = MaskImage.rectTransform.sizeDelta;

        switch (side)
        {
            case Sides.Left:
                sideRect.anchoredPosition = Vector2.zero;
                sideRect.sizeDelta = new Vector2(MaskPos.x, Screen.height);
                break;
            case Sides.Right:
                sideRect.anchoredPosition = new Vector2(MaskPos.x + MaskSize.x, 0);
                sideRect.sizeDelta = new Vector2(Screen.width - (MaskPos.x + MaskSize.x), Screen.height);
                break;
            case Sides.Top:
                sideRect.anchoredPosition = new Vector2(MaskPos.x, MaskPos.y + MaskSize.y);
                sideRect.sizeDelta = new Vector2(MaskSize.x, Screen.height - (MaskPos.y + MaskSize.y));
                break;
            case Sides.Bottom:
                sideRect.anchoredPosition = new Vector2(MaskPos.x, 0);
                sideRect.sizeDelta = new Vector2(MaskSize.x, MaskPos.y);
                break;
        }
    }


}