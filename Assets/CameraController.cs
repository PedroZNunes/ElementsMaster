using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    float accelerationTime = 0.05f;
    Vector3 velocitySmoothing;

    GameObject playerGO;

    void Start () {
        playerGO = GameObject.FindGameObjectWithTag (MyTags.player.ToString ());
    }

    void Update () {
        //TODO: define bounds to the camera
        Vector3 targetPosition = new Vector3 (playerGO.transform.position.x , playerGO.transform.position.y , transform.position.z);
        transform.position = Vector3.SmoothDamp (transform.position , targetPosition , ref velocitySmoothing , accelerationTime);
    }

}
