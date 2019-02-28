using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{

    public LayerMask collisionMask;

    public const float skinWidth = 0.025f;

    
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    [HideInInspector]
    public float horizontalRaySpacing;

    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D boxCollider;

    public RaycastOrigins rayCastOrig;

    // Start is called before the first frame update
    public virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);





        rayCastOrig.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);

        rayCastOrig.bottomRight = new Vector2(bounds.max.x, bounds.min.y);

        rayCastOrig.topLeft = new Vector2(bounds.min.x, bounds.max.y);

        rayCastOrig.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);


        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }



    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

}
