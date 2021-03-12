using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] LayerMask obstacleLayer; 
    [SerializeField] float HSpeed;
    [SerializeField] float VSpeed;
    [SerializeField] float rotFactor; 
    Vector3 movementVector;
    float rotDir;
    [SerializeField] float obstacleStopDistance;
    [SerializeField] bool stopBeforeObstacles;

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

        Vector3 rayVector = (transform.forward * movementVector.normalized.z) + (transform.right * movementVector.normalized.x);
         //if(!Jump.isGrounded)
        //if (Physics.box(transform.position + Vector3.down * 0.48f, rayVector, obstacleStopDistance, obstacleLayer))
        if (Physics.BoxCast(transform.position, transform.lossyScale*0.5f, rayVector,transform.rotation,obstacleStopDistance,obstacleLayer))
        {
            Debug.Log(rb.velocity);
            if(stopBeforeObstacles) 
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
        else {  
        rb.velocity = ((((transform.right * movementVector.x * HSpeed) + (transform.forward * movementVector.z * VSpeed)) * Time.fixedDeltaTime) + Vector3.up * rb.velocity.y);
        }
    }
    void RotateByAngularVelocit()
    {
        rb.angularVelocity = Vector3.up * rotDir * rotFactor;
    }
}
