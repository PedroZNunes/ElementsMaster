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

    float wallSlideSpeedMax = 3;

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


        bool wallSliding = false;
        if (( !controller.collisions.below && (controller.collisions.left || controller.collisions.right ) && velocity.y < 0 )) {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }
        }

            if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal") , Input.GetAxisRaw ("Vertical"));

        if (Input.GetButtonDown ("Jump") && controller.collisions.below) {
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp (velocity.x , targetVelocityX , ref velocityXSmoothing , ( controller.collisions.below ) ? accelerationTimeGrounded : accelerationTimeAirBorne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move (velocity * Time.deltaTime);
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
