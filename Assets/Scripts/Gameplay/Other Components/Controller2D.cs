using UnityEngine;
using System;

/// <summary>
/// Replaces the rigidbody component. deals with collisions using raycast
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    public delegate void OnCollisionEventHandler (GameObject source, GameObject target);
    public event OnCollisionEventHandler OnCollision;

    private const float SKINWIDTH = 0.015f;
    private const float distanceBetweenRays = 0.25f;

    private int horizontalRayCount;
    private int verticalRayCount;

    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    private bool raysDeactivated = false;

    [SerializeField]
    private LayerMask collisionMask;
    [HideInInspector]
    public BoxCollider2D collider;

    private RaycastOrigins raycastOrigins;

    private CollisionInfo collisions;
    public CollisionInfo Collisions { get { return collisions; } }

    private void Awake () {
        collider = GetComponent<BoxCollider2D> ();
    }

    private void Start () {
        CalculateRaySpacing ();
        collisions.movementDirX = 1;
    }

    public void Move ( Vector2 moveAmount) { 
        UpdateRaycastOrigins ();
        collisions.Reset ();

        if (moveAmount.x != 0) {
            collisions.movementDirX = (int) Mathf.Sign (moveAmount.x);
        }
        
        HorizontalCollisions (ref moveAmount);
        
        if (moveAmount.y != 0) {
            VerticalCollisions (ref moveAmount);
        }
        transform.Translate (moveAmount);
    }

    public void DeactivateRays (float time) {
        raysDeactivated = true;
        Invoke ("ResetRays" , time);
    }

    private void ResetRays () {
        raysDeactivated = false;
    }

    private void VerticalCollisions ( ref Vector2 moveAmount ) {
        float directionY = Mathf.Sign (moveAmount.y);
        float rayLength = Mathf.Abs (moveAmount.y) + SKINWIDTH;

        for (int i = 0 ; i < verticalRayCount ; i++) {
            Vector2 rayOrigin = ( directionY == -1 ) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * ( verticalRaySpacing * i + moveAmount.x );
            RaycastHit2D hit = Physics2D.Raycast (rayOrigin , Vector2.up * directionY , rayLength , collisionMask);

            Debug.DrawRay (rayOrigin , Vector2.up * directionY , Color.red);

            if (hit && !raysDeactivated) {
                hit.collider.gameObject.SendMessage ("OnCollision" , gameObject , SendMessageOptions.DontRequireReceiver);

                bool pass = HandleVerticalCollisions (hit , directionY);
                if (!pass) {
                    continue;
                }

                moveAmount.y = ( hit.distance - SKINWIDTH ) * directionY;
                rayLength = hit.distance;

                collisions.above = ( directionY == 1 );
                collisions.below = ( directionY == -1 );
            }
        }
    }

    private void HorizontalCollisions ( ref Vector2 moveAmount ) {
        float directionX = collisions.movementDirX;
        float rayLength = Mathf.Abs (moveAmount.x) + SKINWIDTH;

        if (Mathf.Abs(moveAmount.x) < SKINWIDTH) {
            rayLength = 2 * SKINWIDTH;
        }

        for (int i = 0 ; i < horizontalRayCount ; i++) {
            Vector2 rayOrigin = ( directionX == -1 ) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * ( horizontalRaySpacing * i );
            RaycastHit2D hit = Physics2D.Raycast (rayOrigin , Vector2.right * directionX , rayLength , collisionMask);

            Debug.DrawRay (rayOrigin , Vector2.right * directionX , Color.red);

            if (hit && !raysDeactivated) {
                hit.collider.gameObject.SendMessage ("OnCollision" , gameObject , SendMessageOptions.DontRequireReceiver);

                bool pass = HandleHorizontalCollisions (hit);
                if (!pass) {
                    continue;
                }

                moveAmount.x = ( hit.distance - SKINWIDTH ) * directionX;
                rayLength = hit.distance;

                collisions.right = ( directionX == 1 );
                collisions.left = ( directionX == -1 );
            }
        }
    }

    private void UpdateRaycastOrigins () {
        Bounds bounds = collider.bounds;
        bounds.Expand (SKINWIDTH * -2);

        raycastOrigins.bottomLeft = new Vector2 (bounds.min.x , bounds.min.y);
        raycastOrigins.bottomRight = new Vector2 (bounds.max.x , bounds.min.y);
        raycastOrigins.topLeft  = new Vector2 (bounds.min.x , bounds.max.y);
        raycastOrigins.topRight = new Vector2 (bounds.max.x , bounds.max.y);
    }

    private void CalculateRaySpacing () {
        Bounds bounds = collider.bounds;
        bounds.Expand (SKINWIDTH * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;
        //set the n of rays base off the  maxdistance between them
        horizontalRayCount = Mathf.RoundToInt (boundsHeight / distanceBetweenRays);
        verticalRayCount = Mathf.RoundToInt (boundsWidth / distanceBetweenRays);
        //clamps so there are never less than 2 rays
        horizontalRayCount = Mathf.Clamp (horizontalRayCount , 2 , int.MaxValue);
        verticalRayCount = Mathf.Clamp (verticalRayCount , 2 , int.MaxValue);
        //defines the final spacing
        horizontalRaySpacing = boundsHeight / ( horizontalRayCount - 1 );
        verticalRaySpacing = boundsWidth / ( verticalRayCount - 1 );
    }

    private bool HandleHorizontalCollisions (RaycastHit2D hit) {
        if (hit.collider.isTrigger) {
            return false;
        }
        if (hit.collider.CompareTag (MyTags.passThrough.ToString ())) {
            if (hit.distance == 0) {
                return false;
            }
        }
        return true;
    }

    private bool HandleVerticalCollisions (RaycastHit2D hit, float directionY ) {
        if (hit.collider.isTrigger) {
            return false;
        }

        if (hit.collider.CompareTag (MyTags.passThrough.ToString ())) {
            if (directionY == 1 || hit.distance == 0) {
                return false;
            }
            else if (directionY == -1) {
                collisions.standingOnPassThrough = true;
            }
        }

        return true;
    }

    public struct CollisionInfo {
        public bool standingOnPassThrough;
        public bool above, below;
        public bool left, right;
        public int movementDirX;

        public void Reset () {
            above = below = false;
            left = right = false;
            standingOnPassThrough = false;
        }
    }

    private struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

}
