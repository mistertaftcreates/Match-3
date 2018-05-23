using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScalar : MonoBehaviour {

    private Board board;
    public float cameraOffset;
    public float aspectRatio = 0.625f;
    public float padding = 2;

	// Use this for initialization
	void Start () {
        board = FindObjectOfType<Board>();
        if(board!= null){
            RepositionCamera(board.width - 1, board.height - 1);
        }
	}

    void RepositionCamera(float x, float y){
        Vector3 tempPosition = new Vector3(x/2, y/2, cameraOffset);
        transform.position = tempPosition;
        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        }else{
            Camera.main.orthographicSize = board.height / 2 + padding;
        }
    }
	


}
