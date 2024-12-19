using UnityEngine;
using KeyMouse.MoHide;

public class Player : MonoBehaviour
{
    [SerializeField] private new Transform camera;

    [Header("Player components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;

    [Header("Parent")]
    [SerializeField] private HidingCharacter hidingCharacter;

    [Header("Character stats")]
    public float Health = 100;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float jumpForce = 10;
    private bool _onGround;

    [Header("Death")]
    [SerializeField] private GameObject DeathEffect;
    [SerializeField] private float DeathTimeScaler = 0.2f;

    [Header("Audio")]
    [SerializeField] private AudioSource PlayerAudioSource;
    [SerializeField] private AudioClip DeathClip;

    private const string MOVE_AMOUNT_ANIMATION_VARIABLE = "Move amount";
    private const string JUMP_ANIMATION_VARIABLE = "Jump";

    #region Performing

    private void FixedUpdate()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(vertical) + Mathf.Abs(horizontal));
        Vector3 forwardLook = new Vector3(camera.forward.x, 0, camera.forward.z);
        Vector3 moveDirection = forwardLook * vertical + camera.right * horizontal;

        Movement(moveDirection);

        //Animation
        animator.SetFloat(MOVE_AMOUNT_ANIMATION_VARIABLE, moveAmount);

        //Rotation
        moveDirection += camera.right * horizontal;
        RotationNormal(moveDirection);
    }

    private void Update()
    {
        Jump();
    }

    #endregion

    #region Movement

    private void Movement(Vector3 moveDirection)
    {
        Vector3 velocityDir = moveDirection * moveSpeed;

        velocityDir.y = rb.velocity.y;
        rb.velocity = velocityDir;
    }

    private void RotationNormal(Vector3 rotationDirection)
    {
        Vector3 targetDir = rotationDirection;
        targetDir.y = 0;
        if (targetDir == Vector3.zero)
            targetDir = transform.forward;
        Quaternion loolDir = Quaternion.LookRotation(targetDir);
        Quaternion targetRot = Quaternion.Slerp(transform.rotation, loolDir, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRot;
    }

    private void Jump()
    {
        if (!_onGround) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    #endregion

    #region Checks

    public void CheckIfDead()
    {
        if (Health <= 0)
        {
            // Disable player
            hidingCharacter.currentObject.gameObject.SetActive(false);

            // Disable transformation
            hidingCharacter.BlockTransformation = true;

            // Play audio clip
            PlayerAudioSource.clip = DeathClip;
            PlayerAudioSource.Play();

            // Instantiate particle death effect
            Destroy(Instantiate(DeathEffect, hidingCharacter.currentObject.transform.position, Quaternion.Euler(-90, 0, 0)), 2);

            // Set time to sloaw down
            Time.timeScale = DeathTimeScaler;

            // Disable this script
            this.enabled = false;
        }
    }

    #endregion

    #region On ground

    private void OnCollisionStay(Collision collision)
    {
        SetJumpState(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        SetJumpState(false);
    }

    private void SetJumpState(bool onGround)
    {
        _onGround = onGround;
        animator.SetBool(JUMP_ANIMATION_VARIABLE, !onGround);
    }

    #endregion

}