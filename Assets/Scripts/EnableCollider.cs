using UnityEngine;

public class EnableCollider : MonoBehaviour
{
    public GameObject[] colliders;
    public GameObject gameManager;

    public float maxTimer = 1f;
    private float currentTimer;
    private bool hasEntered;
    void Start()
    {
        currentTimer = maxTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasEntered)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0)
            {
                ActivateColliders();
            }
        }
        if (gameManager.transform.childCount == 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].SetActive(false);
            }
        }

    }

    void OnTriggerEnter()
    {
        hasEntered = true;
    }

    void ActivateColliders()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].SetActive(true);
            hasEntered = false;
            currentTimer = maxTimer;
        }
    }


}