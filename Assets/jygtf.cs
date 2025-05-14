using UnityEngine;

public class jygtf : MonoBehaviour
{
    public GameObject test;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { 
        GameObject nezPref = Instantiate(test);
            for (int i = 0; i<nezPref.transform.childCount; i++) 
            {
            nezPref.transform.GetChild(i).rotation = Quaternion.Euler(0, Random.Range(0,6)*60, 0);
            }
        }
    }
}
