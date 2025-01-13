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
    public TextMeshPro ContadorGrande;
    public Transform Player;
    public GameObject Consola_1;
    public GameObject Consola_2;
    public float TiempoPartida = 2;

    private void Start()
    {
        Contador1.text = "Press E on the console to start the coutdown";
        Contador2.text = "Press E on the console to start the coutdown";
        Contador3.text = "Press E on the console to start the coutdown";
        ContadorGrande.text = "Press E on the console to start the coutdown";
    }
    void Update()
    {
        float interact_Range = 2f;
        Collider[] collider_array = Physics.OverlapSphere(transform.position, interact_Range);

        //StartCoroutine("Countdown", StartCoutdown);
        ContadorGrande.transform.LookAt(new Vector3(Player.position.x, Player.position.y, Player.position.z));

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
        Consola_1.transform.position = new Vector3(0, -8, 0);
        Consola_2.transform.position = new Vector3(0, -8, 0);
        int counter = seconds;
        while (counter > 0)
        {
            Contador1.text = "The match will\nstart in: " + counter.ToString();
            Contador2.text = "The match will\nstart in: "+counter.ToString();
            Contador3.text = "The match will\nstart in: "+counter.ToString();
            ContadorGrande.text = "The match will\nstart in: " + counter.ToString();
            yield return new WaitForSeconds(1);
            counter--;
        }
        Puerta_1.SetActive(false);
        Puerta_2.SetActive(false);
        Contador1.text = "GO";
        Contador2.text = "GO";
        Contador3.text = "GO";
        ContadorGrande.text = "GO";
        StartCoroutine("TiempoRestante", TiempoPartida);
    }
    IEnumerator TiempoRestante(int seconds)
    {
        int counter = seconds;
        while (counter > 0)
        {
            Contador1.text = "Tiempo partida: " + counter.ToString();
            Contador2.text = "Tiempo partida: " + counter.ToString();
            Contador3.text = "Tiempo partida: " + counter.ToString();
            ContadorGrande.text = "Tiempo partida: " + counter.ToString();
            yield return new WaitForSeconds(1);
            counter--;
        }
        Player.transform.position = new Vector3(0, 1, 0);
    }
}
