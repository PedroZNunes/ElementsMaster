using UnityEngine;

public class Objective : MonoBehaviour {

    public delegate void TriggeredEventHandler ( Objective obj );
    public static event TriggeredEventHandler OnTriggered;

    public bool isEnabled = false;

    [SerializeField]
    private Color notTriggeredColor, triggeredColor;

    Collider2D collider;
    private bool isTriggered = false;
    public bool IsTriggered {
        get { return isTriggered; }
        set {
            isTriggered = value;
            ChangeMaterialColor ();
        }
    }

    private void Awake () {
        collider = GetComponent<Collider2D> ();
    }

    public void Reset () {
        isEnabled = true;
        IsTriggered = false;
    }

    /// <summary>
    /// Receives message from the controller2D. checks if its the player and trigger the objective if true.
    /// </summary>
    /// <param name="source">who collided with it</param>
    private void OnController2DTrigger (Collider2D col) {
        if (col.CompareTag (MyTags.player.ToString ())) {
            Trigger ();
        }
    }

    private void Trigger () {
        if (OnTriggered != null) {
            OnTriggered (this);
        }
        
        collider.enabled = isEnabled = false;
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
