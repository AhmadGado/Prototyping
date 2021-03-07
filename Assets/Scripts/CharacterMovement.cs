using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed;
    [SerializeField] float rotFactor;
    [Space]
    [Space]
    [SerializeField] float jumpForce;
    [SerializeField] int maxExtraJumps;
    [SerializeField] int jumpCounter;
    [SerializeField] bool isGrounded = true;
    [SerializeField] float gravity;
    [SerializeField] float fallingFactor;
    bool landing;
    [SerializeField]bool doubleJump;
     
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
        
        if (Input.GetButtonDown("Jump") )
        {
            if(isGrounded)
                Jump();
            else if (doubleJump && !(rb.velocity.y <-5f) && jumpCounter < maxExtraJumps)
            {
                jumpCounter++;
                Jump();
            }
        } 

    }
     
    private void FixedUpdate()
    {
        MovementByPhysics();
        RotateByAngularVelocit();
        ApplyGravity();
    }
  
    void MovementByPhysics()
    {
       rb.velocity = ((((transform.right * movementVector.x) + (transform.forward * movementVector.z)) * speed * Time.fixedDeltaTime) + Vector3.up * rb.velocity.y);
    } 
    void RotateByAngularVelocit()
    { 
        rb.angularVelocity = Vector3.up * rotDir * rotFactor; 
    }
    void Jump()
    {
        isGrounded = false;
        rb.velocity += Vector3.up *  jumpForce;
    } 
    void ApplyGravity()
    {
        //if(rb.velocity.y > 0)
        //if(!isGrounded && !(rb.velocity.y < 0))
        if(!isGrounded)
        {
            rb.velocity += Vector3.up * gravity * fallingFactor;
        }
    }

    IEnumerator ForceDown()
    { 
        yield return new WaitForSeconds(1f);
        rb.velocity += Vector3.up * gravity * fallingFactor;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")){
            isGrounded = true;
            jumpCounter = 0; 
        }
    }

}
