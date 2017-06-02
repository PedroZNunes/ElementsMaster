using UnityEngine;

public class Objective : MonoBehaviour {

    public delegate void TriggeredEventHandler ( Objective obj );
    public static event TriggeredEventHandler Triggered;

    public bool isEnabled = false;

    [SerializeField]
    private Color notTriggeredColor, triggeredColor;

    Collider2D col;
    private bool isTriggered = false;
    public bool IsTriggered {
        get { return isTriggered; }
        set {
            isTriggered = value;
            ChangeMaterialColor ();
        }
    }

    private void Awake () {
        col = GetComponent<Collider2D> ();
    }

    public void Reset () {
        isEnabled = true;
        IsTriggered = false;
    }

    /// <summary>
    /// Receives message from the controller2D. checks if its the player and trigger the objective if true.
    /// </summary>
    /// <param name="source">who collided with it</param>
    public void OnCollision ( GameObject source ) {
        if (source.CompareTag (MyTags.player.ToString ())) {
            Debug.Log ("Collision with Player");
            
            Trigger ();
        }
    }

    private void Trigger () {
        if (Triggered != null) {
            Triggered (this);
        }
        
        col.enabled = isEnabled = false;
        IsTriggered = true;

        Debug.Log ("Player triggered the " + name);
    }

    /// <summary>
    /// changes the color of the objective to mark it as triggered.
    /// </summary>
    private void ChangeMaterialColor () {
        Renderer rend = GetComponent<Renderer> ();
        if (rend != null) {
            if (isTriggered) {
                rend.material.color = triggeredColor;
            }
            else {
                rend.material.color = notTriggeredColor;
            }
        }
    }
}
