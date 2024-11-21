using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;

public class ClientUDP : MonoBehaviour
{
    Socket socket;
    public string IPServer = "127.0.0.1"; // Dirección IP del servidor
    Vector3 playerPosition; // Posición del jugador
    string message; // Mensaje a enviar
    bool running = true; // Bandera para manejar el bucle de envío

    void Start()
    {
        // Inicializar posición y mensaje
        playerPosition = transform.position;
        message = "Position: " + playerPosition.x + "|" + playerPosition.y + "|" + playerPosition.z;

        // Obtener IP del servidor desde JoinInformation si está configurada
        if (JoinInformation.client_Home != null && !string.IsNullOrEmpty(JoinInformation.client_Home.clientIP))
        {
            IPServer = JoinInformation.client_Home.clientIP;
        }

        // Iniciar el cliente
        StartClient();
    }

    void Update()
    {
        // Actualizar la posición del jugador y el mensaje
        playerPosition = transform.position;
        message = "Position: " + playerPosition.x + "|" + playerPosition.y + "|" + playerPosition.z;
    }

    void StartClient()
    {
        // Crear y empezar un nuevo hilo para enviar datos
        Thread sendThread = new Thread(Send);
        sendThread.IsBackground = true; // Permitir que el hilo termine automáticamente al cerrar la aplicación
        sendThread.Start();
    }

    void Send()
    {
        try
        {
            // Configuración del endpoint del servidor
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPServer), 9050);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            while (running)
            {
                // Serializar y enviar datos al servidor
                byte[] data = Encoding.ASCII.GetBytes(message);
                socket.SendTo(data, ipep);
                Debug.Log("Datos enviados al servidor: " + message);

                // Pausa para controlar la frecuencia de envío
                Thread.Sleep(16); // Enviar cada segundo
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Error en el cliente: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        // Detener el bucle y cerrar el socket al destruir el objeto
        running = false;
        if (socket != null)
        {
            socket.Close();
        }
    }
}
