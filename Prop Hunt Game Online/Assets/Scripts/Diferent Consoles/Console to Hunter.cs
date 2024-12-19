using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoletoHunter : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Letras1;

    void Update()
    {
        float interact_Range = 2f;
        Collider[] collider_array = Physics.OverlapSphere(transform.position, interact_Range);
        foreach (Collider collider in collider_array)
        {
            if (collider.name == "Mesh Player")
            {
                Letras1.SetActive(true);
            }
            else
            {
                Letras1.SetActive(false);
            }
        }
    }

    public void Interact()
    {
        Debug.Log("Interact with console Hunter");
    }
}
