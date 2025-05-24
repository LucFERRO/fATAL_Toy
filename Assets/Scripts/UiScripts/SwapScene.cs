using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwapScene : MonoBehaviour
{
    public string SceneExactName;
    public Animator stencilAnimator;

    public void changeSceneToSwapName(string newName)
    {
        SceneExactName = newName;
    }

    public IEnumerator ChangeSceneCoroutine()
    {
        Debug.Log($"[SwapScene] Attempting to change scene to '{SceneExactName}'");
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(2f);
        Debug.Log($"[SwapScene] Loading scene '{SceneExactName}' now.");
        SceneManager.LoadScene(SceneExactName);
    }

    public void SwapTheScene()
    {
        stencilAnimator.SetTrigger("StencielAppearTrigger");
        StartCoroutine(ChangeSceneCoroutine());
    }

    public void SelectTutorial(bool isTutorial)
    {
        CrossSceneTutorialData.isTutorial = isTutorial;
    }
}