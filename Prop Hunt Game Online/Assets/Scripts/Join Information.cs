using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class JoinInformation : MonoBehaviour
{
    public static JoinInformation client_Home;
    public TMP_InputField imputField_IP;
    public string clientIP;
    public TMP_InputField imputField_Name;
    public string clientName = "No Name";

    private void Awake()
    {
        if (client_Home == null)
        {
            client_Home = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void read_IP(string IP)
    {
        clientIP = imputField_IP.text;
        Debug.Log(clientIP);
    }
    //Leer el nombre que te da el player
    public void read_Name(string Name)
    {
        clientName = imputField_Name.text;
        Debug.Log(clientName);
    }

    public void changeScene(string scene)
    {



        SceneManager.LoadScene(scene);
    }


}
