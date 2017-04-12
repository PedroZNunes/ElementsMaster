using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    Dictionary<Vector3 , Block> board = new Dictionary<Vector3 , Block> ();

	void Start () {
        FillBoardList ();
	}

    // Fills the board with the blocks already places in the inspector. All blocks should have integer coordinates.
    // This is temporary until I make a map generator.

    void FillBoardList () {
        board.Clear ();

        Block[] blocksList = FindObjectsOfType<Block> ();
        
        for (int i = 0 ; i < blocksList.Length; i++) {
            Vector2 pos = blocksList[i].transform.position;
            if (!board.ContainsKey (pos)) {
                board.Add (pos, blocksList[i]);
            }
            else {
                board[pos] = blocksList[i];
            }
        }
    }
}
