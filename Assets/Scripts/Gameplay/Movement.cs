using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Movement : MonoBehaviour {

    [SerializeField]
    Jump jump;
    [SerializeField]
    Wall wall;

    bool isWallSliding;
    int wallDirX;
    float gravity;
    float smoothVelocityX;

    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float accelerationTimeAirBorne = 0.4f;
    [SerializeField]
    float accelerationTimeGrounded = 0.1f;

    [SerializeField]
    float rayDeactivateTime = 0.2f;

    float wallUnstickTime;
    Controller2D controller;
    Vector3 velocity;

    Vector2 directionalInput;

    void Awake () {
        controller = GetComponent<Controller2D> ();
    }

    void Start () { 
        CalculateGravity ();

        CalculateJumpVelocity ();

        Debug.Log (string.Format ("Gravity: {0} | Jump Velocity: {1}" , gravity , jump.velocityMax));
    }


    void Update () {
        isWallSliding = false;
        wallDirX = ( controller.collisions.right ) ? 1 : -1;

        HandleMovement ();

        HandleWallSlide ();

        velocity.y += gravity * Time.deltaTime;
        controller.Move (velocity * Time.deltaTime);

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

    }

    public void SetDirectionalInput(Vector2 input ) {
        directionalInput = input;
    }

    public void HandleMovement () {

        float targetVelocityX = directionalInput.x * moveSpeed;
        float accelerationTime;

        if (targetVelocityX != 0) {//if moving
            accelerationTime = ( controller.collisions.below ) ? accelerationTimeGrounded : accelerationTimeAirBorne;
        }
        else { //if stoping
            accelerationTime = ( controller.collisions.below ) ? (accelerationTimeGrounded / 2f) : (accelerationTimeAirBorne / 2f);
        }

        velocity.x = Mathf.SmoothDamp (velocity.x , targetVelocityX , ref smoothVelocityX , accelerationTime);
    }

    public void HandleWallSlide () {
        if (( !controller.collisions.below && ( controller.collisions.left || controller.collisions.right ) && velocity.y < 0 )) {
            isWallSliding = true;

            if (velocity.y < -wall.slideSpeedMax) {
                velocity.y = -wall.slideSpeedMax;
            }

            if (wallUnstickTime > 0) {
                smoothVelocityX = 0;
                velocity.x = 0;
                if (directionalInput.x != wallDirX) {
                    wallUnstickTime -= Time.deltaTime;
                }
                else {
                    wallUnstickTime = wall.stickTime;
                }
            }
            else {
                wallUnstickTime = wall.stickTime;
            }
        }
    }

    public void HandleJump () {
        if (directionalInput.y == -1 && controller.collisions.standingOnPassThrough) {
            HandleFallThrough ();
        }
        else {
            if (isWallSliding) {
                if (wallDirX == directionalInput.x) {
                    Debug.Log ("Climbing");
                    velocity.x = -wallDirX * wall.climb.x;
                    velocity.y = wall.climb.y;
                }
                else if (directionalInput.x == 0) {
                    Debug.Log ("Jumping Off");
                    velocity.x = -wallDirX * wall.jumpOff.x;
                    velocity.y = wall.jumpOff.y;
                }
                else {
                    Debug.Log ("Leaping Away");
                    velocity.x = -wallDirX * wall.leap.x;
                    velocity.y = wall.leap.y;
                }
            }
            else if (controller.collisions.below) {
                velocity.y = jump.velocityMax;
                StartCoroutine (CountJumpLength ());
            }
        }

    }

    public void HandleCancelJump () {
        if (velocity.y > jump.velocityMin) {
            velocity.y = jump.velocityMin;
        }
    }

    public void HandleFallThrough () {
        controller.DeactivateRays (rayDeactivateTime);
    }

    IEnumerator CountJumpLength () {
        float timeCount = 0f;
        float x1 = transform.position.x;
        timeCount += Time.deltaTime;
        yield return null;
        while (!controller.collisions.below) {
            timeCount += Time.deltaTime;
            yield return null;
        }
        float x2 = transform.position.x;
        print (string.Format("Jump - Length: {0:0.000} || Duration {1:0.000}", ( x2 - x1 ),timeCount));
    }

    //deltaMovement = V0 * t + (a(t^2))/2
    //jumpHeight = (gravity * timeToJumpApex^2)/2
    //Solving for gravity
    //gravity = 2*jumpHeight / timeToJumpApex^2
    void CalculateGravity () {
        float halfHeight = controller.collider.size.y / 2f;
        gravity = -( 2 * ( jump.heightMax + halfHeight ) ) / Mathf.Pow (jump.timeToApex , 2);
    }

    //V = V0 + at
    //jumpVelocity = gravity*timeToJumpApex
    void CalculateJumpVelocity () {
        jump.velocityMax = Mathf.Abs (gravity) * jump.timeToApex;
        jump.velocityMin = Mathf.Sqrt (2 * Mathf.Abs (gravity) * jump.heightMin);
    }

    [System.Serializable]
    struct Jump {
        public float heightMin;
        public float heightMax;
        public float timeToApex;
        [HideInInspector]
        public float velocityMax;
        [HideInInspector]
        public float velocityMin;
    }

    [System.Serializable]
    struct Wall {
        public float stickTime;
        public Vector2 jumpOff, leap, climb;
        public float slideSpeedMax;
    }


}
