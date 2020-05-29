using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialMode : ScriptableObject {

    public static Material GetTransparentMaterial()
    {
        Material m = new Material(Shader.Find("Standard"));
        m.SetFloat("_Mode", 2);
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.renderQueue = 3000;
        m.color = new Color(0, 1f, 0, 0.3f);
        return m;
    }
}
