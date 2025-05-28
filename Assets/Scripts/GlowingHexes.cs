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
    [SerializeField]
    public Material materializeMaterial;
    private bool isGlowing;
    private bool isLocked;
    private bool isMaterializing;
    private Vector3 originalScale;

    private GameManager gameManager;
    private Renderer[] renderers;

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private Dictionary<Renderer, Material[]> glowMaterials = new Dictionary<Renderer, Material[]>();
    private Dictionary<Renderer, Material[]> lockedMaterials = new Dictionary<Renderer, Material[]>();
    private Dictionary<Renderer, Material[]> materializeMaterials = new Dictionary<Renderer, Material[]>();


    private static Dictionary<Texture, Material> cachedGlowMaterial = new Dictionary<Texture, Material>();
    private static Dictionary<Texture, Material> cachedLockedMaterial = new Dictionary<Texture, Material>();
    private static Dictionary<Texture, Material> cachedMaterializeMaterial = new Dictionary<Texture, Material>();

    public GameObject splashParticule;
    public static event Action OnDiceDestroyed;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        gameManager = NeighbourTileProcessor.gameManager;
        originalScale = transform.localScale;
        PrepareMaterials();
    }

    public void PrepareMaterials()
    {
        foreach (Renderer r in renderers)
        {
            Material[] originalMats = r.materials;
            Material[] glowMats = new Material[originalMats.Length];
            Material[] lockedMats = new Material[originalMats.Length];
            Material[] materializeMats = new Material[originalMats.Length];
            for (int i = 0; i < originalMats.Length; i++)
            {
                if (!originalMats[i].GetTexture("_BaseMap") || transform.CompareTag("WaterPlane"))
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

                if (!cachedMaterializeMaterial.TryGetValue(originalMats[i].GetTexture("_BaseMap"), out Material mat3))
                {
                    mat3 = new Material(materializeMaterial);
                    Texture baseMap = originalMats[i].GetTexture("_BaseMap");
                    mat3.SetTexture("_BaseMap", baseMap);
                    cachedMaterializeMaterial[baseMap] = mat3;
                }
                materializeMats[i] = mat3;
            }


            originalMaterials[r] = originalMats;
            glowMaterials[r] = glowMats;
            lockedMaterials[r] = lockedMats;
            materializeMaterials[r] = materializeMats;

        }
    }


    public void ToggleGlow()
    {
        foreach (Renderer r in renderers)
        {
            if (r == null || r.transform.CompareTag("WaterPlane")) continue;
            //if (originalMaterials[r][0].GetTexture("_BaseMap"))
            r.materials = isGlowing ? originalMaterials[r] : glowMaterials[r];
        }

        isGlowing = !isGlowing;
        StartCoroutine(ScaleEffect());
    }
    public void ToggleGlow(bool state)
    {
        if (isGlowing == state)
        {
            return;
        }

        ToggleGlow();
    }
    public void ToggleLock()
    {
        foreach (Renderer r in renderers)
        {
            if (r == null || r.transform.CompareTag("WaterPlane"))
            {
                r.material.SetInt("_isLocked", r.material.GetInt("_isLocked") == 1 ? 0 : 1);
            }
            else
            {
                r.materials = isLocked ? originalMaterials[r] : lockedMaterials[r];
            }
        }

        isLocked = !isLocked;
    }
    public void ToggleLock(bool state)
    {
        if (isLocked == state)
        {
            return;
        }

        ToggleLock();
    }
    public void ToggleMaterialize()
    {
        foreach (Renderer r in renderers)
        {
            if (r == null || r.transform.CompareTag("WaterPlane"))
            {
                continue;
            }

            r.materials = isMaterializing ? originalMaterials[r] : materializeMaterials[r];
        }

        isMaterializing = !isMaterializing;
    }
    public void ToggleMaterialize(bool state)
    {
        if (isMaterializing == state)
        {
            return;
        }

        ToggleMaterialize();
    }

    public IEnumerator ClearParticlesCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        for (int i = 1; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i).GetComponent<ParticlesToBeDestroyed>().isToBeDestroyed)
            {
                Debug.Log($"Destroying 'PARTICLES' {transform.parent.GetChild(i)}");
                Destroy(transform.parent.GetChild(i).gameObject);
            }
        }
        //Destroy(particles);
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

    public IEnumerator TransitionDisappear()
    {
        float duration = 0.5f;
        float time = 0;
        while (time < duration)
        {
            float dissolveValue = Mathf.Lerp(0f, 1f, time / duration);
            float dissolveValue2 = Mathf.Lerp(1f, 0f, time / duration);

            foreach (Renderer r in renderers)
            {
                if (r.gameObject.CompareTag("WaterPlane"))
                {
                    r.materials[0].SetFloat("_Dissolve", dissolveValue2);
                }
                else
                {
                    r.materials[0].SetFloat("_Dissolve", dissolveValue);
                }
            }
            time += Time.deltaTime;
            yield return null;
        }
        foreach (Renderer r in renderers)
        {
            if (r.gameObject.CompareTag("WaterPlane"))
            {
                r.materials[0].SetFloat("_Dissolve", 0f);
            }
            else
            {
                r.materials[0].SetFloat("_Dissolve", 1f);
            }
        }
        Destroy(gameObject);
    }
    public IEnumerator TransitionAppear()
    {
        float duration = 0.5f;
        float time = 0;
        while (time < duration)
        {
            float dissolveValue = Mathf.Lerp(1f, 0f, time / duration);
            float dissolveValue2 = Mathf.Lerp(0f, 1f, time / duration);

            foreach (Renderer r in renderers)
            {
                if (r.gameObject.CompareTag("WaterPlane"))
                {
                    r.materials[0].SetFloat("_Dissolve", dissolveValue2);
                }
                else
                {
                    r.materials[0].SetFloat("_Dissolve", dissolveValue);
                }
            }
            time += Time.deltaTime;
            yield return null;
        }

        ToggleMaterialize(false);
        // DESTROY DICE QUI MERDE

        if (gameManager.gameObject.transform.childCount != 0)
        {
            if (OnDiceDestroyed != null)
            {
                Debug.Log("Destroying Dice");
                OnDiceDestroyed.Invoke();
            }

            Destroy(gameManager.gameObject.transform.GetChild(0).gameObject);
        }
    }
}