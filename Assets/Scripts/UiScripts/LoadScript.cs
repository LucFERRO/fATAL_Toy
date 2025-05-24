using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
//using Newtonsoft.Json;
public class LoadScript : MonoBehaviour
{
    public GameObject saveGO; // Prefab to instantiate for each file

    void Start()
    {
        FetchData();
    }
    void FetchData()
    {
        // Define the relative paths
        string savePath = Path.Combine(Application.dataPath, "..", "SaveData", "Save");
        string imagePath = Path.Combine(Application.dataPath, "..", "SaveData", "Images");

        // Ensure the save directory exists
        if (Directory.Exists(savePath))
        {
            // Get all JSON files in the save directory
            string[] files = Directory.GetFiles(savePath, "*.json");
            Debug.Log($"Found {files.Length} save files.");

            foreach (string file in files)
            {
                // Read and parse the JSON file
                string jsonContent = File.ReadAllText(file);
                GameData saveData = JsonUtility.FromJson<GameData>(jsonContent);

                // Use the screenShotId to find the corresponding image
                string screenshotFile = Path.Combine(imagePath, saveData.screenShotId + ".png");
                if (File.Exists(screenshotFile))
                {
                    // Create a new saveGO instance for each file
                    GameObject newSaveGO = Instantiate(saveGO, transform.GetChild(0));
                    newSaveGO.name = Path.GetFileNameWithoutExtension(file); // Set the name to the file name

                    // Set the save name in the UI
                    newSaveGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newSaveGO.name;

                    // Load the screenshot as a Sprite and assign it to the Image component
                    byte[] imageData = File.ReadAllBytes(screenshotFile);
                    Texture2D texture = new Texture2D(2, 2);
                    if (texture.LoadImage(imageData))
                    {
                        Sprite sprite = Sprite.Create(
                            texture,
                            new Rect(0, 0, texture.width, texture.height),
                            new Vector2(0.5f, 0.5f)
                        );
                        newSaveGO.transform.GetChild(1).GetComponent<Image>().sprite = sprite;
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to load image: {screenshotFile}");
                    }

                    Debug.Log($"Found screenshot for {newSaveGO.name}: {screenshotFile}");
                }
                else
                {
                    Debug.LogWarning($"Screenshot not found for save file: {file}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Save directory not found: {savePath}");
        }
    }
}