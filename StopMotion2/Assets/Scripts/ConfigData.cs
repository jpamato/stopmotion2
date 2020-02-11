using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class ConfigData : MonoBehaviour {

	public string filename="config.json";
	public Config config;

	[Serializable]
	public class Config
	{
		public int framerate;
		public int maxFrames;
		public int lastAnimPlayTimes;
		public int loopPlayTimes;

		public int maxSavedAnims;
		public int minFrames2Save;
		public int cropHeightPx;

        public int teaserPause;
    }


	// Use this for initialization
	void Start () {
		//config = JsonUtility.FromJson (Application.streamingAssetsPath + filename);

		string filePath = Path.Combine (Application.streamingAssetsPath + "/", filename);

		if (File.Exists (filePath)) {
			string dataAsJson = File.ReadAllText (filePath);
			config = JsonUtility.FromJson<Config> (dataAsJson);
		}

		StartCoroutine (Data.Instance.savedAnims.LoadAnims());

		Data.Instance.timelineManager.CreateThumbs ();
		Events.OnConfig ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
