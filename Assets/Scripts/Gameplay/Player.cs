using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {
    [SerializeField]
    float jumpHeight = 4;
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

    float wallUnstickTime;

    float jumpVelocity;
    float gravity;
    float velocityXSmoothing; //smoothDamp
    Vector3 velocity;


    Controller2D controller;
	
	void Start () {
        controller = GetComponent<Controller2D> ();

        CalculateGravity (ref gravity);

        CalculateJumpVelocity (ref jumpVelocity);

        Debug.Log (string.Format ("Gravity: {0} | Jump Velocity: {1}" , gravity , jumpVelocity));
	}
	
	void Update () {
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal") , Input.GetAxisRaw ("Vertical"));

        HandleAxisInput (ref velocity, input);

        int wallDirX = ( controller.collisions.right ) ? 1 : -1;

        bool wallSliding = false;
        if (( !controller.collisions.below && (controller.collisions.left || controller.collisions.right ) && velocity.y < 0 )) {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }

            if (wallUnstickTime > 0) {
                velocityXSmoothing = 0;
                velocity.x = 0;
                if (input.x != wallDirX ) {
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

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        if (Input.GetButtonDown ("Jump")) {
            Debug.Log ("Is it wallSliding? " + wallSliding);
            Debug.Log ("Jump!! Is it colliding with the floor? " + controller.collisions.below);
            if (wallSliding) {
                if (wallDirX == input.x) {
                    Debug.Log ("Trying to climb!");
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                else if (input.x == 0) {
                    Debug.Log ("Jumping Off!");
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else {
                    Debug.Log ("Leaping Away!");
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            else if (controller.collisions.below) {
                Debug.Log ("Jump!!");
                velocity.y = jumpVelocity;
            }
            
        }
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


    //deltaMovement = V0 * t + (a(t^2))/2
    //jumpHeight = (gravity * timeToJumpApex^2)/2
    //Solving for gravity
    //gravity = 2*jumpHeight / timeToJumpApex^2
    void CalculateGravity ( ref float gravity ) {
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex,2);
    }

    //V = V0 + at
    //jumpVelocity = gravity*timeToJumpApex
    void CalculateJumpVelocity ( ref float jumpVelocity ) {
        jumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
    }

}
