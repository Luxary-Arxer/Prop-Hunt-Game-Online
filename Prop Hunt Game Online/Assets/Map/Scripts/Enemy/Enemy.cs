using UnityEngine;
using UnityEngine.AI;
using KeyMouse.MoHide;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;

    [Header("Enemy target")]
    private Transform _target;

    [Header("Player camera")]
    public CameraHandler CameraHandler;

    [Header("Enemy gun")]
    [SerializeField] private Weapon rightHandWeapon;
    [SerializeField] private Weapon leftHandWeapon;
    [SerializeField] private float Damage = 34;
    [SerializeField] private AudioSource AudioSource;

    [Header("Enemy properties")]
    [SerializeField, Tooltip("Distance of view of the enemy")] private float ViewDistance = 50; 
    [SerializeField, Tooltip("Distance at which the enemy will see the target anyway")] private float MinDistance = 3f;
    [SerializeField, Tooltip("Angle of view of the enemy")] private float ViewAngle = 90; 
    bool seeTarget;//Does enemy can see target
    
    void Start()
    {
        _animator = transform.GetComponent<Animator>();
        _agent = transform.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        _target = CameraHandler.Target;
        _animator.SetFloat("Move amount", _agent.velocity.magnitude);
        //Find enemy target
        if (_target != null && _target.gameObject.activeSelf)//Check if target exist and not dead
        {
            CheckIfSeeTarget();
        }
        else
        {
            seeTarget = false;
        }
        //If he cannot find the target, then he goes to patrol.
        if (!seeTarget)
        {
            Transform walkPoint = FindObjectOfType<WalkPoint>().transform;
            _agent.destination = walkPoint.position;
        }
    }

    void CheckIfSeeTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation(_target.transform.position - transform.position);

        //Check if distance beween target and enemy is not too high
        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance > ViewDistance)
        {
            seeTarget = false;
            return;
        }

        //Check if angle beween target and enemy is not too high
        float angle = Quaternion.Angle(transform.rotation, lookRotation);//Angle between enemy and target
        if (angle < ViewAngle || distance < MinDistance)
        {
            if (IsTargetProp())
            {
                if (!IsPropMoving())
                {
                    seeTarget = false;
                    return;
                }
            }

            //Set enemy destination
            _agent.destination = _target.position;

            //Check enemy is running or standing
            if (_agent.velocity.magnitude == 0)
            {
                //Set animation to start shooting
                _animator.SetBool("Aim", true);
                _animator.SetTrigger("Shoot");
                //Set enemy rotation
                lookRotation.x = 0;
                lookRotation.z = 0;
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * (_agent.angularSpeed / 2));
            }
            else
            {
                //Set animation to stop shooting
                _animator.SetBool("Aim", false);
            }

            seeTarget = true;
        }
        else
        {
            seeTarget = false;
        }
    }

    public void Shoot()
    {
        if (_target == null)
            return;

        //Play shotgun muzzle flash
        rightHandWeapon.PlayMuzzleFlash();
        leftHandWeapon.PlayMuzzleFlash();

        //Play shotgun audio
        AudioSource.pitch = Time.timeScale;//That's was made for addapting to time scale
        AudioSource.Play();

        //If target have component "HidingCharacter" then enemy takes lives
        if(_target.TryGetComponent(typeof(Player), out Component component))
        {
            component.GetComponent<Player>().Health -= Damage;
        }
        else//If player play as prop
        {
            if (_target.parent.GetChild(0).TryGetComponent(typeof(Player), out Component childComponent))
            {
                childComponent.GetComponent<Player>().Health -= Damage;
            }
        }

    }

    private bool IsTargetProp() => _target.GetComponent<HideObject>();

    private bool IsPropMoving()
    {
        return _target.GetComponent<HideObject>() && _target.GetComponent<Rigidbody>().velocity.magnitude > 0.5f;
    }

}
