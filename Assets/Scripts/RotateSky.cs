using UnityEngine;

public class RotateSky : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    private Material _skybox;
    private void Start()
    {
        _skybox = Instantiate(RenderSettings.skybox);
        RenderSettings.skybox = _skybox;
    }
    void Update()
    {
        _skybox.SetFloat("_Rotation", Time.timeSinceLevelLoad * rotationSpeed);
    }
}
