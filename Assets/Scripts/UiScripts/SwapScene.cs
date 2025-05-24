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
        yield return new WaitForSeconds(2.3f);
        SceneManager.LoadScene(SceneExactName);
    }

    public void SwapTheScene()
    {
        stencilAnimator.SetTrigger("StencielAppearTrigger");
        StartCoroutine(ChangeSceneCoroutine());
    }
}
