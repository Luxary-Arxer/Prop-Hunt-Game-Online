using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumyScript : MonoBehaviour
{
    [SerializeField] public float StaringHealth = 10;
    public float Health;
    public GameObject Dumy;
    private float delay = 10;
    // Start is called before the first frame update
    void Start()
    {
        Health = StaringHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Health == 0)
        {
            Dumy.SetActive(false);
            Counter();
        }
    }

    public void Counter()
    {
        if (delay > 0)
        {
            delay -= Time.fixedDeltaTime;

            if (delay <= 0)
            {
                Dumy.SetActive(true);
            }
        }
    }
}
