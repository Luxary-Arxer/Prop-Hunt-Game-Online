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
    private Vector3 newPosition;
    private string message;
    private float x;
    private float y;
    private float z;
    void Start()
    {
        StartServer();
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
        if (positionData.Length == 3)
        {
            float x = float.Parse(positionData[0]);
            float y = float.Parse(positionData[1]);
            float z = float.Parse(positionData[2]);
            Vector3 newPosition = new Vector3(x, y + 0f, z);

            // Agregar la nueva posición a la cola
            positionQueue.Enqueue(newPosition);
        }
    }
    void Update()
    {
        if (positionQueue.Count > 0) 
        { 
            //string[] positionData = message.Split(':')[1].Trim().Split("|");
            //x = float.Parse(positionData[0]);
            //y = float.Parse(positionData[1]);
            //z = float.Parse(positionData[2]);

            newPosition = positionQueue.Dequeue();
            if (playerCube.activeSelf == false && playerCube.transform.position != newPosition)
            {          
                playerCube.SetActive(true);
            }
            playerCube.transform.position = newPosition;

            Debug.Log("Posición X " + newPosition.x);
            Debug.Log("Posición Z " + newPosition.z);
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
    }
    //void UpdatePlayerCubePosition(string message)
    //{
    //    string[] positionData = message.Split(':')[1].Trim().Split('.');

    //    if (positionData.Length == 3)
    //    {
    //        if (float.TryParse(positionData[0], out float x) &&
    //            float.TryParse(positionData[1], out float y) &&
    //            float.TryParse(positionData[2], out float z))
    //        {
    //            Vector3 newPosition = new Vector3(x, y, z);
    //            if (playerCube != null)
    //            {
    //                playerCube.transform.position = newPosition;
    //                Debug.Log("Posición del cubo actualizada: " + newPosition);
    //            }
    //        }
    //    }
    //}

    void Send(EndPoint remote)
    {
        try
        {
            byte[] data = Encoding.ASCII.GetBytes("Ping");
            socket.SendTo(data, remote);
            Debug.Log("Ping enviado al cliente");
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
