using UnityEngine;

public class adCreator : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject prefabToSpawn;

    private Vector3 firstClickPosition;
    private bool isFirstClickDone = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isFirstClickDone = false;
            Debug.Log("First click choice has been reset.");
        }

        // Check for middle mouse button click
        if (Input.GetMouseButtonDown(2)) 
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!isFirstClickDone)
                {
                    firstClickPosition = hit.point;
                    isFirstClickDone = true;
                    Debug.Log("First click set at position: " + firstClickPosition);
                }
                else
                {
                    Vector3 secondClickPosition = hit.point;
                    Vector3 centerPosition = (firstClickPosition + secondClickPosition) / 2;

                    // Calculate orientation facing towards the camera, parallel to the surface
                    Vector3 forwardOnPlane = Vector3.ProjectOnPlane(mainCamera.transform.forward, hit.normal).normalized;
                    Quaternion orientation = Quaternion.LookRotation(forwardOnPlane, hit.normal);

                    // Calculate the new position, slightly closer to the camera to avoid z-fighting
                    Vector3 positionCloserToCamera = centerPosition - mainCamera.transform.forward * 0.5f;

                    Instantiate(prefabToSpawn, positionCloserToCamera , orientation);

                    // Reset
                    isFirstClickDone = false;
                    Debug.Log("Prefab spawned between: " + firstClickPosition + " and " + secondClickPosition);
                }
            }
        }
    }
}
