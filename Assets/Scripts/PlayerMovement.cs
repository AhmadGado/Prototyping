//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
////Rotation problem with physics (Collision) //Done
////Time.delta time and Time.fixedDeltaTime //Done
////Jump & Double Jump
//public class PlayerMovement : MonoBehaviour
//{

//    [SerializeField] float speed;
//    [SerializeField] float rotFactor;
//    [Space]
//    [Space]
//    [SerializeField] float jumpForce;
//    [SerializeField] float jumpHeight = 2f;
//    [SerializeField] bool isGrounded = true;
//    [SerializeField] float gravity = -9.8f;
//    [SerializeField] float landingFactor = 3f;
//    bool landing;

//    Rigidbody rb; //check for get set getting (next line)!!
//    //Rigidbody rbody { get { return GetComponent<Rigidbody>();} set { value = rbody; } }


//    Vector3 movementVector;
//    float rotDir;
//    // Start is called before the first frame update
//    void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        movementVector = new Vector3(0, 0, 0);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //Movement();
//        //Rotate();
//        movementVector.x = Input.GetAxis("Horizontal");
//        movementVector.z = Input.GetAxis("Vertical");

//        rotDir = Input.GetKey(KeyCode.Q) ? -1f : Input.GetKey(KeyCode.E) ? 1f : 0;



//        //Jump(); //don't ever play with rb in update,, only fixedupdate one-off event is ok
//        if (Input.GetButtonDown("Jump") && isGrounded)
//        {
//            Jump();
//        }


//    }
//    private void FixedUpdate()
//    {
//        MovementByPhysics();
//        RotateByAngularVelocit();

//        //if (!isGrounded && !landing)
//        //{
//        //    ApplyGravity();
//        //}
//    }

//    private void ApplyGravity()
//    {
//        if (rb.velocity.y > 0)
//        {
//            landing = true;
//            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
//            //rb.velocity += Vector3.up * gravity * landingFactor;
//        }
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        Debug.Log("Collision");
//        if (collision.gameObject.CompareTag("Ground"))
//        {
//            isGrounded = true;
//        }
//    }

//    void MovementByPhysics()
//    {
//        // var inputVer = Input.GetAxis("Vertical");
//        //  var inputHor = Input.GetAxis("Horizontal");

//        //  if (inputVer != 0 || inputHor != 0)
//        // {
//        //rb.velocity = (((transform.forward * inputVer)+(transform.right * inputHor)) * speed * Time.fixedDeltaTime); //no need for fixedDeltaTime fixedUpdate function do it for u
//        //rb.velocity = ((((transform.forward * inputVer) + (transform.right * inputHor)) * speed * Time.fixedDeltaTime)+ Vector3.up * rb.velocity.y);
//        rb.velocity = ((((transform.right * movementVector.x) + (transform.forward * movementVector.z)) * speed * Time.fixedDeltaTime) + Vector3.up * rb.velocity.y);

//        //  }
//        //  else
//        //  {
//        // rb.velocity = (Vector3.forward + Vector3.right) * 0;//only those axis not the jump one
//        //       rb.velocity = Vector3.up * rb.velocity.y;//only reset x,z axis not the jump one
//        //   }
//        #region Without Stopping
//        //if (inputVer != 0)
//        //{

//        //    //rb.AddForce(Vector3.forward * inputVer);
//        //    //rb.AddForce(transform.forward * inputVer);
//        //    rb.velocity = (transform.forward * inputVer * speed);
//        //}

//        //if (inputHor != 0)
//        //{
//        //    //rb.AddForce(Vector3.right * inputHor);
//        //    //rb.AddForce(transform.right * inputHor);
//        //    rb.velocity = (transform.right * inputHor * speed);

//        //} 
//        #endregion

//    }

//    void Movement()
//    {

//        #region smooth movement
//        var inputVer = Input.GetAxis("Vertical");
//        var inputHor = Input.GetAxis("Horizontal"); //u can use GetAxisRaw for non-smooth movement

//        if (inputVer != 0)
//        {
//            //transform.position += Vector3.forward * inputVer * speed; //not related to the obj current direction
//            transform.position += transform.forward * inputVer * speed * Time.deltaTime;
//        }

//        if (inputHor != 0)
//        {
//            //transform.position += Vector3.right * inputHor * speed;
//            transform.Translate(Vector3.right * inputHor * speed * Time.deltaTime); //Translate done what the previous line done
//            //transform.Translate(Vector3.right * Mathf.Sign(inputHor)*1 * speed);
//        }
//        #endregion


//        #region non-smooth movement
//        /*  
//        var inputForward = Input.GetKey(KeyCode.W);
//        var inputBack = Input.GetKey(KeyCode.S);
//        var inputRight = Input.GetKey(KeyCode.D);
//        var inputLeft = Input.GetKey(KeyCode.A);
       
//        if (inputForward)
//        {
//            transform.position += Vector3.forward * speed;
//        }
//        else if(inputBack)
//        {
//            transform.position += Vector3.back * speed;

//        }


//        if (inputRight)
//        {
//            transform.position += Vector3.right * speed;

//        }
//        else if(inputLeft)
//        {
//            transform.position += Vector3.left * speed;

//        }*/
//        #endregion


//    }


//    void Rotate()
//    {
//        if (Input.GetKey(KeyCode.E))
//        {
//            transform.Rotate(0, 3.75f * rotFactor * Time.deltaTime, 0); //degrees not radian
//        }

//        if (Input.GetKey(KeyCode.Q))
//        {
//            transform.Rotate(0, -3.75f * rotFactor * Time.deltaTime, 0);

//        }
//        #region Intro to rotation
//        //if (Input.GetKey(KeyCode.E))
//        //{
//        //    transform.Rotate(0, 7.5f, 0); //degrees not radian
//        //}

//        //if (Input.GetKeyDown(KeyCode.Q))
//        //{
//        //    transform.Rotate(0, -30, 0);

//        //} 
//        #endregion
//    }

//    void RotateByAngularVelocit()
//    {

//        rb.angularVelocity = Vector3.up * rotDir * rotFactor;

//        //if (Input.GetKey(KeyCode.E))
//        //{
//        //    //rb.angularVelocity = Vector3.up * 0.5f * rotFactor * Time.fixedDeltaTime; //no need for fixedDeltaTime fixedUpdate function do it for u
//        //    rb.angularVelocity = Vector3.up * 0.5f * rotFactor;
//        //}
//        //else if (Input.GetKey(KeyCode.Q))
//        //{
//        //    rb.angularVelocity = Vector3.up * -0.5f * rotFactor;
//        //}
//        //else
//        //{
//        //    rb.angularVelocity = Vector3.zero;//added after two if statments, 
//        //    //so try it without this condition first
//        //}
//    }

//    void Jump()
//    {
//        isGrounded = false;
//        landing = false;
//        rb.velocity += Vector3.up * (jumpForce);
//    }
//    void OldJump()
//    {
//        //if(transform.position.y < jumpHeight)
//        //isGrounded = false;
//        rb.velocity += Vector3.up * (jumpForce);//not good to change velocity dircetly
//                                                //rb.AddForce(Vector3.up * jumpForce);
//                                                //else if(!isGrounded &&rb.velocity.y >0 )
//                                                //{
//                                                //    Debug.Log("here");
//                                                //    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
//                                                //    rb.AddForce( Vector3.down * gravity * landingFactor);
//                                                //}



//        #region bad jump

//        //if (Input.GetButtonDown("Jump"))
//        //{
//        //    rb.AddForce(Vector3.up * jumpForce);
//        //} 
//        #endregion
//    }


//}
