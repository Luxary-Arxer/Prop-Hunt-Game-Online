using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    public UnityEvent OnGunshoot;
    public float FireColdown;

    private float CurentColdown;
    // Start is called before the first frame update
    void Start()
    {
        CurentColdown = FireColdown;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CurentColdown <= 0f)
            {
                OnGunshoot?.Invoke();
                CurentColdown = FireColdown;
            }
        }
        CurentColdown -= Time.deltaTime;
    }
}
