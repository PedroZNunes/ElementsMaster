using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public enum States { Pause, Opening, Play, Win, Lose, Inactive }
    public States currentState { get; private set; }

    List<Objective> objectives = new List<Objective>();
	
    void Awake () {
        Objective.ObjectiveTriggered += OnObjectiveTriggered;
    }

    void Start () {
        //Gives the start to the whole game.
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

    void Trigger (States stateToTrigger ) {
        if (stateToTrigger != currentState) {
            currentState = stateToTrigger;
            switch (stateToTrigger) {
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
}
