using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float walkSpeed;
    public float jumpSpeed;
    bool jumping = false;

    Rigidbody rb;
    Collider coll;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    

     void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Wall")
        {
            float jAxis = Input.GetAxis("Jump");
            if(jAxis>0f)
            {
                Vector3 jumpVector = new Vector3(0f, jumpSpeed, 0f);
                rb.velocity = rb.velocity + new Vector3(0f,3f,0f);
            }
        }
    }

    void WalkHandler()
    {

        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        float distance = walkSpeed * Time.deltaTime;

        float hAxis = Input.GetAxis("Horizontal");

        

        Vector3 movement = new Vector3(hAxis * distance, 0f, 0f);
        Vector3 currentPos = transform.position;
        Vector3 newPos = currentPos + movement;

        rb.MovePosition(newPos);
    }


    void JumpHandler()
    {
        float jAxis = Input.GetAxis("Jump");

        bool isGrounded = CheckGrounded();

        if(jAxis>0f)
        {
            if (!jumping && isGrounded)
            {
                jumping = true;
                Vector3 jumpVector = new Vector3(0f, jumpSpeed, 0f);
                rb.velocity = rb.velocity + jumpVector;
            }
            
        }
        else
        {
            jumping = false;
        }
    }

    bool CheckGrounded()
    {
        float distToGround = GetComponent<Collider>().bounds.extents.y;

        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, distToGround + 0.01f);

        return isGrounded;
    }


    // Update is called once per frame
    void Update()
    {

        CheckGrounded();
        WalkHandler();
        JumpHandler();
    }
}
