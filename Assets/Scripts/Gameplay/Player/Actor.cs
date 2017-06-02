using UnityEngine;

/// <summary>
/// abstract class used as base for all actors in the game.
/// </summary>
[RequireComponent (typeof (Controller2D))]
public abstract class Actor : MonoBehaviour {
    protected Controller2D controller;
    public Controller2D Controller { get { return controller; } }

    public abstract void Die ();
}
