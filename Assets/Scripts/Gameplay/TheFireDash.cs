using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheFireDash : MonoBehaviour {

    GameObject playerGO;
    Collider2D col;
    float duration = 0f;
    float currentDuration = 0f;
    float delay = 0f;
    float speedBurst = 0f;
    int dirX;
    Movement movement;

    void Awake () {
        playerGO = GameObject.FindGameObjectWithTag (MyTags.player.ToString ());
        col = GetComponent<Collider2D> ();
        movement = playerGO.GetComponent<Movement> ();
    }

    public void Initialize (int dirX, float duration, float delay, float speedBurst) {
        this.duration = duration;
        this.delay = delay;
        this.speedBurst = speedBurst;
        this.dirX = dirX;
    }

    void Update () {
        if (currentDuration < delay) {
            currentDuration += Time.deltaTime;
            movement.SetVelocity (Vector2.zero);
        } else if (currentDuration < duration + delay) {
            movement.SetVelocity (new Vector2 (movement.MaxSpeed.x * speedBurst * dirX, -movement.gravity * Time.deltaTime));
            currentDuration += Time.deltaTime;
        }
        else {
            currentDuration = 0f;
            gameObject.SetActive (false);
        }
    }


    void OnTriggerEnter2D(Collider2D col ) {
        if (col.CompareTag (MyTags.enemy.ToString ())) {
            Debug.Log ("Enemy got struck by the fire sword! Lost dmg HP");
        }
    }


}
