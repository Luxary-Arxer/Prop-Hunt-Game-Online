using UnityEngine;

public class ObjectTransformation : MonoBehaviour
{
    public LayerMask transformLayer; // Capa para los objetos transformables
    public float maxDistance = 5f; // Distancia máxima para transformar
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main; // Obtiene la cámara principal
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryTransform();
        }
    }

    void TryTransform()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // Ray desde el centro de la pantalla
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, transformLayer))
        {
            GameObject targetObject = hit.collider.gameObject;

            // Verifica que tenga un MeshFilter y un MeshRenderer
            MeshFilter targetMeshFilter = targetObject.GetComponent<MeshFilter>();
            MeshRenderer targetMeshRenderer = targetObject.GetComponent<MeshRenderer>();

            if (targetMeshFilter != null && targetMeshRenderer != null)
            {
                MeshFilter playerMeshFilter = GetComponent<MeshFilter>();
                MeshRenderer playerMeshRenderer = GetComponent<MeshRenderer>();

                // Cambia el Mesh y el Material del jugador
                playerMeshFilter.mesh = targetMeshFilter.mesh;
                playerMeshRenderer.materials = targetMeshRenderer.materials;

                Debug.Log("Transformed into: " + targetObject.name);
            }
        }
    }
}
