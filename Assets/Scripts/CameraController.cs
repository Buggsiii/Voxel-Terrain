using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private bool _isEnabled = false;
    private Camera _cam;

    private Camera Cam
    {
        get
        {
            if (_cam == null)
                _cam = GetComponent<Camera>();
            return _cam;
        }
    }

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    public void SetPosition(Bounds bounds)
    {
        if (!_isEnabled) return;

        float cameraDistance = 1.4f; // Constant factor
        Vector3 objectSizes = bounds.max - bounds.min;
        float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * _cam.fieldOfView); // Visible height 1 meter in front
        float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
        distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object

        Vector3 pos = bounds.center - distance * _cam.transform.forward;
        transform.position = pos;
    }
}
