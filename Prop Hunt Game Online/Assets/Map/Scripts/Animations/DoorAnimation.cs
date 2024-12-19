using System.Collections;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 targetRotation;

    public float Speed = 1f;
    private bool _openDoor;

    void Start() => Speed *= 0.1f;       

    void Update()
    {
        if (_openDoor)
        {
            transform.localPosition = MoveObject(targetPosition);
            transform.localRotation = RotateObject(targetRotation);

            if (transform.localPosition == targetPosition && transform.localRotation == Quaternion.Euler(targetRotation))
                _openDoor = false;
        }
    }

    public IEnumerator OpenDoor()
    {
        _openDoor = true;
        yield return null;
    }

    Vector3 MoveObject(Vector3 targetPosition) => Vector3.Lerp(transform.localPosition, targetPosition, Speed);

    Quaternion RotateObject(Vector3 targetRotation) => Quaternion.Lerp(transform.localRotation, Quaternion.Euler(targetRotation), Speed);

}
