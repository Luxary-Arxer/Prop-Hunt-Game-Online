using UnityEngine;

public class PlayerToProp : MonoBehaviour
{
    [SerializeField] private Transform modelParent; // El contenedor de modelos
    [SerializeField] private GameObject currentModel; // El modelo actual del jugador
    [SerializeField] private float transformDistance = 5f; // Distancia máxima para transformarse
    [SerializeField] private LayerMask transformLayer; // Capa de los objetos transformables
    [SerializeField] public bool TeamHunter = false;
    public GameObject CaraterMesh;

    public GameObject GunMesh;
    [SerializeField] private LayerMask Enemy_Layer; // Capa de los enemigos
    [SerializeField] private float ShootDistance = 50f; // Distancia máxima para transformarse
    public float Damage = 1;
    public float DamageRange;
    //Cada player tiene 3 de vida
    public float Vida = 3;

    public GameObject transformTarget; // Referencia al objeto en el que te transformas.
    private Collider originalCollider;  // Colisionador original del jugador.


    public SkinnedMeshRenderer Player_Renderer;
    public Material Material_Hunter, Material_Alien;



    //Id del objeto
    public int PlayerProp_Id;
    public GameObject Mest_Player;
    public void ReadProp_Id()
    {
        IDProps NewProp = Mest_Player.GetComponentInChildren<IDProps>();
        if (Mest_Player.GetComponentInChildren<IDProps>() != null)
        {
            PlayerProp_Id = NewProp.Id_Prop;
        }

    }
    private void Start()
    {
        PlayerTeam();
    }


    void Update()
    {

        // Dibujar un rayo desde el centro de la cámara
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.T))
        {
            TeamHunter = !TeamHunter;
        }
        PlayerTeam();

        if (TeamHunter == true){
            CaraterMesh.layer = 6;
            CaraterMesh.SetActive(true);
            currentModel.SetActive(false);
            Shoot();

        }
        // Para saver que tipo de player es
        if (TeamHunter == false)
        {
            ReadProp_Id();
            CaraterMesh.layer = 7;
            // Verificar si el rayo impacta con un objeto en la capa transformable
            if (Physics.Raycast(ray, out hit, transformDistance, transformLayer))
            {
                Debug.Log("Apuntando a un objeto transformable: " + hit.collider.name);

                // Si presionamos 'F' nos transformamos en el objeto
                if (Input.GetKeyDown(KeyCode.F))
                {
                    TransformIntoObject(hit.collider.gameObject);
                }
            }
            // Si presionamos 'R' resetea el mash al inical
            if (Input.GetKeyDown(KeyCode.R))
            {
                CaraterMesh.SetActive(true);
                currentModel.SetActive(false);

                TransformPlayer();
            }
        }

        Consolas();

    }

    public void Shoot()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, ShootDistance, Enemy_Layer))
        {
            Debug.Log("Apuntando a un objeto Disparable: " + hit.collider.name);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (hit.collider.gameObject.TryGetComponent(out PlayerToProp enemy))
                {
                    enemy.Vida -= 1;
                }
                if (hit.collider.gameObject.TryGetComponent(out PlayerToProp dumy))
                {
                    dumy.Vida -= 1;
                }
            }
        }
    }

    private void TransformIntoObject(GameObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("El objeto objetivo es nulo. No se puede transformar.");
            return;
        }

        Debug.Log("Transformándose en: " + targetObject.name);

        // Destruir el modelo actual
        if (currentModel != null)
        {
            CaraterMesh.SetActive(false);
            Destroy(currentModel);
            Debug.Log("Modelo actual destruido.");
        }

        // Crear un nuevo modelo basado en el objetivo
        GameObject newModel = Instantiate(targetObject, modelParent);
        newModel.layer = 7;
        newModel.transform.localPosition = new Vector3(0f,0.6f,-0.5f);
        newModel.transform.localRotation = Quaternion.identity;

        // Actualizar la referencia del modelo actual
        currentModel = newModel;
        Debug.Log("Transformación completada en: " + targetObject.name);
    }

    void TransformPlayer()
    {
        if (transformTarget != null)
        {
            // Cambiar el colisionador del jugador al colisionador del objeto de transformación.
            CapsuleCollider playerCollider = GetComponent<CapsuleCollider>();
            Collider targetCollider = transformTarget.GetComponent<Collider>();

            if (targetCollider != null)
            {
                // Guardar el colisionador original.
                originalCollider = playerCollider;
                // Desactivar el colisionador actual.
                playerCollider.enabled = false;
                // Añadir el colisionador del objeto destino.
                transformTarget.GetComponent<Collider>().enabled = true;

                // Cambiar a la cámara o control del objeto destino si es necesario.
            }
        }
    }

    //Mira la variable y canvia que pude hacer el player
    void PlayerTeam()
    {
        if (TeamHunter == false)
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

    void Consolas()
    {
        //Funcionalidad de las difrenetes consolas
        if (Input.GetKeyDown(KeyCode.E))
        {
            float interact_Range = 2f;
            Collider[] collider_array = Physics.OverlapSphere(transform.position, interact_Range);
            foreach (Collider collider in collider_array)
            {
                if (collider.TryGetComponent(out ConsoletoHunter Console_Hunter))
                {
                    Console_Hunter.Interact();
                    TeamHunter = true;
                }
                if (collider.TryGetComponent(out ConsoletoAlien Console_Alien))
                {
                    Console_Alien.Interact();
                    TeamHunter = false;
                }
                if (collider.TryGetComponent(out ConsoletoStart Console_Start))
                {
                    Console_Start.Interact();
                    if (TeamHunter == true)
                    {
                        transform.position = new Vector3(0, -34, 6.5f);
                    }
                    else
                    {
                        transform.position = new Vector3(2, -37, 5);
                    }
                }
            }
        }     
    }

}
