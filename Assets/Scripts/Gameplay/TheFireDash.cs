using UnityEngine;

public class TheFireDash : MonoBehaviour {

    private float duration = 0f;
    private float currentDuration = 0f;
    private float delay = 0f;

    private float speedBurst = 0f;
    private int dirX ;

    private Movement movement;

    void Awake () {
        movement = GetComponentInParent<Movement> ();
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
            movement.Velocity = Vector2.zero;
        } else if (currentDuration < duration + delay) {
            movement.Velocity = new Vector2 (movement.MaxSpeed.x * speedBurst * dirX, -movement.gravity * Time.deltaTime);
            currentDuration += Time.deltaTime;
        }
        else {
            currentDuration = 0f;
            movement.Velocity /= 2;
            gameObject.SetActive (false);
        }
    }

    void OnTriggerEnter2D(Collider2D col ) {
        if (col.CompareTag (MyTags.enemy.ToString ())) {
            Debug.Log ("Enemy got struck by the fire dash! Lost xxx HP");
        }
    }
}
