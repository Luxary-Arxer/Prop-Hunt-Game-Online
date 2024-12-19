using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursorehide : MonoBehaviour
{
    bool cursorActive;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (cursorActive == false)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                cursorActive = true;
            }
            else if (cursorActive == true)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                cursorActive = false;
            }
        }
    }
}
