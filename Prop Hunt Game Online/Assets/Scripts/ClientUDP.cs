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

    void Start()
    {
        IPServer = JoinInformation.client_Home.clientIP;
        StartClient();
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
            // Get the player position
            Vector3 playerPosition = transform.position;

            // Send position data
            string message = "Position: " + playerPosition.x + "," + playerPosition.y + "," + playerPosition.z;
            byte[] data = Encoding.ASCII.GetBytes(message);
            socket.SendTo(data, ipep);
            Debug.Log("Position sent: " + message);

            // Try receiving the server response only once
            if (!connected)
            {
                Receive();
                connected = true;
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
}
