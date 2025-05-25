using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class LoadGalleryData : MonoBehaviour
{
    private string fileName;
    static private SwapScene swapScene;
    static private Animator stencilAnimator;
    void Start()
    {
        fileName = transform.GetChild(2).GetComponent<TextMeshProUGUI>().text;
        if (swapScene == null)
        {
            CrossSceneData.dataFileName = Path.GetFileName(fileName.Trim());
            swapScene = transform.parent.parent.parent.GetComponent<SwapScene>();
        }        
        if (stencilAnimator == null)
        {
            stencilAnimator = transform.parent.parent.parent.GetComponent<SwapScene>().stencilAnimator;
        }
    }

    public void LoadChosenGalleryData()
    {

        Debug.Log("Loading chosen gallery data " + fileName);
        CrossSceneData.dataFileName = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "SaveData", "Save",Path.GetFileName(fileName.Trim())));
        CrossSceneData.dataFileName += ".json";
        Debug.Log(CrossSceneData.dataFileName);
        SwapTheScene();

    }
    public IEnumerator ChangeSceneCoroutine()
    {
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(2.2f);
        SceneManager.LoadScene("GalleryWIP");
    }

    public void SwapTheScene()
    {
        stencilAnimator.SetTrigger("StencielAppearTrigger");
        StartCoroutine(ChangeSceneCoroutine());
    }

}
