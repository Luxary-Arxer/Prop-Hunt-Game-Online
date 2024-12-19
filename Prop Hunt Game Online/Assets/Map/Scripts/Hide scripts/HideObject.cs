using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KeyMouse.MoHide
{
    [RequireComponent(typeof(Rigidbody))]
    public class HideObject : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        [Header("Camera")]
        [HideInInspector] public Transform CameraTransform;

        [Header("Camera offset")]
        public float CameraDistance = 3;
        public float CameraHeight = 1.5f;
        public float CameraSideDistance = 0.75f;

        [Header("Characteristics")]
        [SerializeField] private float speed = 1500;
        [SerializeField] private float rotationSpeed = 50;
        [SerializeField] private float rotationDamping = 0.1f;
        [SerializeField] private float jumpForce = 500;
        private KeyCode _freezeRotationKey = KeyCode.F;

        [Header("Effects")]
        private GameObject _selectEffect;

        [Header("Events")]
        [SerializeField] private UnityEvent OnTransform;

        [Header("Private data")]
        private bool _isActivated;
        private bool _onGround;

        #region Initializing

        private void Start() => _rigidbody = GetComponent<Rigidbody>();

        public void InitializeHideObject(Transform cameraTransform, KeyCode freezeRotationKey)
        {
            OnTransform?.Invoke();
            Activate();

            CameraTransform = cameraTransform;
            _freezeRotationKey = freezeRotationKey;

            DestroySelectEffect();
        }

        private void Activate()
        {
            _isActivated = true;
        }

        #endregion

        #region Performing
        private void FixedUpdate()
        {
            if (!_isActivated) return;

            // Movement
            float verticalAxis = Input.GetAxis("Vertical");
            float horizontalAxis = Input.GetAxis("Horizontal");
            Vector3 forwardLook = new Vector3(CameraTransform.forward.x, 0, CameraTransform.forward.z);
            Vector3 moveDirection = forwardLook * verticalAxis + CameraTransform.right * horizontalAxis;

            Movement(moveDirection, verticalAxis, horizontalAxis);

            // Stabilize object
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Stabilize();
            }
        }

        private void Update()
        {
            if (!_isActivated) return;

            // Check if the object can jump
            if (_onGround)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    Jump();
            }

            // Freeze rotation
            if (Input.GetKeyDown(_freezeRotationKey))
                FreezeRotation();
        }
        #endregion

        #region Movement
        private void Movement(Vector3 moveDir, float vertical, float horizontal)
        {
            //Movement
            _rigidbody.AddForce(moveDir * Time.fixedDeltaTime * speed * 100);

            //Rotation    
            _rigidbody.AddTorque(-CameraTransform.forward * horizontal * rotationSpeed);
            _rigidbody.AddTorque(CameraTransform.right * vertical * rotationSpeed);
        }


        private void Stabilize()
        {
            Quaternion targetRotation = CameraTransform.rotation;

            float kp = Mathf.Pow(6f * rotationSpeed, 2) * 0.25f;
            float kd = 4.5f * rotationSpeed * rotationDamping;
            float g = 1 / (1 + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
            float ksg = kp * g;
            float kdg = (kd + kp * Time.fixedDeltaTime) * g;

            Quaternion q = targetRotation * Quaternion.Inverse(transform.rotation);
            if (q.w < 0)
            {
                q.x *= -1;
                q.y *= -1;
                q.z *= -1;
                q.w *= -1;
            }

            q.ToAngleAxis(out float angle, out Vector3 axis);
            axis.Normalize();
            axis *= Mathf.Deg2Rad;

            Vector3 torque = ksg * axis * angle + -_rigidbody.angularVelocity * kdg;

            _rigidbody.AddTorque(torque, ForceMode.Acceleration);
        }

        private void Jump() => _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        private void FreezeRotation() => _rigidbody.freezeRotation = !_rigidbody.freezeRotation;

        #endregion

        #region Selection effect
        public void SpawnSelectEffect(Material selectionMaterial)
        {
            if (selectionMaterial == null) return;

            MeshFilter[] meshFilters = FindObjectsMeshes();
            
            if (_selectEffect == null && meshFilters.Length > 0)
            {                
                GameObject selectionEffectObject = CreatPropMeshRenderer(meshFilters, selectionMaterial);
                selectionEffectObject.transform.SetParent(transform);
                _selectEffect = selectionEffectObject;
            }
        }

        public void DestroySelectEffect()
        {
            if (_selectEffect != null)
                DestroyImmediate(_selectEffect);
        }

        private GameObject CreatPropMeshRenderer(MeshFilter[] propMeshes, Material selectionMaterial)
        {
            GameObject selectionContainerObject = new GameObject("Selection effect");
            selectionContainerObject.transform.position = transform.position;
            selectionContainerObject.transform.rotation = transform.rotation;
            selectionContainerObject.transform.localScale *= 1.01f;

            for (int i = 0; i < propMeshes.Length; i++)
            {
                GameObject selectionEffectObject = new GameObject($"Selection mesh {i}");
                selectionEffectObject.transform.position = propMeshes[i].transform.position;
                selectionEffectObject.transform.rotation = propMeshes[i].transform.rotation;

                MeshFilter selectionEffectMeshFilter = selectionEffectObject.AddComponent<MeshFilter>();
                selectionEffectMeshFilter.mesh = propMeshes[i].mesh;

                MeshRenderer selectionEffectRenderer = selectionEffectObject.AddComponent<MeshRenderer>();
                selectionEffectRenderer.material = selectionMaterial;

                selectionEffectObject.transform.SetParent(selectionContainerObject.transform);
            }

            return selectionContainerObject;
        }

        private MeshFilter[] FindObjectsMeshes()
        {
            MeshFilter[] meshfilters;

            meshfilters = transform.GetComponentsInChildren<MeshFilter>();

            return meshfilters;
        }

        #endregion

        #region Collision detection
        private void OnCollisionStay(Collision collision) => _onGround = true;

        private void OnCollisionExit(Collision collision) => _onGround = false;
        #endregion
    }

}