using UnityEngine;

/// <summary>
/// component attached to the player. works as a buff.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TheFireDash : MonoBehaviour {

    private float duration = 0f;
    private float currentDuration = 0f;
    private float delay = 0f;

    private float speedBurst = 0f;
    private int dirX ;

    private Movement movement;

    private void Awake () {
        movement = GetComponentInParent<Movement> ();
    }

    public void Initialize (int dirX, float duration, float delay, float speedBurst) {
        this.duration = duration;
        this.delay = delay;
        this.speedBurst = speedBurst;
        this.dirX = dirX;
    }

    private void Update () {
        movement.affectedByGravity = false;
        if (currentDuration < delay) { //delay
            currentDuration += Time.deltaTime;
            movement.Velocity = Vector2.zero;
        }
        else if (currentDuration < duration + delay) { //full speed
            movement.Velocity = new Vector2 (movement.MoveSpeed * speedBurst * dirX , 0);
            currentDuration += Time.deltaTime;
        }
        else { //end of buff
            currentDuration = 0f;
            movement.Velocity /= 2;
            movement.affectedByGravity = true;
            gameObject.SetActive (false);
        }
    }

    private void OnTriggerEnter2D (Collider2D col ) {
        if (col.CompareTag (MyTags.enemy.ToString ())) {
            Debug.Log ("Enemy got struck by the fire dash! Lost xxx HP");
        }
    }
}
