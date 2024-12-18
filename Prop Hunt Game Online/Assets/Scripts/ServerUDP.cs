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

    // Diccionario para manejar múltiples clientes (clave: EndPoint, valor: datos del cliente)
    private Dictionary<EndPoint, ClientData> clients = new Dictionary<EndPoint, ClientData>();

   
    public GameObject clientPrefab;
    public GameObject serverPrefab;

    // Cola para gestionar la creación de clientes y actualizaciones en el hilo principal
    private Queue<ClientUpdate> clientUpdateQueue = new Queue<ClientUpdate>();

    // Clase para manejar la información de los clientes
    private class ClientData
    {
        public GameObject playerObject; // Representa al cliente en la escena
        public Vector3 position;
        public Vector3 rotation;
    }

    // Clase para manejar las actualizaciones de los clientes
    private class ClientUpdate
    {
        public EndPoint remote;
        public Vector3 position;
        public Vector3 rotation;
        public bool isNewClient;
    }

    void Start()
    {
        StartServer();
        //serverPrefab = gameObject;
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
                string message = Encoding.ASCII.GetString(data, 0, recv);

                Debug.Log($"Mensaje recibido de {remote}: {message}");

                if (message.Contains("Position:"))
                {
                    UpdateClientData(remote, message); // Actualizar posición/rotación del cliente
                }

                BroadcastData(); // Enviar datos de todos los clientes
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
        if (positionData.Length == 6)
        {
            float x = float.Parse(positionData[0]);
            float y = float.Parse(positionData[1]);
            float z = float.Parse(positionData[2]);
            float rotX = float.Parse(positionData[3]);
            float rotY = float.Parse(positionData[4]);
            float rotZ = float.Parse(positionData[5]);

            bool isNewClient = !clients.ContainsKey(remote);

            lock (clientUpdateQueue)
            {
                clientUpdateQueue.Enqueue(new ClientUpdate
                {
                    remote = remote,
                    position = new Vector3(x, y, z),
                    rotation = new Vector3(rotX, rotY, rotZ),
                    isNewClient = isNewClient
                });
            }
        }
    }

    void AddNewClient(EndPoint remote)
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
            rotation = Vector3.zero
        };

        Debug.Log($"Se creó un objeto para el cliente: {remote}");
    }

    void BroadcastData()
    {
        try
        {

            // Obtener la posición y rotación del GameObject del servidor


            // Construir el mensaje con los datos del servidor
            string messageToSend = $"Position:{serverPosition.x}|{serverPosition.y}|{serverPosition.z}|" +
                                   $"{serverRotation.x}|{serverRotation.y}|{serverRotation.z}";
            foreach (var client in clients)
            {
                EndPoint clientEndPoint = client.Key;
                //ClientData data = client.Value;

                // Formato compatible con el cliente
                //string messageToSend = $"Position:{data.position.x}|{data.position.y}|{data.position.z}|{data.rotation.x}|{data.rotation.y}|{data.rotation.z}";
                //string messageToSend = $"Position:{gameObject.transform.position.x}|{gameObject.transform.position.y}|{gameObject.transform.position.z}|{gameObject.transform.rotation.x}|{gameObject.transform.rotation.y}|{gameObject.transform.rotation.z}";

                byte[] sendData = Encoding.ASCII.GetBytes(messageToSend);
                socket.SendTo(sendData, clientEndPoint);

                Debug.Log($"Datos enviados al cliente {clientEndPoint}: {messageToSend}");
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Error al enviar datos: " + e.Message);
        }
    }

    void Update()
    {

         serverPosition = serverPrefab.transform.position;
         serverRotation = serverPrefab.transform.eulerAngles;
        // Procesar la cola de actualizaciones de clientes en el hilo principal
        lock (clientUpdateQueue)
        {
            while (clientUpdateQueue.Count > 0)
            {
                ClientUpdate clientUpdate = clientUpdateQueue.Dequeue();
                if (clientUpdate.isNewClient)
                {
                    AddNewClient(clientUpdate.remote);
                }

                // Actualizar los datos del cliente en el hilo principal
                if (clients.ContainsKey(clientUpdate.remote))
                {
                    clients[clientUpdate.remote].position = clientUpdate.position;
                    clients[clientUpdate.remote].rotation = clientUpdate.rotation;

                    GameObject playerObject = clients[clientUpdate.remote].playerObject;
                    playerObject.transform.position = clientUpdate.position;
                    playerObject.transform.eulerAngles = clientUpdate.rotation;

                    Debug.Log("Posición actualizada del jugador " + clientUpdate.remote.ToString() + ": " + clientUpdate.position);
                }
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