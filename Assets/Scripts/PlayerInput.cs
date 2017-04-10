using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {

    Player player;

	void Awake () {
        player = GetComponent<Player> ();
	}
	
	void Update () { //TODO proceed with refactor.
        Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal") , Input.GetAxisRaw ("Vertical"));
        if (player.movement != null) {
            player.movement.SetDirectionalInput (directionalInput);
        }
    }
}
