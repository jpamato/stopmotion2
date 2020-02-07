using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameBtn : MonoBehaviour {

	public int id;
	public Image image;

	public Color emptyColor;

	// Use this for initialization
	void Start () {
		//image = GetComponent<Image> ();
	}

	public void Create(Sprite s, int _id){
		if (s != null) {
			if (image == null)
				image = GetComponent<Image> ();

			image.sprite = s;
			image.color = Color.white;
		}
		id = _id;
	}

	public void SetSprite(Sprite s){
		if (image == null)
			image = GetComponent<Image> ();
		if (s != null) {
			image.sprite = s;
			image.color = Color.white;
		} else {
			image.sprite = s;
			image.color = emptyColor;
		}
	}

	public void ShowFrame(){
		Events.ShowFrame (id);
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}

