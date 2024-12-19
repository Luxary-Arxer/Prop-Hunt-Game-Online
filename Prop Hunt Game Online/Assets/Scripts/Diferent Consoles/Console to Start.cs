using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoletoStart : MonoBehaviour
{

    public GameObject Letras;

    void Update()
    {
        float interact_Range = 2f;
        Collider[] collider_array = Physics.OverlapSphere(transform.position, interact_Range);
        foreach (Collider collider in collider_array)
        {
            if (collider.name == "Mesh Player")
            {
                Letras.SetActive(true);
            }
            else
            {
                Letras.SetActive(false);
            }
        }
    }

    public void Interact()
    {
        Debug.Log("Interact with console Start");
    }
}
