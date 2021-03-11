using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] LayerMask obstacleLayer; 
    [SerializeField] float speed;
    [SerializeField] float rotFactor;

    Vector3 movementVector;
    float rotDir;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movementVector = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        movementVector.x = Input.GetAxis("Horizontal");
        movementVector.z = Input.GetAxis("Vertical");
        rotDir = Input.GetKey(KeyCode.Q) ? -1f : Input.GetKey(KeyCode.E) ? 1f : 0;
    }


    private void FixedUpdate()
    {
        MovementByPhysics();
        RotateByAngularVelocit(); 
    }

    void MovementByPhysics()
    {
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward);
        if (Physics.Raycast(transform.position+Vector3.up*0.5f, transform.forward, 1f, obstacleLayer))
        {
            Debug.Log("yes");
        }
        rb.velocity = ((((transform.right * movementVector.x) + (transform.forward * movementVector.z)) * speed * Time.fixedDeltaTime) + Vector3.up * rb.velocity.y);
    }
    void RotateByAngularVelocit()
    {
        rb.angularVelocity = Vector3.up * rotDir * rotFactor;
    }
}
