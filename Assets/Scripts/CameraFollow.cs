using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [SerializeField]
    float verticalOffset;
    [SerializeField]
    Controller2D target;
    [SerializeField]
    Vector2 focusAreaSize;

    FocusArea focusArea;
    Coroutine cameraIdling;

    [SerializeField]
    LookAhead lookAhead;
    [SerializeField]
    Vector2 smoothTimeIdle;
    [SerializeField]
    Vector2 smoothTimeMoving;
    [SerializeField]
    float smoothChangeTime;
    Vector2 smoothChangeVelocity;

    float smoothMaxSpeed = 3f;
    bool isFocusMoving = false;
    Vector2 currentSmoothTime;
    Vector2 smoothVelocity;

    void Start () {
        focusArea = new FocusArea (target.collider.bounds , focusAreaSize);
        currentSmoothTime = smoothTimeMoving;
    }


    void LateUpdate () {
        focusArea.Update (target.collider.bounds);
        isFocusMoving = ( Vector3.Magnitude (focusArea.velocity) != 0 );

        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;
        Vector2 targetSmoothTime = ( isFocusMoving ) ? smoothTimeMoving : smoothTimeIdle;
        
        if (isFocusMoving )
        lookAhead.directionX = ( focusArea.velocity.x != 0 ) ? Mathf.Sign (focusArea.velocity.x) : lookAhead.directionX;

        if (targetSmoothTime != currentSmoothTime) {
            currentSmoothTime = Vector2.SmoothDamp (currentSmoothTime , targetSmoothTime , ref smoothChangeVelocity , smoothChangeTime , smoothMaxSpeed , Time.deltaTime);
        }

        lookAhead.targetX = lookAhead.directionX * lookAhead.distanceX;
        lookAhead.currentX = Mathf.SmoothDamp (lookAhead.currentX , lookAhead.targetX , ref smoothVelocity.x , currentSmoothTime.x);

        focusPosition.y = Mathf.SmoothDamp (transform.position.y , focusPosition.y , ref smoothVelocity.y , currentSmoothTime.y);

        focusPosition += Vector2.right * lookAhead.currentX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
        
    }

    void OnDrawGizmos () {
        Gizmos.color = new Color (0 , 1 , 1 , 0.4f);
        Gizmos.DrawCube (focusArea.center , focusAreaSize);
    }

    struct FocusArea {
        public Vector2 center;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea ( Bounds bounds, Vector2 size ) {
            left = bounds.center.x - (size.x / 2);
            right = bounds.center.x + (size.x / 2);
            bottom = bounds.min.y;
            top = bounds.min.y + size.y;

            velocity = Vector2.zero;
            center = new Vector2 (( left + right ) / 2 , ( top + bottom ) / 2 );
            print ("Center at Focus Area constructor: " + center);
        }

        public void Update ( Bounds boxBounds ) {
            float shiftX = 0;
            float shiftY = 0;

            CalculateShiftAmounts (boxBounds, ref shiftX, ref shiftY);

            left += shiftX;
            right += shiftX;
            bottom += shiftY;
            top += shiftY;

            center = new Vector2 (( left + right ) / 2 , ( top + bottom) / 2 );
            velocity = new Vector2 (shiftX , shiftY);
            //print (string.Format ("left: {0}, right: {1}, bottom {2}, top {3}, center: {4}" , left , right , bottom , top, center));
            
        }

        private void CalculateShiftAmounts ( Bounds bounds , ref float shiftX, ref float shiftY) {
            //Debug.Log ("Bounds : " + bounds);
            if (bounds.min.x < left) {
                shiftX = bounds.min.x - left;
            }
            else if (bounds.max.x > right) {
                shiftX = bounds.max.x - right;
            }

            if (bounds.min.y < bottom) {
                shiftY = bounds.min.y - bottom;
            }
            else if (bounds.max.y > top) {
                shiftY = bounds.max.y - top;
            }
        }
    }
    [System.Serializable]
    struct LookAhead {
        [HideInInspector]
        public float currentX;
        [HideInInspector]
        public float targetX;
        [HideInInspector]
        public float directionX;
        
        public float distanceX;
        Vector2 velocity;
    }

}
