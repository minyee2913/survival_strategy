using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Vector2 _mouseInput;
    [SerializeField]
    private float mouseSensitivity = 2f;
    [SerializeField]
    Transform head;
    [SerializeField]
    private float currentRotX;
    [SerializeField]
    private float minVerticalAngle, maxVerticalAngle;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MouseRotate();
    }

    void MouseRotate() {
        _mouseInput.x = Input.GetAxis("Mouse X") * mouseSensitivity;
        _mouseInput.y = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentRotX = Mathf.Clamp(currentRotX - _mouseInput.y, minVerticalAngle, maxVerticalAngle);

        transform.Rotate(Vector3.up * _mouseInput.x);
        head.localRotation = Quaternion.Euler(currentRotX, 0, 0);
    }
}
