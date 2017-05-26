using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAI : MonoBehaviour {

    public enum State { Spawning, Roaming, Chasing, Attacking, Dying}
    public State currentState;

    [SerializeField]
    private LayerMask collisionLayerMask;
    [SerializeField]
    private LayerMask aggroLayerMask;

    [SerializeField]
    private Vector2 aggroBoxSize;
    [SerializeField]
    private int checkAggroInterval = 4;

    [SerializeField]
    private float forwardRayDistance = 5f;
    [SerializeField]
    private float downwardRayDistance = 4.9f;

    private int raySpacing = 1;
    private float raySkinWidth = 0.1f;

    private Vector2 targetPos;

    private const float minJumpDistance = 0.2f;
    private const float maxHeightJumpDistance = 0.46525f;
    private int playerDirection = 1;
    private bool isJumping;
    private bool fullLengthJump;
    private Coroutine checkingAggro;

    private Enemy enemy;
    private Movement movement;
    private Player player;
    private Controller2D controller;
    private BoxCollider2D col;


    private void Awake () {
        enemy = GetComponent<Enemy> ();
        movement = GetComponent<Movement> ();
        controller = GetComponent<Controller2D>();

        col = GetComponent<BoxCollider2D> ();
        player = FindObjectOfType<Player> ();
    }

    private void Update () {
        targetPos = player.transform.position;

        playerDirection = ( targetPos.x - transform.position.x < 0f ) ? -1 : 1;

        RunStateBehaviour ();
    }

    private void OnDrawGizmos () {
        Gizmos.DrawWireCube (transform.position , aggroBoxSize);
    }

    private void HandleSpawning () {
        //spawningAnimation
        //by the end of the animation, trigger roaming
        Trigger (State.Roaming);
    }
    
    private void HandleRoaming () {
        //walk around
    }

    private IEnumerator CheckAggro () {
        WaitForSeconds waitInterval = new WaitForSeconds (checkAggroInterval);

        while (gameObject.activeSelf) {
            Debug.Log ("Checking aggro.");
            Collider2D hit = Physics2D.OverlapBox (transform.position, aggroBoxSize , 0f , aggroLayerMask);
            if (hit != null) {
                //if unreachable, check the player pos against the 
                Debug.Log ("Found the player. " + hit.name);
                Trigger (State.Chasing);
            }
            else {
                Debug.Log ("Found nothing.");
                Trigger (State.Roaming);
            }
            yield return waitInterval;
        }
    }

    private void HandleChasing () {
        //running animation

        int moveDirection = playerDirection;
        //ground is a face with normal poiting upwards
        //Dealing with Jumps
        if (controller.Collisions.below) {
            // test if there's ground ahead
            //I'll cast a few rays downward to check for ground ahead.
            Vector2 rayOrigin = new Vector2 (); //= new Vector2 (transform.position.x + ( ( col.size.x / 2 ) * moveDirection ) , transform.position.y + ( col.size.y / 2 ));
            Vector2 rayDirection = Vector2.down; // = new Vector2 (2 * moveDirection , -1);
            rayOrigin = new Vector2 (transform.position.x + moveDirection , transform.position.y);
            Debug.DrawRay (rayOrigin , rayDirection * downwardRayDistance , Color.yellow);
            RaycastHit2D hit = Physics2D.Raycast (rayOrigin , rayDirection , downwardRayDistance , collisionLayerMask);
            print (hit.distance);
            if (hit && hit.normal == Vector2.up) {

                if (hit.distance > col.size.y) {
                    //Hole ahead.
                    //no ground ahead;
                    //Check the size of the hole.
                    //cast um ray pro lado do enemy pra ver aonde começa o bloco mais proximo.
                    bool isJumpable = false;
                    float jumpVelocity = 0f; //==========================================================================
                    rayDirection = new Vector2 (-moveDirection , 0);
                    forwardRayDistance = 2f;
                    hit = Physics2D.Raycast (rayOrigin , rayDirection , forwardRayDistance , collisionLayerMask);
                    if (hit) {
                        //found the block closest to the hole. now lets try and jump it from the edge.
                        rayOrigin = new Vector2 (hit.collider.bounds.center.x + ( hit.collider.bounds.extents.x * moveDirection ) , hit.collider.bounds.max.y);

                        for (jumpVelocity = movement.JumpVelocityMin ; jumpVelocity <= movement.JumpVelocityMax ; jumpVelocity++) {
                            if (movement.JumpVelocityMax - jumpVelocity < 1) {
                                jumpVelocity = movement.JumpVelocityMax;
                            }
                            if (TryJumping (movement.MoveSpeed * moveDirection , jumpVelocity)) {
                                isJumpable = true;
                                break;
                            }
                        }
                    }
                    //Trying to jump over the hole
                    if (isJumpable) {
                        if (TryJumping (movement.MoveSpeed * playerDirection , jumpVelocity)) {
                            movement.HandleJump (jumpVelocity);
                        }
                    }
                    else {
                        Trigger (State.Roaming);
                    }
                }
                else {
                    //Can move. Ground Ahead.

                    //Casting rays forward from the center.
                    rayOrigin = new Vector2 (transform.position.x + ( ( col.size.x / 2 ) * moveDirection ) , transform.position.y);
                    rayDirection = Vector2.right * moveDirection;
                    Debug.DrawRay (rayOrigin , rayDirection , Color.green);
                    hit = Physics2D.Raycast (rayOrigin , rayDirection , forwardRayDistance , collisionLayerMask);
                    if (hit) {
                        //Block ahead.

                        //Testing if there's enough jump space.
                        if (hit.distance > minJumpDistance) {
                            //Try to jump over it in all possible jump velocitys
                            for (float jumpVelocity = movement.JumpVelocityMin ; jumpVelocity <= movement.JumpVelocityMax ; jumpVelocity++) {
                                //Making sure that he tries the max jump velocity before giving up
                                if (movement.JumpVelocityMax - jumpVelocity < 1) {
                                    jumpVelocity = movement.JumpVelocityMax;
                                }
                                //Tries to jump at said move speed and jump velocity.
                                if (TryJumping (movement.MoveSpeed * playerDirection , jumpVelocity)) {
                                    movement.HandleJump (jumpVelocity);
                                    break;
                                }
                            }
                            //Currently I am not checking if the enemy was able to jump over the hole or not.
                        }
                        else {
                            moveDirection = -moveDirection;
                        }

                    }
                    else {
                        //no blocks ahead.
                    }

                }
            }

            //stuck
            if (controller.Collisions.below && controller.Collisions.right && controller.Collisions.left) {
                if (TryJumping (movement.MoveSpeed * playerDirection , movement.JumpVelocityMax)) {
                    movement.HandleJump (movement.JumpVelocityMax);
                }
            }
        }
        else {
            //enemy not grounded.
        }
        movement.SetDirectionalInput (Vector2.right * moveDirection);
    }

    private bool TryJumping (float targetVelocityX , float jumpVelocity) {
        //se wall distance < (distancia minima para conseguir um pulo sem esbarrar na parede), pule baixo e desacelerando
        if (movement.Velocity.x == 0 && targetVelocityX == 0) {
            return false;
        }

        bool doJump = false;

        Vector2 origin = new Vector2 (transform.position.x + ( ( col.size.x / 2 )  ) * playerDirection , transform.position.y - (( col.size.y / 2 ) - raySkinWidth));
        
        Vector2 startingVelocity = new Vector2 (movement.Velocity.x , jumpVelocity);
        Vector2 currentVelocity = startingVelocity;

        float smoothVelocityX = startingVelocity.x;
        //bent ray
        for (int rayCount = 0 ; rayCount < 100 ; rayCount++) {
            float acceleration = ( rayCount == 0 ) ? movement.AccelerationTimeGrounded : movement.AccelerationTimeAirBorne;

            if (targetVelocityX != startingVelocity.x) {
                currentVelocity.x = Mathf.SmoothDamp (currentVelocity.x , targetVelocityX , ref smoothVelocityX , acceleration);
            }
            currentVelocity.y += Movement.Gravity * Time.fixedDeltaTime;

            Vector2 direction = currentVelocity.normalized;
            float distance = currentVelocity.magnitude * Time.fixedDeltaTime;
            
            RaycastHit2D hit = Physics2D.Raycast (origin , direction , distance , collisionLayerMask);
            Debug.DrawRay (origin , direction * distance , Color.cyan, 2f);
            if (hit) {
                //testar se ele acertou subindo ou descendo.
                if ((hit.normal == Vector2.up)) {
                    doJump = true;
                }
                break;
            }

            origin = origin + (direction * distance);
        }
        return doJump;
    }

    private void HandleAttacking () {
        //attacking animation

        //this happens if the distance to the player < attack range
        //does the animation
        //if the attack hits (collider?)
        //deal damage
    }

    private void HandleDying () {
        //dying animation
        //should take the collider away in the first fra;me and deactivate any kind of outside influence

        //unsubscribes from events (on disable could do this)
        //
    }

    public void Trigger (State newState ) {
        if (newState != currentState) {
            currentState = newState;
            switch (currentState) {
                case State.Spawning:
                    break;
                case State.Roaming:
                    if (checkingAggro != null) {
                        StopCoroutine (checkingAggro);
                    }
                    checkingAggro = StartCoroutine (CheckAggro ());
                    break;
                case State.Chasing:
                    break;
                case State.Attacking:
                    break;
                case State.Dying:
                    break;
                default:
                    break;
            }
            
            Debug.Log (currentState);
        }
    }

    private void RunStateBehaviour () {
        switch (currentState) {
            case State.Spawning:
                HandleSpawning ();
                break;
            case State.Roaming:
                HandleRoaming ();
                break;
            case State.Chasing:
                HandleChasing ();
                break;
            case State.Attacking:
                HandleAttacking ();
                break;
            case State.Dying:
                HandleDying ();
                break;
            default:
                break;
        }
    }

}
