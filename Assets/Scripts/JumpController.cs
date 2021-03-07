using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] TMPro.TextMeshProUGUI velTxt;
    [SerializeField] float jumpForce;
    [SerializeField] bool waitBeforeJump;
    [SerializeField] float delayBeforeJump;
    //[SerializeField] float delayBetweenJumps;
    [SerializeField] float jumpHeight;
    [SerializeField] int maxExtraJumps;
    [SerializeField] int jumpCounter;
    [SerializeField] float multipleJumpMultiplier;
    [SerializeField] float doubleJumpHeight;
    [SerializeField] bool isGrounded = true;
    [SerializeField] float gravity;
    [SerializeField] float fallingFactor;
    [SerializeField] float maxFallingVelocity;
    [SerializeField] float slowFallingFactor;
    [SerializeField] float allowedFallingVelToMultipleJump;
    [SerializeField] float jumpStoppingMultiplier;
    
    bool landing;
    [SerializeField]float landingVelocity;
    [SerializeField] bool doubleJump;
    [SerializeField] bool stopInPeak;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        landingVelocity = fallingFactor;
        slowFallingFactor = 0.022f * fallingFactor;
    }

    // Update is called once per frame
    void Update()
    {
        velTxt.text = rb.velocity.y.ToString();
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded && !landing)
                if (!waitBeforeJump) //you can use bool for other scripts to know if this gonna wait before jumping
                    Jump();
                else {
                    //StartCoroutine(nameof(Jump),delayBeforeJump); //name of problem with overloaded methods
                    //StartCoroutine(Jump(delayBeforeJump)); //also u can use the next
                    Invoke(nameof(Jump), delayBeforeJump);
                }

            //else if (doubleJump && !(rb.velocity.y < allowedFallingVelToMultipleJump * maxExtraJumps) && jumpCounter < maxExtraJumps)
            //{
            //    jumpHeight *= doubleJumpHeight;
            //    jumpCounter++;
            //    //DoubleJump();
            //    MultipleJump(jumpCounter+1); //plus the first normal jump
            //}
        }

        if(landing && Input.GetButton("Jump"))
        {
            landingVelocity = slowFallingFactor;
        }
        else if(landingVelocity != fallingFactor )
        {
            landingVelocity = fallingFactor;
        }
        //if(rb.velocity.y < 0)
    }
    private void FixedUpdate()
    {
        ApplyGravity();
    }


    void Jump()
    {
        //isGrounded = false;
        rb.velocity += Vector3.up * jumpForce;
    }

    IEnumerator Jump(float delay = 2)
    { 
        yield return new WaitForSeconds(delay); 
        rb.velocity += Vector3.up * jumpForce;
    }
    void DoubleJump()
    {
        rb.velocity += Vector3.up * jumpForce* multipleJumpMultiplier;
    } 
    void MultipleJump(int jumpNo)
    {
        rb.velocity += Vector3.up * jumpForce* multipleJumpMultiplier * jumpNo;
    }

    void ApplyGravity() //need refactoring more than one method
    {
        if (!isGrounded)
        {
            if (transform.position.y > (jumpHeight) + 0.1f) //0.1 to ensure it exceeds
            {
                //rb.velocity -= new Vector3(rb.velocity.x, jumpStoppingMultiplier* Time.fixedDeltaTime, rb.velocity.z);
                //rb.velocity = Vector3.Lerp(rb.velocity,new Vector3(rb.velocity.x, 0, rb.velocity.z),0.2f);
                //if (rb.velocity.y > -fallingFactor)
                //    rb.velocity -= new Vector3(rb.velocity.x, jumpStoppingMultiplier * Time.fixedDeltaTime, rb.velocity.z);

                if (stopInPeak)
                {
                    if (!(rb.velocity.y < 0))
                        rb.velocity += Vector3.up * gravity * 0.1f;
                    //startcourtine
                    return;
                }
            }
            if (!(rb.velocity.y < 0))
            {
                rb.velocity += Vector3.up * gravity;
                //rb.AddForce(Vector3.up * gravity, ForceMode.);
            }
            //if (!(rb.velocity.y <0))
            //{
            //    Debug.Log("heres");
            //    rb.AddForce(Vector3.up * gravity,ForceMode.);
            //}
            //else
            else if (rb.velocity.y > maxFallingVelocity)
            {
                landing = true;
                rb.velocity += Vector3.up * gravity * landingVelocity;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            rb.velocity = Vector3.up * 0; //to avoid euler number problem //or do not modify the velocity directly when applying gravity, as this can result in unrealistic behaviour -
            landing = false;
            if(jumpCounter>0)
            jumpHeight /= doubleJumpHeight;
            jumpCounter = 0;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;

        }

    }

}
