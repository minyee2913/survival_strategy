using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Vector2 _mouseInput;
    [SerializeField]
    private float mouseSensitivity = 2f;
    [SerializeField]
    Transform head;
    [SerializeField]
    private float currentRotX, currentRotY;
    [SerializeField]
    private float minVerticalAngle, maxVerticalAngle;
    void Start()
    {
        
    }

    public void MouseRotate(bool toTransform) {
        _mouseInput.x = Input.GetAxis("Mouse X") * mouseSensitivity;
        _mouseInput.y = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentRotX = Mathf.Clamp(currentRotX - _mouseInput.y, minVerticalAngle, maxVerticalAngle);

        if (toTransform) {
            head.localRotation = Quaternion.Lerp(head.localRotation, Quaternion.Euler(currentRotX, 0, 0), 6 * Time.deltaTime);
            transform.Rotate(Vector3.up * _mouseInput.x);

            currentRotY = 0;
        } else {
            currentRotY = currentRotY + _mouseInput.x;
            
            head.localRotation = Quaternion.Euler(currentRotX, currentRotY, 0);
        }
    }
}
