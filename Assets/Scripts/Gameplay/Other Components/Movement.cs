using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Controller2D))]
public class Movement : MonoBehaviour {

    [SerializeField]
    private Jump jump;
    public float maxJumpHeight { get { return jump.heightMax; } }
    public float JumpVelocityMin { get { return jump.velocityMin; } }
    public float JumpVelocityMax { get { return jump.velocityMax; } }
    public bool isJumping { get; private set; }

    [SerializeField]
    private Wall wall;
    private float wallUnstickTime; //time during which the player sticks to the side of the wall after the input received points otherwise
    public bool isWallSliding { get; private set; }
    private int wallDirX; //wall face direction

    public static float Gravity { get; private set; }
    public bool affectedByGravity = true; //is it currently being affected by gravity?

    [SerializeField]
    private float moveSpeed = 10f;
    public float MoveSpeed { get { return moveSpeed; } }

    [SerializeField]
    private float accelerationTimeAirBorne = 0.4f;
    [SerializeField]
    private float accelerationTimeGrounded = 0.1f;

    public float AccelerationTimeAirBorne { get { return accelerationTimeAirBorne; } }
    public float AccelerationTimeGrounded { get { return accelerationTimeGrounded; } }

    [SerializeField]
    private float rayDeactivateTime = 0.2f; //deactivating rays permit the player to fall-through the platforms

    private float smoothVelocityX;

    private Controller2D controller;
    private Vector3 velocity;
    public Vector2 Velocity {
        get { return velocity; }
        set { velocity = new Vector3 (value.x , value.y , 0); }
    }

    private Vector2 directionalInput; 

    public int DirX { get { return controller.Collisions.movementDirX; } }

    /// <summary>
    /// if bound to the player, calculates the gravity
    /// </summary>
    private void Awake () {
        controller = GetComponent<Controller2D> ();
        if (GetComponent<Player> () != null) {
            CalculateGravity ();
        }
    }

    private void Start () {
        CalculateJumpVelocity ();
        //Debug.Log (string.Format ("Gravity: {0} | Jump Velocity: {1}" , Gravity , jump.velocityMax));
    }

    private void FixedUpdate () {
        isWallSliding = false;
        wallDirX = ( controller.Collisions.right ) ? 1 : -1;

        HandleMovement ();
        if (wall.slideSpeedMax != 0)
            HandleWallSlide ();

        if (affectedByGravity) {
            velocity.y += Gravity * Time.deltaTime;
        }

        controller.Move (velocity * Time.deltaTime);

        if (controller.Collisions.above || controller.Collisions.below) {
            velocity.y = 0;
        }
    }

    public void AddForce ( Vector2 force ) {
        Velocity = force;
    }

    public void SetDirectionalInput(Vector2 input ) {
        directionalInput = input;
    }

    public void HandleMovement () {
        float targetVelocityX = directionalInput.x * moveSpeed;
        float accelerationTime;

        accelerationTime = ( controller.Collisions.below ) ? accelerationTimeGrounded : accelerationTimeAirBorne;

        velocity.x = Mathf.SmoothDamp (velocity.x , targetVelocityX , ref smoothVelocityX , accelerationTime);
    }

    public void HandleWallSlide () {
        if (( !controller.Collisions.below && ( controller.Collisions.left || controller.Collisions.right ) && velocity.y < 0 )) {
            isWallSliding = true;

            if (velocity.y < -wall.slideSpeedMax) {
                velocity.y = -wall.slideSpeedMax;
            }

            if (wallUnstickTime > 0) {
                smoothVelocityX = 0;
                velocity.x = 0;
                if (directionalInput.x != wallDirX) {
                    wallUnstickTime -= Time.deltaTime;
                }
                else {
                    wallUnstickTime = wall.stickTime;
                }
            }
            else {
                wallUnstickTime = wall.stickTime;
            }
        }
    }

    public void HandleJump () {
        if (directionalInput.y == -1 && controller.Collisions.standingOnPassThrough) {
            HandleFallThrough ();
        }
        else {
            if (isWallSliding) {
                if (wallDirX == directionalInput.x) { //Climbing
                    velocity.x = -wallDirX * wall.climb.x;
                    velocity.y = wall.climb.y;
                }
                else if (directionalInput.x == 0) { //Jumping off 
                    velocity.x = -wallDirX * wall.jumpOff.x;
                    velocity.y = wall.jumpOff.y;
                }
                else { //Leaping away
                    velocity.x = -wallDirX * wall.leap.x;
                    velocity.y = wall.leap.y;
                }
            }
            else if (controller.Collisions.below) {
                velocity.y = jump.velocityMax;
                //StartCoroutine (TrackHeightAndLength ());
            }
        }
    }

    public void HandleJump (float jumpVelocity ) {
        if (directionalInput.y == -1 && controller.Collisions.standingOnPassThrough) {
            HandleFallThrough ();
        }

        if (controller.Collisions.below) {
            velocity.y = jumpVelocity;
        }
    }

    public void HandleCancelJump () {
        if (velocity.y > jump.velocityMin) {
            velocity.y = jump.velocityMin;
        }
    }

    public void HandleFallThrough () {
        controller.DeactivateRays (rayDeactivateTime);
    }

    //private IEnumerator TrackHeightAndLength () {
    //    float timeCount = 0f;
    //    float x1 = transform.position.x;
    //    timeCount += Time.deltaTime;
    //    float initialHeight = transform.position.y;
    //    float currentJumpHeight = initialHeight;
    //    yield return null;
    //    while (!controller.Collisions.below) {
    //        currentJumpHeight = transform.position.y - initialHeight;
    //        timeCount += Time.deltaTime;
    //        yield return null;
    //    }
    //    float x2 = transform.position.x;
    //    print (string.Format ("Jump - Length: {0:0.000} || Duration {1:0.00000000}" , ( x2 - x1 ) , timeCount));
    //}

    //deltaMovement = V0 * t + (a(t^2))/2
    //jumpHeight = (gravity * timeToJumpApex^2)/2
    //Solving for gravity
    //gravity = 2*jumpHeight / timeToJumpApex^2


    private void CalculateGravity () {
        float halfHeight = controller.collider.bounds.extents.y;
        Gravity = -( 2 * ( jump.heightMax + halfHeight ) ) / Mathf.Pow (jump.timeToApex , 2);
    }

    //V = V0 + at
    //jumpVelocity = gravity*timeToJumpApex
    private void CalculateJumpVelocity () {
        jump.velocityMax = Mathf.Abs (Gravity) * jump.timeToApex;
        jump.velocityMin = Mathf.Sqrt (2 * Mathf.Abs (Gravity) * jump.heightMin);
    }

    [System.Serializable]
    private struct Jump {
        public float heightMin;
        public float heightMax;
        public float timeToApex;
        [HideInInspector]
        public float velocityMax;
        [HideInInspector]
        public float velocityMin;
    }

    [System.Serializable]
    private struct Wall {
        public float stickTime;
        public Vector2 jumpOff, leap, climb;
        public float slideSpeedMax;
    }
}
