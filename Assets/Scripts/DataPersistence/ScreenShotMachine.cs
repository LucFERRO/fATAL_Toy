using UnityEngine;

public class ScreenShotMachine : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            GenerateGuid();
            screenShot();
        }
    }

    public void LoadData(GameData data)
    {
        // Load data logic here
    }

    public void SaveData(ref GameData data)
    {
        data.screenShotId = id;
    }

    private void screenShot()
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, id + ".png");
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("Screenshot saved to: " + filePath);
    }

}
