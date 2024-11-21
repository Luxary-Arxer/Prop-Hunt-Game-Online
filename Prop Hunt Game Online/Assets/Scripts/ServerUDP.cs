using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;

public class ServerUDP : MonoBehaviour
{
    Socket socket;
    public GameObject playerCube; // Objeto que se moverá según los datos del cliente
    public GameObject serverObject; // Objeto controlado por el servidor
    public int numPlayers;


    private Queue<Vector3> positionQueue = new Queue<Vector3>();
    private bool running = true;

    private Vector3 newPosition_server;
    private Vector3 newRotation_server;

    Vector3 playerPosition; // Posición del jugador
    Vector3 playerRotation; // Rotación del jugador

    private string message;
    private string message_enviar;
    private float x;
    private float y;
    private float z;
    void Start()
    {
        StartServer();
        // Inicializar posición y mensaje
        playerPosition = transform.position;
        message = "Position: " + playerPosition.x + "|" + playerPosition.y + "|" + playerPosition.z;
    }

    void StartServer()
    {
        Debug.Log("Iniciando servidor UDP...");

        try
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(ipep);

            Debug.Log("Servidor iniciado en el puerto 9050");

            Thread receiveThread = new Thread(Receive);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (SocketException e)
        {
            Debug.LogError("Error al iniciar el servidor: " + e.Message);
        }
    }

    void Receive()
    {
        byte[] data = new byte[1024];
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)sender;
        Debug.Log("Mensaje running: " + running);
        try
        {
            while (running)
            {
                int recv = socket.ReceiveFrom(data, ref remote);
                message = Encoding.ASCII.GetString(data, 0, recv);
                Debug.Log("Mensaje recibido del cliente: " + message);

                if (message.Contains("Position:"))
                {
                    UpdatePositionQueue(message);
                    UpdateRotation(message);
                }
                Send(remote);
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
            Vector3 newPosition = new Vector3(x, y + 0f, z);

            /*
            float x_rotation = float.Parse(positionData[3]);
            float y_rotation = float.Parse(positionData[4]);
            float z_rotation = float.Parse(positionData[5]);
            Vector3 newRotation = new Vector3(x, y, z);
            */

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
            

            // Agregar la nueva posición a la cola
            positionQueue.Enqueue(newRotation);
        }
    }

    void Update()
    {
        if (positionQueue.Count > 0) 
        {

            newPosition_server = positionQueue.Dequeue();

            newRotation_server = positionQueue.Dequeue();

            if (playerCube.activeSelf == false && playerCube.transform.position != newPosition_server)
            {          
                playerCube.SetActive(true);
            }
            playerCube.transform.position = newPosition_server;

            playerCube.transform.eulerAngles = newRotation_server;

            Debug.Log("Posición X " + newPosition_server.x + "Posición Z " + newPosition_server.z);
            //Crear una copia del player asset para tener mas de 1 cliente no funciona
            /*
            if (numPlayers == 1)
            {
                GameObject playerClient = GameObject.Instantiate(playerCube);
                playerClient.SetActive(true);
                playerClient.transform.position = newPosition;
            }
            */
        }
        //Enviar la posicion del player
        playerPosition = transform.position;
        playerRotation = transform.eulerAngles;
        message_enviar = "Position: " + playerPosition.x + "|" + playerPosition.y + "|" + playerPosition.z
        + "|" + playerRotation.x + "|" + playerRotation.y + "|" + +playerRotation.z;
    }

    void Send(EndPoint remote)
    {
        try
        {
            byte[] data = Encoding.ASCII.GetBytes(message_enviar);
            socket.SendTo(data, remote);
            Debug.Log("Datos enviados al cliente: " + message_enviar);

        }
        catch (SocketException e)
        {
            Debug.LogError("Error al enviar datos: " + e.Message);
        }
    }
    private void OnDestroy()
    {
        running = false;
        if (socket != null)
        {
            socket.Close();
        }
    }
}
