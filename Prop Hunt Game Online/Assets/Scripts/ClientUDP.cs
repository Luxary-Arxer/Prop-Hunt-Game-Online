using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ClientUDP : MonoBehaviour
{
    Socket socket;
    public string IPServer;
    private bool connected = false;
    public string NamePlayer;
    Vector3 playerPosition;
    string message;

    void Start()
    {
        playerPosition = transform.position;
        message = "Position: " + playerPosition.x + "," + playerPosition.y + "," + playerPosition.z;

        IPServer = JoinInformation.client_Home.clientIP;
        Debug.Log("Server IP: " + IPServer);
        NamePlayer = JoinInformation.client_Home.clientName;
        read_Name(NamePlayer);
        Debug.Log("Player name: " + NamePlayer);

        StartClient();
    }

    private void Update()
    {
        playerPosition = transform.position;
        message = "Position: " + playerPosition.x + "," + playerPosition.y + "," + playerPosition.z;
    }

    public void StartClient()
    {
        Thread sendThread = new Thread(Send);
        sendThread.Start();
    }

    void Send()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPServer), 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        while (true)
        {
            // Send position data

            byte[] data = Encoding.ASCII.GetBytes(message);
            socket.SendTo(data, ipep);
            Debug.Log("Position sent: " + message);

            // Try receiving the server response only once
            if (!connected)
            {
                Receive();
                connected = true;
                Debug.Log("HAY CONEXION YIPY");
            }

            Thread.Sleep(500); // Send data every 500ms
        }
    }

    void Receive()
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)sender;
        byte[] data = new byte[1024];

        try
        {
            int recv = socket.ReceiveFrom(data, ref remote);
            string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("Message received from server: " + receivedMessage);
        }
        catch (SocketException e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
        }
    }

    public void read_Name(string Name)
    {
        NamePlayer = Name;
        DisplayPlayerName.NamePlayer_1 = NamePlayer;
        Debug.Log("Player name updated to: " + NamePlayer);
    }
}
