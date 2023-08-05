using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAIController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 14f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float detectionRange = 45f;
    [SerializeField] private float pushForce = 20f;
    [SerializeField] private bool isPushed;

    EnemyFeatures characterFeatures;
    public Transform currentTarget;
    public Animator animator;
    private Rigidbody rb;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.angularSpeed = rotationSpeed;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Freeze rigidnody rotation.
        rb.AddForce(-Vector3.up * 20, ForceMode.Impulse);
        characterFeatures = GetComponent<EnemyFeatures>();
        characterFeatures.movement = transform.forward * moveSpeed;
    }


    private void Update()
    {
        if (!GameManager.Instance.isGameActive) return;

        if (characterFeatures.isDead)
        {
            agent.enabled = false;
            return;
        }
        MoveToTarget();
        FindBestTarget();
    }
    void MoveToTarget()
    {
        // If you have been pushed by someone, don't move until your velocity.magnitude is zero.
        if (isPushed == true)
        {
            agent.enabled = false;
            if (rb.velocity == Vector3.zero)
            {
                isPushed = false;
                agent.enabled = true;
                animator.SetBool("isPushed", false);
            }
            return;
        }

        //if (!characterFeatures.GroundCheck())
        //{
        //    agent.enabled = false;
        //}
        //else
        //{
        //    agent.enabled = true;
        //}

        if (currentTarget != null && agent.enabled && characterFeatures.GroundCheck())
        {
            //// Create a vector towards the target
            //characterFeatures.movement = (currentTarget.position - transform.position).normalized;
            //characterFeatures.movement.y = 0;

            //// Rotate to the target
            //Quaternion targetDirection = Quaternion.LookRotation(characterFeatures.movement);

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDirection, rotationSpeed * Time.fixedDeltaTime);
            //// Move
            //rb.velocity = transform.forward * moveSpeed;
            agent.SetDestination(currentTarget.position);
        }
        animator.SetFloat("speed", rb.velocity.magnitude);
        
    }

    // Find the closest target by looking at the colliders of the objects within the detectionRange.
    private void FindBestTarget()
    {
        Collider[] targetColliders = Physics.OverlapSphere(transform.position, detectionRange);
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (Collider target in targetColliders)
        {
            if ((target.CompareTag("PickUp") && target.GetComponentInChildren<StopObject>().isGrounded)
                || target.TryGetComponent(out CharacterFeatures enemy0) && target.gameObject != gameObject)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < closestDistance)
                {
                    // Update the target object according to the conditions.
                    if (target.CompareTag("PickUp"))
                    {
                        closestDistance = distance;
                        closestTarget = target.transform;
                    }
                    else if (target.TryGetComponent(out CharacterFeatures enemy))
                    {
                        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();

                        // If the target's mass + 'n' is less than my rb chase the taret.
                        if (targetRigidbody.mass + 3 < rb.mass && enemy.GroundCheck())
                        {
                            closestDistance = distance;
                            closestTarget = target.transform;
                        }
                    }
                }
            }
        }

        currentTarget = closestTarget;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out CharacterFeatures enemy))
        {
            Vector3 pushDirection = -collision.contacts[0].normal.normalized;
            pushDirection.y = 0;
            //StartCoroutine(Vibrate()); // Start Vibration
            characterFeatures.SetLastPushedPlayer(enemy);
            PushCharacter(enemy.GetComponent<Rigidbody>(), pushDirection);
            isPushed = true;
            animator.SetBool("isPushed", true);
        }
    }

    private void PushCharacter(Rigidbody pushedRigidbody, Vector3 direction)
    {
        pushedRigidbody.AddForce(direction * gameObject.GetComponent<Rigidbody>().mass * pushForce, ForceMode.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        if (currentTarget != null) Gizmos.DrawLine(transform.position, currentTarget.position);
    }
}
