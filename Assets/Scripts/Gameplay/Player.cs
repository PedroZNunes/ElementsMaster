using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {
    [SerializeField]
    float jumpHeightMax = 4;
    [SerializeField]
    float jumpHeightMin = 1;
    [SerializeField]
    float timeToJumpApex = .4f; //time to reach the apex
    [SerializeField]
    float moveSpeed = 6f;
    [SerializeField]
    float accelerationTimeAirBorne = 0.4f;
    [SerializeField]
    float accelerationTimeGrounded = 0.1f;
    [SerializeField]
    Vector2 wallJumpClimb, wallJumpOff, wallLeap;
    [SerializeField]
    float wallSlideSpeedMax = 3;
    [SerializeField]
    float wallStickTime = 0.25f;
    [SerializeField]
    float deactivateRaysTime = 0.2f;

    float wallUnstickTime;

    float jumpVelocityMax;
    float jumpVelocityMin;
    float gravity;
    float velocityXSmoothing; //smoothDamp
    Vector3 velocity;


    Controller2D controller;
	
	void Start () {
        controller = GetComponent<Controller2D> ();

        CalculateGravity (ref gravity);

        CalculateJumpVelocity (ref jumpVelocityMax, ref jumpVelocityMin);


        Debug.Log (string.Format ("Gravity: {0} | Jump Velocity: {1}" , gravity , jumpVelocityMax));
	}

    void Update () { //TODO: REFACTOR THE CODE DOWN BELOW IN A SEPARATE CLASS THAT HANDLES PLAYER AI. THEN ADD THIS AS COMPONENT AND GET THE COMPONENT AT AWAKE(); ALSO PUTS CLASS AS REQUIRED COMPONENT
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal") , Input.GetAxisRaw ("Vertical"));

        HandleAxisInput (ref velocity , input);

        int wallDirX = ( controller.collisions.right ) ? 1 : -1;

        bool wallSliding = false;
        HandleWallSliding (ref wallSliding , ref input , ref wallDirX);

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }


        HandleJump (ref wallSliding , ref input , ref wallDirX);

        velocity.y += gravity * Time.deltaTime;
        controller.Move (velocity * Time.deltaTime);
    }
    



    void HandleAxisInput (ref Vector3 velocity, Vector2 input) {
        
        float targetVelocityX = input.x * moveSpeed;
        if (targetVelocityX != 0) {
            velocity.x = Mathf.SmoothDamp (velocity.x , targetVelocityX , ref velocityXSmoothing , ( controller.collisions.below ) ? accelerationTimeGrounded : accelerationTimeAirBorne);
        }
        else {
            velocity.x = Mathf.SmoothDamp (velocity.x , targetVelocityX , ref velocityXSmoothing , ( controller.collisions.below ) ? accelerationTimeGrounded / 2 : accelerationTimeAirBorne / 2);
        }
        
    }

    void HandleWallSliding (ref bool wallSliding, ref Vector2 input, ref int wallDirX) {
        if (( !controller.collisions.below && ( controller.collisions.left || controller.collisions.right ) && velocity.y < 0 )) {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }

            if (wallUnstickTime > 0) {
                velocityXSmoothing = 0;
                velocity.x = 0;
                if (input.x != wallDirX) {
                    wallUnstickTime -= Time.deltaTime;
                }
                else {
                    wallUnstickTime = wallStickTime;
                }
            }
            else {
                wallUnstickTime = wallStickTime;
            }
        }
    }

    void HandleJump ( ref bool wallSliding , ref Vector2 input , ref int wallDirX ) {
        if (Input.GetButtonDown ("Jump")) {
            if (input.y == -1 && controller.collisions.standingOnPassThrough) {
                //FallThrough
                controller.DeactivateRays (deactivateRaysTime);
            }
            else {
                Debug.Log ("Is it wallSliding? " + wallSliding);
                Debug.Log ("Is it colliding with the floor? " + controller.collisions.below);
                if (wallSliding) {
                    if (wallDirX == input.x) {
                        Debug.Log ("Climbing");
                        velocity.x = -wallDirX * wallJumpClimb.x;
                        velocity.y = wallJumpClimb.y;
                    }
                    else if (input.x == 0) {
                        Debug.Log ("Jumping Off");
                        velocity.x = -wallDirX * wallJumpOff.x;
                        velocity.y = wallJumpOff.y;
                    }
                    else {
                        Debug.Log ("Leaping Away");
                        velocity.x = -wallDirX * wallLeap.x;
                        velocity.y = wallLeap.y;
                    }
                }
                else if (controller.collisions.below) {
                    Debug.Log ("Jump");
                    velocity.y = jumpVelocityMax;
                }
            }
        }

        if (Input.GetButtonUp ("Jump")) {
            if (velocity.y > jumpVelocityMin) {
                velocity.y = jumpVelocityMin;
            }
        }

    }

    //deltaMovement = V0 * t + (a(t^2))/2
    //jumpHeight = (gravity * timeToJumpApex^2)/2
    //Solving for gravity
    //gravity = 2*jumpHeight / timeToJumpApex^2
    void CalculateGravity ( ref float gravity ) {
        float halfHeight = controller.collider.size.y / 2f;
        gravity = -(2 * (jumpHeightMax + halfHeight)) / Mathf.Pow(timeToJumpApex,2);
    }

    //V = V0 + at
    //jumpVelocity = gravity*timeToJumpApex
    void CalculateJumpVelocity ( ref float jumpVelocityMax, ref float jumpVelocityMin ) {
        jumpVelocityMax = Mathf.Abs (gravity) * timeToJumpApex;
        jumpVelocityMin = Mathf.Sqrt (2 * Mathf.Abs (gravity) * jumpHeightMin);
    }

}
