using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: Stop At Peak
//TODO: SphereCast for checking if is ground
//TODO: 
public class JumpSystem : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] TMPro.TextMeshProUGUI velTxt;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float jumpForce;
    [SerializeField] bool waitBeforeJump;
    [SerializeField] float delayBeforeJump;
    [SerializeField] bool jumpBreaking;
    [SerializeField] float jumpHeight;
    [SerializeField] [Range(0f, 1f)] float jumpHeightBreakThreeshold;
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
    [SerializeField] bool landing;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        velTxt.text = rb.velocity.y.ToString();
        if (Input.GetButtonDown("Jump") && !IsInvoking(nameof(Jump)))
        {
            if (isGrounded && !landing)
                if (!waitBeforeJump){//you can use bool for other scripts to know if this gonna wait before jumping
                    Jump();
                }
                else{
                    Invoke(nameof(Jump), delayBeforeJump);//delay only works with first jump
                }

            else if (extraJumps && jumpCounter <= maxExtraJumps && !(rb.velocity.y < allowedFallingVelToMultipleJump * maxExtraJumps)){
                //better to use breaking with multiple jump for limiting the sec jump whenever it starts
                MultipleJump(jumpCounter);
            }
        }

        if (landing && Input.GetButton("Jump")){
            DragResistance(dragResistanceValue);
        }

        if (!landing && Input.GetKeyDown(KeyCode.LeftControl)){
            stopAtPeak = true;
        }

    }
    private void FixedUpdate()
    {
        CheckIsGrounded();
        Land();
    }

    void CheckIsGrounded()
    { 
        Debug.DrawRay(transform.position+ Vector3.down * 0.4f, Vector3.down*0.25f,Color.red);
        //Physics.SphereCast(transform.position + Vector3.down * 0.4f, 2f, Vector3.down, out hit, 0.25f, groundLayer);
        isGrounded = Physics.Raycast(transform.position + Vector3.down * 0.4f, Vector3.down, 0.25f, groundLayer);
        //isGrounded = Physics.SphereCast(transform.position + Vector3.down * 0.4f, 2f, Vector3.down, out hit, 2f, groundLayer);
        if (isGrounded)
            landing = false; 
     }

    private void Land()
    {
        if (!isGrounded)
        {
            if (!stopAtPeak && jumpBreaking)
                BreakJump();
            if (rb.velocity.y > maxFallingVelocity) //momkn tb2a tdrbha f l fallingVelocity  //to add more force
            {
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
              if (rb.velocity.y > -fallingFactor)
                rb.velocity -= new Vector3(0, jumpBreakingMultiplier * Time.fixedDeltaTime, 0); //jumpbreakingmultiplier -- try one with force accelaeratoon
        }
    }
    void ResumeFalling()
    {
        stopAtPeak = false;
    }
    void StopAtPeak()
    {
        if (rb.velocity.y > 0 && transform.position.y >= (jumpHeight - 0.1f)) //- 0.1f to ensure it doesn't exceed the limit with big value
        {        
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }
    float GetCurrentTotalJumpHeight()//not genral height but instatneus height 
    {  
        return jumpHeight + (extraJumps ? (extraJumpHeight * (jumpCounter - 1f)) : 0); //badl l line elly fo2//u can make it based on array or collecions
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
        rb.velocity += Vector3.up * jumpForce * multipleJumpMultiplier;
        //Debug.Log(String.Format("{0:0.##}", transform.position.y));

    }

    void ApplyGravity(float factor = 1)
    {
        rb.AddForce(Vector3.up * gravity * factor, ForceMode.Acceleration);
    }

    void DragResistance(float dragFactor = 1)
    {
        rb.AddForce(((Vector3.up * 0.5f) * (rb.velocity.y * rb.velocity.y)) * dragFactor, ForceMode.Acceleration); //from air resistance drag formula
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //isGrounded = true;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //to avoid euler number problem //but notice that you shouldn't modify the velocity directly when applying gravity, as this can result in unrealistic behaviour -
            //landing = false;
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