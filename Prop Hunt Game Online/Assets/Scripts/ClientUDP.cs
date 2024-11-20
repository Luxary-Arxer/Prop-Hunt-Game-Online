using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ClientUDP : MonoBehaviour
{
    Socket socket;
    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    string clientText;
    private string IPServer;

    //public DisplayPlayerName Name;
    public string NamePlayer = "No Name";

    // Start is called before the first frame update
    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
        NamePlayer = DisplayPlayerName.NamePlayer;
    }

    public void StartClient()
    {
        Thread mainThread = new Thread(Send);
        mainThread.Start();
    }

    void Update()
    {
        UItext.text = clientText;
    }

    void Send()
    {
        //TO DO 2
        //Unlike with TCP, we don't "connect" first,
        //we are going to send a message to establish our communication so we need an endpoint
        //We need the server's IP and the port we've binded it to before
        //Again, initialize the socket
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPServer), 9050);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //TO DO 2.1 
        //Send the Handshake to the server's endpoint.
        //This time, our UDP socket doesn't have it, so we have to pass it
        //as a parameter on it's SendTo() method
        byte[] data = Encoding.ASCII.GetBytes("Player name: " + NamePlayer);
        socket.SendTo(data, ipep);

        clientText = "Handshake sent to " + ipep.ToString();

        //TO DO 5
        //We'll wait for a server response,
        //so you can already start the receive thread
        Thread receive = new Thread(Receive);
        receive.Start();
    }

    //TO DO 5
    //Same as in the server, in this case the remote is a bit useless
    //since we already know it's the server who's communicating with us
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
            }
        }
        catch (SocketException e)
        {
            clientText += "\nError receiving data: " + e.Message;
        }
    }

    //Leer Ip que te da el player
    public void read_IP(string IP)
    {
        IPServer = IP;
        Debug.Log(IPServer);
    }
    //Leer el nombre que te da el player
    public void read_Name(string Name)
    {
        NamePlayer = Name;
        DisplayPlayerName.NamePlayer = NamePlayer;
        Debug.Log(NamePlayer);
    }
}
