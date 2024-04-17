using UnityEngine;

public class AdCreatorTool : MonoBehaviour
{
    public GameObject adPrefab;
    public float minLength = 0.1f;
    public float maxLength = 0.5f;
    public float minWidth = 0.1f;
    public float maxWidth = 0.5f;

    void Update()
    {
        if (Input.GetMouseButtonDown(2)) // Middle mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                SpawnPrefab(hit.point, hit.normal);
            }
        }
    }

    void SpawnPrefab(Vector3 position, Vector3 normal)
    {
        Vector3 toCamera = (Camera.main.transform.position - position).normalized;
        Vector3 forward = -Vector3.Cross(normal, Vector3.Cross(toCamera, normal)).normalized;
        Quaternion lookAtCamera = Quaternion.LookRotation(forward, normal);

        Quaternion finalRotation = Quaternion.Euler(0, lookAtCamera.eulerAngles.y, lookAtCamera.eulerAngles.z);

        Vector3 adjustedPosition = position + (toCamera * 0.1f);

        GameObject spawnedAd = GameObject.Instantiate(adPrefab, adjustedPosition, finalRotation);
        Vector3 scale = new Vector3(Random.Range(minWidth, maxWidth), 1.0f, Random.Range(minLength, maxLength));
        spawnedAd.transform.localScale = scale;
    }
}
