using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowingHexes : MonoBehaviour
{
    [SerializeField]
    public Material glowMaterial;
    [SerializeField]
    public Material lockedGlowMaterial;

    private bool isGlowing;
    private Vector3 originalScale;

    private Renderer[] renderers;

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private Dictionary<Renderer, Material[]> glowMaterials = new Dictionary<Renderer, Material[]>();

    private static Dictionary<Texture, Material> cachedGlowMaterial = new Dictionary<Texture, Material>();

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalScale = transform.localScale;
        PrepareMaterials();
    }

    public void PrepareMaterials()
    {
        foreach (Renderer r in renderers)
        {
            Material[] originalMats = r.materials;
            Material[] glowMats = new Material[originalMats.Length];

            for (int i = 0; i < originalMats.Length; i++)
            {
                if (!cachedGlowMaterial.TryGetValue(originalMats[i].GetTexture("_BaseMap"), out Material mat))
                {
                    mat = new Material(glowMaterial);
                    Texture baseMap = originalMats[i].GetTexture("_BaseMap");
                    mat.SetTexture("_BaseMap", baseMap);
                    cachedGlowMaterial[baseMap] = mat;
                }
                glowMats[i] = mat;
            }

            originalMaterials[r] = originalMats;
            glowMaterials[r] = glowMats;
        }
    }

    public void ToggleGlow()
    {
        foreach (Renderer r in renderers)
        {
            if (r == null) continue;
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

    private IEnumerator ScaleEffect()
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
}
