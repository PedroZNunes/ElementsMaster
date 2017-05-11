using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This will be used for map generating. This is responsible for testing the map and seeing wether it works or not.
[RequireComponent(typeof(Player))]
public class PlayerAI : MonoBehaviour {

    int jumpsRequired;
    int wallJumpsRequired;
    float runDuration;
    //include here a bunch of statistics that the map generator should look at in order to determine whether it is a good map or not.
    
	void Start () {
		//Vector3[] ObjectivePositions = FindObjectsWithTag(MyTags.Objectives.ToString());
        //State[] goalStates;
        //for(state state in goalstates)
        //  goalStates[i].pos = objectivepositions[i]
        //Set Initial State
        //  initialState = new State (initial parameters)
	}
	
	
	void Update () {
		//loop through list of objectives trying to get to all of them in some sequence. 
        //while (i < goalstates.length)
        //random i
        //
        //the pathing should be available from any objective to any other objective.
        //if succeed, returns true; if not, false;
	}

    ///bool TryPath(Subgrid subgrid ){
    ///
    ///}


}

struct State {
    //bunch of bools to define each state
    bool isGrounded;
    bool isJumping;
    Vector2 pos;

    //public State (initial state parameters)
    //initialize all parameters
    //set all else to 0 or false
}