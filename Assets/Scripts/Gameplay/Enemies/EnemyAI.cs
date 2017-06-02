using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[RequireComponent (typeof (Enemy))]
public class EnemyAI : MonoBehaviour {

    public enum State { Spawning, Roaming, Chasing, Attacking, Dying}
    public State currentState;

    private enum RoamingState { Walking, Waiting }
    private RoamingState roamingState;

    private delegate void OnTryJumpHandler ( bool isSuccessful);
    private event OnTryJumpHandler OnTryJumpEvent;

    [SerializeField]
    private LayerMask collisionLayerMask; //layer that trigger the collision
    [SerializeField]
    private LayerMask aggroLayerMask; //layers that trigger the aggro

    [SerializeField]
    private Vector2 aggroBoxSize; //the size of the box used to find targets
    [SerializeField]
    private int checkAggroInterval = 4; //interval between checking if that is anything to chase.
    private Coroutine checkAggro; //the coroutine responsible for checking the aggro

    [SerializeField]
    private float forwardRayDistance = 3f; //length of the ray being cast forward to find either the player or an obstacle
    [SerializeField]
    private float downwardRayDistance = 4.9f; //length of the ray responsible for findind the ground ahead.

    private const float groundRayOffsetX = 0.9f; //distance between the ground ray and the enemy
    private const float raySkinWidth = 0.015f; //used to compensate the collision system. a ray cast forward from below the enemy cant collide with the ground.

    private Vector2 targetPos; 
    private const int bentRayMaxcount = 50; //max number of rays to be used on bent rays for prediction.
    private const float jumpDistanceMin = 0.2f; //minimum jump distance 
    private const float jumpDistanceIdeal = 0.46525f; //ideal jumping distance for having a full height jump
    private const int tryJumpMaxCount = 5; //max number of jumps to be tried before giving up
    private int tryJumpCount = 0; //current jump count
    private const float jumpResetTimeMax = 2f; //time to reset the jump count max
    private float jumpResetTimer = 0f; //current time to reset the jump count max

    [SerializeField]
    private DelayTime roamingDelay; //for how much time does it walk and for how much time does it wait
    private float currentRoamingDelay; //current delay (after random)
    private float currentRoamingTime = 0f; 
    private float roamingRandomSeed = 0f; //used to determine which way to walk when roaming randomly

    private int playerDirection = 1; //direction in X where the player should be 
    
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

        OnTryJumpEvent += OnTryJump;
    }

    public void StartCheckingAggro () {
        if (checkAggro == null) {
            StartCoroutine (CheckAggro ());
        }
    }

    public void StopCheckingAggro () {
        if (checkAggro != null) {
            StopCoroutine (checkAggro);
        }
    }

    private void Update () {
        targetPos = player.transform.position;

        playerDirection = ( targetPos.x - transform.position.x < 0f ) ? -1 : 1;

        RunStateBehaviour ();
    }

    private void OnDrawGizmos () {
        Gizmos.DrawWireCube (transform.position , aggroBoxSize);
    }

    /// <summary>
    /// responsible for avoiding that the enemy tries to jump forever. it resets after a couple of tries.
    /// </summary>
    /// <param name="isSuccessful">did it jump successfully</param>
    private void OnTryJump (bool isSuccessful ) {
        if (isSuccessful) {
            tryJumpCount = 0;
        }
        else {
            tryJumpCount++;
            if (tryJumpCount >= tryJumpMaxCount) {
                Trigger (State.Roaming);
            }
        }
        //Debug.LogFormat ("Tried to jump. Success? {0}. count: {1}. t: {2}" , isSuccessful , tryJumpCount , Time.time);
    }

    private void ResetJumpTimer () {
        jumpResetTimer = 0f;
        tryJumpCount = 0;
    }

    /// <summary>
    /// tries to find the player every couple of seconds (checkAggroInterval)
    /// </summary>
    private IEnumerator CheckAggro () {
        WaitForSeconds waitInterval = new WaitForSeconds (checkAggroInterval);

        while (gameObject.activeSelf) {
            Collider2D hit = Physics2D.OverlapBox (transform.position , aggroBoxSize , 0f , aggroLayerMask);
            if (hit != null) {
                //Debug.LogFormat ("{0} checking aggro. Found the player. t: {1}" , gameObject.name, Time.time);
                Trigger (State.Chasing);
            }
            else {
                //Debug.LogFormat ("{0} checking aggro. Found nothing. t: {1}" , gameObject.name , Time.time);
                Trigger (State.Roaming);
            }
            yield return waitInterval;
        }
    }

    private void HandleSpawning () {
        //spawningAnimation
        //by the end of the animation, trigger roaming
        Trigger (State.Roaming);
    }
    
    /// <summary>
    /// roams around every couple of seconds.
    /// </summary>
    private void HandleRoaming () {

        currentRoamingTime += Time.deltaTime;
        
        int moveDirection = (roamingRandomSeed < 0.5f) ? -1 : 1;

        switch (roamingState) {
            case RoamingState.Walking:
                if (currentRoamingTime >= currentRoamingDelay) {
                    roamingRandomSeed = Random.value;
                    currentRoamingTime = 0f;
                    currentRoamingDelay = Random.Range (roamingDelay.waiting, roamingDelay.waiting + roamingDelay.rangeOffset);
                    roamingState = RoamingState.Waiting;
                }

                if (controller.Collisions.below) {
                    // test if there's ground ahead
                    //I'll cast a one ray downward to check for ground ahead.
                    Vector2 rayOrigin = new Vector2 ();
                    Vector2 rayDirection = Vector2.down;
                    rayOrigin = new Vector2 (transform.position.x + moveDirection * groundRayOffsetX , transform.position.y);
                    Debug.DrawRay (rayOrigin , rayDirection * downwardRayDistance , Color.yellow);
                    RaycastHit2D hit = Physics2D.Raycast (rayOrigin , rayDirection , downwardRayDistance , collisionLayerMask);
                    if (hit.distance > ( col.size.y / 2 ) + raySkinWidth || !hit || hit.normal != Vector2.up) {
                        //no ground ahead. go back.
                        moveDirection = -moveDirection;
                    }
                    else {
                        //ground ahead.
                        rayOrigin = new Vector2 (transform.position.x + ( ( col.size.x / 2 ) * moveDirection ) , transform.position.y);
                        rayDirection = Vector2.right * moveDirection;
                        Debug.DrawRay (rayOrigin , rayDirection , Color.green);
                        int layerMask = ( collisionLayerMask | aggroLayerMask );
                        hit = Physics2D.Raycast (rayOrigin , rayDirection , forwardRayDistance , layerMask);
                        if (hit) {
                            if (hit.collider.GetComponent<Block> () != null) {
                                //Block ahead.
                                if (hit.distance <= 0.5f) {
                                    moveDirection = -moveDirection;
                                }
                            }
                        }
                    }
                }
                movement.SetDirectionalInput (Vector2.right * moveDirection);
                break;

            case RoamingState.Waiting:
                if (currentRoamingTime >= currentRoamingDelay) {
                    currentRoamingTime = 0f;
                    currentRoamingDelay = Random.Range (roamingDelay.walking , roamingDelay.walking + roamingDelay.rangeOffset);
                    roamingState = RoamingState.Walking;
                }

                if (Mathf.Abs(movement.Velocity.x) >= 1f) {
                    movement.SetDirectionalInput (Vector2.right * -Mathf.Sign (movement.Velocity.x));
                }
                else {
                    movement.SetDirectionalInput (Vector2.zero);
                }
                break;

            default:
                roamingState = RoamingState.Waiting;
                break;
        }
    }

    /// <summary>
    /// chases the player shooting bent rays to predict how physics will affect it. 
    /// </summary>
    private void HandleChasing () {
        //running animation
        jumpResetTimer += Time.deltaTime;
        if (jumpResetTimer >= jumpResetTimeMax) {
            ResetJumpTimer ();
        }

        int moveDirection = playerDirection;
        //ground is a face with normal poiting upwards
        //Dealing with Jumps
        if (controller.Collisions.below) {
            // test if there's ground ahead
            //I'll cast a one ray downward to check for ground ahead.
            Vector2 rayOrigin = new Vector2 (); 
            Vector2 rayDirection = Vector2.down;
            
            rayOrigin = new Vector2 (transform.position.x + moveDirection * groundRayOffsetX , transform.position.y);
            Debug.DrawRay (rayOrigin , rayDirection * downwardRayDistance , Color.yellow);
            RaycastHit2D hit = Physics2D.Raycast (rayOrigin , rayDirection , downwardRayDistance , collisionLayerMask);
            if (hit.distance > (col.size.y / 2) + raySkinWidth || !hit || hit.normal != Vector2.up) {
                //no ground ahead;
                //Trying to jump over the hole
                Vector2? closestLandingPos = null;
                float closestJumpVelocity = 0f;
                for (float jumpVelocity = 0 ; jumpVelocity <= movement.JumpVelocityMax ; jumpVelocity++) {
                    if (jumpVelocity > 0 && jumpVelocity < movement.JumpVelocityMin) {
                        jumpVelocity = movement.JumpVelocityMin;
                    }

                    if (movement.JumpVelocityMax - jumpVelocity < 1) {
                        jumpVelocity = movement.JumpVelocityMax;
                    }

                    //checking if this ray is the best alternative
                    Vector2? landingPos = CastBentRay (movement.MoveSpeed * moveDirection , jumpVelocity);
                    if (landingPos.HasValue) {
                        if (closestLandingPos.HasValue) {
                            if (( landingPos.Value - targetPos ).sqrMagnitude < ( closestLandingPos.Value - targetPos ).sqrMagnitude) {
                                closestLandingPos = landingPos.Value;
                                closestJumpVelocity = jumpVelocity;
                            }
                        }
                        else {
                            closestLandingPos = landingPos.Value;
                            closestJumpVelocity = jumpVelocity;
                        }
                    }
                }

                //checking if the jump is possible. if it is, use the most appropriate jump velocity as predicted by the ray.
                if (closestLandingPos.HasValue) {
                    if (OnTryJumpEvent != null)
                        OnTryJumpEvent (true);
                    movement.HandleJump (closestJumpVelocity);
                }
                else {
                    if (OnTryJumpEvent != null)
                        OnTryJumpEvent (false);
                }

            }
            else {
                //Can move. Ground Ahead.
                //Casting rays forward from the center.
                rayOrigin = new Vector2 (transform.position.x + ( ( col.size.x / 2 ) * moveDirection ) , transform.position.y);
                rayDirection = Vector2.right * moveDirection;
                Debug.DrawRay (rayOrigin , rayDirection , Color.green);
                int layerMask = (collisionLayerMask | aggroLayerMask);
                hit = Physics2D.Raycast (rayOrigin , rayDirection , forwardRayDistance , layerMask);
                if (hit) {
                    if (hit.collider.GetComponent<Block> () != null) {
                        //Block ahead.
                        //Testing if there's enough jump space.
                        if (hit.distance > jumpDistanceIdeal) {
                            //Try to jumping in different velocitys
                            Vector2? closestLandingPos = null;
                            float closestJumpVelocity = 0f;
                            for (float jumpVelocity = movement.JumpVelocityMin ; jumpVelocity <= movement.JumpVelocityMax ; jumpVelocity++) {
                                if (movement.JumpVelocityMax - jumpVelocity < 1) {
                                    jumpVelocity = movement.JumpVelocityMax;
                                }

                                Vector2? landingPos = CastBentRay (movement.MoveSpeed * moveDirection , jumpVelocity);
                                if (landingPos.HasValue) {
                                    if (closestLandingPos.HasValue) {
                                        if (( landingPos.Value - targetPos ).sqrMagnitude < ( closestLandingPos.Value - targetPos ).sqrMagnitude) {
                                            closestLandingPos = landingPos.Value;
                                            closestJumpVelocity = jumpVelocity;
                                        }
                                    }
                                    else {
                                        closestLandingPos = landingPos.Value;
                                        closestJumpVelocity = jumpVelocity;
                                    }
                                }
                            }

                            if (closestLandingPos.HasValue) {
                                if (OnTryJumpEvent != null)
                                    OnTryJumpEvent (true);

                                movement.HandleJump (closestJumpVelocity);
                            }
                            else {
                                if (OnTryJumpEvent != null)
                                    OnTryJumpEvent (false);
                            }
                        }
                        else {
                            moveDirection = -moveDirection;
                        }
                    }
                }
                else {
                    //no blocks ahead
                }

            }

            //stuck
            if (controller.Collisions.below && controller.Collisions.right && controller.Collisions.left) {
                if (CastBentRay (movement.MoveSpeed * playerDirection , movement.JumpVelocityMax) != null) {
                    movement.HandleJump (movement.JumpVelocityMax);
                }
            }
        }
        else {
            //enemy not grounded.
        }
        movement.SetDirectionalInput (Vector2.right * moveDirection);
    }

    /// <summary>
    /// casts a bunch of rays in sequence to have a prediction of the physics.
    /// </summary>
    /// <param name="targetVelocityX"></param>
    /// <param name="jumpVelocity"></param>
    /// <returns> a nullable vector2 representing the position where the ray lands (if any) </returns>
    private Vector2? CastBentRay (float targetVelocityX , float jumpVelocity) {
        //se wall distance < (distancia minima para conseguir um pulo sem esbarrar na parede), pule baixo e desacelerando
        Vector2? landingPosition = null;

        if (movement.Velocity.x == 0 && targetVelocityX == 0) {
            return landingPosition;
        }

        Vector2 origin = new Vector2 (transform.position.x + ( ( col.size.x / 2 ) ) * playerDirection , transform.position.y - ( ( col.size.y / 2 ) - raySkinWidth ));
        
        Vector2 startingVelocity = new Vector2 (movement.Velocity.x , jumpVelocity);
        Vector2 currentVelocity = startingVelocity;

        float smoothVelocityX = startingVelocity.x;
        //bent ray
        for (int rayCount = 0 ; rayCount < bentRayMaxcount ; rayCount++) {
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
                if (hit.normal == Vector2.up) {
                    landingPosition = hit.point;
                }
                break;
            }

            origin = origin + (direction * distance);
        }
        return landingPosition;
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

    /// <summary>
    /// triggers behaviour state.
    /// </summary>
    public void Trigger (State newState ) {
        if (newState != currentState) {
            currentState = newState;
            switch (currentState) {
                case State.Spawning:
                    break;
                case State.Roaming:
                    break;
                case State.Chasing:
                    tryJumpCount = 0;
                    break;
                case State.Attacking:
                    break;
                case State.Dying:
                    break;
                default:
                    break;
            }
            //Debug.Log (currentState);
        }
    }

    /// <summary>
    /// runs the behaviour for the current AI state.
    /// </summary>
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

    [Serializable]
    public struct DelayTime {
        public int waiting;
        public int walking;
        public int rangeOffset;
    }

}
