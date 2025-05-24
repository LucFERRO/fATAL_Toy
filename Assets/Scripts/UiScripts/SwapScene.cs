using UnityEngine;
using UnityEngine.SceneManagement;

public class SwapScene : MonoBehaviour
{
    public string SceneExactName;
    public GameObject maskStencil;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Backspace))
    //    {
    //        Debug.Log("backspace");
    //        maskStencil.SetActive(true);
    //    }
    //}

        public void changeSceneToSwapName( string newName)
    {
        SceneExactName = newName;
    }

    public void SwapTheScene()
    {
        SceneManager.LoadScene(SceneExactName);
    }
}
