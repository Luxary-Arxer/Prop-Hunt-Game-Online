using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactwithconsole : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float interact_Range = 2f;
            Collider[] collider_array = Physics.OverlapSphere(transform.position, interact_Range);
            foreach (Collider collider in collider_array)
            {
                if (collider.TryGetComponent(out ConsoletoHunter Console_Hunter)){ 
                    Console_Hunter.Interact(); }
                Debug.Log(collider);
            }
        }
    }
}
