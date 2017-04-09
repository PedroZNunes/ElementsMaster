using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class Block : MonoBehaviour {

    [SerializeField]
    Constrains contrains;

    [SerializeField]
    public GameObject prefab { get; protected set; }

    public Vector2 size { get; protected set; }

    public BoxCollider2D boxCollider { get; protected set; }

    protected virtual void Awake () {
        boxCollider = GetComponent<BoxCollider2D> ();
        size = boxCollider.bounds.size;
    }

    void GetConstrains (out bool isResizableX, out bool isResizableY) {
        isResizableX = contrains.isResizableX;
        isResizableY = contrains.isResizableY;
    }

    [System.Serializable]
    public struct Constrains {
        public bool isResizableX;
        public bool isResizableY;
    }

}
