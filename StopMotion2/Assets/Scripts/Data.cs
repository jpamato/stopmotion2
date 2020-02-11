using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Data : MonoBehaviour
{

    public bool isArcade;
	public Vector2 defaultCamSize = new Vector2 (1280, 768);

	public InputManager inputManager;
	public TimelineManager timelineManager;
	public ConfigData configData;
	public SavedAnims savedAnims;

    const string PREFAB_PATH = "Data";
    
    static Data mInstance = null;

	public States state;
	public enum States{
		live,
		frame,
		playing,
        teaser
	}

	public static Data Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<Data>();

                if (mInstance == null)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>(PREFAB_PATH)) as GameObject;
                    mInstance = go.GetComponent<Data>();
                }
            }
            return mInstance;
        }
    }
    public string currentLevel;
    public void LoadScene(string aLevelName)
    {
        this.currentLevel = aLevelName;
        Time.timeScale = 1;
        SceneManager.LoadScene(aLevelName);
    }

    void Awake()
    {
		QualitySettings.vSyncCount = 1;

        if (!mInstance)
            mInstance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        if(isArcade)
            Cursor.visible = false;

        DontDestroyOnLoad(this.gameObject);

		inputManager = GetComponent<InputManager> ();
		timelineManager = GetComponent<TimelineManager> ();
		//config = JsonUtility.FromJson<Config> ();
		configData = GetComponent<ConfigData> ();
		savedAnims = GetComponent<SavedAnims> ();

    }
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

	public void Reset(){
		PlayerPrefs.DeleteAll ();
		Data.Instance.Exit ();
	}

	public void Exit(){
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#elif UNITY_WEBPLAYER
		Application.OpenURL(webplayerQuitURL);
		#else
		Application.Quit();
		#endif
	}
}
