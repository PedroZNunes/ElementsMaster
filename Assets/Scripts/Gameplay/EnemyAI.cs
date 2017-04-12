using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    public enum State { Spawning, Idle, Searching, Chasing, Attacking, Dying}
    public State currentState;

    Enemy enemy;

    void Awake () {
        enemy = GetComponent<Enemy> ();
    }

    void HandleSpawning () {
        //spawningAnimation
    }

    void HandleIdle () {
        //idle animation
    }

    void HandleSearching () {
        //idle animation

        //find player position
        //send to pathfinder for a path towards that location
        //wait until it finds somehting.
    }

    void HandleChasing () {
        //running animation

        //read that article about movement planing.
        //follow list of target positions walking towards each unless hit something
        //send input to movement script in form of vector
    }

    void HandleAttacking () {
        //attacking animation

        //this happens if the distance to the player < attack range
        //does the animation
        //if the attack hits (collider?)
        //deal damage
    }

    void HandleDying () {
        //dying animation
        //should take the collider away in the first frame and deactivate any kind of outside influence

        //unsubscribes from events (on disable could do this)
        //
    }

    void Trigger (State newState ) {
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
    
    void RunStateBehaviour () {
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
