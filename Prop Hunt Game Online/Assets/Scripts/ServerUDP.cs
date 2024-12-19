using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;

public class ServerUDP : MonoBehaviour
{
    private Socket socket;
    private bool running = true;
    Vector3 serverPosition;
    Vector3 serverRotation;
    bool serverTeamHunter;
    private Dictionary<EndPoint, ClientData> clients = new Dictionary<EndPoint, ClientData>();

    public GameObject clientPrefab;
    public GameObject serverPrefab;

    private Queue<ClientUpdate> clientUpdateQueue = new Queue<ClientUpdate>();

    private class ClientData
    {
        public GameObject playerObject;
        public Vector3 position;
        public Vector3 rotation;
        public bool isHunter;
        public GameObject mesh;
    }

    private class ClientUpdate
    {
        public EndPoint remote;
        public Vector3 position;
        public Vector3 rotation;
        public bool isHunter;
    }

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

            Debug.Log("Servidor iniciado en " + ipep.Address.ToString());

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
        EndPoint remote = new IPEndPoint(IPAddress.Any, 0);

        while (running)
        {
            int recv = socket.ReceiveFrom(data, ref remote);

            if (recv > 0)
            {
                string message = Encoding.ASCII.GetString(data, 0, recv);

                if (message.Contains("Position:"))
                {
                    ProcessMessage(remote, message);
                }
            }
        }
    }

    void ProcessMessage(EndPoint remote, string message)
    {
        string[] positionData = message.Split(':')[1].Trim().Split("|");
        if (positionData.Length == 7)
        {
            float x = float.Parse(positionData[0]);
            float y = float.Parse(positionData[1]);
            float z = float.Parse(positionData[2]);
            float x_rotation = float.Parse(positionData[3]);
            float y_rotation = float.Parse(positionData[4]);
            float z_rotation = float.Parse(positionData[5]);
            bool isHunter = bool.Parse(positionData[6]);

            ClientUpdate newUpdate = new ClientUpdate
            {
                remote = remote,
                position = new Vector3(x, y, z),
                rotation = new Vector3(x_rotation, y_rotation, z_rotation),
                isHunter = isHunter
            };

            clientUpdateQueue.Enqueue(newUpdate);
        }
    }

    void BroadcastData()
    {
        foreach (var client in clients)
        {
            ClientData data = client.Value;
            string messageToSend = $"Position:{data.position.x}|{data.position.y}|{data.position.z}|" +
                                   $"{data.rotation.x}|{data.rotation.y}|{data.rotation.z}|" +
                                   $"{data.isHunter.ToString()}|" +
                                   $"{(data.mesh != null ? data.mesh.name : "default_mesh")}";
            byte[] sendData = Encoding.ASCII.GetBytes(messageToSend);
            socket.SendTo(sendData, client.Key);

            Debug.Log($"Datos enviados al cliente {client.Key}: {messageToSend}");
        }
    }

    void Update()
    {
        while (clientUpdateQueue.Count > 0)
        {
            ClientUpdate update = clientUpdateQueue.Dequeue();

            if (!clients.ContainsKey(update.remote))
            {
                clients[update.remote] = new ClientData();
            }

            ClientData clientData = clients[update.remote];
            clientData.position = update.position;
            clientData.rotation = update.rotation;
            clientData.isHunter = update.isHunter;

            GameObject clientObject = Instantiate(clientPrefab, clientData.position, Quaternion.Euler(clientData.rotation));
            clientObject.SetActive(true);
            clientObject.GetComponent<PlayerToProp>().Hunter = update.isHunter;

            if (update.isHunter)
            {
                clientObject.GetComponent<PlayerToProp>().CaraterMesh.SetActive(false); // Mesh principal
                clientObject.GetComponent<PlayerToProp>().currentModel.SetActive(true);
            }
            else
            {
                GameObject propMesh = Resources.Load<GameObject>("PropMesh"); // Change this with your actual prefab path
                GameObject prop = Instantiate(propMesh, clientData.position, Quaternion.identity);
                prop.SetActive(true);
                clientData.mesh = prop;
            }
        }
    }

    private void OnDestroy()
    {
        running = false;
        socket.Close();
    }
}
