using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class GlowingHexes : MonoBehaviour
{
    [SerializeField]
    public Material glowMaterial;
    [SerializeField]
    public Material lockedMaterial;
    private bool isGlowing;
    private Vector3 originalScale;

    private Renderer[] renderers;

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private Dictionary<Renderer, Material[]> glowMaterials = new Dictionary<Renderer, Material[]>();
    private Dictionary<Renderer, Material[]> lockedMaterials = new Dictionary<Renderer, Material[]>();

    private static Dictionary<Texture, Material> cachedGlowMaterial = new Dictionary<Texture, Material>();
    private static Dictionary<Texture, Material> cachedLockedMaterial = new Dictionary<Texture, Material>();

    public GameObject splashParticule;


    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalScale = transform.localScale;
        PrepareMaterials();
    }

    private void Update()
    {
        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                if (mat.HasProperty("_WaveTime"))
                {
                    float t = mat.GetFloat("_WaveTime");
                    mat.SetFloat("_WaveTime", t + Time.deltaTime);
                }
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            DissolveTest();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            foreach (Renderer r in renderers)
            {
                r.materials[0].SetFloat("_Dissolve", 0.5f);
                //r.materials = glowMaterials[r];
            }
        }
    }

    public void PrepareMaterials()
    {
        foreach (Renderer r in renderers)
        {
            Material[] originalMats = r.materials;
            Material[] glowMats = new Material[originalMats.Length];
            Material[] lockedMats = new Material[originalMats.Length];
            for (int i = 0; i < originalMats.Length; i++)
            {
                if (!originalMats[i].GetTexture("_BaseMap"))
                {

                    continue;
                }
                if (!cachedGlowMaterial.TryGetValue(originalMats[i].GetTexture("_BaseMap"), out Material mat))
                {
                    mat = new Material(glowMaterial);
                    Texture baseMap = originalMats[i].GetTexture("_BaseMap");
                    mat.SetTexture("_BaseMap", baseMap);
                    cachedGlowMaterial[baseMap] = mat;
                }
                glowMats[i] = mat;
                if (!cachedLockedMaterial.TryGetValue(originalMats[i].GetTexture("_BaseMap"), out Material mat2))
                {
                    mat2 = new Material(lockedMaterial);
                    Texture baseMap = originalMats[i].GetTexture("_BaseMap");
                    mat2.SetTexture("_BaseMap", baseMap);
                    cachedLockedMaterial[baseMap] = mat2;
                }
                lockedMats[i] = mat2;
            }


            originalMaterials[r] = originalMats;
            glowMaterials[r] = glowMats;
            lockedMaterials[r] = lockedMats;

        }
    }


    public void ToggleGlow()
    {
        foreach (Renderer r in renderers)
        {
            if (r == null) continue;
            //if (originalMaterials[r][0].GetTexture("_BaseMap"))
            r.materials = isGlowing ? originalMaterials[r] : glowMaterials[r];
        }

        isGlowing = !isGlowing;
        StartCoroutine(ScaleEffect());
    }
    public void ToggleGlow(bool state)
    {
        if (isGlowing == state) return;
        ToggleGlow();
    }
    public void ToggleLock()
    {
        foreach (Renderer r in renderers)
        {
            if (r == null) continue;
            r.materials = isGlowing ? originalMaterials[r] : lockedMaterials[r];
        }

        isGlowing = !isGlowing;
        //StartCoroutine(ScaleEffect());
    }
    public void ToggleLock(bool state)
    {
        if (isGlowing == state) return;
        ToggleLock();
    }

    public IEnumerator ClearParticlesCoroutine(GameObject particles, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(particles);
    }

    public void SplashEffect()
    {
        GameObject splashParticuleSpawned = Instantiate(splashParticule, transform.parent);
        Debug.Log("SPLASH!");
        splashParticuleSpawned.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
        StartCoroutine(ClearParticlesCoroutine(splashParticuleSpawned, 1f));
    }

    public IEnumerator ScaleEffect()
    {
        Vector3 targetScale = originalScale * 0.8f;
        float duration = 0.2f;
        float time = 0;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    public IEnumerator TransitionEffect()
    {
        float duration = 0.2f;
        float time = 0;
        //foreach (Renderer r in renderers)
        //{
        //    Debug.Log(glowMaterials[r][0].GetFloat("_IsDissolving"));
        //    glowMaterials[r][0].SetFloat("_IsDissolving", 1);
        //    Debug.Log(glowMaterials[r][0].GetFloat("_IsDissolving"));
        //    Debug.Log("_IsDissolvingTrue");
        //}
        while (time < duration)
        {
            Debug.Log("Dissolving_1");
            float dissolveValue = Mathf.Lerp(0f, 1f, time / duration);

            foreach (Renderer r in renderers)
            {
                Debug.Log("Dissolving_2 " + dissolveValue);
                r.materials[0].SetFloat("_Dissolve", dissolveValue);
                
                //r.materials = glowMaterials[r];
                Debug.Log("Dissolving_3 " + r.materials[0].GetFloat("_Dissolve"));
            }
            time += Time.deltaTime;
        }

        Debug.Log("Dissolving_4");
        Destroy(gameObject);
        Debug.Log("Dissolving_5");
        yield return null;

    }

    public void DissolveTest()
    {
        Debug.Log("DissolveTest_1");
        foreach (Renderer r in renderers)
        {
            //glowMaterials[r][0].SetFloat("_Dissolve", 0.5f);
            r.materials = glowMaterials[r];
        }
    }

}