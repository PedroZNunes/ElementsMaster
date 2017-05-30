using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public abstract class Actor : MonoBehaviour {
    protected Controller2D controller;
    public Controller2D Controller { get { return controller; } }

    public abstract void Die ();
        
}
