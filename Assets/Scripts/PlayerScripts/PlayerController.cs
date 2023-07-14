using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public Animator animator;
    PlayerFeatures characterFeatures;

    [SerializeField] private float moveSpeed = 14f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float pushForce = 25f;
    public bool isPushed;

    private Vector2 firstTouchPosition;
    private Vector2 lastTouchPosition;
    Vector2 dragDirection; // direction vector drawn on the screen


    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.AddForce(-Vector3.up * 20, ForceMode.Impulse);
        rb.freezeRotation = true; // Freeze rigidnody rotation.
        characterFeatures = GetComponent<PlayerFeatures>();
        dragDirection = transform.forward;
        characterFeatures.movement = transform.forward * moveSpeed;
    }

    void Update()
    {
        if (!GameManager.Instance.isGameActive) return;

        if (isPushed == true)
        {
            if (rb.velocity.magnitude <= 0.1)
            {
                isPushed = false;
                animator.SetBool("isPushed", false);
            }
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Set firstTouchPosition on first touch of screen.
                    firstTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    // Update lastTouchPosition as long as the screen is touched
                    lastTouchPosition = touch.position;
                    break;
            }
        }

        // Calculate the direction of the line drawn on the screen.
        dragDirection = lastTouchPosition - firstTouchPosition;
        dragDirection.Normalize();

        // Check if dragDirection magnitude is below the minimum threshold
        if (dragDirection.magnitude < 0.1f)
        {
            dragDirection = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.isGameActive == true )
        {
            if (!characterFeatures.GroundCheck())
            {
                animator.SetBool("isGrounded", false);
                return;
            }

            // If we were pushed by someone, wait for the push to finish.
            if (isPushed) return;

            // Move the character in the direction of the drag
            characterFeatures.movement = new Vector3(dragDirection.x, 0, dragDirection.y) * moveSpeed;
            rb.velocity = characterFeatures.movement;
            animator.SetFloat("speed" , rb.velocity.magnitude);

            // Rotate a character according to the drag direction.
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(dragDirection.x, 0f, dragDirection.y));
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }  
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out CharacterFeatures enemy))
        {
            Vector3 pushDirection = -collision.contacts[0].normal.normalized;
            pushDirection.y = 0;
            characterFeatures.SetLastPushedPlayer(enemy);
            PushCharacter(enemy.GetComponent<Rigidbody>(), pushDirection);
            isPushed = true;
            animator.SetBool("isPushed", true);
        }
    }

    private void PushCharacter(Rigidbody pushedRigidbody, Vector3 direction)
    {
        pushedRigidbody.AddForce(direction * gameObject.GetComponent<Rigidbody>().mass * 20, ForceMode.Impulse);
    }

}
