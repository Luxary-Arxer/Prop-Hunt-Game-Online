using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerName : MonoBehaviour
{
    public TextMesh obj_text;
    public InputField display;
    public static string NamePlayer_1 = "No Name";

    void Start()
    {
        obj_text.text = PlayerPrefs.GetString(NamePlayer_1);
    }

    void Update()
    {
        obj_text.text = NamePlayer_1;
        PlayerPrefs.SetString("PlayerUsername", obj_text.text);
        PlayerPrefs.Save();
    }
}
