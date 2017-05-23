using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    public enum State { Spawning, Idle, Searching, Chasing, Attacking, Dying}
    public State currentState;

    private Enemy enemy;
    private Player player;
    private Controller2D controller;
    [SerializeField]
    private float aggroDistance = 20f;

    

    private void Awake () {
        enemy = GetComponent<Enemy> ();
        player = GetComponent<Player> ();
        controller = enemy.Controller;
    }

    private void Update () {
        RunStateBehaviour ();
    }

    private void HandleSpawning () {
        //spawningAnimation
    }

    private void HandleIdle () {
        //idle animation
    }

    private void HandleSearching<T> () {
        //idle animation

            Vector2 targetPos = player.transform.position;
            float distance = Vector2.Distance (targetPos , transform.position);

            if (Mathf.Abs (distance) < aggroDistance)  {
                Trigger (State.Chasing);
            }
        //find player position
        //send to pathfinder for a path towards that location
        //wait until it finds somehting.
    }

    private void HandleChasing () {
        //running animation
        int dirX = 1;
        Vector3 targetPos = player.transform.position;
        if (targetPos.x - transform.position.x < 0f) {
            dirX = -1;
        }
        else {
            dirX = 1;
        }
        //

        float distanceBetweenRays = 0.25f;

        float boundsHeight = enemy.movement.maxJumpHeight;
        //set the n of rays base off the  maxdistance between them
        int rayCount = Mathf.RoundToInt (boundsHeight / distanceBetweenRays);
        rayCount = Mathf.Clamp (rayCount , 2 , int.MaxValue);
        //defines the final spacing
        float raySpacing = boundsHeight / ( rayCount - 1 );
        for (int i = 0 ; i < rayCount ; i++) {
            Vector2 rayOrigin = (dirX == -1)? 
            rayOrigin += Vector2.up * ( horizontalRaySpacing * i );
            RaycastHit2D hit = Physics2D.Raycast ()
        }
        

        enemy.movement.SetDirectionalInput (Vector2.right);
        //send input to movement script in form of vector
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
        //should take the collider away in the first frame and deactivate any kind of outside influence

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
