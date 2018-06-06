using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{
    wait,
    move
}

public enum TileKind
{
	Breakable,
    Blank,
    Normal
}

[System.Serializable]
public class TileType{
	public int x;
	public int y;
	public TileKind tileKind;
}

public class Board : MonoBehaviour {
   

    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
	public GameObject breakableTilePrefab;
    public GameObject[] dots;
    public GameObject destroyParticle;
	public TileType[] boardLayout;
    private bool[,] blankSpaces;
	private BackgroundTile[,] breakableTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
	private FindMatches findMatches;
	public int basePieceValue = 20;
	private int streakValue = 1;
	private ScoreManager scoreManager;
	private SoundManager soundManager;
	public float refillDelay = 0.5f;
	public int[] scoreGoals;


	// Use this for initialization
	void Start () {
		soundManager = FindObjectOfType<SoundManager>();
		scoreManager = FindObjectOfType<ScoreManager>();
		breakableTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
		blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        SetUp();
	}
    

	public void GenerateBlankSpaces(){
		for (int i = 0; i < boardLayout.Length; i ++)
		{
			if(boardLayout[i].tileKind == TileKind.Blank)
			{
				blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
			}
		}
	}

    public void GenerateBreakableTiles()
	{
		//Look at all the tiles in the layout
		for (int i = 0; i < boardLayout.Length; i ++)
		{
			//if a tile is a "Jelly" tile
			if(boardLayout[i].tileKind == TileKind.Breakable)
			{
				//Create a "Jelly" tile at that position;
				Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
				GameObject tile = Instantiate(breakableTilePrefab,tempPosition, Quaternion.identity);
				breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
			}
		}
	}

    private void SetUp(){
		GenerateBlankSpaces();
		GenerateBreakableTiles();
        for (int i = 0; i < width; i ++){
			for (int j = 0; j < height; j++)
			{
				if (!blankSpaces[i, j])
				{
					Vector2 tempPosition = new Vector2(i, j + offSet);
					Vector2 tilePosition = new Vector2(i, j);
					GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
					backgroundTile.transform.parent = this.transform;
					backgroundTile.name = "( " + i + ", " + j + " )";

					int dotToUse = Random.Range(0, dots.Length);

					int maxIterations = 0;

					while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
					{
						dotToUse = Random.Range(0, dots.Length);
						maxIterations++;
						Debug.Log(maxIterations);
					}
					maxIterations = 0;

					GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
					dot.GetComponent<Dot>().row = j;
					dot.GetComponent<Dot>().column = i;
					dot.transform.parent = this.transform;
					dot.name = "( " + i + ", " + j + " )";
					allDots[i, j] = dot;
				}
			}

        }
    }

    private bool MatchesAt(int column, int row, GameObject piece){
        if(column > 1 && row > 1){
			if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
			{
				if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
				{
					return true;
				}
			}
			if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
			{
				if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
				{
					return true;
				}
			}

        }else if(column <= 1 || row <= 1){
            if(row > 1){
				if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
				{
					if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
					{
						return true;
					}
				}
            }
            if (column > 1)
            {
				if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
				{
					if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
					{
						return true;
					}
				}
            }
        }

        return false;
    }

    
    private bool ColumnOrRow(){
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
        {
            foreach (GameObject currentPiece in findMatches.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if(dot.row == firstPiece.row){
                    numberHorizontal++;
                }
                if(dot.column == firstPiece.column){
                    numberVertical++;
                }
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);

    }

    private void CheckToMakeBombs(){
        if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7){
            findMatches.CheckBombs();
        }
        if(findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8){
            if(ColumnOrRow()){
                //Make a color bomb
                //is the current dot matched?
                if(currentDot != null){
                    if(currentDot.isMatched){
                        if(!currentDot.isColorBomb){
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }else{
                        if(currentDot.otherDot != null){
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if(otherDot.isMatched){
                                if(!otherDot.isColorBomb){
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }else{
                //Make a adjacent bomb
                //is the current dot matched?
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isAdjacentBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isAdjacentBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void DestroyMatchesAt(int column, int row){
        if(allDots[column, row].GetComponent<Dot>().isMatched){
            //How many elements are in the matched pieces list from findmatches?
            if(findMatches.currentMatches.Count >= 4){
                CheckToMakeBombs();
            }

            //Does a tile need to break?
			if(breakableTiles[column, row]!=null)
			{
				//if it does, give one damage.
				breakableTiles[column, row].TakeDamage(1);
				if(breakableTiles[column, row].hitPoints <= 0)
				{
					breakableTiles[column, row] = null;
				}
                
			}
            //Does the sound manager exist?
			if(soundManager != null)
			{
				soundManager.PlayRandomDestroyNoise();
			}
            GameObject particle = Instantiate(destroyParticle, 
                                              allDots[column, row].transform.position, 
                                              Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column, row]);
			scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches(){
        for (int i = 0; i < width; i ++){
            for (int j = 0; j < height; j++){
                if (allDots[i, j] != null){
                    
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());
    }

	private IEnumerator DecreaseRowCo2()
	{
		for (int i = 0; i < width; i ++)
		{
			for (int j = 0; j < height; j ++)
			{
				//if the current spot isn't blank and is empty. . . 
				if(!blankSpaces[i,j] && allDots[i,j] == null)
				{
					//loop from the space above to the top of the column
					for (int k = j + 1; k < height; k ++)
					{
						//if a dot is found. . .
						if(allDots[i, k]!= null)
						{
							//move that dot to this empty space
							allDots[i, k].GetComponent<Dot>().row = j;
							//set that spot to be null
							allDots[i, k] = null;
							//break out of the loop;
							break;
						}
					}
				}
			}
		}
		yield return new WaitForSeconds(refillDelay * 0.5f);
		StartCoroutine(FillBoardCo());
	}

    private IEnumerator DecreaseRowCo(){
        int nullCount = 0;
        for (int i = 0; i < width; i ++){
            for (int j = 0; j < height; j ++){
                if(allDots[i, j] == null){
                    nullCount++;
                }else if(nullCount > 0){
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
		yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard(){
        for (int i = 0; i < width; i ++){
            for (int j = 0; j < height; j ++){
				if(allDots[i, j] == null && !blankSpaces[i,j]){
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
					int maxIterations = 0;

					while(MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
					{
						maxIterations++;
						dotToUse = Random.Range(0, dots.Length);
					}

					maxIterations = 0;
					GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;

                }
            }
        }
    }

    private bool MatchesOnBoard(){
        for (int i = 0; i < width; i ++){
            for (int j = 0; j < height; j ++){
                if(allDots[i, j]!= null){
                    if(allDots[i, j].GetComponent<Dot>().isMatched){
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo(){
        RefillBoard();
		yield return new WaitForSeconds(refillDelay);

        while(MatchesOnBoard()){
			streakValue ++;
			DestroyMatches();
			yield return new WaitForSeconds(2 * refillDelay);
            
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        

		if(IsDeadlocked())
		{
			StartCoroutine(ShuffleBoard());
			Debug.Log("Deadlocked!!!");
		}
		yield return new WaitForSeconds(refillDelay);
        currentState = GameState.move;
		streakValue = 1;

    }

	private void SwitchPieces(int column, int row, Vector2 direction)
	{
		//Take the second piece and save it in a holder
		GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        //switching the first dot to be the second position
		allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
		//Set the first dot to be the second dot
		allDots[column, row] = holder;
	}

    private bool CheckForMatches()
	{
		for (int i = 0; i < width; i ++)
		{
			for (int j = 0; j < height; j ++)
			{
				if(allDots[i,j]!= null)
				{
					//Make sure that one and two to the right are in the
					//board
					if (i < width - 2)
					{
						//Check if the dots to the right and two to the right exist
						if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
						{
							if (allDots[i + 1, j].tag == allDots[i, j].tag
							   && allDots[i + 2, j].tag == allDots[i, j].tag)
							{
								return true;
							}
						}

					}
					if (j < height - 2)
					{
						//Check if the dots above exist
						if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
						{
							if (allDots[i, j + 1].tag == allDots[i, j].tag
							   && allDots[i, j + 2].tag == allDots[i, j].tag)
							{
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	public bool SwitchAndCheck(int column, int row, Vector2 direction)
	{
		SwitchPieces(column, row, direction);
		if(CheckForMatches())
		{
			SwitchPieces(column, row, direction);
			return true;
		}
		SwitchPieces(column, row, direction);
		return false;
	}

    private bool IsDeadlocked()
	{
		for (int i = 0; i < width; i ++)
		{
			for (int j = 0; j < height; j ++)
			{
				if(allDots[i,j]!= null)
				{
					if(i < width - 1)
					{
						if(SwitchAndCheck(i, j, Vector2.right))
						{
							return false;
						}
					}
					if(j < height - 1)
					{
						if(SwitchAndCheck(i,j,Vector2.up))
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	private IEnumerator ShuffleBoard()
	{
		yield return new WaitForSeconds(0.5f);
		//Create a list of game objects
		List<GameObject> newBoard = new List<GameObject>();
		//Add every piece to this list
		for (int i = 0; i < width; i ++)
		{
			for (int j = 0; j < height; j ++)
			{
				if(allDots[i,j]!= null)
				{
					newBoard.Add(allDots[i, j]);
				}
			}
		}
		yield return new WaitForSeconds(0.5f);
		//for every spot on the board. . . 
		for (int i = 0; i < width; i ++)
		{
			for (int j = 0; j < height; j ++)
			{
				//if this spot shouldn't be blank
				if(!blankSpaces[i,j])
				{
					//Pick a random number
					int pieceToUse = Random.Range(0, newBoard.Count);
                    
                    //Assign the column to the piece
					int maxIterations = 0;

					while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
						pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                        Debug.Log(maxIterations);
                    }
					//Make a container for the piece
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxIterations = 0;
					piece.column = i;
                    //Assign the row to the piece
					piece.row = j;
                    //Fill in the dots array with this new piece
					allDots[i, j] = newBoard[pieceToUse];
                    //Remove it from the list
					newBoard.Remove(newBoard[pieceToUse]);
				}
			}
		}
        //Check if it's still deadlocked
		if(IsDeadlocked())
		{
			StartCoroutine(ShuffleBoard());
		}
	}

}
