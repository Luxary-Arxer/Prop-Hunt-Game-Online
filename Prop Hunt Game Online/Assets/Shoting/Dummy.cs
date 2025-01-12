using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Dummy : MonoBehaviour
{
    public float MaxHealth;
    public float CurrentHealth;
    public float RespawnTime;
    public GameObject Mesh;
    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    private void Update()
    {
        if (CurrentHealth <= 0) {

            Mesh.SetActive(false);
            StartCoroutine("Countdown", RespawnTime);         

        }
    }

    IEnumerator Countdown(int seconds)
    {
        int counter = seconds;
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
        }
        Mesh.SetActive(true);
        CurrentHealth = MaxHealth;
    }
}