using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller : RaycastController

{

    public override void Start()
    {
        base.Start();

    }
    float maxClimbAngle = 85;
    float maxDescendAngle = 70;

    
    public CollisionInfo collisions;

    // Start is called before the first frame update
    

    private void Update()
    {   
    }


    public void Move(Vector3 velocity)
    {
        

        UpdateRaycastOrigins();

        collisions.Reset();

        DescendSlope(ref velocity);

        HorizontalCollisions(ref velocity);

        Physics2D.SyncTransforms();

        VerticalCollisions(ref velocity);

        

        

        transform.Translate(velocity);
    }


    void HorizontalCollisions(ref Vector3 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);
        float rayLenght = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (dirX == -1) ? rayCastOrig.bottomLeft : rayCastOrig.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i );

            Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLenght, Color.black);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLenght, collisionMask); 
            if(hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    float distToSlopeStart = 0;
                    if(slopeAngle != collisions.slopeAngleOld)
                    {
                        distToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distToSlopeStart * dirX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distToSlopeStart * dirX;
                }

                if(!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * dirX;
                    rayLenght = hit.distance;
                }

                if(collisions.climbingSlope)
                {
                    velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                }

                collisions.left = dirX == -1;
                collisions.right = dirX == 1;
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float dirY = Mathf.Sign(velocity.y);
        float rayLenght = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (dirY == -1) ? rayCastOrig.bottomLeft : rayCastOrig.topLeft ;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            
            Debug.DrawRay(rayOrigin, Vector2.up * dirY* rayLenght, Color.black);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLenght, collisionMask);
            if (hit)
            {

                velocity.y = (hit.distance - skinWidth) * dirY;
                rayLenght = hit.distance;

                if(collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = dirY == -1;
                collisions.above = dirY == 1;
            }
        }
        if(collisions.climbingSlope)
        {
            float directionX = Mathf.Sin(velocity.x);
            rayLenght = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1)?rayCastOrig.bottomLeft:rayCastOrig.bottomRight)+Vector2.up*velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, collisionMask);

            if(hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDist = Mathf.Abs(velocity.x);
        float climbVelY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;

        
        if (velocity.y <= climbVelY)
        { 
            velocity.y = climbVelY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
        
    }

    void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);

        Vector2 rayOrigin = (directionX == -1) ? rayCastOrig.bottomRight : rayCastOrig.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if(hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle!=0 && slopeAngle <= maxDescendAngle)
            {
                if(Mathf.Sign(hit.normal.x) == directionX)
                {
                    if(hit.distance-skinWidth <= Mathf.Tan(slopeAngle*Mathf.Deg2Rad*Mathf.Abs(velocity.x)))
                    {
                        float moveDist = Mathf.Abs(velocity.x);
                        float descVelY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(velocity.x);
                        velocity.y -= descVelY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;

                    }

                }
            }
        }
    }

      
    }


   public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
     

   

    
    

