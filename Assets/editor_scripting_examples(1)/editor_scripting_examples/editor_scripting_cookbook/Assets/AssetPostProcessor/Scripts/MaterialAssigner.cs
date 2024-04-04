using UnityEngine;

/**
 * This is a really short and simply script that assigns any material you assign to it, 
 * to all meshrenderers in any of the child objects.
 * 
 * Usage:
 * - drop on parent object, assign material and watch all child objects get the same material
 * 
 * @author J.C. Wichman, Inner Drive Studios.
 */
public class MaterialAssigner : MonoBehaviour
{
    private MeshRenderer[] _childMeshRenderers;

    public Material material;
    private Material _oldMaterial;

    private void OnValidate()
    {
		//lazy initialization
        if (_childMeshRenderers == null) _childMeshRenderers = GetComponentsInChildren<MeshRenderer>();

		//if the material has changed, assign it to all child meshrenderers
        if (_oldMaterial != material)
        {
            foreach (MeshRenderer mr in _childMeshRenderers) {
                mr.sharedMaterial = material;
            }
            _oldMaterial = material;
        }
        
    }
}
