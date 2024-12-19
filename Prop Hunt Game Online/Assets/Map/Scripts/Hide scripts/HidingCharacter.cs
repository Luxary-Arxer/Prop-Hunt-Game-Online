using UnityEngine;
using UnityEngine.Events;

namespace KeyMouse.MoHide
{

    public class HidingCharacter : MonoBehaviour
    {
        [Header("Current object")]
        public Transform currentObject;
        private Transform _hoveredObject;

        [Header("Main character")]
        [SerializeField] private GameObject Character;

        [Header("Camera")]
        [SerializeField] private CameraHandler cameraHandler;
        [SerializeField] private Transform CameraTransform;

        [Header("Charcter camera offset")]
        [SerializeField] private float CameraDistance = 2.4f;
        [SerializeField] private float CameraHeight = 0.75f;
        [SerializeField] private float CameraSideDistance = 0.75f;

        [Header("Transformation parameters")]
        [SerializeField] private KeyCode TransformationKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode TransformationIntoPlayerKey = KeyCode.Q;
        [SerializeField] private KeyCode freezeRotationKey = KeyCode.F;
        [Space]
        [Min(0), SerializeField] private float TransformationDistance = 20;
        public bool BlockTransformation { private get; set; }

        [Header("Effects")]
        [SerializeField] private GameObject TransformationEffect;
        [SerializeField] private Vector3 transformationOffset;
        [SerializeField] private float transformationDuration;
        [Space]
        [SerializeField] private Material selectionEffectMaterial;

        [Header("Events")]
        [SerializeField] private UnityEvent OnTransform;

        #region Initializing
        private void Start()
        {
            SetCameraParameters(currentObject.gameObject);
        }
        #endregion

        #region Performing
        private void Update()
        {
            TransformationRay();

            // Turning back into character
            if (Input.GetKeyDown(TransformationIntoPlayerKey))
                TurnBackIntoCharacter();
        }
        #endregion

        #region Ray
        private void TransformationRay()
        {
            if (BlockTransformation) return;

            // Raycast
            RaycastHit hit;
            if (Physics.Raycast(CameraTransform.position, CameraTransform.forward, out hit, TransformationDistance))
            {
                // Try to know if I can transform in this object
                HideObject hoveredHideObject = hit.transform.GetComponent<HideObject>();
                if (hoveredHideObject != null && hit.transform != currentObject)
                {
                    // Spawn hover effect
                    if (_hoveredObject)
                    {
                        if (_hoveredObject != hit.transform)
                        {
                            _hoveredObject.GetComponent<HideObject>().DestroySelectEffect();
                            hoveredHideObject.SpawnSelectEffect(selectionEffectMaterial);
                            _hoveredObject = hit.transform;
                        }
                    }
                    else
                    {
                        hoveredHideObject.SpawnSelectEffect(selectionEffectMaterial);
                        _hoveredObject = hit.transform;
                    }

                    if (Input.GetKeyDown(TransformationKey))
                    {
                        CloneObject(hit.transform.gameObject);
                    }
                }
                else
                {
                    ResetHoverObject(); // Destroys old hover effect
                }
            }
            else
            {
                ResetHoverObject();
            }
        }

        private void ResetHoverObject()
        {
            if (_hoveredObject)
            {
                _hoveredObject.GetComponent<HideObject>().DestroySelectEffect();
                _hoveredObject = null;
            }
        }
        #endregion

        #region Transformation
        private void CloneObject(GameObject targetObject)
        {
            Transform previousObject = currentObject;
            previousObject.gameObject.SetActive(false);

            // Save old object velocity
            Vector3 velocity = previousObject.GetComponent<Rigidbody>().velocity;

            // Destroy target object select effect
            targetObject.GetComponent<HideObject>().DestroySelectEffect();

            // Instantiate object prefab
            GameObject clonedObject = Instantiate(targetObject, previousObject.position + Vector3.up, previousObject.rotation);
            clonedObject.transform.SetParent(transform);
            HideObject hideObject = clonedObject.GetComponent<HideObject>();

            // Delete old target transform
            if (previousObject.gameObject != Character)
                Destroy(previousObject.gameObject);

            // Set camera parameters
            SetCameraParameters(clonedObject, hideObject);

            // Initialize hiding object
            hideObject.InitializeHideObject(CameraTransform, freezeRotationKey);

            // Set old object velocity
            clonedObject.GetComponent<Rigidbody>().velocity = velocity;

            // Set current object
            currentObject = clonedObject.transform;

            // Make player disapear
            Character.SetActive(false);

            // Instantiate effect
            InstantiateTransformationEffect(clonedObject.transform.position);

            //Call event
            OnTransform?.Invoke();
        }

        private void TurnBackIntoCharacter()
        {
            if (currentObject == Character.transform)
                return;

            Transform previousObject = currentObject;

            // Set player to apear
            Character.SetActive(true);
            Character.transform.position = previousObject.position;

            // Delete old target transform
            Destroy(previousObject.gameObject);

            // Set camera parameters
            SetCameraParameters(Character);

            // Set current object
            currentObject = Character.transform;

            // Instantiate effect
            InstantiateTransformationEffect(Character.transform.position);

            //Call event
            OnTransform?.Invoke();
        }
        #endregion

        #region Camera
        private void SetCameraParameters(GameObject targetObject, HideObject hideObject = null)
        {
            // Set camera target
            cameraHandler.Target = targetObject.transform;

            // Set camera offsets
            Vector3 camOffset = Vector3.zero;
            if (hideObject != null)
                camOffset = new Vector3(hideObject.CameraSideDistance, hideObject.CameraHeight, hideObject.CameraDistance); 
            else
                camOffset = new Vector3(CameraSideDistance, CameraHeight, CameraDistance); 

            cameraHandler.SetPositionAgainstPlayer(camOffset.x, camOffset.y, camOffset.z);
        }
        #endregion

        #region Transformation effect
        private void InstantiateTransformationEffect(Vector3 effectPosition)
        {
            Destroy(Instantiate(TransformationEffect, effectPosition + transformationOffset, Quaternion.identity), transformationDuration);
        }
        #endregion
    }

}