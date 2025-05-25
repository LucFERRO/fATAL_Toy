using System;
using System.IO;
using UnityEngine;
using System.Collections;

public class ScreenShotMachine : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;
    public GameObject UI;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        id = GetComponent<DataPersistenceManager>().sessionId;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ScreenShot();
        }
    }

    public void LoadData(GameData data)
    {
        // Load data logic here
    }

    public void SaveData(ref GameData data)
    {
        data.screenShotId = id;
        data.timestamp = DateTime.Now.ToString("dd-MM-yyyy / HH:mm:ss");
    }

    public void ScreenShot()
    {
        StartCoroutine(TakeScreenshot());
    }

    private IEnumerator TakeScreenshot()
    {
        UI.SetActive(false);
        Cursor.visible = false;
        GetComponent<DataPersistenceManager>().uiManager.isInventoryOpen = true;
        // Wait for the end of the frame to ensure UI and cursor updates are applied
        yield return new WaitForEndOfFrame();


        string timestamp = DateTime.Now.ToString("dd-MM-yyyy / HH:mm:ss");
        string relativePath = Path.Combine(Application.dataPath, "..", "SaveData", "Images");
        Directory.CreateDirectory(relativePath); // Ensure the directory exists
        string filePath = Path.Combine(relativePath, id + ".png");

        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("Screenshot saved to: " + filePath);

        // Re-enable UI and cursor after taking the screenshot
        UI.SetActive(true);
        Cursor.visible = true;
    }
}