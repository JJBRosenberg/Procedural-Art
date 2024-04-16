using UnityEngine;

public class RandomizeMaterialStrength : MonoBehaviour
{
    public string strengthPropertyName = "_Strength";

    Renderer renderer;
    Material originalMaterial;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material;
            Material newMaterial = new Material(originalMaterial);
            float randomStrength = Random.value;
            newMaterial.SetFloat(strengthPropertyName, randomStrength);
            renderer.material = newMaterial;
        }
    }
}
