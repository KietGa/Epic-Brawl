using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float basecurrentWalkSpeed = 10f;
    [SerializeField] private float airControl = 0.5f;
    [SerializeField] private float jumpHeight = 5f;
    private float currentWalkSpeed;
    private Vector2 input;
    private Rigidbody rb;
    public bool isWalking { get; set; }
    private bool jumping;
    private bool grounded;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentWalkSpeed = basecurrentWalkSpeed;
    }

    private void Update()
    {
        jumping = Input.GetButton("Jump");
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        grounded = false;
    }

    private void FixedUpdate()
    {
        if (grounded)
        {
            if (jumping)
            {
                isWalking = false;
                rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
            }
            else if (input.magnitude > 0.5f)
            {
                isWalking = true;
                rb.AddForce(CalculateMovement(currentWalkSpeed), ForceMode.VelocityChange);
            }
            else
            {
                isWalking = false;
                var velocity1 = rb.velocity;
                velocity1 = new Vector3(velocity1.x * 0.2f * Time.fixedDeltaTime, velocity1.y, velocity1.z * 0.2f * Time.fixedDeltaTime);
                rb.velocity = velocity1;
            }
        }
        else
        {
            isWalking = false;
            
            if (input.magnitude > 0.5f)
            {
                rb.AddForce(CalculateMovement(currentWalkSpeed * airControl), ForceMode.VelocityChange);
            }
            else
            {
                var velocity1 = rb.velocity;
                velocity1 = new Vector3(velocity1.x * 0.2f * Time.fixedDeltaTime, velocity1.y, velocity1.z * 0.2f * Time.fixedDeltaTime);
                rb.velocity = velocity1;
            }
        }
    }

    Vector3 CalculateMovement(float speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);

        targetVelocity *= speed;
        Vector3 velocity = rb.velocity;

        if (input.magnitude > 0.5f)
        {
            Vector3 velocityChange = targetVelocity - velocity;
            velocity.x = Mathf.Clamp(velocityChange.x, -currentWalkSpeed, currentWalkSpeed);
            velocity.z = Mathf.Clamp(velocityChange.z, -currentWalkSpeed, currentWalkSpeed);

            return new Vector3(velocityChange.x, 0, velocityChange.z);
        }
        else
        {
            return new Vector3();
        }
    }

    public void SetBaseSpeed()
    {
        currentWalkSpeed = basecurrentWalkSpeed;
    }

    public void InscreseSpeedTemp(float speed, float time)
    {
        StartCoroutine(InscreseSpeed(speed, time));
    }

    private IEnumerator InscreseSpeed(float speed, float time)
    {
        currentWalkSpeed += speed;
        yield return new WaitForSeconds(time);
        SetBaseSpeed();
    }

    public void DescreseSpeed(float speed)
    {
        currentWalkSpeed -= speed;
    }
}
