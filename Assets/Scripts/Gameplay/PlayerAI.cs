using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This will be used for map generating. This is responsible for testing the map and seeing wether it works or not.
[RequireComponent(typeof(Player))]
public class PlayerAI : MonoBehaviour {

    int jumpsRequired;
    int wallJumpsRequired;
    float runDuration;
    //include here a bunch of statistics that the map generator should look at in order to determine wether it is a good map or not.
    

	// Use this for initialization
	void Start () {
		//assemble list of objectives
	}
	
	// Update is called once per frame
	void Update () {
		//loop through list of objectives trying to get to all of them in some sequence. 
        //the pathing should be available from any objective to any other objective.
        //if succeed, returns true; if not, false;
	}
}
