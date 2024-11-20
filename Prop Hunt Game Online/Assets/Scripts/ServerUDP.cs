using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ServerUDP : MonoBehaviour
{
    Socket socket;

    public GameObject UItextObj;
    TextMeshProUGUI UItext;

    //public DisplayPlayerName Name;
    public string NamePlayer = "No Name";

    private void Awake()
    {
        NamePlayer = ChangeScene.server_Home.serverName;
        Debug.Log(NamePlayer);
    }

    void Start()
    {
        //UItext = UItextObj.GetComponent<TextMeshProUGUI>();
        startServer();
    }

    public void startServer()
    {
        Debug.Log("Starting UDP Server...");

        //UDP doesn't keep track of our connections like TCP
        //This means that we "can only" reply to other endpoints,
        //since we don't know where or who they are
        //We want any UDP connection that wants to communicate with 9050 port to send it to our socket.
        //So as with TCP, we create a socket and bind it to the 9050 port. 
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep); // Vincula el socket al puerto 9050

        Debug.Log("\nServer started on port 9050");

        //Our client is sending a handshake, the server has to be able to recieve it
        //It's time to call the Receive thread
        Thread newConnection = new Thread(Receive);
        newConnection.Start();
    }

    /*
    void Update()
    {
        UItext.text = serverText;
    }
    */
    void Receive()
    {
        byte[] data = new byte[1024];
        Debug.Log("\nWaiting for new Client...");

        //We don't know who may be comunicating with this server, so we have to create an
        //endpoint with any address and an IpEndpoint from it to reply to it later.
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)(sender);

        //Loop the whole process, and start receiveing messages directed to our socket
        //(the one we binded to a port before)
        //When using socket.ReceiveFrom, be sure send our remote as a reference so we can keep
        //this adress (the client) and reply to it on TO DO 4
        while (true)
        {
            try
            {
               int recv = socket.ReceiveFrom(data, ref Remote);
                string message = Encoding.ASCII.GetString(data, 0, recv);

                //serverText += "\nMessage received from " + Remote.ToString() + ": " + message;

                //When our UDP server receives a message from a random remote, it has to send a ping,
                //Call a send thread
                Thread sendThread = new Thread(() => Send(Remote));
                sendThread.Start();
            }
            catch (SocketException e)
            {
                //serverText += "\nError receiving data: " + e.Message;
            }
        }
    }

    void Send(EndPoint Remote)
    {
        //Use socket.SendTo to send a ping using the remote we stored earlier.
        byte[] data = Encoding.ASCII.GetBytes("Ping");
        socket.SendTo(data, Remote);
        //serverText += "\nPing sent to " + Remote.ToString();
    }
}
