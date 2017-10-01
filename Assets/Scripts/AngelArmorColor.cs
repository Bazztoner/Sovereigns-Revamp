using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelArmorColor : MonoBehaviour
{
    public Color baseMetal;
    public Color edgePlating;
    public Color fabric;
    public Color leather;
    public Color feather;
    [Range(0, 1)]public float tintAlpha;
    public Renderer[] rend;

    void Start ()
    {
        for (int i = 0; i < rend.Length; i++)
        {
            rend[i].material.SetColor("_BaseMetal", baseMetal);
            rend[i].material.SetColor("_EdgePlating", edgePlating);
            rend[i].material.SetColor("_Fabric", fabric);
            rend[i].material.SetColor("_Leather", leather);
            rend[i].material.SetColor("_Feather", feather);
            rend[i].material.SetFloat("_TintAlpha", tintAlpha);
        }     
    }
}
