using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class Block : MonoBehaviour {

    public GameObject prefab;

    public BoxCollider2D boxCollider { get; protected set; }

    protected virtual void Awake () {
        boxCollider = GetComponent<BoxCollider2D> ();
    }
}
