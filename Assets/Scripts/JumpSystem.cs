using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: Stop At Peak
public class JumpSystem : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] TMPro.TextMeshProUGUI velTxt;
    [SerializeField] float jumpForce;
    [SerializeField] bool waitBeforeJump;
    [SerializeField] float delayBeforeJump;
    [SerializeField] bool jumpBreaking;
    [SerializeField] float jumpHeight;
    [SerializeField] [Range(0f,1f)] float jumpHeightBreakThreeshold;
    [SerializeField] bool stopAtPeak;
    [SerializeField] bool isGrounded = true;
    [SerializeField] float gravity;
    [SerializeField] float fallingFactor;
    [SerializeField] float maxFallingVelocity;
    [SerializeField] float slowFallingFactor;
    [SerializeField] float dragResistanceValue;
    [SerializeField] float jumpBreakingMultiplier;
    //float landingVelocity;
    [Space]
    [SerializeField] bool extraJumps;
    [SerializeField] int maxExtraJumps;
    [SerializeField] int jumpCounter;
    [SerializeField] float allowedFallingVelToMultipleJump; 
    [SerializeField] float multipleJumpMultiplier;
    [SerializeField] float extraJumpHeight;
    [SerializeField]bool landing;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //landingVelocity = fallingFactor;
    }

    // Update is called once per frame
    void Update()
    {
        velTxt.text = rb.velocity.y.ToString();
        if (Input.GetButtonDown("Jump") && !IsInvoking(nameof(Jump)))
        { 
            if (isGrounded && !landing)
                if (!waitBeforeJump)
                {//you can use bool for other scripts to know if this gonna wait before jumping
                    Jump();
                    //jumpCounter++; 
                }
                else
                {
                    Invoke(nameof(Jump), delayBeforeJump);//delay only works with single jump -- update -- now works with first jump
                                                          //jumpCounter++;
                }

            else if (extraJumps && jumpCounter <= maxExtraJumps && !(rb.velocity.y < allowedFallingVelToMultipleJump * maxExtraJumps))
            {
                 //better to use breaking with multiple jump for limiting the sec jump whenever it starts
                //jumpCounter++;
                MultipleJump(jumpCounter);
            }
        }
         
        if (landing && Input.GetButton("Jump"))
        {
            DragResistance(dragResistanceValue);
        }

        if (!landing && Input.GetKeyDown(KeyCode.LeftControl))
        {
            stopAtPeak = true;
        }
        //if (landing && Input.GetButton("Jump"))
        //{
        //    landingVelocity = slowFallingFactor;
        //}
        //else if (landingVelocity != fallingFactor)
        //{
        //    landingVelocity = fallingFactor;
        //}
        //Debug.Log(jumpHeight + (extraJumps ? (extraJumpHeight * (jumpCounter-1f)) : 0.0f) );

    }
    private void FixedUpdate()
    {
        //if (stopAtPeak)
        //{
        //    StopAtPeak();
        //    Invoke(nameof(ResumeFalling), 2f);
        //}
        //else
        //{ 
        //Land();
        //}
        Land();
    }

    private void Land()
    {
        if (!isGrounded)
        {
            if (!stopAtPeak && jumpBreaking)
                BreakJump();
            #region old
            //if (!(rb.velocity.y < 0))//don't use it with breakJump unless you need that behaviour
            //{
            //    Debug.Log("qq");
            //    ApplyGravity(landingVelocity);
            //}

            //else if (rb.velocity.y < 0 && rb.velocity.y > maxFallingVelocity) //momkn tb2a tdrbha f l landingspeed  //to add more force
            //{
            //    Debug.Log("ss");
            //    landing = true;
            //    //ApplyGravity(landingVelocity); 
            //} 
            #endregion
            if (rb.velocity.y > maxFallingVelocity) //momkn tb2a tdrbha f l fallingVelocity  //to add more force
            { 
                //ApplyGravity(landingVelocity);
                ApplyGravity(fallingFactor);
                 
                if (rb.velocity.y < 0)//so u can fall slower only on landing
                    landing = true;
            }
        }
    }
    void BreakJump()
    {
        if (transform.position.y > (GetCurrentTotalJumpHeight()) - jumpHeightBreakThreeshold) //-JHBThreeshold to deaccelarate before breaking the limit
        {
            //Debug.Log(String.Format("{0:0.##}", transform.position.y));

            //if (jumpBreaking && rb.velocity.y > -fallingFactor)
            if (rb.velocity.y > -fallingFactor)
                rb.velocity -= new Vector3(0, jumpBreakingMultiplier * Time.fixedDeltaTime, 0); //jumpbreakingmultiplier -- try one with force accelaeratoon

            //if (stopInPeak) //need to be a single method and need integration with landing noraml method
            //{
            //    StopAtPeak();
            //}
        }
    }
    void ResumeFalling()
    {
        stopAtPeak = false;
    }
    void StopAtPeak()
    {
        //if (transform.position.y >= (GetCurrentTotalJumpHeight() - 0.1f)) //- 0.1f to ensure it doesn't exceed the limit with big value
        if (rb.velocity.y > 0 && transform.position.y >= (jumpHeight - 0.1f)) //- 0.1f to ensure it doesn't exceed the limit with big value
        {
            //ApplyGravity(0.1f); 
            //rb.AddForce(Vector3.up * rb.velocity.y, ForceMode.VelocityChange);            
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            //stopAtPeak = false;
        }
    }
    float GetCurrentTotalJumpHeight() { //not genral height but instatneus height 
        //Debug.Log(jumpHeight + (extraJumps ? (extraJumpHeight * (jumpCounter- 1f)) : 0.0f));

        //return jumpHeight + (extraJumps?(extraJumpHeight * (jumpCounter)):0); //u can make it based on array or collecions
        return jumpHeight + (extraJumps?(extraJumpHeight * (jumpCounter-1f)):0); //badl l line elly fo2
    }
    void Jump()
    {
        jumpCounter++; 
        rb.velocity += Vector3.up * jumpForce;
    } 
    void DoubleJump()
    {
        rb.velocity += Vector3.up * jumpForce * multipleJumpMultiplier;
    }
    void MultipleJump(int jumpNo)
    {
        jumpCounter++; 
        //rb.velocity += Vector3.up * jumpForce * multipleJumpMultiplier * jumpNo;
        rb.velocity += Vector3.up * jumpForce * multipleJumpMultiplier;
        //Debug.Log(String.Format("{0:0.##}", transform.position.y));

    }

    void ApplyGravity(float factor =1)
    {
        //rb.velocity += Vector3.up * gravity * factor;
        rb.AddForce(Vector3.up * gravity * factor, ForceMode.Acceleration);
        //rb.AddForce(Vector3.up * gravity * factor,ForceMode.Impulse);//use the prev line l7d mat7ot if use mass or not
    }

    void DragResistance(float dragFactor = 1)
    {
        rb.AddForce(((Vector3.up*0.5f)*(rb.velocity.y*rb.velocity.y))*dragFactor,ForceMode.Acceleration); //from air resistance drag formula
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            rb.velocity = new Vector3(rb.velocity.x, 0,rb.velocity.z); //to avoid euler number problem //or do not modify the velocity directly when applying gravity, as this can result in unrealistic behaviour -
            landing = false; 
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