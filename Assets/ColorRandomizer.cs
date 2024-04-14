using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    public Material[] materials;

    void Start()
    {
        if (materials.Length > 0)
        {
            Material chosenMaterial = materials[Random.Range(0, materials.Length)];
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = chosenMaterial;
            }
        }
    }
}
