using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    private const int ROWS = 40;
    private const int COLUMNS = 40;

    public IntVector2 extents = new IntVector2 (ROWS / 2 , COLUMNS / 2);

    private Dictionary<Vector3 , Block> board = new Dictionary<Vector3 , Block> ();

    private List<SubGrid> subGrids = new List<SubGrid> ();

    void OnValidate () {
        if (ROWS % 2 != 0 || COLUMNS % 2 != 0) { //rows and columns should be even.
            Debug.LogError ("Rows or columns are not even numbers");
        }
    }

	void Start () {
        FillBoardList ();
	}

    // Fills the board with the blocks already placed in the inspector. All blocks should have integer coordinates.
    // This is temporary until I make a map generator.

    private void FillBoardList () {
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

    private void CreateSubGrids () {
        //divide the grid in sub grids 1x1;
        for (int i = -extents.x ; i < extents.x ; i++) {
            for (int j = -extents.y ; j < extents.y ; j++) {
                subGrids.Add (new SubGrid (new Vector2 (i , j) , Vector2.one));
            }
        }

    //  choose randomly a vertice to merge ( vector2 (random.range int, random.range int))
    //FixMe: This does not work.vv I need a list containing all vertices of the map. 
    //http://www-cs-students.stanford.edu/~amitp/game-programming/grids/
    //this can help you. maybe we should associate the faces to vertices. the faces are currently centered on the ints, which make the vertices 0.5fs.
        Vector2 randomVertice = new Vector2 (Random.Range (-extents.x , extents.x) , Random.Range (-extents.y, extents.y));
        SubGrid newSubGrid;
        bool isMerged = TryMergeSubGrids (randomVertice , out newSubGrid);
    }

    ///  o grid resultante não pode ser enorme.
    ///done with the subgrids.

    private bool TryMergeSubGrids ( Vector2 vertice, out SubGrid newSubGrid ) {
        List<SubGrid> ToMerge = new List<SubGrid> ();
        for (int i = 0 ; i < subGrids.Count; i++) {
            if (subGrids[i].Contains(vertice)) {
                ToMerge.Add (subGrids[i]);
            }
        }
        if (ToMerge.Count > 1) {
            SubGrid testGrid = new SubGrid ();
            for (int i = 0 ; i < ToMerge.Count ; i++) {
                testGrid.bounds.Encapsulate (ToMerge[i].bounds);
            }
            if (( testGrid.bounds.center.y == testGrid.bounds.center.y && testGrid.bounds.center.x != testGrid.bounds.center.x ) ||
                     testGrid.bounds.center.y != testGrid.bounds.center.y && testGrid.bounds.center.x == testGrid.bounds.center.x) { //the biggest if of my life
                for (int i = 0 ; i < ToMerge.Count ; i++) {
                    subGrids.Remove (ToMerge[i]);
                }
                subGrids.Add (testGrid);
                newSubGrid = testGrid;
                return true;
            }
        }
        newSubGrid = new SubGrid (Vector2.zero , Vector2.zero);
        return false;
    }
}


///START TESTING PLATFORMS AND BUILDING LEVEL
///loop (subgrids) could think of grids as gigantic nodes or subgroup of nodes. we should go from the player to the objectives first.
///     BuildPlatform(subgrid[i])
///     If the player can reach that and if the player can still reach every other platform he has been in, great.
///     the player should be able to always get to every grid on the board. 
///     if (playerai.trypath (subgrid[i]) && (playerai.tryallpaths()))
///         great. continue;
///     else
///         loop again the same grid. i--;
///     

public struct IntVector2 {
    public int x;
    public int y;

    public IntVector2 zero {
        get {
            return new IntVector2 (0 , 0);
        }
    }

    public IntVector2 one {
        get {
            return new IntVector2 (1 , 1);
        }
    }

    public IntVector2 (int x, int y ) {
        this.x = x;
        this.y = y;
    }   
}

public class SubGrid {

    public Bounds bounds = new Bounds ();
    public Vector2 size { get { return bounds.size; } set { bounds.size = value; } }
    public Vector2 center { get { return bounds.center; } set { bounds.center = value; } }

    public SubGrid () {
        bounds.size = Vector2.zero;
        bounds.center = Vector2.zero;
    }

    public SubGrid (Vector2 center) {
        ResetSize ();
        bounds.center = center;
    }

    public SubGrid (Vector2 size, Vector2 center ) {
        bounds.size = size;
        bounds.center = center;
    }

    public bool Contains (Vector2 vertice ) {
        return bounds.Contains (vertice);
    }

    public void ResetSize () {
        size = Vector2.one;
    }

    public Bounds GetBounds () {
        return bounds;
    }   
}
