using UnityEngine;

public class ChipOpacity : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame

    private void SetMaterialTransparent(Material mat)
    {
        if (mat == null) return;
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    public void SetChipOpacity(Chip chip, float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        if (chip == null) return;

        var renderers = chip.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            // Use instance materials so only this chip is changed
            var mats = r.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                var m = mats[i];
                SetMaterialTransparent(m);
                if (m.HasProperty("_Color"))
                {
                    Color c = m.color;
                    c.a = alpha;
                    m.color = c;
                }
            }
            r.materials = mats;
        }
    }
}
