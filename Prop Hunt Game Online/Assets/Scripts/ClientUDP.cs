using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;

public class ClientUDP : MonoBehaviour
{
    private Socket socket;
    public string IPServer; 
    private Vector3 playerPosition; 
    private Vector3 playerRotation; 
    private string message; // Mensaje a enviar
    private bool running = true; // Bandera para manejar el bucle de envío
    public GameObject serverObject;
    private Queue<Vector3> positionQueue = new Queue<Vector3>();
    private Vector3 newPosition_server;
    private Vector3 newRotation_server;
    private Vector3 lastPosition;
    private Vector3 lastRotation;

    void Start()
    {
        // Inicializar posición y mensaje
        playerPosition = transform.position;
        message = "Position: " + playerPosition.x + "|" + playerPosition.y + "|" + playerPosition.z;
        lastPosition = serverObject.transform.position;
        lastRotation = serverObject.transform.eulerAngles;

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
        InitializeSocket(); // Inicializamos el socket antes de usarlo
        StartClient(); // Iniciar el cliente para recibir y enviar datos
    }

    void Update()
    {
        if (positionQueue.Count > 2)
        {
            newPosition_server = positionQueue.Dequeue();
            newRotation_server = positionQueue.Dequeue();

            if (!serverObject.activeSelf && serverObject.transform.position != newPosition_server)
            {
                serverObject.SetActive(true);
            }

            // Interpolación de la posición y rotación para suavizar el movimiento
            serverObject.transform.position = Vector3.Lerp(lastPosition, newPosition_server, 0.5f);
            serverObject.transform.eulerAngles = Vector3.Lerp(lastRotation, newRotation_server, 0.5f);

            lastPosition = serverObject.transform.position;
            lastRotation = serverObject.transform.eulerAngles;

            //serverObject.transform.position = newPosition_server;
           // serverObject.transform.eulerAngles = newRotation_server;


        }

        // Actualizar la posición del jugador y el mensaje
        playerPosition = transform.position;
        playerRotation = transform.eulerAngles;
        message = "Position: " + playerPosition.x + "|" + playerPosition.y + "|" + playerPosition.z
            + "|" + playerRotation.x + "|" + playerRotation.y + "|" + playerRotation.z;
        //Debug.Log("Posición X " + newPosition_server.x + " Posición Z " + newPosition_server.z);
    }

    void InitializeSocket()
    {
        try
        {
            // Configuración del endpoint del servidor
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPServer), 9050);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect(ipep); // Asegurarse de que el socket se conecta al servidor
            Debug.Log("Socket inicializado correctamente.");
        }
        catch (SocketException e)
        {
            Debug.LogError("Error al inicializar el socket: " + e.Message);
        }
    }

    void StartClient()
    {
        // Crear y empezar un nuevo thread para enviar datos
        Thread sendThread = new Thread(Send);
        sendThread.IsBackground = true; // Permitir que el thread termine automáticamente al cerrar la aplicación
        sendThread.Start();

        // Crear un nuevo thread para recibir datos
        Thread receiveThread = new Thread(Receive);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void Send()
    {
        try
        {
            // Verifica que el socket está inicializado
            if (socket == null)
            {
                Debug.LogError("El socket no está inicializado correctamente.");
                return;
            }

            // Configuración del endpoint del servidor (ya se inicializa en InitializeSocket)
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPServer), 9050);

            while (running)
            {
                // Serializar y enviar datos al servidor
                byte[] data = Encoding.ASCII.GetBytes(message);
                socket.SendTo(data, ipep);
                Debug.Log("Datos enviados al servidor: " + message);

                
                Thread.Sleep(16); // 60FPS
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Error en el cliente: " + e.Message);
        }
    }

    void Receive()
    {
        if (socket == null)
        {
            Debug.LogError("El socket no está inicializado correctamente.");
            return;
        }

        byte[] data = new byte[1024];
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)sender;
        Debug.Log("Esperando mensajes...");

        try
        {
            while (running)
            {
                // Recibir mensaje sin bloquear innecesariamente el hilo
                int recv = socket.ReceiveFrom(data, ref remote);
               
                if (recv > 0) // Solo procesar si se recibieron datos
                {
                    // Convertir solo cuando sea necesario
                    string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                    Debug.Log($"Mensaje recibido de {remote}: {receivedMessage}");
                    // Solo procesar mensajes relevantes
                    if (receivedMessage.Contains("Position:"))
                    {
                        UpdatePositionQueue(receivedMessage);
                        UpdateRotation(receivedMessage);
                    }
                }
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
