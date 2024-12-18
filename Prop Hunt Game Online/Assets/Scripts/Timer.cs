using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI cronometroTexto; // Texto del cronómetro
    public RawImage fondo; // Fondo que se activará
    public TextMeshProUGUI mensajeTexto; // Mensaje que se activará

    private float tiempo; // Tiempo en segundos
    private bool enMarcha;

    void Start()
    {
        tiempo = 0f; // Iniciar en 0
        enMarcha = true; // Activar el cronómetro

     
        if (fondo != null)
            fondo.gameObject.SetActive(false);

        if (mensajeTexto != null)
            mensajeTexto.gameObject.SetActive(false);
    }

    void Update()
    {
        if (enMarcha)
        {
            tiempo += Time.deltaTime; // Aumentar el tiempo cada frame
            int minutos = Mathf.FloorToInt(tiempo / 60); // Obtener minutos
            int segundos = Mathf.FloorToInt(tiempo % 60); // Obtener segundos

            // Actualizar el texto del cronómetro
            cronometroTexto.text = $"{minutos:00}:{segundos:00}";

            // Verificar si han pasado 5 minutos (300 segundos)
            if (tiempo >= 300f)
            {
                ActivarFondoYMensaje();
                enMarcha = false; // Detener el cronómetro
            }
        }
    }

    void ActivarFondoYMensaje()
    {
        if (fondo != null)
        {
            fondo.gameObject.SetActive(true); // Activar el fondo
        }

        if (mensajeTexto != null)
        {
            mensajeTexto.gameObject.SetActive(true); // Activar el texto
        }

        Debug.Log("¡5 minutos alcanzados!");
    }

    public void DetenerCronometro()
    {
        enMarcha = false; 
    }

    public void ReiniciarCronometro()
    {
        tiempo = 0f; // Reiniciar el tiempo
        enMarcha = true; // Volver a activar el cronómetro

        
        if (fondo != null)
            fondo.gameObject.SetActive(false);

        if (mensajeTexto != null)
            mensajeTexto.gameObject.SetActive(false);
    }
}
