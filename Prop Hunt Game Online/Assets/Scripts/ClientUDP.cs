using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using System;

public class ClientUDP : MonoBehaviour
{
    private Socket socket;
    public string IPServer; 
    private Vector3 playerPosition; 
    private Vector3 playerRotation; 
    private string message; // Mensaje a enviar
    private bool running = true; // Bandera para manejar el bucle de envío
    public GameObject serverObject;
    private Queue<Vector3> positionQueue = new Queue<Vector3>();
    private Vector3 newPosition_server;
    private Vector3 newRotation_server;
    private Vector3 lastPosition;
    private Vector3 lastRotation;

    public GameObject player;
    private PlayerToProp mesh;
    public bool TeamHunter = false;
    public int PlayerProp_Id = -1;

    public GameObject changeOtherPlayer;
    private Change_OtherPlayers newmesh;

    List<ClientData> otherClients = new List<ClientData>();
    private List<GameObject> clientGameObjects = new List<GameObject>();
    public GameObject clients;
    private int myClientID = -1;

    private class ClientData
    {
        public int clientID;
        public Vector3 position;
        public Vector3 rotation;
        public int playerPropId;
        public bool hunter;

    }
    void Start()
    {
        newmesh = changeOtherPlayer.GetComponent<Change_OtherPlayers>();
        mesh = player.GetComponent<PlayerToProp>();
        // Inicializar posición y mensaje
        playerPosition = transform.position;
        message = "Position: " + playerPosition.x + "|" + playerPosition.y + "|" + playerPosition.z;
        lastPosition = serverObject.transform.position;
        lastRotation = serverObject.transform.eulerAngles;

        // Obtener IP del servidor desde JoinInformation si está configurada
        if (JoinInformation.client_Home != null && !string.IsNullOrEmpty(JoinInformation.client_Home.clientIP))
        {
            IPServer = JoinInformation.client_Home.clientIP;
        }
        else
        {
            IPServer = "127.0.0.1";
        }

        // Iniciar el cliente
        InitializeSocket(); // Inicializamos el socket antes de usarlo
        StartClient(); // Iniciar el cliente para recibir y enviar datos
    }

    void Update()
    {
        if (positionQueue.Count > 2)
        {
            newPosition_server = positionQueue.Dequeue();
            newRotation_server = positionQueue.Dequeue();

            if (!serverObject.activeSelf && serverObject.transform.position != newPosition_server)
            {
                serverObject.SetActive(true);
            }

            // Interpolación de la posición y rotación para suavizar el movimiento
            serverObject.transform.position = Vector3.Lerp(lastPosition, newPosition_server, 0.5f);
            serverObject.transform.eulerAngles = Vector3.Lerp(lastRotation, newRotation_server, 0.5f);

            lastPosition = serverObject.transform.position;
            lastRotation = serverObject.transform.eulerAngles;

            //serverObject.transform.position = newPosition_server;
           // serverObject.transform.eulerAngles = newRotation_server;


        }

        // Actualizar la posición del jugador y el mensaje
        playerPosition = transform.position;
        playerRotation = transform.eulerAngles;
        message = "Position: " + playerPosition.x + "|" + playerPosition.y + "|" + playerPosition.z
            + "|" + playerRotation.x + "|" + playerRotation.y + "|" + playerRotation.z + "|" + mesh.PlayerProp_Id + "|" + mesh.TeamHunter;
        //Debug.Log("Posición X " + newPosition_server.x + " Posición Z " + newPosition_server.z);

        CreateOtherPlayers();
    }

    void InitializeSocket()
    {
        try
        {
            // Configuración del endpoint del servidor
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPServer), 9050);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect(ipep); // Asegurarse de que el socket se conecta al servidor
            Debug.Log("Socket inicializado correctamente.");
        }
        catch (SocketException e)
        {
            Debug.LogError("Error al inicializar el socket: " + e.Message);
        }
    }

    void StartClient()
    {
        // Crear y empezar un nuevo thread para enviar datos
        Thread sendThread = new Thread(Send);
        sendThread.IsBackground = true; // Permitir que el thread termine automáticamente al cerrar la aplicación
        sendThread.Start();

        // Crear un nuevo thread para recibir datos
        Thread receiveThread = new Thread(Receive);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void Send()
    {
        try
        {
            // Verifica que el socket está inicializado
            if (socket == null)
            {
                Debug.LogError("El socket no está inicializado correctamente.");
                return;
            }

            // Configuración del endpoint del servidor (ya se inicializa en InitializeSocket)
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPServer), 9050);

            while (running)
            {
                // Serializar y enviar datos al servidor
                byte[] data = Encoding.ASCII.GetBytes(message);
                socket.SendTo(data, ipep);
                Debug.Log("Datos enviados al servidor: " + message);

                
                Thread.Sleep(16); // 60FPS
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Error en el cliente: " + e.Message);
        }
    }

    void Receive()
    {
        if (socket == null)
        {
            Debug.LogError("El socket no está inicializado correctamente.");
            return;
        }

        byte[] data = new byte[1024];
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)sender;
        Debug.Log("Esperando mensajes...");

        try
        {
            while (running)
            {
                // Recibir mensaje sin bloquear innecesariamente el hilo
                int recv = socket.ReceiveFrom(data, ref remote);
               
                if (recv > 0) // Solo procesar si se recibieron datos
                {
                    // Convertir solo cuando sea necesario
                    string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                    Debug.Log($"Mensaje recibido de {remote}: {receivedMessage}");
                    // Solo procesar mensajes relevantes
                    if (receivedMessage.Contains("Position:"))
                    {
                        if (myClientID ==-1)
                        {
                            SetMyClientIDFromLastMessage(receivedMessage);
                        }
                        ProcessPositions(receivedMessage);
                        string correctedMessage = GetCorrectedMessage(receivedMessage);
                        Debug.Log($"Mensaje corregido " + correctedMessage);
                        UpdatePositionQueue(correctedMessage);
                        UpdateRotation(correctedMessage);
                        UpdateProp(correctedMessage);
                    }
                }
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Error al recibir datos: " + e.Message);
        }
    }

    void ProcessPositions(string receivedMessage)
    {
        // Separar el mensaje por cada 'Position:'
        string[] positionSections = receivedMessage.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

        Debug.Log($"positionSections " + positionSections);


        // Contador para encontrar la segunda posición
        int positionCounter = 0;

        foreach (var section in positionSections)
        {
            // Comprobamos si la sección contiene "Position:" al inicio
            if (section.Contains("Position:"))
            {
                // Aumentamos el contador cada vez que encontramos "Position:"
                positionCounter++;

                // Solo procesamos desde la segunda "Position:"
                if (positionCounter > 1)
                {
                    // Extraer los datos de la posición
                    string[] positionData = section.Split(":")[1].Trim().Split("|");
                    if (positionData.Length >= 8)
                    {
                        int playerId = int.Parse(positionData[8]);  // Usamos el último campo como ID
                        float x = float.Parse(positionData[0]);
                        float y = float.Parse(positionData[1]);
                        float z = float.Parse(positionData[2]);
                        Vector3 position = new Vector3(x, y, z);

                        float x_rotation = float.Parse(positionData[3]);
                        float y_rotation = float.Parse(positionData[4]);
                        float z_rotation = float.Parse(positionData[5]);
                        Vector3 rotation = new Vector3(x_rotation, y_rotation, z_rotation);

                        int propid = int.Parse(positionData[6]);
                        Debug.Log($"propid " + int.Parse(positionData[6]));

                        bool hunt = bool.Parse(positionData[7]);
                        // Buscar si el cliente ya existe en la lista
                        ClientData existingClient = otherClients.Find(client => client.clientID == playerId);
                        if (existingClient == null)
                        {
                            // Si no existe, añadirlo
                            ClientData newClientData = new ClientData
                            {
                                clientID = playerId,
                                position = position,
                                rotation = rotation,
                                playerPropId = propid,
                                hunter = hunt
                            };
                            otherClients.Add(newClientData);
                            Debug.Log($"Nuevo cliente agregado: {playerId}");
                        }
                        else
                        {
                            // Si ya existe, actualizar su posición y rotación
                            existingClient.position = position;
                            existingClient.rotation = rotation;
                            existingClient.playerPropId = propid;
                            existingClient.hunter = hunt;

                            Debug.Log($"Cliente actualizado: {playerId} a posición: {position} y rotación: {rotation}");
                        }
                    }
                }
            }
        }

    }

    private string GetCorrectedMessage(string message)
    {
        // Encuentra la parte que sigue después de "Position:"
        int positionIndex = message.IndexOf("Position:") + "Position:".Length;
        string positionDataString = message.Substring(positionIndex);

        // Si hay un "&", cortamos la cadena hasta ese punto para evitar procesar lo que sigue
        int endIndex = positionDataString.IndexOf("&");
        if (endIndex >= 0)
        {
            positionDataString = positionDataString.Substring(0, endIndex);
        }

        return "Position:" + positionDataString;
    }

    void UpdatePositionQueue(string message)
    {
        string[] positionData = message.Split(':')[1].Trim().Split("|");
        if (positionData.Length == 8)
        {
            float x = float.Parse(positionData[0]);
            float y = float.Parse(positionData[1]);
            float z = float.Parse(positionData[2]);
            Vector3 newPosition = new Vector3(x, y, z);

            // Agregar la nueva posición a la cola
            positionQueue.Enqueue(newPosition);
        }
    }

    void UpdateRotation(string message)
    {
        string[] positionData = message.Split(':')[1].Trim().Split("|");
        if (positionData.Length == 8)
        {
            float x_rotation = float.Parse(positionData[3]);
            float y_rotation = float.Parse(positionData[4]);
            float z_rotation = float.Parse(positionData[5]);
            Vector3 newRotation = new Vector3(x_rotation, y_rotation, z_rotation);

            // Agregar la nueva rotación a la cola
            positionQueue.Enqueue(newRotation);
        }
    }

    void UpdateProp(string message)
    {
        string[] positionData = message.Split(':')[1].Trim().Split("|");
        if (positionData.Length == 8)
        {
            if (int.Parse(positionData[6]) != 0)
            {
                newmesh.PlayerProp_Id = int.Parse(positionData[6]);
                
            }
            newmesh.Hunter = bool.Parse(positionData[7]);
            Debug.Log("Prophunter" + newmesh.PlayerProp_Id + "bool:" + newmesh.Hunter);

        }
    }

    void CreateOtherPlayers()
    {

        // Clonamos la lista para evitar modificarla durante la iteración
        List<ClientData> clientDataCopy = new List<ClientData>(otherClients);

        // Recorremos la lista de otros clientes
        foreach (ClientData clientData in clientDataCopy)
        {
            // Omitimos al jugador con el ID igual a myClientID
            if (clientData.clientID == myClientID)
            {
                continue;
            }

            // Comprobamos si ya existe un GameObject para este cliente
            GameObject existingClient = clientGameObjects.Find(go => go.name == "Client_" + clientData.clientID);

            if (existingClient == null)
            {
                // Si no existe, creamos una nueva instancia del prefab de cliente
                GameObject clientObj = Instantiate(clients);
                clientObj.name = "Client_" + clientData.clientID;  // Asignamos un nombre único al GameObject
                clientObj.transform.position = clientData.position;  // Asignamos la posición
                clientObj.transform.eulerAngles = clientData.rotation;  // Asignamos la rotación

                // Asignamos el PlayerProp_Id y Hunter al componente correspondiente en el prefab
                Change_OtherPlayers clientScript = clientObj.GetComponent<Change_OtherPlayers>();
                if (clientScript != null)
                {
                    clientScript.PlayerProp_Id = clientData.playerPropId;
                    clientScript.Hunter = TeamHunter; 
                }

                // Agregamos el nuevo GameObject a la lista
                clientGameObjects.Add(clientObj);
            }
            else
            {
                // Si ya existe, solo actualizamos la posición y la rotación
                existingClient.transform.position = clientData.position;
                existingClient.transform.eulerAngles = clientData.rotation;
                Change_OtherPlayers clientScript = existingClient.GetComponent<Change_OtherPlayers>();
                if (clientScript != null)
                {
                    Debug.Log("PlayerProp_Id" + clientData.playerPropId);
                    clientScript.PlayerProp_Id = clientData.playerPropId;
                    clientScript.Hunter = clientData.hunter;
                }
            }
        }
    }

    void SetMyClientIDFromLastMessage(string receivedMessage)
    {
        // Separar el mensaje por cada 'Position:'
        string[] positionSections = receivedMessage.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

        // Asegurarse de que haya al menos una sección
        if (positionSections.Length > 0)
        {
            // Tomamos la última sección del mensaje recibido
            string lastSection = positionSections[positionSections.Length - 1];

            // Verificamos si esta sección contiene "Position:"
            if (lastSection.Contains("Position:"))
            {
                // Extraer los datos de la posición
                string[] positionData = lastSection.Split(":")[1].Trim().Split("|");

                // Verificamos si la sección tiene al menos 8 valores
                if (positionData.Length >= 8)
                {
                    // Asignamos el último valor como el `myClientID`
                    myClientID = int.Parse(positionData[8]);  // Usamos el último campo como ID
                    Debug.Log($"Último ID recibido y asignado: {myClientID}");
                }
            }
        }
    }

    private void OnDestroy()
    {
        // Detener el bucle y cerrar el socket al destruir el objeto
        running = false;
        if (socket != null)
        {
            socket.Close();
        }
    }
}
