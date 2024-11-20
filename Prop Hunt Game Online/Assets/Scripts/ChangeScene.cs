using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ChangeScene : MonoBehaviour
{
    public static ChangeScene server_Home;
    public TMP_InputField imputField_Name;
    public string serverName = "No Name";

    private void Awake()
    {
        if (server_Home == null)
        {
            server_Home = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        serverName = "No Name";
    }
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
