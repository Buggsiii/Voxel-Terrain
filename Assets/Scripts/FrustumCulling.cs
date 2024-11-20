using UnityEngine;

public class FrustumCulling : MonoBehaviour
{
    [SerializeField]
    private bool _isEnabled = false;
    private TerrainGenerator _terrainGenerator;
    private Camera _cam;
    private float _chunkRadius;

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

    private void Start()
    {
        _terrainGenerator = FindAnyObjectByType<TerrainGenerator>();

        Vector3 chunkSize = _terrainGenerator.TerrainSettings.ChunkSize;
        _chunkRadius = Mathf.Sqrt(chunkSize.x * chunkSize.x + chunkSize.y * chunkSize.y + chunkSize.z * chunkSize.z) / 2f;
    }

    private void Update()
    {
        if (!_isEnabled) return;

        Plane[] planes = GetFrustumPlanes();

        foreach (Transform chunk in _terrainGenerator.transform)
        {
            Vector3 chunkCenter =
                chunk.position +
                (Vector3)_terrainGenerator.TerrainSettings.ChunkSize
                / 2f;

            float distance = 0;
            if (IsInsideFrustum(chunkCenter, planes, ref distance))
                chunk.gameObject.SetActive(true);
            else chunk.gameObject.SetActive(distance < -_chunkRadius);
        }
    }

    private bool IsInsideFrustum(Vector3 point, Plane[] planes, ref float distance)
    {
        for (int i = 0; i < planes.Length; i++)
        {
            float dis = DistancePointPlane(point, planes[i]);

            if (dis < 0)
            {
                distance = dis;
                return false;
            }
        }

        return true;
    }

    private Plane[] GetFrustumPlanes()
    {
        float fov = Cam.fieldOfView;
        float aspect = Cam.aspect;
        float far = Cam.farClipPlane;
        float near = Cam.nearClipPlane;

        Plane[] planes = new Plane[6];

        // Calculate half heights
        float halfHeight = Mathf.Tan(fov * Mathf.Deg2Rad / 2);

        Vector3 point = Cam.transform.position;

        // Right plane
        Vector3 rightNormal = (-Cam.transform.right + halfHeight * aspect * Cam.transform.forward).normalized;
        planes[0] = new(rightNormal, point);

        // Left plane
        Vector3 leftNormal = (Cam.transform.right + halfHeight * aspect * Cam.transform.forward).normalized;
        planes[1] = new(leftNormal, point);

        // Top plane
        Vector3 topNormal = (Cam.transform.up + halfHeight * Cam.transform.forward).normalized;
        planes[2] = new(topNormal, point);

        // Bottom plane
        Vector3 bottomNormal = (Cam.transform.up + halfHeight * Cam.transform.forward).normalized;
        planes[3] = new(bottomNormal, point);

        // Far plane
        Vector3 farNormal = -Cam.transform.forward;
        Vector3 farPoint = Cam.transform.position + Cam.transform.forward * far;
        planes[4] = new(farNormal, farPoint);

        // Near plane
        Vector3 nearNormal = Cam.transform.forward;
        Vector3 nearPoint = Cam.transform.position + nearNormal * near;
        planes[5] = new(nearNormal, nearPoint);

        return planes;
    }

    // private float NearestDistanceToPoint(Vector3 point, Plane[] planes, out bool inside)
    // {
    //     float distance = float.MaxValue;
    //     inside = true;
    //     foreach (Plane plane in planes)
    //     {
    //         float d = DistancePointPlane(point, plane, out bool isInside);

    //         if (!isInside)
    //             inside = false;

    //         if (d < distance)
    //             distance = d;
    //     }

    //     return distance;
    // }

    // private bool IsBehindCamera(Vector3 point)
    // {
    //     Vector3 camToCenter = point - Cam.transform.position;
    //     return Vector3.Dot(camToCenter, Cam.transform.forward) < 0;
    // }

    private float DistancePointPlane(Vector3 point, Plane plane)
    {
        float a = plane.normal.x;
        float b = plane.normal.y;
        float c = plane.normal.z;
        float d = plane.distance;

        float x0 = point.x;
        float y0 = point.y;
        float z0 = point.z;

        float distance = a * x0 + b * y0 + c * z0 + d;

        return distance / Mathf.Sqrt(a * a + b * b + c * c);
    }

    private void OnGUI()
    {
        _isEnabled = GUI.Toggle(new Rect(10, 260, 200, 20), _isEnabled, "Frustum Culling");

        float rotationY = GUI.HorizontalSlider(new Rect(10, 280, 200, 20), transform.localEulerAngles.y, 0, 360);
        float deltaRotationY = rotationY - transform.localEulerAngles.y;
        transform.Rotate(0, deltaRotationY, 0, Space.Self);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw frustum
        Gizmos.color = Color.red;
        Gizmos.matrix = Cam.transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, Cam.fieldOfView, Cam.farClipPlane, Cam.nearClipPlane, Cam.aspect);

        Gizmos.matrix = Matrix4x4.identity;

        Gizmos.color = Color.green;
        Plane[] planes = GetFrustumPlanes();

        foreach (Plane plane in planes)
        {
            // Draw normal
            Gizmos.DrawRay(Cam.transform.position, plane.normal);
        }
    }
}
