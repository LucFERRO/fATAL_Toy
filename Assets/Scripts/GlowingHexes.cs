using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowingHexes : MonoBehaviour
{
    [SerializeField]
    private Material[] originalMaterials;
    [SerializeField]
    private Material[] glowMaterials;
    private static Dictionary<Color, Material> cachedGlowMaterial = new Dictionary<Color, Material>();

    public Material glowMaterial;
    public Material lockedGlowMaterial;
    [SerializeField]
    private Renderer objRenderer;
    [SerializeField]
    private bool isGlowing;

    private Vector3 originalScale;

    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        PrepareMaterials();
        originalScale = transform.localScale;
    }

    public void PrepareMaterials()
    {
        if (objRenderer == null)
        {
            return;
        }

        originalMaterials = objRenderer.materials;
        glowMaterials = new Material[originalMaterials.Length*2];

        for (int i = 0; i < originalMaterials.Length; i++)
        {
            if (!cachedGlowMaterial.TryGetValue(originalMaterials[i].color, out Material mat))
            {
                Debug.Log(transform.GetComponent<NeighbourTileProcessor>().isLocked);
                mat = new Material(glowMaterial);
                mat.color = originalMaterials[i].color;
                cachedGlowMaterial[originalMaterials[i].color] = mat;
            }
            glowMaterials[i] = mat;
        }
        //for (int i = 0; i < originalMaterials.Length; i++)
        //{
        //    if (!cachedGlowMaterial.TryGetValue(originalMaterials[i].color, out Material mat))
        //    {
        //        Debug.Log(transform.GetComponent<GridCoordinates>().isLocked);
        //        mat = new Material(lockedGlowMaterial);
        //        mat.color = originalMaterials[i].color;
        //        cachedGlowMaterial[originalMaterials[i].color] = mat;
        //    }
        //    glowMaterials[i] = mat;
        //}

    }

    public void ToggleGlow()
    {
        if (objRenderer == null) return;
        objRenderer.materials = isGlowing ? originalMaterials : glowMaterials;
        isGlowing = !isGlowing;
        StartCoroutine(ScaleEffect());
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

    public void ToggleGlow(bool state)
    {
        if (isGlowing == state || objRenderer == null) return;
        isGlowing = !state;
        ToggleGlow();
    }
}