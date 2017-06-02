using UnityEngine;

/// <summary>
/// responsible for following the player around
/// </summary>
public class CameraFollow : MonoBehaviour {

    [SerializeField]
    private float verticalOffset;
    [SerializeField]
    private Controller2D target;

    [SerializeField]
    private Vector2 focusAreaSize;  //square zone that the camera focuses on. whenever it moves, the camera goes with it. 
    private FocusArea focusArea;    //this area smoothens the camera movement and prevent unnecessary movement at every tiny player displacement

    [SerializeField]
    private LookAhead lookAhead; //variables responsible for repositioning the camera a bit in front of the player

    [SerializeField]
    private Vector2 smoothTimeIdle;
    [SerializeField]
    private Vector2 smoothTimeMoving;
    [SerializeField]
    private float smoothChangeTime;
    private Vector2 currentSmoothTime;

    private Vector2 smoothChangeVelocity;
    private Vector2 smoothVelocity;
    private float smoothMaxSpeed = 3f;
    
    private bool isFocusMoving = false;

    private void Start () {
        if (target == null) {
            target = FindObjectOfType<Player> ().GetComponent<Controller2D>();
        }

        focusArea = new FocusArea (target.collider.bounds , focusAreaSize);
        currentSmoothTime = smoothTimeMoving;
    }


    private void LateUpdate () {
        focusArea.Update (target.collider.bounds);
        isFocusMoving = ( Vector3.Magnitude (focusArea.velocity) != 0 );

        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;
        Vector2 targetSmoothTime = ( isFocusMoving ) ? smoothTimeMoving : smoothTimeIdle;
        
        if (isFocusMoving )
        lookAhead.directionX = ( focusArea.velocity.x != 0 ) ? Mathf.Sign (focusArea.velocity.x) : lookAhead.directionX;

        if (targetSmoothTime != currentSmoothTime) {
            currentSmoothTime = Vector2.SmoothDamp (currentSmoothTime , targetSmoothTime , ref smoothChangeVelocity , smoothChangeTime , smoothMaxSpeed , Time.deltaTime);
        }

        lookAhead.targetX = lookAhead.directionX * lookAhead.distanceX;
        lookAhead.currentX = Mathf.SmoothDamp (lookAhead.currentX , lookAhead.targetX , ref smoothVelocity.x , currentSmoothTime.x);

        focusPosition.y = Mathf.SmoothDamp (transform.position.y , focusPosition.y , ref smoothVelocity.y , currentSmoothTime.y);

        focusPosition += Vector2.right * lookAhead.currentX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    private void OnDrawGizmos () {
        Gizmos.color = new Color (0 , 1 , 1 , 0.4f);
        Gizmos.DrawCube (focusArea.center , focusAreaSize);
    }

    private struct FocusArea {
        public Vector2 center;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea ( Bounds bounds, Vector2 size ) {
            left = bounds.center.x - (size.x / 2);
            right = bounds.center.x + (size.x / 2);
            bottom = bounds.min.y;
            top = bounds.min.y + size.y;

            velocity = Vector2.zero;
            center = new Vector2 (( left + right ) / 2 , ( top + bottom ) / 2 );
            print ("Center at Focus Area constructor: " + center);
        }

        public void Update ( Bounds boxBounds ) {
            float shiftX = 0;
            float shiftY = 0;

            CalculateShiftAmounts (boxBounds, ref shiftX, ref shiftY);

            left += shiftX;
            right += shiftX;
            bottom += shiftY;
            top += shiftY;

            center = new Vector2 (( left + right ) / 2 , ( top + bottom) / 2 );
            velocity = new Vector2 (shiftX , shiftY);
            //print (string.Format ("left: {0}, right: {1}, bottom {2}, top {3}, center: {4}" , left , right , bottom , top, center));
            
        }

        private void CalculateShiftAmounts ( Bounds bounds , ref float shiftX, ref float shiftY) {
            //Debug.Log ("Bounds : " + bounds);
            if (bounds.min.x < left) {
                shiftX = bounds.min.x - left;
            }
            else if (bounds.max.x > right) {
                shiftX = bounds.max.x - right;
            }

            if (bounds.min.y < bottom) {
                shiftY = bounds.min.y - bottom;
            }
            else if (bounds.max.y > top) {
                shiftY = bounds.max.y - top;
            }
        }
    }

    [System.Serializable]
    private struct LookAhead {
        [HideInInspector]
        public float currentX;
        [HideInInspector]
        public float targetX;
        [HideInInspector]
        public float directionX;
        
        public float distanceX;
        private Vector2 velocity;
    }

}
