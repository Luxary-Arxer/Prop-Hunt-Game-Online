using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;

public class ServerUDP : MonoBehaviour
{
    Socket socket;
    public GameObject playerCube; // Objeto que se moverá según los datos del cliente
    public GameObject serverObject; // Objeto controlado por el servidor

    private bool running = true;

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

        try
        {
            while (running)
            {
                int recv = socket.ReceiveFrom(data, ref remote);
                string message = Encoding.ASCII.GetString(data, 0, recv);
                Debug.Log("Mensaje recibido del cliente: " + message);

                if (message.Contains("Position:"))
                {
                    UpdatePlayerCubePosition(message);
                }

                Send(remote);
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Error al recibir datos: " + e.Message);
        }
    }

    void UpdatePlayerCubePosition(string message)
    {
        string[] positionData = message.Split(':')[1].Trim().Split(',');

        if (positionData.Length == 3)
        {
            if (float.TryParse(positionData[0], out float x) &&
                float.TryParse(positionData[1], out float y) &&
                float.TryParse(positionData[2], out float z))
            {
                Vector3 newPosition = new Vector3(x, y, z);
                if (playerCube != null)
                {
                    playerCube.transform.position = newPosition;
                    Debug.Log("Posición del cubo actualizada: " + newPosition);
                }
            }
        }
    }

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
