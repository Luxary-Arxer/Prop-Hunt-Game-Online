using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_OtherPlayers : MonoBehaviour
{
    [SerializeField] private Transform modelParent; // El contenedor de modeloss
    [SerializeField] private GameObject currentModel; // El modelo actual del jugador
    [SerializeField] public bool Hunter = false;
    public GameObject CaraterMesh;
    public int PlayerProp_Id;
    public GameObject GunMesh;
    public SkinnedMeshRenderer Player_Renderer;
    public Material Material_Hunter, Material_Alien;
    // Start is called before the first frame update

    public List<GameObject> Props = new List<GameObject>();
    void Start()
    {
        PlayerTeam();
    }

    // Update is called once per frame
    void Update()
    {

        PlayerTeam();

        if (Hunter == true)
        {
            CaraterMesh.layer = 6;
            CaraterMesh.SetActive(true);
            currentModel.SetActive(false);
            //Shoot();

        }
        if (Hunter == false)
        {
            CaraterMesh.layer = 7;

            switch (PlayerProp_Id)
            {
                case -2:
                    CaraterMesh.SetActive(true);
                    currentModel.SetActive(false);
                    break;
                case -1:
                    CaraterMesh.SetActive(true);
                    currentModel.SetActive(false);
                    break;
                case 0:
                    Tranform(Props[1]);
                    break;

                case 1:
                    Tranform(Props[2]);
                    break;
                case 2:
                    Tranform(Props[3]);
                    break;
                case 3:
                    Tranform(Props[4]);
                    break;
                case 4:
                    Tranform(Props[5]);
                    break;
                case 5:
                    Tranform(Props[6]);
                    break;
                case 6:
                    Tranform(Props[7]);
                    break;
                case 7:
                    Tranform(Props[8]);
                    break;
                case 8:
                    Tranform(Props[9]);
                    break;
                case 9:
                    Tranform(Props[10]);
                    break;


            }
        }
    }

    void Tranform(GameObject NewProp)
    {
        CaraterMesh.SetActive(false);
        if (currentModel != null)
        {
            Destroy(currentModel); // Destruir el modelo anterior
        }
        // Crear un nuevo modelo basado en el objetivo
        GameObject newModel = Instantiate(NewProp, modelParent);
        newModel.layer = 7;
        newModel.transform.localPosition = new Vector3(0f, 0.6f, -0.5f);
        newModel.transform.localRotation = Quaternion.identity;
        // Actualizar la referencia del modelo actual
        currentModel = newModel;
    }
    void PlayerTeam()
    {
        if (Hunter == false)
        {
            Material[] materials = new Material[Player_Renderer.sharedMaterials.Length];
            materials[0] = Material_Alien;
            Player_Renderer.sharedMaterials = materials;
            GunMesh.SetActive(false);
        }
        else
        {
            Material[] materials = new Material[Player_Renderer.sharedMaterials.Length];
            materials[0] = Material_Hunter;
            Player_Renderer.sharedMaterials = materials;
            GunMesh.SetActive(true);
        }
    }
}
