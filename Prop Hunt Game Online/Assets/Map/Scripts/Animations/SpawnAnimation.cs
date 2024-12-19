using System;
using System.Collections;
using UnityEngine;

public class SpawnAnimation : MonoBehaviour
{
    [SerializeField] private float Duration = 0.25f;
    [SerializeField] private Vector3 startSize = Vector3.one * 0.1f;
    [SerializeField] private Vector3 finalSize = Vector3.one;

    public event Action OnAnimationEnd;

    public void StartSpawnAnimation()
    {
        StartCoroutine(SpawningAnimation(Duration, startSize, finalSize));
    }

    public void StartSpawnAnimation(float duration, Vector3 startSize, Vector3 finalSize)
    {
        StartCoroutine(SpawningAnimation(duration, startSize, finalSize));
    }

    private IEnumerator SpawningAnimation(float duration, Vector3 startSize, Vector3 finalSize)
    {
        duration = duration * 100;
        transform.localScale = startSize; // Set start scale

        // Making the object bigger
        for (float i = startSize.x; i < finalSize.x; i += (finalSize.x - startSize.x) / duration)
        {
            transform.localScale = new Vector3(i, i, i);
            yield return new WaitForSeconds(0.01f);
        }

        OnAnimationEnd?.Invoke();
    }
}
