using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float lifeTime = 3;

    private void Update()
    {

        lifeTime -= Time.deltaTime;
        //Debug.Log(lifeTime);
        if (lifeTime < 0)
            Destroy(gameObject);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Dummy>() != null)
        {
            other.GetComponent<Dummy>().CurrentHealth -= damage;
        }
        Destroy(gameObject);
    }
}