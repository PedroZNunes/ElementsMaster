using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D), typeof (Movement))]
public class Player : MonoBehaviour {

    [HideInInspector]
    public Movement movement;

    Controller2D controller;

	void Awake () {
        controller = GetComponent<Controller2D> ();
        movement = GetComponent<Movement> ();
	}

}
