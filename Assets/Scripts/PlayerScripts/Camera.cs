using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform player; 
    public float sensitivity = 2f;

    private Vector3 offset; 
    private float yaw; 

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Save the starting offset set in Scene view
        offset = transform.position - player.position;
        yaw = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Horizontal mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        yaw += mouseX;

        // Rotate the offset around the player
        Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
        transform.position = player.position + rotation * offset;

        // Make the camera look at the player
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}