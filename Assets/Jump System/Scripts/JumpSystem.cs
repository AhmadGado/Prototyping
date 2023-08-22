using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: Stop At Peak
//TODO: SphereCast for checking if is ground
//TODO: Break jump on high surfaces.. add current position
public class JumpSystem : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] TMPro.TextMeshProUGUI velTxt;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Jump jump;
    [SerializeField] bool waitBeforeJump;
    [SerializeField] float delayBeforeJump;
    [SerializeField] bool jumpBreaking;
    [SerializeField] [Range(0f, 1f)] float jumpHeightBreakThreeshold;
    [SerializeField] bool stopAtPeak;
    [SerializeField] bool isGrounded = true;
    [SerializeField] float gravity;
    [SerializeField] float fallingFactor;
    [SerializeField] float dragResistanceValue;
    [SerializeField] float jumpBreakingMultiplier;
    //float landingVelocity;
    [Space]
    [SerializeField] bool extraJumps;
    [SerializeField] int maxExtraJumps;
    [SerializeField] int jumpCounter;
    [SerializeField] float allowedFallingVelToMultipleJump;
    [SerializeField][Range(0f,0.5f)] float multipleJumpMultiplier;
    [SerializeField] float extraJumpHeight;
    [SerializeField] bool landing;
    RaycastHit hit;
    [SerializeField] float rayLength;
    [SerializeField] float rayCenterOffset;
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

            else if (extraJumps && jumpCounter <= maxExtraJumps && !(rb.velocity.y < allowedFallingVelToMultipleJump)){
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

        if (!isGrounded)
        {
            if (Physics.Raycast(transform.position + Vector3.down * 0.48f,transform.forward,1f,groundLayer))
            {
                
            }
        }
    }

    void CheckIsGrounded()
    {
        //Debug.DrawRay(transform.position+ Vector3.down * 0.4f, Vector3.down*0.25f,Color.red);
        //Physics.SphereCast(transform.position + Vector3.down * 0.4f, 2f, Vector3.down, out hit, 0.25f, groundLayer);
        //isGrounded = Physics.Raycast(transform.position + Vector3.down * 0.4f, Vector3.down, 0.25f, groundLayer)  ;
        //isGrounded = Physics.SphereCast(transform.position + Vector3.down*0.4f, 0.5f, Vector3.down, out hit, 2f, groundLayer);
        //isGrounded = Physics.BoxCast(transform.position +(transform.up * -1f), Vector3.one, transform.up * -1, out hit, transform.rotation, 0.5f);
        isGrounded = Physics.BoxCast(transform.position+Vector3.up* rayCenterOffset,new Vector3(0.5f, 0.19f, 0.5f), transform.up * -1f, out hit, transform.rotation, rayLength);

        if(hit.collider != null)
        {
            Debug.Log(hit.collider.name);
        }
        if (isGrounded)
            landing = false; 
     }

    private void Land()
    {
        if (!isGrounded)
        {

            if (!stopAtPeak && jumpBreaking)
                BreakJump();
            if (rb.velocity.y > jump.MaxFallingVelocity) //momkn tb2a tdrbha f l fallingVelocity  //to add more force
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
        if (rb.velocity.y > 0 && transform.position.y >= (jump.JumpHeight - 0.1f)) //- 0.1f to ensure it doesn't exceed the limit with big value
        {        
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }
    float GetCurrentTotalJumpHeight()//not genral height but instatneus height 
    {  
        return jump.JumpHeight + (extraJumps ? (extraJumpHeight * (jumpCounter - 1f)) : 0); //badl l line elly fo2//u can make it based on array or collecions
    }
    void Jump()
    {
        jumpCounter++;
        rb.velocity += Vector3.up * jump.JumpForce;
    }
    void DoubleJump()
    {
        rb.velocity += Vector3.up * jump.JumpForce * multipleJumpMultiplier;
    }
    void MultipleJump(int jumpNo)
    {
        jumpCounter++;
          rb.AddForce((Vector3.up * jump.JumpForce * multipleJumpMultiplier), ForceMode.VelocityChange);
     }

    void ApplyGravity(float factor = 1)
    {

        rb.AddForce(Vector3.up * gravity * factor, ForceMode.Acceleration);
    }

    void DragResistance(float dragFactor = 1)
    {
        rb.AddForce(((Vector3.up * jump.SlowFallingFactor ) * (rb.velocity.y * rb.velocity.y)) * dragFactor, ForceMode.Acceleration); //from air resistance drag formula
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
             rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //to avoid euler number problem //but notice that you shouldn't modify the velocity directly when applying gravity, as this can result in unrealistic behaviour -
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
    

    //private void OnDrawGizmos()
    //{
    //    if (hit.collider != null && hit.collider.CompareTag("Ground")) 
    //        Gizmos.color = Color.red;
    //    else         
    //        Gizmos.color = Color.green;
    //    //Gizmos.DrawWireCube(transform.position + Vector3.up * (rayCenterOffset+(transform.localScale.y)), new Vector3(1f, 1f, 1f));
    //    Gizmos.DrawRay(transform.position + Vector3.up * rayCenterOffset, transform.up * -(rayLength+0.5f));

    // }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;

    //    //Check if there has been a hit yet
    //    if (hit.collider != null && hit.collider.CompareTag("Ground"))
    //    {
    //        //Draw a Ray forward from GameObject toward the hit
    //        Gizmos.DrawRay(transform.position, transform.up*-1 * hit.distance);
    //        //Draw a cube that extends to where the hit exists
    //        Gizmos.DrawWireCube(transform.position + transform.up*-1 * hit.distance, transform.localScale);
    //    }
    //    ////If there hasn't been a hit yet, draw the ray at the maximum distance
    //    else
    //    {
    //        Gizmos.color = Color.green;

    //        //Draw a Ray forward from GameObject toward the maximum distance
    //        //    if() 
    //        Gizmos.DrawRay(transform.position, transform.up * - 1);
    //        //Draw a cube at the maximum distance
    //        Gizmos.DrawWireCube(transform.position + transform.up * -1f, transform.localScale);
    //    }
    //}
}


[System.Serializable]
public class Jump
{
   [SerializeField] float jumpForce;   
   [SerializeField] float jumpHeight;   
   [SerializeField] float maxFallingVelocity;
   [SerializeField] float slowFallingFactor;

    public float JumpForce { get => jumpForce; set => jumpForce = value; }
    public float JumpHeight { get => jumpHeight; set => jumpHeight = value; }
    public float MaxFallingVelocity { get => maxFallingVelocity; set => maxFallingVelocity = value; }
    public float SlowFallingFactor { get => slowFallingFactor; set => slowFallingFactor = value; }
}
[System.Serializable]
public class ExtraJump : Jump
{ 
    [Range(0f, 0.5f)] float multipleJumpMultiplier;
};
 