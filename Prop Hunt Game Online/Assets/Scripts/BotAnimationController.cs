using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAnimationController : MonoBehaviour
{
    private Animator anim;
    private Vector3 lastPosition; // Última posición del bot.
    private float speed; // Para almacenar la velocidad.
    private bool isGrounded; // Para saber si está en el suelo.

    private float timeAccumulator = 0f; // Acumulador de tiempo para controlar la frecuencia del cálculo de velocidad.
    private float updateInterval = 0.1f; // Intervalo de actualización en segundos (50 ms). 

    void Start()
    {
        anim = GetComponent<Animator>();
        lastPosition = transform.position; // Inicializamos lastPosition con la posición actual.
    }

    void Update()
    {
        timeAccumulator += Time.deltaTime;
        if (timeAccumulator >= updateInterval)
        {
            // Calcular el movimiento del bot (comparando la posición actual con la anterior).
            Vector3 movement = transform.position - lastPosition;
            speed = new Vector3(movement.x, 0, movement.z).magnitude; // Calculamos la magnitud de la velocidad (solo en X y Z).


            anim.SetFloat("Speed", speed); // Aquí usamos "Speed" para la transición a locomotion.
            // Verificamos si el bot está en el suelo utilizando un raycast.
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f); 
            anim.SetBool("Grounded", isGrounded); 

            // Guardamos la posición actual para el siguiente frame.
            lastPosition = transform.position;

            timeAccumulator = 0f;
        }
    }
}
    

