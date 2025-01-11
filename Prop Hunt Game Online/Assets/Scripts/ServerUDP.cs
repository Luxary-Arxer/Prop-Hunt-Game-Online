using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using System;

public class ServerUDP : MonoBehaviour
{
    private Socket socket;
    private bool running = true;
    Vector3 serverPosition;
    Vector3 serverRotation;

    private Change_OtherPlayers newmesh;

    private PlayerToProp mesh;

    // Diccionario para manejar múltiples clientes (clave: EndPoint, valor: datos del cliente)
    private Dictionary<EndPoint, ClientData> clients = new Dictionary<EndPoint, ClientData>();


    public GameObject clientPrefab;
    public GameObject serverPrefab;

    // Cola para gestionar la creación de clientes y actualizaciones en el hilo principal
    private Queue<ClientUpdate> clientUpdateQueue = new Queue<ClientUpdate>();

    private int nextClientId = 1;

    // Clase para manejar la información de los clientes
    private class ClientData
    {
        public GameObject playerObject; // Representa al cliente en la escena
        public Vector3 position;
        public Vector3 rotation;
        public int clientID;
    }

    // Clase para manejar las actualizaciones de los clientes
    private class ClientUpdate
    {
        public EndPoint remote;
        public Vector3 position;
        public Vector3 rotation;
        public bool isNewClient;
        public int playerPropId;
        public int clientID;
    }

    void Start()
    {
        mesh = serverPrefab.GetComponent<PlayerToProp>();
        newmesh = clientPrefab.GetComponent<Change_OtherPlayers>();
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

            Debug.Log("Servidor iniciado en el puerto 9050");

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

        try
        {
            while (running)
            {
                int recv = socket.ReceiveFrom(data, ref remote);

                if (recv > 0)
                {


                    string message = Encoding.ASCII.GetString(data, 0, recv);

                    Debug.Log($"Mensaje recibido de {remote}: {message}");

                    if (message.Contains("Position:"))
                    {
                        UpdateClientData(remote, message); // Actualizar posición/rotación del cliente
                    }
                    Thread.Sleep(5);
                }
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Error al recibir datos: " + e.Message);
        }
    }

    void UpdateClientData(EndPoint remote, string message)
    {
        string[] positionData = message.Split(':')[1].Trim().Split("|");
        if (positionData.Length == 8)
        {
            float x = float.Parse(positionData[0]);
            float y = float.Parse(positionData[1]);
            float z = float.Parse(positionData[2]);
            float rotX = float.Parse(positionData[3]);
            float rotY = float.Parse(positionData[4]);
            float rotZ = float.Parse(positionData[5]);
            int playerPropId = int.Parse(positionData[6]);
            bool teamHunter = bool.Parse(positionData[7]);
            bool isNewClient = !clients.ContainsKey(remote);

            // Asignar un ID único al cliente si es nuevo
            int clientID = 0;
            if (isNewClient)
            {
                clientID = nextClientId++;
            }

            lock (clientUpdateQueue)
            {
                clientUpdateQueue.Enqueue(new ClientUpdate
                {
                    remote = remote,
                    position = new Vector3(x, y, z),
                    rotation = new Vector3(rotX, rotY, rotZ),
                    isNewClient = isNewClient,
                    playerPropId = playerPropId,
                    clientID = clientID
                });
            }
        }
    }

    void AddNewClient(EndPoint remote, int clientID)
    {
        Debug.Log($"Nuevo cliente conectado: {remote}");

        // Instanciar un nuevo GameObject para el cliente en el hilo principal
        GameObject newPlayerObject = Instantiate(clientPrefab, Vector3.zero, Quaternion.identity);
        newPlayerObject.name = $"Player_{remote}";

        // Agregar al diccionario de clientes
        clients[remote] = new ClientData
        {
            playerObject = newPlayerObject,
            position = Vector3.zero,
            rotation = Vector3.zero,
            clientID = clientID
        };

        Debug.Log($"Se creó un objeto para el cliente: {remote}");
    }


    void Update()
    {

        serverPosition = serverPrefab.transform.position;
        serverRotation = serverPrefab.transform.eulerAngles;

        lock (clientUpdateQueue)
        {
            try
            {

                try
                {
                    // Construir los datos del servidor
                    string serverData = $"Position:{serverPosition.x}|{serverPosition.y}|{serverPosition.z}|" +
                                        $"{serverRotation.x}|{serverRotation.y}|{serverRotation.z}|{mesh.PlayerProp_Id}|{mesh.TeamHunter}";

                    foreach (var client in clients)
                    {
                        EndPoint clientEndPoint = client.Key;

                        // Crear un mensaje que incluye datos del servidor + todos los clientes
                        StringBuilder messageToSend = new StringBuilder(serverData);

                        foreach (var otherClient in clients)
                        {
                            Vector3 pos = otherClient.Value.position;
                            Vector3 rot = otherClient.Value.rotation;
                          

                            // Agregar la información de cada cliente
                            messageToSend.Append($"&Position:{pos.x}|{pos.y}|{pos.z}|" +
                                                 $"{rot.x}|{rot.y}|{rot.z}|{otherClient.Value.playerObject.GetComponent<Change_OtherPlayers>().PlayerProp_Id}|" +
                                                 $"{otherClient.Value.playerObject.GetComponent<Change_OtherPlayers>().Hunter}|{otherClient.Value.clientID}");
                        }

                        byte[] sendData = Encoding.ASCII.GetBytes(messageToSend.ToString());
                        socket.SendTo(sendData, clientEndPoint);

                        Debug.Log($"Datos enviados al cliente {clientEndPoint}: {messageToSend}");
                    }
                }
                catch (SocketException e)
                {
                    Debug.LogError("Error al enviar datos: " + e.Message);
                }
                while (clientUpdateQueue.Count > 0)
                {
                    ClientUpdate clientUpdate = clientUpdateQueue.Dequeue();
                    if (clientUpdate.isNewClient)
                    {
                        AddNewClient(clientUpdate.remote, clientUpdate.clientID);
                    }

                    // Actualizar los datos del cliente
                    if (clients.ContainsKey(clientUpdate.remote))
                    {
                        clients[clientUpdate.remote].position = clientUpdate.position;
                        clients[clientUpdate.remote].rotation = clientUpdate.rotation;

                        GameObject playerObject = clients[clientUpdate.remote].playerObject;
                        playerObject.transform.position = clientUpdate.position;
                        playerObject.transform.eulerAngles = clientUpdate.rotation;
                        playerObject.GetComponent<Change_OtherPlayers>().PlayerProp_Id = clientUpdate.playerPropId;

                        Debug.Log("Posición actualizada del jugador " + clientUpdate.remote.ToString() + ": " + clientUpdate.position);
                    }
                }

            }


            catch (InvalidOperationException ex)
            {
                Debug.LogError("Error al procesar la cola: " + ex.Message);


            }
        }
    }

    private void OnDestroy()
    {
        running = false;
        if (socket != null)
        {
            socket.Close();
        }
    }
}
