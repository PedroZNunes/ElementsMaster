using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Actor : MonoBehaviour {
    protected Controller2D controller;
    public Controller2D Controller { get { return controller; } }
}
