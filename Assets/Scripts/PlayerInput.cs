using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {

    Player player;

	void Awake () {
        player = GetComponent<Player> ();
	}
	
	void Update () {
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
}
