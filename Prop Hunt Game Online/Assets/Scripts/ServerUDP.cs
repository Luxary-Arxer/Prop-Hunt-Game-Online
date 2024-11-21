using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ServerUDP : MonoBehaviour
{
    Socket socket;

    /*
    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    */
    public GameObject cube; // El cubo que se moverá
    private Vector3 playerPosition;


    public GameObject UItextObj;
    TextMeshProUGUI UItext;

    public string NamePlayer = "No Name";

    private void Awake()
    {
        NamePlayer = ChangeScene.server_Home.serverName;
        read_Name(NamePlayer);
        Debug.Log(NamePlayer);
    }

    void Start()
    {
        startServer();
        //UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    public void startServer()
    {
        Debug.Log("Starting UDP Server...");

        //serverText = "Starting UDP Server...";
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep); // Vincula el socket al puerto 9050
        Debug.Log("\nServer started on port 9050");
        //serverText += "\nServer started on port 9050";

        Thread newConnection = new Thread(Receive);
        newConnection.Start();
    }
    void Update()
    {
        //UItext.text = serverText;
    }
    void Receive()
    {
        byte[] data = new byte[1024];
        Debug.Log("Waiting for new Client...");

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)sender;

        while (true)
        {
            try
            {
                int recv = socket.ReceiveFrom(data, ref remote);
                string message = Encoding.ASCII.GetString(data, 0, recv);

                // Process message from client (e.g., player name or position data)
                Debug.Log("Message received from " + remote.ToString() + ": " + message);

                // If the message is position data, log the received position
                if (message.Contains("Position"))
                {
                    Debug.Log("Position received: " + message);
                }

                // Handle response by pinging back the client
                Thread sendThread = new Thread(() => Send(remote));
                sendThread.Start();
            }
            catch (SocketException e)
            {
                Debug.LogError("Error receiving data: " + e.Message);
            }
        }
    }

    void Send(EndPoint remote)
    {
        byte[] data = Encoding.ASCII.GetBytes("Ping");
        socket.SendTo(data, remote);
        Debug.Log("Ping sent to " + remote.ToString());
    }

    public void read_Name(string Name)
    {
        NamePlayer = Name;
        DisplayPlayerName.NamePlayer_1 = NamePlayer;
        Debug.Log("Player name set to: " + NamePlayer);
    }
}
