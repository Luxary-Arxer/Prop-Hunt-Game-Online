using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGun : MonoBehaviour
{
    /*
    [SerializeField] private LayerMask Enemy_Layer; // Capa de los enemigos
    [SerializeField] private float ShootDistance = 50f; // Distancia máxima para transformarse
    public float Damage = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, ShootDistance, Enemy_Layer))
        {
            Debug.Log("Apuntando a un objeto Disparable_2: " + hit.collider.name);

            Shoot();
        }
        
    }
    // Update is called once per frame
    public void Shoot()
    {



        if (hit.collider.gameObject.TryGetComponent(out PlayerToProp enemy))
        {
            enemy.Vida -= 1;
        }
        if (hit.collider.gameObject.TryGetComponent(out DumyScript dumy))
        {
            dumy.Health -= 1;
        }
    }

    */
}
