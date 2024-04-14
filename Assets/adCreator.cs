using UnityEngine;

public class adCreator : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject prefabToSpawn;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Instantiate(prefabToSpawn, hit.point + Vector3.up * 0.1f, Quaternion.identity);
            }
        }
    }
}
