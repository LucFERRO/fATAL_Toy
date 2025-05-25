using UnityEngine;

public class CanvasBulldogSearcher : MonoBehaviour
{
    public GameObject rootObject;

    void Start()
    {
        if (rootObject == null) rootObject = this.gameObject;
        CanvasGroup[] groups = rootObject.GetComponentsInChildren<CanvasGroup>(true);
        foreach (var group in groups)
        {
            Debug.Log($"CanvasGroup: {group.gameObject.name}, alpha {group.alpha}");
            if (group.alpha < 1f)
            {
                Debug.LogWarning($"CanvasGroup with alpha < 1 found on GameObject: {group.gameObject.name} (alpha={group.alpha})", group.gameObject);
            }
        }
    }
}
