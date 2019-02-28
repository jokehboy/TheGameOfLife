using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]

public class Player : MonoBehaviour
{

    Controller controller;
    Vector3 velocity;
    public float moveSpeed;

    public float jumpHeight = 4;
    public float timeToApex = .4f;

    public float accelerationTimeGround = .1f;
    public float accelerationTimeAir = .2f;

    public float jumpVel;
    float gravity;
    float velocitySmoothVal;

    

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToApex, 2);
        jumpVel = Mathf.Abs(gravity) * timeToApex;
        print(gravity + jumpVel);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        controller.Move(velocity * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(controller.collisions.below)
        {
            float jAxis = Input.GetAxis("Jump");
            if (jAxis > 0f)
            {
                velocity.y = jumpVel;
            }
        }

        float targetVelX = input.x * moveSpeed;
        velocity.y += gravity * Time.deltaTime;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelX, ref velocitySmoothVal, (controller.collisions.below)?accelerationTimeGround :accelerationTimeAir);

             
    }
}
