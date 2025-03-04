using UnityEngine;
using UnityEngine.UI;

public class ProtoRandomVectors : MonoBehaviour
{
    private CraftCalculator calculator;
    public FaceComponent[] facesArray;
    public GameObject resultParent;
    public GameObject[] resultGameObjects;

    public Material[] materialArray;
    public Color[] colorArray;

    void Start()
    {
        calculator = transform.GetComponent<CraftCalculator>();
        //resultGameObjects = new GameObject[facesArray.Length];
        //for (int i = 0; i < facesArray.Length; i++)
        //{
        //    resultGameObjects[i] = resultParent.transform.GetChild(i).gameObject;
        //}
        colorArray = new Color[3];
        for (int i = 0; i < colorArray.Length; i++)
        {
            colorArray[i] = new Color(i == 0 ? 255 : 0, i == 1 ? 255 : 0, i == 2 ? 255 : 0);
        }
        Debug.Log($"colorArrayLength {colorArray.Length}");
    }

    Color GetColorFromArray(FaceComponent face)
    {
        Color chosenColor;
        if (face.faceColor == "red")
        {
            chosenColor = colorArray[0];
        }
        else if (face.faceColor == "blue")
        {
            chosenColor = colorArray[2];
        }
        else
        {
            chosenColor = colorArray[1];
        }
        return chosenColor;
    }

    int GetVectorImageFromArray(FaceComponent face)
    {
        int chosenIndex = 0;
        if (face.faceVector.x > 0)
        {
            chosenIndex = 1;
        } else if (face.faceVector.x < 0)
        {
            chosenIndex = 3;
        }
        if (face.faceVector.y > 0)
        {
            chosenIndex = 0;
        }
        else if (face.faceVector.y < 0)
        {
            chosenIndex = 2;
        }

        return chosenIndex;
    }

    // Update is called once per frame
    public void UpdateSprites()
    {
        Debug.Log("test "+ resultGameObjects.Length);
        for (int i = 0; i < facesArray.Length; i++)
        {
            GameObject resultSlot = resultGameObjects[i];
            Color chosenColor = GetColorFromArray(facesArray[i]);
            Material chosenMaterial = materialArray[GetVectorImageFromArray(facesArray[i])];
            //if (i == 1) { 
            //    Debug.Log(chosenColor.ToString());
            //    Debug.Log(chosenMaterial.name);
            //    Debug.Log("should be "+ facesArray[i].faceColor + " and " + facesArray[i].faceVector.x + " : " + facesArray[i].faceVector.y);
            //}
            resultSlot.transform.GetChild(0).GetComponent<RawImage>().color = chosenColor;
            resultSlot.transform.GetChild(1).GetComponent<RawImage>().material = chosenMaterial;
        }
    }
}
