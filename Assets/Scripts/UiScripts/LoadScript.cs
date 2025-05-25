using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
//using Newtonsoft.Json;
public class LoadScript : MonoBehaviour
{
    public GameObject saveGO; // Prefab to instantiate for each file
    public TextMeshProUGUI pageCounterText; // Assign in inspector
    public Button nextPageButton; // Assign in inspector
    public Button prevPageButton; // Assign in inspector

    private List<string> saveFiles = new List<string>();
    private int currentPage = 0;
    private int itemsPerPage = 6;
    private int totalPages = 0;
    private string savePath;
    private string imagePath;

    private bool isLoadMenuOn;
    private Animator loadMenuAnimator;

    void Start()
    {
        savePath = Path.Combine(Application.dataPath, "..", "SaveData", "Save");
        imagePath = Path.Combine(Application.dataPath, "..", "SaveData", "Images");
        loadMenuAnimator = GetComponent<Animator>();

        // Assign button listeners
        nextPageButton.onClick.AddListener(NextPage);
        prevPageButton.onClick.AddListener(PrevPage);

        FetchAllSaveFiles();
        ShowPage(0);
    }

    public void ToggleLoadMenu()
    {
        isLoadMenuOn = !isLoadMenuOn;
        loadMenuAnimator.SetBool("IsLoadMenuOpen", isLoadMenuOn);
    }

    void FetchAllSaveFiles()
    {
        saveFiles.Clear();
        if (Directory.Exists(savePath))
        {
            saveFiles.AddRange(Directory.GetFiles(savePath, "*.json"));
            totalPages = Mathf.CeilToInt(saveFiles.Count / (float)itemsPerPage);
        }
        else
        {
            Debug.LogWarning($"Save directory not found: {savePath}");
            totalPages = 0;
        }
    }

    void ShowPage(int page)
    {
        // Remove old items
        foreach (Transform child in transform.GetChild(0))
        {
            Destroy(child.gameObject);
        }

        if (saveFiles.Count == 0)
        {
            pageCounterText.text = "0 / 0";
            nextPageButton.interactable = false;
            prevPageButton.interactable = false;
            return;
        }

        currentPage = Mathf.Clamp(page, 0, totalPages - 1);

        int startIdx = currentPage * itemsPerPage;
        int endIdx = Mathf.Min(startIdx + itemsPerPage, saveFiles.Count);

        for (int i = startIdx; i < endIdx; i++)
        {
            string file = saveFiles[i];
            string jsonContent = File.ReadAllText(file);
            GameData saveData = JsonUtility.FromJson<GameData>(jsonContent);

            string screenshotFile = Path.Combine(imagePath, saveData.screenShotId + ".png");
            if (File.Exists(screenshotFile))
            {
                GameObject newSaveGO = Instantiate(saveGO, transform.GetChild(0));
                newSaveGO.name = Path.GetFileNameWithoutExtension(file);
                newSaveGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Data N:{(6*currentPage) + i + 1}";
                newSaveGO.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = saveData.timestamp;
                newSaveGO.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = newSaveGO.name;

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
            }
        }

        // Update page counter and button states
        pageCounterText.text = $"{currentPage + 1} / {totalPages}";
        prevPageButton.interactable = currentPage > 0;
        nextPageButton.interactable = currentPage < totalPages - 1;
    }

    public void NextPage()
    {
        if (currentPage < totalPages - 1)
            ShowPage(currentPage + 1);
    }

    public void PrevPage()
    {
        if (currentPage > 0)
            ShowPage(currentPage - 1);
    }
}
