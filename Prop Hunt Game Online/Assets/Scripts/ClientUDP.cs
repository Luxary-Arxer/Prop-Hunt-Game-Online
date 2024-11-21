using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ClientUDP : MonoBehaviour
{
    Socket socket;
    /*
    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    */
    public string IPServer;
    public string NamePlayer;
    public GameObject player; // El objeto que representa al jugador (su avatar o similar)

    public void Awake()
    {
        IPServer = JoinInformation.client_Home.clientIP;
        NamePlayer = JoinInformation.client_Home.clientName;
        Debug.Log(IPServer);
    }

    void Start()
    {
        Send();
    }

    void Update()
    {
        //UItext.text = clientText;
        // Enviar la posición del jugador en cada frame
        SendPlayerPosition();
    }

    void Send()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPServer), 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        byte[] data = Encoding.ASCII.GetBytes("Player name: " + NamePlayer);
        socket.SendTo(data, ipep);
        //clientText = "Handshake sent to " + ipep.ToString();
        Thread receive = new Thread(Receive);
        receive.Start();
    }

    void SendPlayerPosition()
    {
        if (player != null)
        {
            Vector3 position = player.transform.position; // Obtener la posición del jugador
            string positionMessage = position.x + "," + position.y + "," + position.z;

            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPServer), 9050);
            byte[] data = Encoding.ASCII.GetBytes(positionMessage);
            socket.SendTo(data, ipep);
        }
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
                Debug.Log("Message received: " + receivedMessage);
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
        }
    }
}
