using UnityEngine;
using System;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {

    Player player;
    Mastery mastery;

    static public event Action PressPauseEvent;


    private void Awake () {
        player = GetComponent<Player> ();
        mastery = GetComponentInChildren<Mastery> ();
    }

    private void Update () {
        RunStateBehaviour ();
    }

    /// <summary>
    /// checks for input depending on the current game state.
    /// </summary>
    private void RunStateBehaviour () {
        switch (GameManager.currentState) {
            case GameManager.States.Pause:
                CheckPause ();
                break;
            case GameManager.States.Opening:
                break;
            case GameManager.States.Play:
                CheckPause ();
                CheckMovement ();
                CheckActions ();
                break;
            case GameManager.States.Win:
                CheckMovement ();
                break;
            case GameManager.States.Lose:
                break;
            case GameManager.States.Inactive:
                break;
            default:
                break;
        }
    }

    private void CheckMovement () {
        if (player.movement != null) {
            Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal") , Input.GetAxisRaw ("Vertical"));
            player.movement.SetDirectionalInput (directionalInput);

            if (Input.GetButtonDown ("Jump")) {
                player.movement.HandleJump ();
            }

            if (Input.GetButtonUp ("Jump")) {
                player.movement.HandleCancelJump ();
            }
        }
    }

    private void CheckPause () {
        if (Input.GetButtonDown ("Cancel")) {
            if (PressPauseEvent != null) {
                PressPauseEvent ();
            }
        }
    }

    private void CheckActions () {
        if (Input.GetButtonDown ("Fire1")) {
            mastery.Spell1 ();
        }
        else if (Input.GetButtonDown ("Fire2")) {
            mastery.Spell2 ();
        }
        else if (Input.GetButtonDown ("Fire3")) {
            mastery.Spell3 ();
        }
        else if (Input.GetButtonDown ("Fire4")) {
            mastery.Spell4 ();
        }
    }
}
