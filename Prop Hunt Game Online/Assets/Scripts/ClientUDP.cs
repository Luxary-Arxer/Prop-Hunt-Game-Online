using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ClientUDP : MonoBehaviour
{
    Socket socket;

    string clientText;

    public string IPServer;
    public string NamePlayer;

    public void Awake()
    {
        IPServer = JoinInformation.client_Home.clientIP;

        Debug.Log("Server IP: " + IPServer);

        NamePlayer = JoinInformation.client_Home.clientName;
        read_Name(NamePlayer);

        Debug.Log("Player name: " + NamePlayer);
    }

    void Start()
    {
        Send();
    }

    public void StartClient()
    {
        Thread mainThread = new Thread(Send);
        mainThread.Start();
    }

    void Update()
    {
        Send();

        //UItext.text = clientText;
        // Enviar la posición del jugador en cada frame
        SendPlayerPosition();
    }

    void Send()
    {
        // Send position data every frame
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPServer), 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Simulating player position (can be replaced with actual player position)
        Vector3 playerPosition = transform.position;

        string message = "Position: " + playerPosition.x + "," + playerPosition.y + "," + playerPosition.z;
        byte[] data = Encoding.ASCII.GetBytes(message);
        socket.SendTo(data, ipep);

        clientText = "Position sent to " + ipep.ToString() + ": " + message;
        Debug.Log(clientText);

        // Receive server response
        Thread receive = new Thread(Receive);
        receive.Start();
    }

    void Receive()
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)sender;
        byte[] data = new byte[1024];

        try
        {
            while (true)
            {
                int recv = socket.ReceiveFrom(data, ref remote);
                string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                clientText += "\nMessage received from " + remote.ToString() + ": " + receivedMessage;

                // Log the server's response
                Debug.Log("Message received from server: " + receivedMessage);
            }
        }
        catch (SocketException e)
        {
            clientText += "\nError receiving data: " + e.Message;
        }
    }

    public void read_IP(string IP)
    {
        IPServer = IP;
        Debug.Log("Server IP set to: " + IPServer);
    }

    public void read_Name(string Name)
    {
        NamePlayer = Name;
        DisplayPlayerName.NamePlayer_1 = NamePlayer;
        Debug.Log("Player name updated to: " + NamePlayer);
    }
}
