using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{
    public float fowardSpeed = 8f, backSpeed = 4f, MaxSpeed = 50f, turnSpeed = 180f;
    public float maxRotateAmgle = 30f, wheelRotateSpeed = 10f, wheelTurnSpeed = 10f, maxRotateAngle;
    public float carRotateSpeed = 5f;
    public List<GameObject> wheels = new List<GameObject>();
    public bool isGrounded = false;
    public LayerMask groundMask;
    public Rigidbody rb;
    public float driveInput, turnInput;

    void Start()
    {
        rb.transform.parent = null;
    }


    void Update()
    {
        CheckGround();
        UpdateInputs();
        SpeedControl();
        transform.position = rb.position;
        ForwardWheels();
        RotateWheels();
    }
    void RotateWheels()
    {
        for (int i = 0; i < 2; i++)
        {
            Transform currentWheel = wheels[i].transform;
            Vector3 rotation = new Vector3(currentWheel.localRotation.x, maxRotateAngle * turnInput, 0);
            Quaternion quatRot = Quaternion.Euler(rotation);
            currentWheel.localRotation = Quaternion.Lerp(currentWheel.localRotation, quatRot, Time.deltaTime * wheelRotateSpeed);
        }
    }

    void ForwardWheels()
    {

        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float flatSpeed = Mathf.Clamp(driveInput, -1, 1); // ограничит скорость в диапазоне от -1 до 1

        for (int i = 0; i < wheels.Count; i++)
        {
            wheels[i].transform.Rotate(flatVel.magnitude * Time.deltaTime * wheelTurnSpeed * flatSpeed, 0, 0);
        }
    }
    private void FixedUpdate()
    {
       if (isGrounded)
        {
            rb.AddForce(transform.forward * driveInput);

        }
        else
        {
            rb.AddForce(Vector3.down * 20f);
        }
    }
    void CheckGround()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(rb.position, Vector3.down, out hit, 1f, groundMask);
        if (isGrounded)
        {
            RotateCar(hit.normal);
        }
    }
    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVel.magnitude > MaxSpeed)
        {
            rb.velocity = flatVel.normalized * MaxSpeed;
        }
    }
    void UpdateInputs()
    {
        float axis = Input.GetAxis("Vertical");
        if (axis > 0) 
        {
            driveInput = axis * fowardSpeed;
        }
        else if (axis < 0)
        {
            driveInput = axis * backSpeed;
        }
        turnInput = Input.GetAxis("Horizontal");
        if (isGrounded)
        {
            float angle = turnInput * turnSpeed * Time.deltaTime;
            Quaternion rotation = transform.rotation * Quaternion.Euler(0, angle, 0);
            transform.rotation = rotation;
        }
    }
    void RotateCar(Vector3 normal)
    {

        Quaternion rot = Quaternion.FromToRotation
        (transform.up, normal) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation,rot,carRotateSpeed * Time.deltaTime);
    }
}
