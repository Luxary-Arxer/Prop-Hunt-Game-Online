using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class JoinInformation : MonoBehaviour
{
    public static JoinInformation scene_Home;
    public TMP_InputField imputField_IP;
    public string serverIP;
    public TMP_InputField imputField_Name;
    public string serverName;

    private void Awake()
    {
        if (scene_Home == null)
        {
            scene_Home = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void read_IP(string IP)
    {
        serverIP = imputField_IP.text;
        Debug.Log(serverIP);
    }
    //Leer el nombre que te da el player
    public void read_Name(string Name)
    {
        serverName = imputField_Name.text;
        Debug.Log(serverName);
    }

    public void changeScene(string scene)
    {



        SceneManager.LoadScene(scene);
    }


}
