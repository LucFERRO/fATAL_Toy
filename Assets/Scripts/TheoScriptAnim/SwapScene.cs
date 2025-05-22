using UnityEngine;
using UnityEngine.SceneManagement;

public class SwapScene : MonoBehaviour
{
    public string SceneExactName;


    public void changeSceneToSwapName( string newName)
    {
        SceneExactName = newName;
    }

    public void SwapTheScene()
    {
        SceneManager.LoadScene(SceneExactName);
    }
}
