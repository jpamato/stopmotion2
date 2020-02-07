using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SavedAnims : MonoBehaviour {

	public List<Anim> anims;

	public string saveName = "anim";

	public string nameSeparator = "_";
	public char dataSeparator = '#';
	public string imageFileExtension = ".png";

	public int next2Play;
	public int next2Save;

	[Serializable]
	public class Anim{
		public string id;
		public List<TimelineManager.Frame> timeline;

		public void AddFrame(Texture2D t, int id_){
			TimelineManager.Frame f = new TimelineManager.Frame (t, id_);
			if (timeline == null)
				timeline = new List<TimelineManager.Frame> ();
			timeline.Add (f);
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Anim GetNextAnim(){
		int n = next2Play;
		next2Play++;
		if (next2Play >= anims.Count)
			next2Play = 0;

		return anims [n];
	}

	public void Save(List<TimelineManager.Frame> tl){

		if (anims == null)
			anims = new List<Anim> ();

		if (anims.Count>=Data.Instance.configData.config.maxSavedAnims) {
			anims [next2Save].timeline = tl;
			anims [next2Save].id = ""+next2Save;
			StartCoroutine (Save2Disk(anims [next2Save]));
			next2Save++;
			if (next2Save >= Data.Instance.configData.config.maxSavedAnims)
				next2Save = 0;			
		} else {
			Anim a = new Anim ();
			a.timeline = tl;
			a.id = "" + anims.Count;
			anims.Add (a);
			StartCoroutine (Save2Disk(a));
		}

		next2Play = 0;
	}

	IEnumerator Save2Disk(Anim anim){
		Debug.Log (Application.persistentDataPath);
		string data = Application.persistentDataPath + dataSeparator + anim.id + dataSeparator + anim.timeline.Count;
		PlayerPrefs.SetString (saveName + anim.id, data);
		foreach(TimelineManager.Frame frame in anim.timeline){
			byte[] bytes = frame.tex.EncodeToPNG ();
			string filename = Application.persistentDataPath+"\\"+saveName+nameSeparator+anim.id+nameSeparator+frame.id+imageFileExtension;
			//Debug.Log (filename);
			System.IO.File.WriteAllBytes (filename, bytes);
		}
		yield return null;
	}

	public IEnumerator LoadAnims(){		
		for (int i = 0; i < Data.Instance.configData.config.maxSavedAnims; i++) {
			string data = PlayerPrefs.GetString (saveName + i);
			//Debug.Log (i + ": |"+data+"|");
			if (data == "") {
				i = Data.Instance.configData.config.maxSavedAnims;
			} else {			
				string[] pData = data.Split (dataSeparator);
				//Debug.Log (pData.Length);
				string path = pData [0];
				string animID = pData [1];
				int frameCount = int.Parse (pData [2]);
				Anim a = new Anim ();
				a.id = "" + i;
				for (int j = 0; j < frameCount; j++)
					a.AddFrame (TextureUtils.LoadLocal (path + "\\" + saveName + nameSeparator + i + nameSeparator + j + imageFileExtension), j);

				anims.Add (a);
			}
		}

		if(anims.Count>0)
			Data.Instance.timelineManager.PlaySaved ();

		yield return null;
	}
}
