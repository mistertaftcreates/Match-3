using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour {

	private Board board;
	public float hintDelay;
	private float hintDelaySeconds;
	public GameObject hintParticle;
	public GameObject currentHint;

	// Use this for initialization
	void Start () {
		board = FindObjectOfType<Board>();
		hintDelaySeconds = hintDelay;
	}
	
	// Update is called once per frame
	void Update () {
		hintDelaySeconds -= Time.deltaTime;
		if(hintDelaySeconds<= 0 && currentHint == null)
		{
			MarkHint();
			hintDelaySeconds = hintDelay;
		}
           
	}

    //First, I want to find all possible matches on the board
	List<GameObject> FindAllMatches()
	{
		List<GameObject> possibleMoves = new List<GameObject>();
		for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (i < board.width - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
							possibleMoves.Add(board.allDots[i, j]);
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
							possibleMoves.Add(board.allDots[i, j]);

                        }
                    }
                }
            }
        }
		return possibleMoves;
	}
    //Pick one of those matches randomly
	GameObject PickOneRandomly()
	{
		List<GameObject> possibleMoves = new List<GameObject>();
		possibleMoves = FindAllMatches();
		if(possibleMoves.Count > 0)
		{
			int pieceToUse = Random.Range(0, possibleMoves.Count);
			return possibleMoves[pieceToUse];
		}
		return null;
	}
    //Create the hint behind the chosen match
    private void MarkHint()
	{
		GameObject move = PickOneRandomly();
        if(move != null)
		{
			currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
		}
	}
    //Destroy the hint.
    public void DestroyHint()
	{
		if(currentHint != null)
		{
			Destroy(currentHint);
			currentHint = null;
			hintDelaySeconds = hintDelay;
		}
	}
}
