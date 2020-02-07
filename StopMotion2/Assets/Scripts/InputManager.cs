using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour {

	public bool raycastWorld, raycastUI, dragging;

	public List<RaycastResult> UIhits;

	// Use this for initialization
	void Start () {
		Cursor.visible = false;
		UIhits = new List<RaycastResult> ();
	}
	
	// Update is called once per frame
	void Update () {
	
		//Mousse Button
		if (Input.GetMouseButtonDown (0)) {
			if (raycastWorld) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast (ray, out hit)) {				
					Events.OnMouseCollide (hit.transform.gameObject);
					Debug.Log (hit.transform.name);
				}
			}

			if (raycastUI) {
				//GetUIUnderMouse ();
			}
		}

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if (Data.Instance.state == Data.States.playing)
				Events.OnAnyKey ();
			else
				Events.OnKeyRed ();
			//print ("key A");
		}else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (Data.Instance.state == Data.States.playing)
				Events.OnAnyKey ();
			else
				Events.OnKeyYellow ();
			//print ("key S");
		}else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (Data.Instance.state == Data.States.playing)
				Events.OnAnyKey ();
			else
				Events.OnKeyGreen ();
			//print ("key D");
		}else if (Input.GetKeyDown (KeyCode.P)) {
			Events.OnKeyP ();
			//print ("key P");
		}

		if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.R))
		{
			Debug.Log ("Reset pressed");
			Data.Instance.Reset ();
		}

		if (Input.GetMouseButtonUp (0)) {
			if(dragging){
				//GetUIUnderMouse ();
				dragging = false;
			}
		}
	}
	

}
