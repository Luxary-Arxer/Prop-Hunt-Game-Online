using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerName : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMesh obj_text;
    public InputField display;
    public static string NamePlayer = "Unnamed Player";


    void Start()
    {
        obj_text.text = PlayerPrefs.GetString(NamePlayer);
    }

    // Update is called once per frame
    void Update()
    {
        obj_text.text = NamePlayer;
        PlayerPrefs.SetString("PlayerUsername", obj_text.text);
        PlayerPrefs.Save();
    }
}
