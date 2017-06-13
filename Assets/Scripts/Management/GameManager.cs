using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour {

    public enum States { Pause, Opening, Play, Win, Lose, Inactive }
    public static States currentState { get; private set; }

    public delegate void OnPauseHandler ( bool isPaused );
    public static event OnPauseHandler OnPause; //to be used by UI

    public delegate void OnStateChangedHandler ( States previousState, States newState );
    public static event OnStateChangedHandler stateChangedEvent;
    
    private List<Objective> objectives = new List<Objective>();
	
    public static bool isPaused { get; private set; }

    private void Awake () {
        Objective.OnTriggered += OnObjectiveTriggered;
        PlayerInput.PressPauseEvent += Pause;
    }

    private void Start () {
        //Gives the start to the whole game.
        //later it will wait for the map to load.
        Trigger (States.Opening);
    }

    private void Update () {
        GameStateSwitch ();
    }

    private void StartPlay () {
        for (int i = 0 ; i < objectives.Count; i++) {
            objectives[i].Reset();
        }
    }

    private void StartOpening () {
        foreach (Objective obj in GameObject.FindObjectsOfType<Objective> ()) {
            objectives.Add (obj);
        }
        Trigger (States.Play);
    }

    private void GameStateSwitch () {
        switch (currentState) {
            case States.Pause:
                break;
            case States.Opening:
                break;
            case States.Play:
                break;
            case States.Win:
                break;
            case States.Lose:
                break;
            case States.Inactive:
                break;
            default:
                Trigger (States.Inactive);
                break;
        }
    }

    /// <summary>
    /// this function fires up when an objective is triggered by the player. counts the remaining objectives and checks for win state.
    /// </summary>
    private void OnObjectiveTriggered (Objective objectiveTriggered) {
        objectives.Remove (objectiveTriggered);

        if (objectives.Count <= 0) {
            Trigger (States.Win);
        }
    }

    /// <summary>
    /// trigger new state
    /// </summary>
    /// <param name="newState"></param>
    private void Trigger (States newState ) {
        if (newState != currentState) {

            if (stateChangedEvent != null) {
                stateChangedEvent (currentState , newState);
            }

            currentState = newState;

            switch (newState) {
                case States.Pause:
                    Debug.Log ("Triggered Pause");
                    break;
                case States.Opening:
                    Debug.Log ("Triggered Opening");
                    StartOpening ();
                    break;
                case States.Play:
                    Debug.Log ("Triggered Play");
                    StartPlay ();
                    break;
                case States.Win:
                    Debug.Log ("Triggered Win");
                    break;
                case States.Lose:
                    Debug.Log ("Triggered Lose");
                    break;
                case States.Inactive:
                    Debug.Log ("Triggered Inactive");
                    break;
                default:
                    break;
            }
            
        }
    }

    /// <summary>
    /// pause / unpause
    /// </summary>
    public static void Pause () {
        if (!isPaused) {
            isPaused = true;
            Time.timeScale = 0f;
        }
        else if (isPaused) {
            isPaused = false;
            Time.timeScale = 1f;
        }

        if (OnPause != null) {
            OnPause (isPaused);
        }
    }

}
