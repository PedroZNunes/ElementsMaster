﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {
    

    [SerializeField]
    Jump jump;
    [SerializeField]
    Wall wall;

    [SerializeField]
    float moveSpeed = 6f;
    [SerializeField]
    float accelerationTimeAirBorne = 0.4f;
    [SerializeField]
    float accelerationTimeGrounded = 0.1f;
    [SerializeField]
    float deactivateRaysTime = 0.2f;

    float wallUnstickTime;

    float gravity;
    float smoothVelocityX; //smoothDamp
    Vector3 velocity;

    Controller2D controller;
	
	void Start () {
        controller = GetComponent<Controller2D> ();

        CalculateGravity (ref gravity);

        CalculateJumpVelocity (ref jump.velocityMax, ref jump.velocityMin);


        Debug.Log (string.Format ("Gravity: {0} | Jump Velocity: {1}" , gravity , jump.velocityMax));
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
            velocity.x = Mathf.SmoothDamp (velocity.x , targetVelocityX , ref smoothVelocityX , ( controller.collisions.below ) ? accelerationTimeGrounded : accelerationTimeAirBorne);
        }
        else {
            velocity.x = Mathf.SmoothDamp (velocity.x , targetVelocityX , ref smoothVelocityX , ( controller.collisions.below ) ? accelerationTimeGrounded / 2 : accelerationTimeAirBorne / 2);
        }
        
    }

    void HandleWallSliding (ref bool wallSliding, ref Vector2 input, ref int wallDirX) {
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
                    Debug.Log ("Jump");
                    velocity.y = jump.velocityMax;
                }
            }
        }

        if (Input.GetButtonUp ("Jump")) {
            if (velocity.y > jump.velocityMin) {
                velocity.y = jump.velocityMin;
            }
        }

    }

    //deltaMovement = V0 * t + (a(t^2))/2
    //jumpHeight = (gravity * timeToJumpApex^2)/2
    //Solving for gravity
    //gravity = 2*jumpHeight / timeToJumpApex^2
    void CalculateGravity ( ref float gravity ) {
        float halfHeight = controller.collider.size.y / 2f;
        gravity = -(2 * (jump.heightMax + halfHeight)) / Mathf.Pow(jump.timeToApex,2);
    }

    //V = V0 + at
    //jumpVelocity = gravity*timeToJumpApex
    void CalculateJumpVelocity ( ref float jumpVelocityMax, ref float jumpVelocityMin ) {
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
