using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AngelArmorColor : MonoBehaviour
{
    public Color baseMetal;
    public Color edgePlating;
    public Color fabric;
    public Color leather;
    public Color feather;
    [Range(0, 1)]public float tintAlpha;
    public Renderer[] rend;

    public void ExecuteColorizer()
    {
        GetAllParts();

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

    void GetAllParts()
    {
        rend = transform.parent.GetComponentsInChildren<Renderer>()
               .Where(x => x.gameObject.name == "Left Shin" ||
                      x.gameObject.name == "Left UpLeg" ||
                      x.gameObject.name == "Right Shin" ||
                      x.gameObject.name == "Right UpLeg" ||
                      x.gameObject.name == "Left Arm" ||
                      x.gameObject.name == "Left PreArm" ||
                      x.gameObject.name == "LSP" ||
                      x.gameObject.name == "LS" ||
                      x.gameObject.name == "Left Arm 1" ||
                      x.gameObject.name == "Right PreArm" ||
                      x.gameObject.name == "RSP" ||
                      x.gameObject.name == "RS" ||
                      x.gameObject.name == "Helmet" ||
                      x.gameObject.name == "Waist" ||
                      x.gameObject.name == "ChestPlate" ||
                      x.gameObject.name == "Left Boot" ||
                      x.gameObject.name == "Left Glove" ||
                      x.gameObject.name == "Right Boot" ||
                      x.gameObject.name == "Right Glove" ||
                      x.gameObject.name == "Waistcoat")
                .ToArray();
    }
}
