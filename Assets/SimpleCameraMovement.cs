using UnityEngine;

public class SimpleCameraMovement : MonoBehaviour
{
    public float speed = 5.0f;  // Speed of the camera

    void Update()
    {
        float xMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;  // A and D key for horizontal movement
        float zMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;    // W and S key for forward/backward movement

        // Move the camera in the x and z directions based on input
        transform.Translate(xMovement, 0, zMovement);
    }
}
