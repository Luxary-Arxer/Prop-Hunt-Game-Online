using UnityEngine;

public class WalkPoint : MonoBehaviour
{
    [SerializeField] private GameObject nextPoint;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Enemy>() != null)
        {
            nextPoint.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
