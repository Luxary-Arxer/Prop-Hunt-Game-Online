using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class JoinInformation : MonoBehaviour
{
    public static JoinInformation scene1;
    public TMP_InputField imputField_IP;
    public string serverIP;
    public TMP_InputField imputField_Name;
    public string serverName = "No Name";

    public void changeScene(string scene)
    {
        serverIP = imputField_IP.text;
        serverName = imputField_Name.text;
        SceneManager.LoadScene(scene);
    }
}
