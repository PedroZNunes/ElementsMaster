using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Movement : MonoBehaviour {

    [SerializeField]
    private Jump jump;
    public float maxJumpHeight { get { return jump.heightMax; } }
    public float minJumpVelocity { get { return jump.velocityMin; } }
    public bool isJumping { get; private set; }

    [SerializeField]
    private Wall wall;
    private float wallUnstickTime;
    public bool isWallSliding { get; private set; }
    private int wallDirX;

    public static float Gravity { get; private set; }
    public Vector2 MaxSpeed {
        get { return new Vector2 (moveSpeed , jump.velocityMax); }
    }

    [SerializeField]
    private float moveSpeed = 10f;
    [SerializeField]
    private float accelerationTimeAirBorne = 0.4f;
    [SerializeField]
    private float accelerationTimeGrounded = 0.1f;

    public float AccelerationTimeAirBorne { get { return accelerationTimeAirBorne; } }
    public float AccelerationTimeGrounded { get { return accelerationTimeGrounded; } }

    [SerializeField]
    private float rayDeactivateTime = 0.2f;

    private float smoothVelocityX;
    private Controller2D controller;
    private Vector3 velocity;
    public Vector2 Velocity {
        get { return velocity; }
        set { velocity = new Vector3 (value.x , value.y , 0); }
    }

    private Vector2 directionalInput;

    public int DirX { get { return controller.Collisions.movementDirX; } }

    void Awake () {
        controller = GetComponent<Controller2D> ();
        if (GetComponent<Player> () != null) {
            CalculateGravity ();
        }
    }

    void Start () {
        CalculateJumpVelocity ();
        Debug.Log (string.Format ("Gravity: {0} | Jump Velocity: {1}" , Gravity , jump.velocityMax));
    }


    void FixedUpdate () {
        isWallSliding = false;
        wallDirX = ( controller.Collisions.right ) ? 1 : -1;

        HandleMovement ();
        if (wall.slideSpeedMax != 0)
            HandleWallSlide ();

        velocity.y += Gravity * Time.deltaTime;
        controller.Move (velocity * Time.deltaTime);

        if (controller.Collisions.above || controller.Collisions.below) {
            velocity.y = 0;
        }
    }

    public void SetDirectionalInput(Vector2 input ) {
        directionalInput = input;
    }

    public void HandleMovement () {
        float targetVelocityX = directionalInput.x * moveSpeed;
        float accelerationTime;

        //if (targetVelocityX != 0) {//if moving
        accelerationTime = ( controller.Collisions.below ) ? accelerationTimeGrounded : accelerationTimeAirBorne;
        //}
        //else { //if stoping
        //accelerationTime = ( controller.Collisions.below ) ? (accelerationTimeGrounded / 2f) : (accelerationTimeAirBorne / 2f);
        //}

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
                if (wallDirX == directionalInput.x) {
                    Debug.Log ("Climbing");
                    velocity.x = -wallDirX * wall.climb.x;
                    velocity.y = wall.climb.y;
                }
                else if (directionalInput.x == 0) {
                    Debug.Log ("Jumping Off");
                    velocity.x = -wallDirX * wall.jumpOff.x;
                    velocity.y = wall.jumpOff.y;
                }
                else {
                    Debug.Log ("Leaping Away");
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

    public void HandleCancelJump () {
        if (velocity.y > jump.velocityMin) {
            velocity.y = jump.velocityMin;
        }
    }

    public void HandleFallThrough () {
        controller.DeactivateRays (rayDeactivateTime);
    }

    ////maybe I`ll need this later
    //IEnumerator TrackHeightAndLength () {
    //    float timeCount = 0f;
    //    float x1 = transform.position.x;
    //    timeCount += Time.deltaTime;
    //    float initialHeight = transform.position.y;
    //    currentJumpHeight = initialHeight;
    //    yield return null;
    //    while (!controller.collisions.below) {
    //        currentJumpHeight = transform.position.y - initialHeight;
    //        timeCount += Time.deltaTime;
    //        yield return null;
    //    }
    //    currentJumpHeight = 0f;
    //    float x2 = transform.position.x;
    //    print (string.Format("Jump - Length: {0:0.000} || Duration {1:0.000}", ( x2 - x1 ),timeCount));
    //}

    //deltaMovement = V0 * t + (a(t^2))/2
    //jumpHeight = (gravity * timeToJumpApex^2)/2
    //Solving for gravity
    //gravity = 2*jumpHeight / timeToJumpApex^2
    private void CalculateGravity () {
        float halfHeight = controller.collider.size.y / 2f;
        Gravity = -( 2 * ( jump.heightMax + halfHeight ) ) / Mathf.Pow (jump.timeToApex , 2);
    }

    //V = V0 + at
    //jumpVelocity = gravity*timeToJumpApex
    private void CalculateJumpVelocity () {
        jump.velocityMax = Mathf.Abs (Gravity) * jump.timeToApex;
        jump.velocityMin = Mathf.Sqrt (2 * Mathf.Abs (Gravity) * jump.heightMin);
    }

    [System.Serializable]
    struct Jump {
        public float heightMin;
        public float heightMax;
        public float timeToApex;
        [HideInInspector]
        public float velocityMax;
        [HideInInspector]
        public float velocityMin;
    }

    [System.Serializable]
    struct Wall {
        public float stickTime;
        public Vector2 jumpOff, leap, climb;
        public float slideSpeedMax;
    }
}
