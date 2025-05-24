using UnityEngine;
using UnityEngine.UI;

public class ClickthroughStencil : MonoBehaviour
{
    void Start()
    {
        transform.GetComponent<Image>().raycastTarget = false;
    }
}