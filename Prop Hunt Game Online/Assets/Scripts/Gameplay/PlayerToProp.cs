using UnityEngine;

public class PlayerToProp : MonoBehaviour
{
    [SerializeField] private Transform modelParent; // El contenedor de modelos
    [SerializeField] private GameObject currentModel; // El modelo actual del jugador
    [SerializeField] private float transformDistance = 5f; // Distancia máxima para transformarse
    [SerializeField] private LayerMask transformLayer; // Capa de los objetos transformables
    public bool hunter = false;
    void Update()
    {

        // Dibujar un rayo desde el centro de la cámara
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        RaycastHit hit;

        // Verificar si el rayo impacta con un objeto en la capa transformable
        if (Physics.Raycast(ray, out hit, transformDistance, transformLayer))
        {
            Debug.Log("Apuntando a un objeto transformable: " + hit.collider.name);

            // Si presionamos 'E' nos transformamos en el objeto
            if (Input.GetKeyDown(KeyCode.E) && hunter == false)
            {
                TransformIntoObject(hit.collider.gameObject);
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
            Destroy(currentModel);
            Debug.Log("Modelo actual destruido.");
        }

        // Crear un nuevo modelo basado en el objetivo
        GameObject newModel = Instantiate(targetObject, modelParent);
        newModel.transform.localPosition = Vector3.zero;
        newModel.transform.localRotation = Quaternion.identity;

        // Actualizar la referencia del modelo actual
        currentModel = newModel;
        Debug.Log("Transformación completada en: " + targetObject.name);
    }
}
