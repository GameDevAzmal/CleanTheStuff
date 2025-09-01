using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{  
    public Transform player;      
    public float distance = 5f;  
    public float height = 2f;     
    public float sensitivity = 2f;

    private float yaw = 0f;

    void Start()
    {
       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        yaw += mouseX;

        
        Quaternion rotation = Quaternion.Euler(0, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);
        transform.position = player.position + offset;

        
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}
