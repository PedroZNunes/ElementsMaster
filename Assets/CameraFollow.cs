using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    enum CameraState {Moving, Idle};
    CameraState cameraState;

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
    Smooth smoothIdle;
    [SerializeField]
    Smooth smoothMoving;
    [SerializeField]
    float idleSeedMin, idleSeedMax;
    [SerializeField]
    float smoothChangingTime;
    
    Smooth currentSmooth;

    void Start () {
        focusArea = new FocusArea (target.collider.bounds , focusAreaSize);
        cameraState = CameraState.Idle;
    }


    void LateUpdate () {
        focusArea.Update (target.collider.bounds);
        if (Vector3.Magnitude (focusArea.velocity) == 0f && cameraState != CameraState.Idle) {
            cameraState = CameraState.Idle;
        }
        else if (Vector3.Magnitude (focusArea.velocity) > 0f && cameraState != CameraState.Moving) {
            cameraState = CameraState.Moving;
        }
        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        CalculateMovement (ref focusPosition);
        
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
        
    }

    void HandleCameraMoving (ref Vector2 focusPosition) {
        Debug.Log ("Camera Moving");

        lookAhead.directionX = Mathf.Sign (focusArea.velocity.x);
        lookAhead.targetX = lookAhead.directionX * lookAhead.distanceX;
        lookAhead.currentX = Mathf.SmoothDamp (lookAhead.currentX , lookAhead.targetX , ref smoothMoving.smoothVelocity.x , smoothMoving.smoothTime.x);

        focusPosition += Vector2.right * lookAhead.currentX;
    }

    void HandleCameraIdle ( ref Vector2 focusPosition ) {
        Debug.Log ("Camera Idle");

        lookAhead.currentX = Mathf.SmoothDamp (lookAhead.currentX , lookAhead.targetX , ref smoothIdle.smoothVelocity.x , smoothIdle.smoothTime.x);

        focusPosition += Vector2.right * lookAhead.currentX;
    }

    void CalculateMovement (ref Vector2 focusPosition) {
        switch (cameraState) {
            case CameraState.Moving:
                HandleCameraMoving (ref focusPosition);
                break;
            case CameraState.Idle:
                HandleCameraIdle (ref focusPosition);
                break;
            default:
                cameraState = CameraState.Idle;
                break;
        }
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

    [System.Serializable]
    struct Smooth {
        [HideInInspector]
        public Vector2 smoothVelocity;
        public Vector2 smoothTime;
    }
}
