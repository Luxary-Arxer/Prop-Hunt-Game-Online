using UnityEngine;

[CreateAssetMenu(menuName = "Camera/Properties")]
public class CameraProperties : ScriptableObject
{
    public float turnSmooth = 0.1f;
    public float pivotSpeed = 9f;
    public float Y_rot_speed = 7f;
    public float X_rot_speed = 7f;
    public float minAngle = -35f;
    public float maxAngle = 50f;
    public float normalZ = -3f;
    public float normalX = 0.75f;
    public float normalY = 1.5f;
}
