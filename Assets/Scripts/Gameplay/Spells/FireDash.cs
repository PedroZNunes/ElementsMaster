using UnityEngine;

public class FireDash : Spell {

    [SerializeField]
    private GameObject theFireDashGO;
    private TheFireDash theFireDash;

    [SerializeField]
    private float delay = 0.2f;
    [SerializeField]
    private float duration = 0.5f;
    [SerializeField]
    private float speedMultiplier = 2f;


    void Awake () {
        if (holder == null)
            holder = GameObject.FindWithTag (MyTags.player.ToString ()).transform;
        theFireDash = theFireDashGO.GetComponent<TheFireDash> ();
    }

    public override void Cast ( ) {
        if (CanCast ()) {
            base.Cast ();
            //rotate the effect to the desired position 
            theFireDashGO.transform.localScale = new Vector2 (Mathf.Abs(theFireDashGO.transform.localScale.x) * castDirX , theFireDashGO.transform.localScale.y);
            theFireDashGO.SetActive (true);
            //activate the effect
            theFireDash.Initialize (castDirX , duration, delay, speedMultiplier);

        }
    }

}
