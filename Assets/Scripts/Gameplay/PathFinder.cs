using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for maping the level in terms of possible movements. this will use A*
public class PathFinder : MonoBehaviour {

    // There are 2 ways to implement this.
    // 1 - Get a list of where the character can go to through the same scheme I used in towers mazing, but for platformer.
    //      This would mean: List<pos> cameFrom as output.
    //      This method needs to know every possible reach the jump has. I can try this giving the path finder all physics the player has.
    //      It needs to know how wallSliding works. 
    //      I have an idea how to do it this way.
    // 2 - Let the AI play the game and simulate all possible jumps while it tries to reach the goal. This could lead to many unsuccessful attempts.

}
