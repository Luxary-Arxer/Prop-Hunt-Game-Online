using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ClientTCP : MonoBehaviour
{
    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    string clientText;
    Socket server;
    private string IPServer;

    //public DisplayPlayerName Name;
    public string NamePlayer= "No Name";

    // Start is called before the first frame update
    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
        NamePlayer = DisplayPlayerName.NamePlayer;
            //"Unnamed Player_TCP";
    }

    // Update is called once per frame
    void Update()
    {
        UItext.text = clientText;
    }

    public void StartClient()
    {
        // Iniciar el hilo para conectar al servidor
        Thread connect = new Thread(Connect);
        connect.Start();
    }

    void Connect()
    {
        //TO DO 2
        //Create the server endpoint so we can try to connect to it.
        //You'll need the server's IP and the port we binded it to before
        //Also, initialize our server socket.
        //When calling connect and succeeding, our server socket will create a
        //connection between this endpoint and the server's endpoint
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPServer), 9050);
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            // Intentar conectar al servidor
            server.Connect(ipep);
            clientText = "Connected to server at " + ipep.ToString();
        }
        catch (SocketException e)
        {
            clientText = "Connection failed: " + e.Message;
            return;
        }

        //TO DO 4
        //With an established connection, we want to send a message so the server aacknowledges us
        //Start the Send Thread
        Thread sendThread = new Thread(Send);
        sendThread.Start();

        //TO DO 7
        //If the client wants to receive messages, it will have to start another thread. Call Receive()
        Thread receiveThread = new Thread(Receive);
        receiveThread.Start();
    }

    void Send()
    {
        //TO DO 4
        //Using the socket that stores the connection between the 2 endpoints, call the TCP send function with
        //an encoded message
        try
        {
            string message = "Player name: " + NamePlayer;
            byte[] data = Encoding.ASCII.GetBytes(message);
            server.Send(data);
            clientText += "\nSent: " + message;
        }
        catch (SocketException e)
        {
            clientText += "\nError sending data: " + e.Message;
        }
    }

    //TO DO 7
    //Similar to what we already did with the server, we have to call the Receive() method from the socket.
    void Receive()
    {
        byte[] data = new byte[1024];
        int recv;

        while (true)
        {
            try
            {
                recv = server.Receive(data); 
                if (recv == 0) 
                    break;

                string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                clientText += "\nReceived: " + receivedMessage;
            }
            catch (SocketException e)
            {
                clientText += "\nError receiving data: " + e.Message;
                break;
            }
        }
    }

    //Leer Ip que te da el player
    public void read_IP(string IP)
    {
        IPServer = IP;
        Debug.Log(IPServer);
    }
    //Leer el nombre que te da el player
    public  void  read_Name(string Name)
    {
        NamePlayer = Name;
        DisplayPlayerName.NamePlayer = NamePlayer;
        Debug.Log(NamePlayer);
    }
}
