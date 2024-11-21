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

    // Cube to move based on player position
    public GameObject playerCube;

    private void Awake()
    {
        // Initialize the player name
        string NamePlayer = "Server"; // Can be customized
        read_Name(NamePlayer);
        Debug.Log(NamePlayer);
    }

    void Start()
    {
        startServer();
    }

    public void startServer()
    {
        Debug.Log("Starting UDP Server...");

        // UDP binding to receive data on port 9050
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep);

        Debug.Log("\nServer started on port 9050");

        // Thread to receive data
        Thread newConnection = new Thread(Receive);
        newConnection.Start();
    }

    void Receive()
    {
        byte[] data = new byte[1024];
        Debug.Log("\nWaiting for new Client...");

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)(sender);

        // Loop to continuously receive data
        while (true)
        {
            try
            {
                int recv = socket.ReceiveFrom(data, ref Remote);
                string message = Encoding.ASCII.GetString(data, 0, recv);

                // Log when a message is received from a client
                Debug.Log("Received message from " + Remote.ToString() + ": " + message);

                // Check for position update message
                if (message.Contains("Position:"))
                {
                    // Parse the position
                    string[] positionData = message.Split(':')[1].Trim().Split(',');

                    if (positionData.Length == 3)
                    {
                        // Parse position values from the message
                        float x = float.Parse(positionData[0]);
                        float y = float.Parse(positionData[1]);
                        float z = float.Parse(positionData[2]);

                        // Update the cube's position
                        UpdateCubePosition(new Vector3(x, y, z));
                    }
                }

                // Start a new thread to send a ping back to the client (or acknowledge)
                Thread sendThread = new Thread(() => Send(Remote));
                sendThread.Start();
            }
            catch (SocketException e)
            {
                Debug.LogError("Error receiving data: " + e.Message);
            }
        }
    }

    // Method to update the cube's position based on the received data
    void UpdateCubePosition(Vector3 newPosition)
    {
        if (playerCube != null)
        {
            playerCube.transform.position = newPosition;
            Debug.Log("Cube position updated: " + newPosition);
        }
        else
        {
            Debug.LogError("Player Cube not assigned in the server!");
        }
    }

    void Send(EndPoint Remote)
    {
        byte[] data = Encoding.ASCII.GetBytes("Ping");
        socket.SendTo(data, Remote);
        Debug.Log("Ping sent to " + Remote.ToString());
    }

    public void read_Name(string Name)
    {
        Debug.Log("Server name set to: " + Name);
    }
}
