using UnityEngine;

public class RandomizeColor : MonoBehaviour
{
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();

        // Get the current properties of the material
        renderer.GetPropertyBlock(propBlock);

        // Change the color property
        Color baseColor = renderer.material.color; // Get the original color
        Color randomColor = new Color(
            baseColor.r + Random.Range(-0.1f, 0.1f), // Randomize red
            baseColor.g + Random.Range(-0.1f, 0.1f), // Randomize green
            baseColor.b + Random.Range(-0.1f, 0.1f), // Randomize blue
            1.0f // Alpha should probably remain the same
        );

        // Clamp values to ensure they remain valid
        randomColor.r = Mathf.Clamp01(randomColor.r);
        randomColor.g = Mathf.Clamp01(randomColor.g);
        randomColor.b = Mathf.Clamp01(randomColor.b);

        propBlock.SetColor("_Color", randomColor);

        // Apply the modified property block back to the renderer
        renderer.SetPropertyBlock(propBlock);
    }
}
