using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDash : Spell {

    [SerializeField]
    protected GameObject theFireDashGO;
    private TheFireDash theFireDash;

    [SerializeField]
    float delay = 0.2f;
    [SerializeField]
    float duration = 0.5f;
    [SerializeField]
    float speedBurstMultiplier = 0f;

    Animator animator;
    Player player;

    void Awake () {
        if (holder == null)
            holder = GameObject.FindWithTag (MyTags.player.ToString ()).transform;
        theFireDash = theFireDashGO.GetComponent<TheFireDash> ();
        animator = GetComponentInParent<Animator> ();
        player = GetComponentInParent<Player> ();
    }

    public override void Cast ( int dirX , GameObject owner ) {
        if (CanCast ()) {
            //rotate the effect to the desired position 
            theFireDashGO.transform.localScale = new Vector2 (dirX , theFireDashGO.transform.localScale.y);
            theFireDashGO.SetActive (true);
            //activate the effect
            theFireDash.Initialize (dirX, duration, delay, speedBurstMultiplier);

        }
    }

}
