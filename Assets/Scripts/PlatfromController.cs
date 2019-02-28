using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatfromController : RaycastController
{
    
    public LayerMask passengerMask;
    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    public float speed;
    int fromWaypointIntIndex;
    float percentBetweenWaypoints;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for(int i =0; i<localWaypoints.Length;i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateRaycastOrigins();


        Vector3 velocity = CalculatePlatformMovement();

        MovePassengers(velocity);

        transform.Translate(velocity);
    }

    void MovePassengers(Vector3 velocity)
    {
        HashSet<Transform> movePassengers = new HashSet<Transform>();
        float dirX = Mathf.Sign(velocity.x);
        float dirY = Mathf.Sign(velocity.y);

        //Vertically moving platform
        if (velocity.y != 0)
        {
            float rayLenght = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (dirY == -1) ? rayCastOrig.bottomLeft : rayCastOrig.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);

                Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLenght, Color.black);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLenght, passengerMask);

                if (hit)
                {
                    if (!movePassengers.Contains(hit.transform))
                    {
                        movePassengers.Add(hit.transform);
                        float pushX = (dirY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * dirY;

                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }


                }
            }
        }

        //horizontally moving platform
        if (velocity.x != 0)
        {
            float rayLenght = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (dirX == -1) ? rayCastOrig.bottomLeft : rayCastOrig.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);

                Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLenght, Color.black);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLenght, passengerMask);

                if (hit)
                {
                    if (!movePassengers.Contains(hit.transform))
                    {
                        movePassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * dirX;
                        float pushY = 0;

                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }

            }
        }

        //Passenger on top of horiz or vert moving platform

        if(dirY==-1||velocity.y==0 && velocity.x!=0)
        {
            float rayLenght = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin =  rayCastOrig.topLeft + Vector2.right * (verticalRaySpacing * i);

                Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLenght, Color.black);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up , rayLenght, passengerMask);

                if (hit)
                {
                    if (!movePassengers.Contains(hit.transform))
                    {
                        movePassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }


                }
            }
        }
    }

    Vector3 CalculatePlatformMovement()
    {
        int toWaypointIndex = fromWaypointIntIndex + 1;
        float distBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIntIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed/distBetweenWaypoints;

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIntIndex], globalWaypoints[toWaypointIndex] , percentBetweenWaypoints);

        if(percentBetweenWaypoints>=1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIntIndex++;
            if(fromWaypointIntIndex>=globalWaypoints.Length-1)
            {
                fromWaypointIntIndex = 0;
                System.Array.Reverse(globalWaypoints);
            }
        }


        return newPos - transform.position;

    }

    private void OnDrawGizmos()
    {
        if(localWaypoints!= null)
        {
            Gizmos.color = Color.blue;
            float size = .3f;
            for(int i = 0; i <localWaypoints.Length;i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i]: localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }





}

