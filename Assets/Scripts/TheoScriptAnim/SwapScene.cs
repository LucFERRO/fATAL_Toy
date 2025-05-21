using UnityEngine;
using UnityEngine.SceneManagement;

public class SwapScene : MonoBehaviour
{
    public string mainSceneExactName;

    public void SwapToMain()
    {
        SceneManager.LoadScene(mainSceneExactName);
    }
}
