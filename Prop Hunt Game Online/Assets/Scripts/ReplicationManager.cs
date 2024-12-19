using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplicationManager : MonoBehaviour
{
    //public GameObject puerta; testing
    private ServerUDP scriptServer;
    private ClientUDP scriptClient;
    bool door;
    void Awake()
    {
        DontDestroyOnLoad(gameObject); 
    }
    void Start()
    {
        //scriptServer = puerta.GetComponent<ServerUDP>();
        //scriptClient = puerta.GetComponent<ClientUDP>();
        door = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
