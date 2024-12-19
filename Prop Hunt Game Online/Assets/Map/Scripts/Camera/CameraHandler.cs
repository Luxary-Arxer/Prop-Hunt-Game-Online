using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Transform Target;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform pivot;

    [Header("CameraProperties")]
    public CameraProperties CameraProperties;

    [Header("Layers")]
    public LayerMask IgnoreLayer;

    [Header("Camera parameters")]
    private float _delta;
    private float _mouseX;
    private float _mouseY;
    private float _smoothX;
    private float _smoothY;
    private float _smoothXVelocity;
    private float _smoothYVelocity;
    private float _lookAngle;
    private float _titleAngle;

    #region Assigning
    public void SetPositionAgainstPlayer(float distanceToSide, float height, float distance)
    {
        CameraProperties.normalX = distanceToSide;
        CameraProperties.normalY = height;
        CameraProperties.normalZ = -distance;
    }
    #endregion

    #region Camera movement
    void Update()
    {
        if (Target != null)
            CameraUpdate();
    }

    private void CameraUpdate()
    {
        _delta = Time.deltaTime;
        HandlePosition();
        HandleRotation(); 
        Vector3 targetPosition = Vector3.Lerp(transform.position, Target.position, 1);
        transform.position = targetPosition;
    }

    private void HandlePosition()
    {
        float targetX = CameraProperties.normalX;
        float targetY = CameraProperties.normalY;
        float targetZ = CameraProperties.normalZ;
        float pivotSpeed = CameraProperties.pivotSpeed;

        //Check if have object in front of the camera
        RaycastHit hit;
        Vector3 lookPos = new Vector3(transform.position.x, transform.position.y + targetY, transform.position.z);
        Vector3 dir = cameraTransform.position - lookPos;
        float dist = Mathf.Abs(CameraProperties.normalZ);
        if (Physics.SphereCast(lookPos, 0.15f/*camera size*/, dir, out hit, dist, ~IgnoreLayer))
        {
            if (!GameObject.ReferenceEquals(hit.transform.gameObject, Target.gameObject))
            {
                float distanceToWall = Vector3.Distance(hit.point, lookPos);
                float maxDistance = targetZ * 0.01f;
                float minDistance = float.NegativeInfinity;

                targetZ = Mathf.Clamp((distanceToWall * -1 ) + 0.3f, minDistance, maxDistance);
                targetX = ((targetZ * -1) / dist) * targetX; // Change camera x position if it touches a wall

                pivotSpeed = pivotSpeed * 10; // To make camera faster change position when it's in a wall
            }
        }

        //Pivot position
        Vector3 pivotPosition = pivot.localPosition;
        pivotPosition.x = targetX;
        pivotPosition.y = targetY;

        //Camera position
        Vector3 newCameraPosition = cameraTransform.localPosition;
        newCameraPosition.z = targetZ;

        float t = _delta * pivotSpeed;
        pivot.localPosition = Vector3.Lerp(pivot.localPosition, pivotPosition, t);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newCameraPosition, t);
    }

    private void HandleRotation()
    {
        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");
        if (CameraProperties.turnSmooth > 0)
        {
            _smoothX = Mathf.SmoothDamp(_smoothX, _mouseX, ref _smoothXVelocity, CameraProperties.turnSmooth);
            _smoothY = Mathf.SmoothDamp(_smoothY, _mouseY, ref _smoothYVelocity, CameraProperties.turnSmooth);
        }
        else
        {
            _smoothX = _mouseX;
            _smoothY = _mouseY;
        }

        _lookAngle += _smoothX * CameraProperties.X_rot_speed;
        Quaternion targetRot = Quaternion.Euler(0, _lookAngle, 0);
        transform.rotation = targetRot;

        _titleAngle -= _smoothY * CameraProperties.Y_rot_speed;
        _titleAngle = Mathf.Clamp(_titleAngle, CameraProperties.minAngle, CameraProperties.maxAngle);
        pivot.localRotation = Quaternion.Euler(_titleAngle, 0, 0);
    }

    #endregion

}