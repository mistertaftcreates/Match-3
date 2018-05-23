using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleChecker : MonoBehaviour {

    public Vector2 firstMousePosition;
    public Vector2 currentMousePosition;
    public float angleBetween;
    public bool isControlling;
    public string swipeDirection;

	// Use this for initialization
	void Start () {
        isControlling = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetMouseButtonDown(0)){
            firstMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isControlling = true;
        }
        if(Input.GetMouseButtonUp(0)){
            isControlling = false;
        }

        if(isControlling){
            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //angleBetween = Vector2.Angle(firstMousePosition.normalized, currentMousePosition.normalized);
            if ((currentMousePosition.x - firstMousePosition.x) != 0f)
            {
                angleBetween = Mathf.Atan2(currentMousePosition.y - firstMousePosition.y, currentMousePosition.x - firstMousePosition.x) * 180 / Mathf.PI;
            }
            else if (currentMousePosition.y > firstMousePosition.y)
            {
                angleBetween = 90f;
            }
            else
            {
                angleBetween = -90f;
            }
        }

        if (angleBetween < 45f || angleBetween > -45f){
            swipeDirection = "right";
        }
        if (angleBetween >= 45f && angleBetween < 135f){
            swipeDirection = "up";
        }
        if (angleBetween >= 135f || angleBetween < -135f){
            swipeDirection = "left";
        }
        if (angleBetween >= -135f && angleBetween < -45f){
            swipeDirection = "down";
        }
	}
}
