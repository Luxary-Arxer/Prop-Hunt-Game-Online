using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;

public class ClientUDP : MonoBehaviour
{
    Socket socket;
    public string IPServer; // Dirección IP del servidor
    Vector3 playerPosition; // Posición del jugador
    Vector3 playerRotation; // Rotación del jugador
    string message; // Mensaje a enviar
    bool running = true; // Bandera para manejar el bucle de envío
    public GameObject serverObject;
    private Queue<Vector3> positionQueue = new Queue<Vector3>();
    private Vector3 newPosition_server;
    private Vector3 newRotation_server;

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
        else
        {
            IPServer = "127.0.0.1";
        }

        // Iniciar el cliente
        StartClient();
    }

    void Update()
    {
        if (positionQueue.Count > 0)
        {
            newPosition_server = positionQueue.Dequeue();
            newRotation_server = positionQueue.Dequeue();

            if (serverObject.activeSelf == false && serverObject.transform.position != newPosition_server)
            {
                serverObject.SetActive(true);
            }

            serverObject.transform.position = newPosition_server;
            serverObject.transform.eulerAngles = newRotation_server;
        }

        // Actualizar la posición del jugador y el mensaje
        playerPosition = transform.position;
        playerRotation = transform.eulerAngles;
        message = "Position: " + playerPosition.x + "|" + playerPosition.y + "|" + playerPosition.z
            + "|" + playerRotation.x + "|" + playerRotation.y + "|" + playerRotation.z;
        Debug.Log("Posición X " + newPosition_server.x + " Posición Z " + newPosition_server.z);
    }

    void StartClient()
    {
        // Crear y empezar un nuevo hilo para enviar datos
        Thread sendThread = new Thread(Send);
        sendThread.IsBackground = true; // Permitir que el hilo termine automáticamente al cerrar la aplicación
        sendThread.Start();

        // Crear y empezar un nuevo hilo para recibir datos
        Thread receiveThread = new Thread(Receive);
        receiveThread.IsBackground = true;
        receiveThread.Start();
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

    void Receive()
    {
        byte[] data = new byte[1024];
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)sender;
        Debug.Log("Esperando mensajes...");

        try
        {
            while (running)
            {
                int recv = socket.ReceiveFrom(data, ref remote);
                string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                Debug.Log("Mensaje recibido del servidor: " + receivedMessage);

                if (receivedMessage.Contains("Position:"))
                {
                    UpdatePositionQueue(receivedMessage);
                    UpdateRotation(receivedMessage);
                }
                // Pausar para no sobrecargar la CPU
                Thread.Sleep(10);
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Error al recibir datos: " + e.Message);
        }
    }

    void UpdatePositionQueue(string message)
    {
        string[] positionData = message.Split(':')[1].Trim().Split("|");
        if (positionData.Length == 6)
        {
            float x = float.Parse(positionData[0]);
            float y = float.Parse(positionData[1]);
            float z = float.Parse(positionData[2]);
            Vector3 newPosition = new Vector3(x, y, z);

            // Agregar la nueva posición a la cola
            positionQueue.Enqueue(newPosition);
        }
    }

    void UpdateRotation(string message)
    {
        string[] positionData = message.Split(':')[1].Trim().Split("|");
        if (positionData.Length == 6)
        {
            float x_rotation = float.Parse(positionData[3]);
            float y_rotation = float.Parse(positionData[4]);
            float z_rotation = float.Parse(positionData[5]);
            Vector3 newRotation = new Vector3(x_rotation, y_rotation, z_rotation);

            // Agregar la nueva rotación a la cola
            positionQueue.Enqueue(newRotation);
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
