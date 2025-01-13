using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Doors_Coutdown : MonoBehaviour
{

    public GameObject Letras;
    public GameObject Puerta_1;
    public GameObject Puerta_2;
    public float StartCoutdown = 10;
    public TextMeshPro Contador1;
    public TextMeshPro Contador2;
    public TextMeshPro Contador3;

    void Update()
    {
        float interact_Range = 2f;
        Collider[] collider_array = Physics.OverlapSphere(transform.position, interact_Range);

        //StartCoroutine("Countdown", StartCoutdown);

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
    IEnumerator Countdown(int seconds)
    {
        int counter = seconds;
        while (counter > 0)
        {
            Contador1.text = "The match will\nstart in:" + counter.ToString();
            Contador2.text = "The match will\nstart in:"+counter.ToString();
            Contador3.text = "The match will\nstart in:"+counter.ToString();
            yield return new WaitForSeconds(1);
            counter--;
        }
        Puerta_1.SetActive(false);
        Puerta_2.SetActive(false);
        Contador1.text = "GO";
        Contador2.text = "GO";
        Contador3.text = "GO";
    }
}
