using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAI : MonoBehaviour {

    public enum State { Spawning, Idle, Searching, Chasing, Attacking, Dying}
    public State currentState;

    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float aggroDistance = 20f;

    [SerializeField]
    private float rayDistance = 5f;
    private int raySpacing = 1;
    private float raySkin = 0.1f;

    private Vector2 targetPos;
    
    private const float maxHeightJumpDistance = 0.46525f;
    private int playerDirection = 1;
    private bool isJumping;
    private bool fullLengthJump;
    
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

    private void HandleSpawning () {
        //spawningAnimation
    }

    private void HandleIdle () {
        //idle animation
    }

    private void HandleSearching () {
        //idle animation
            float distance = Vector2.Distance (targetPos , transform.position);

            if (Mathf.Abs (distance) < aggroDistance)  {
                Trigger (State.Chasing);
            }
        //send to pathfinder for a path towards that location
        //wait until it finds something.
    }

    private void HandleChasing () {
        //running animation
        int moveDirection = playerDirection;
        //ground is a face with normal poiting upwards
        //Dealing with Jumps
        if (controller.Collisions.below) {
            // test if there's ground ahead
            //Casting a ray in a diagonal to check for upcoming holes. this is weird.
            Vector2 rayOrigin = new Vector2 (transform.position.x + ( ( col.size.x / 2 ) * moveDirection ) , transform.position.y + ( col.size.y / 2 ));
            Vector2 rayDirection = new Vector2 (2 * moveDirection , -1);
            //float groundRayDistance = 10f;
            //Debug.DrawRay (rayOrigin , rayDirection * groundRayDistance , Color.red);
            //RaycastHit2D groundHit = Physics2D.OverlapPoint (transform.position , rayDirection , groundRayDistance , layerMask);
            Vector2 pointOffset = new Vector2 (2 , -1);
            Vector2 point = new Vector2 (transform.position.x + pointOffset.x , transform.position.y - pointOffset.y);
            Collider2D pointCol = Physics2D.OverlapPoint (point , layerMask);

            //Can move. Ground Ahead.
            //if (groundHit && groundHit.normal == Vector2.up) {
            if (pointCol != null) {
                //test if there's a block blocking the way.
                //Casting rays forward from the center.
                rayOrigin -= Vector2.up * ( col.size.y / 2 );
                rayDirection = Vector2.right * moveDirection;
                Debug.DrawRay (rayOrigin , rayDirection , Color.green);
                RaycastHit2D hit = Physics2D.Raycast (rayOrigin , rayDirection , rayDistance , layerMask);
                if (hit) {
                    //Block ahead.
                    //Trying to jump over it.
                    for (float jumpVelocity = movement.JumpVelocityMin ; jumpVelocity <= movement.JumpVelocityMax ; jumpVelocity++) {
                        //Making sure that he tries the max jump length
                        if (movement.JumpVelocityMax - jumpVelocity < 1) {
                            jumpVelocity = movement.JumpVelocityMax;
                        }

                        if (TryJumping (movement.MoveSpeed * playerDirection, jumpVelocity)) {
                            movement.HandleJump (jumpVelocity);
                            break;
                        }
                    }
                    //if he does not succeed in jumping over the block, we just move forward
                    //We need to check if the block is too close for max height jumping
                    if (hit.distance < maxHeightJumpDistance) {
                        moveDirection = -playerDirection;
                    }
                }
                //no blocks ahead.
            }
            else {
                //no ground ahead;
                //Check the size of the hole.
                //cast um ray pro lado do enemy pra ver aonde começa o bloco mais proximo.
                rayDirection = new Vector2 (point.x - transform.position.x , 0);
                rayDistance = 2f;
                RaycastHit2D hit = Physics2D.Raycast (point , rayDirection, rayDistance, layerMask);
                if (hit) {
                    //found the block closest to the enemy. now cast ray forward to check if is it jumpable
                    rayOrigin = new Vector2 (hit.collider.bounds.center.x, hit.collider.bounds.max.y); 
                    if (TryJumping ())
                }
                //casta um ray do bloco mais próximo pra frente até achar alguma coisa em uma distancia maxima de 9.
                //se ele achar, ele tenta pular.

                //Trying to jump over the hole
                bool jumped = false;
                for (float jumpVelocity = movement.JumpVelocityMin ; jumpVelocity <= movement.JumpVelocityMax ; jumpVelocity++) {
                    if (movement.JumpVelocityMax - jumpVelocity < 1) {
                        jumpVelocity = movement.JumpVelocityMax;
                    }
                    if (TryJumping (movement.MoveSpeed * playerDirection , jumpVelocity)) {
                        movement.HandleJump (jumpVelocity);

                        break;
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

        Vector2 origin = new Vector2 (transform.position.x + ( ( col.size.x / 2 )  ) * playerDirection , transform.position.y - (( col.size.y / 2 ) - raySkin));
        
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
            
            RaycastHit2D hit = Physics2D.Raycast (origin , direction , distance , layerMask);
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
            switch (currentState) {
                case State.Spawning:
                    break;
                case State.Idle:
                    break;
                case State.Searching:
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
            currentState = newState;
        }
    }

    private void RunStateBehaviour () {
        switch (currentState) {
            case State.Spawning:
                HandleSpawning ();
                break;
            case State.Idle:
                HandleIdle ();
                break;
            case State.Searching:
                HandleSearching ();
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
