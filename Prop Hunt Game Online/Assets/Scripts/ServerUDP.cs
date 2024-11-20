using System; // Asegúrate de incluir este espacio de nombres
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;

public class ServerUDP : MonoBehaviour
{
    Socket socket;
    public GameObject cube; // El cubo que se moverá
    private Vector3 playerPosition;

    private void Start()
    {
        startServer();
    }

    public void startServer()
    {
        Debug.Log("Starting UDP Server...");
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep); // Vincula el socket al puerto 9050
        Debug.Log("\nServer started on port 9050");
        Thread newConnection = new Thread(Receive);
        newConnection.Start();
    }

    void Receive()
    {
        byte[] data = new byte[1024];
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)sender;

        while (true)
        {
            try
            {
                int recv = socket.ReceiveFrom(data, ref remote);
                string message = Encoding.ASCII.GetString(data, 0, recv);

                // Suponiendo que la posición se manda como "x,y,z"
                string[] positionData = message.Split(',');
                if (positionData.Length == 3)
                {
                    float x = float.Parse(positionData[0]);
                    float y = float.Parse(positionData[1]);
                    float z = float.Parse(positionData[2]);

                    playerPosition = new Vector3(x, y, z);
                    UpdateCubePosition(); // Actualizar la posición del cubo en el servidor
                }
            }
            catch (SocketException e)
            {
                Debug.LogError("Error receiving data: " + e.Message);
            }
            catch (Exception e)
            {
                Debug.LogError("Error: " + e.Message); // Asegúrate de tener una segunda capa de catch
            }
        }
    }

    void UpdateCubePosition()
    {
        if (cube != null)
        {
            cube.transform.position = playerPosition;
        }
    }
}
