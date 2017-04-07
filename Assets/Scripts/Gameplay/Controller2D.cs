using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    const float SKINWIDTH = 0.015f;

    [SerializeField]
    int horizontalRayCount = 4;
    [SerializeField]
    int verticalRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    bool raysDeactivated = false;

    [SerializeField]
    LayerMask collisionMask;
    [HideInInspector]
    public BoxCollider2D collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    void Awake () {
        collider = GetComponent<BoxCollider2D> ();
    }

    void Start () {
        CalculateRaySpacing ();
        collisions.faceDirection = 1;
    }

    public void Move ( Vector3 velocity) { 
        UpdateRaycastOrigins ();
        collisions.Reset ();

        if (velocity.x != 0) {
            collisions.faceDirection = (int) Mathf.Sign (velocity.x);
        }
        
        HorizontalCollisions (ref velocity);
        
        if (velocity.y != 0) {
            VerticalCollisions (ref velocity);
        }
        transform.Translate (velocity);
    }

    public void DeactivateRays (float time) {
        raysDeactivated = true;
        Invoke ("ResetRays" , time);
    }

    public void ResetRays () {
        raysDeactivated = false;
    }

    void VerticalCollisions ( ref Vector3 velocity ) {
        float directionY = Mathf.Sign (velocity.y);
        float rayLength = Mathf.Abs (velocity.y) + SKINWIDTH;

        for (int i = 0 ; i < verticalRayCount ; i++) {
            Vector2 rayOrigin = ( directionY == -1 ) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * ( verticalRaySpacing * i + velocity.x );
            RaycastHit2D hit = Physics2D.Raycast (rayOrigin , Vector2.up * directionY , rayLength , collisionMask);

            Debug.DrawRay (rayOrigin , Vector2.up * directionY * rayLength , Color.red);

            if (hit && !raysDeactivated) {
                if (hit.collider.CompareTag (MyTags.passThrough.ToString ())) {
                    if (directionY == 1 || hit.distance == 0) {
                        continue;
                    } else if (directionY == -1) {
                        collisions.standingOnPassThrough = true;
                    }
                }

                velocity.y = ( hit.distance - SKINWIDTH ) * directionY;
                rayLength = hit.distance;

                collisions.above = ( directionY == 1 );
                collisions.below = ( directionY == -1 );
            }
        }
    }

    void HorizontalCollisions ( ref Vector3 velocity ) {
        float directionX = collisions.faceDirection;
        float rayLength = Mathf.Abs (velocity.x) + SKINWIDTH;

        if (Mathf.Abs(velocity.x) < SKINWIDTH) {
            rayLength = 2 * SKINWIDTH;
        }

        for (int i = 0 ; i < horizontalRayCount ; i++) {
            Vector2 rayOrigin = ( directionX == -1 ) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * ( horizontalRaySpacing * i );
            RaycastHit2D hit = Physics2D.Raycast (rayOrigin , Vector2.right * directionX , rayLength , collisionMask);

            Debug.DrawRay (rayOrigin , Vector2.right * directionX * rayLength , Color.red);

            if (hit) {

                if (hit.collider.CompareTag (MyTags.passThrough.ToString ())) {
                    if (hit.distance == 0) {
                        continue;
                    }
                }

                velocity.x = ( hit.distance - SKINWIDTH ) * directionX;
                rayLength = hit.distance;

                collisions.right = ( directionX == 1 );
                collisions.left = ( directionX == -1 );
            }
        }
    }


    void UpdateRaycastOrigins () {
        Bounds bounds = collider.bounds;
        bounds.Expand (SKINWIDTH * -2);

        raycastOrigins.bottomLeft = new Vector2 (bounds.min.x , bounds.min.y);
        raycastOrigins.bottomRight = new Vector2 (bounds.max.x , bounds.min.y);
        raycastOrigins.topLeft  = new Vector2 (bounds.min.x , bounds.max.y);
        raycastOrigins.topRight = new Vector2 (bounds.max.x , bounds.max.y);
    }

    void CalculateRaySpacing () {
        Bounds bounds = collider.bounds;
        bounds.Expand (SKINWIDTH * -2);

        horizontalRayCount = Mathf.Clamp (horizontalRayCount , 2 , int.MaxValue);
        verticalRayCount = Mathf.Clamp (verticalRayCount , 2 , int.MaxValue);

        horizontalRaySpacing = bounds.size.y / ( horizontalRayCount - 1 );
        verticalRaySpacing = bounds.size.x / ( verticalRayCount - 1 );
    }

    public struct CollisionInfo {
        public bool standingOnPassThrough;
        public bool above, below;
        public bool left, right;
        public int faceDirection;

        public void Reset () {
            above = below = false;
            left = right = false;
            standingOnPassThrough = false;
        }
    }

    struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

}
