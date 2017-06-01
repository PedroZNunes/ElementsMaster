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

    void Awake () {
        Objective.Triggered += OnObjectiveTriggered;
        PlayerInput.PressPauseEvent += Pause;
    }

    void Start () {
        //Gives the start to the whole game.
        //later it will wait for the map to load async.
        Trigger (States.Opening);
    }

    void StartPlay () {
        for (int i = 0 ; i < objectives.Count; i++) {
            objectives[i].Reset();
        }
    }

    void StartOpening () {
        foreach (Objective obj in GameObject.FindObjectsOfType<Objective> ()) {
            objectives.Add (obj);
        }
        Trigger (States.Play);
    }

    void Update () {
        GameStateSwitch ();
	}

    void GameStateSwitch () {
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

    void OnObjectiveTriggered (object source, ObjectiveTriggeredEventArgs e) {
        Objective obj = e.objectiveTriggered.GetComponent<Objective>();
        objectives.Remove (obj);

        if (objectives.Count <= 0) {
            Trigger (States.Win);
        }
    }

    void Trigger (States newState ) {
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

    static public void Pause () {
        if (!isPaused) {
            isPaused = true;
            Time.timeScale = 0f;
        }
        else if (isPaused) {
            isPaused = false;
            Time.timeScale = 1f;
        }
        FireOnPauseEvent ();
    }

    static void FireOnPauseEvent () {
        if (OnPause != null) {
            OnPause (isPaused);
        }
    }

}
