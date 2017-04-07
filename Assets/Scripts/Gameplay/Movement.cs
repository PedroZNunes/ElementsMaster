using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Movement : MonoBehaviour {

    [SerializeField]
    Jump jump;
    [SerializeField]
    Wall wall;

    [SerializeField]
    float moveSpeed;
    
    float gravity;
    float smoothVelocityX;

    [SerializeField]
    float accelerationTimeAirBorne = 0.4f;
    [SerializeField]
    float accelerationTimeGrounded = 0.1f;

    [SerializeField]
    float deactivateRaysTime = 0.2f;

    float wallUnstickTime;
    Controller2D controller;
    Vector3 velocity;

    void Awake () {
        controller = GetComponent<Controller2D> ();
    }

    void Start () { 
        CalculateGravity (ref gravity);

        CalculateJumpVelocity (ref jump.velocityMax , ref jump.velocityMin);

        Debug.Log (string.Format ("Gravity: {0} | Jump Velocity: {1}" , gravity , jump.velocityMax));
    }

    void Update () {
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal") , Input.GetAxisRaw ("Vertical"));
        
        HandleMovement (ref velocity , input);

        int wallDirX = ( controller.collisions.right ) ? 1 : -1;
        bool wallSliding = false;

        HandleWallSlide (ref wallSliding , input , wallDirX);

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        HandleJump (wallSliding , input , wallDirX);

        velocity.y += gravity * Time.deltaTime;
        controller.Move (velocity * Time.deltaTime);
    }

    public void HandleMovement ( ref Vector3 velocity , Vector2 input ) {

        float targetVelocityX = input.x * moveSpeed;
        float accelerationTime;

        if (targetVelocityX != 0) {//if moving
            accelerationTime = ( controller.collisions.below ) ? accelerationTimeGrounded : accelerationTimeAirBorne;
        }
        else { //if stoping
            accelerationTime = ( controller.collisions.below ) ? (accelerationTimeGrounded / 2f) : (accelerationTimeAirBorne / 2f);
        }

        velocity.x = Mathf.SmoothDamp (velocity.x , targetVelocityX , ref smoothVelocityX , accelerationTime);
    }

    public void HandleWallSlide ( ref bool wallSliding , Vector2 input , int wallDirX ) {
        if (( !controller.collisions.below && ( controller.collisions.left || controller.collisions.right ) && velocity.y < 0 )) {
            wallSliding = true;

            if (velocity.y < -wall.slideSpeedMax) {
                velocity.y = -wall.slideSpeedMax;
            }

            if (wallUnstickTime > 0) {
                smoothVelocityX = 0;
                velocity.x = 0;
                if (input.x != wallDirX) {
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

    void HandleJump ( bool wallSliding , Vector2 input , int wallDirX ) {
        if (Input.GetButtonDown ("Jump")) {
            if (input.y == -1 && controller.collisions.standingOnPassThrough) {
                //FallThrough
                controller.DeactivateRays (deactivateRaysTime);
            }
            else {
                if (wallSliding) {
                    if (wallDirX == input.x) {
                        Debug.Log ("Climbing");
                        velocity.x = -wallDirX * wall.climb.x;
                        velocity.y = wall.climb.y;
                    }
                    else if (input.x == 0) {
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
                    StartCoroutine (CountJumpLenght ());
                }
            }
        }

        if (Input.GetButtonUp ("Jump")) {
            if (velocity.y > jump.velocityMin) {
                velocity.y = jump.velocityMin;
            }
        }

    }

    IEnumerator CountJumpLenght () {
        float timeCount = 0f;
        float x1 = transform.position.x;
        yield return null;
        while (!controller.collisions.below) {
            timeCount += Time.deltaTime;
            yield return null;
        }
        float x2 = transform.position.x;
        print ("Lenght: " + ( x2 - x1 ));
    }

    //deltaMovement = V0 * t + (a(t^2))/2
    //jumpHeight = (gravity * timeToJumpApex^2)/2
    //Solving for gravity
    //gravity = 2*jumpHeight / timeToJumpApex^2
    void CalculateGravity ( ref float gravity ) {
        float halfHeight = controller.collider.size.y / 2f;
        gravity = -( 2 * ( jump.heightMax + halfHeight ) ) / Mathf.Pow (jump.timeToApex , 2);
    }

    //V = V0 + at
    //jumpVelocity = gravity*timeToJumpApex
    void CalculateJumpVelocity ( ref float jumpVelocityMax , ref float jumpVelocityMin ) {
        jumpVelocityMax = Mathf.Abs (gravity) * jump.timeToApex;
        jumpVelocityMin = Mathf.Sqrt (2 * Mathf.Abs (gravity) * jump.heightMin);
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
