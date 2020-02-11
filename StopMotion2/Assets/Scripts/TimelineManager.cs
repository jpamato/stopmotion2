using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimelineManager : MonoBehaviour {

	public GameObject editMode,playMode,teaserMode;

	public Texture2D currentFrame;
	public List<Frame> timeline;
	public List<FrameBtn> thumbs;

	public GameObject btnContainer;
	public GameObject frameBtn;

	public RawImage playMonitor,liveMonitor;


	public Image frame3, frame2, frame1;

	public ScrollRect scrollRect;
	public Scrollbar scrollbar;

	public float speed;
	float time;

	int framesIdCount;
	int cabezal;

	bool playingLast;
	int playingTimes;

	public int selId;

    int teaserPauseCount;

	[Serializable]
	public class Frame{
		public Texture2D tex;
		public int id;

		public Frame(Texture2D t, int id_){
			tex = t;
			id = id_;
		}

		public Frame(Frame f){
			tex = f.tex;
			id = f.id;
		}
	}

	public void SaveFrame(WebCamTexture webTex){
		if (framesIdCount >= Data.Instance.configData.config.maxFrames)
			return;
		int size = webTex.height - Data.Instance.configData.config.cropHeightPx;
		currentFrame = new Texture2D(size,size);
		int x0 = (int)(webTex.width * 0.5f - size * 0.5);
		int y0 = (int)(webTex.height * 0.5f - size * 0.5);
		currentFrame.SetPixels(webTex.GetPixels(x0,y0,currentFrame.width,currentFrame.height));
		currentFrame.Apply ();
		timeline.Add(new Frame(currentFrame,framesIdCount));

		FrameBtn fb = thumbs.Find (x => x.id == framesIdCount);
		Sprite sprite = Sprite.Create (currentFrame, new Rect (0, 0, currentFrame.width, currentFrame.height), Vector2.zero);
		fb.SetSprite (sprite);

		/*GameObject btn = Instantiate (frameBtn, btnContainer.transform);
		RectTransform btnFt = frameBtn.transform as RectTransform;
		if (framesIdCount * btnFt.sizeDelta.x > Screen.width*0.5f) {
			RectTransform rt = btnContainer.transform as RectTransform;
			rt.sizeDelta = new Vector2 (rt.sizeDelta.x + btnFt.sizeDelta.x, rt.sizeDelta.y);
		}

		FrameBtn fb = btn.GetComponent<FrameBtn> ();
		//Sprite sprite = new Sprite ();
		Sprite sprite = Sprite.Create (currentFrame, new Rect (0, 0, currentFrame.width, currentFrame.height), Vector2.zero);
		fb.Create (sprite, framesIdCount);*/

		framesIdCount++;
		SetScroll ();

		SetLastFramesMonitor ();
	}

	void SetLastFramesMonitor(){
		if (timeline.Count > 1) {
			frame3.transform.parent.gameObject.SetActive (true);
			frame3.sprite = Sprite.Create (timeline [timeline.Count - 2].tex, new Rect (0, 0, currentFrame.width, currentFrame.height), Vector2.zero);
		} else {
			frame3.transform.parent.gameObject.SetActive (false);
		}
		if (timeline.Count > 0) {
			frame2.transform.parent.gameObject.SetActive (true);
			frame2.sprite = Sprite.Create (timeline [timeline.Count - 1].tex, new Rect (0, 0, currentFrame.width, currentFrame.height), Vector2.zero);
		} else {
			frame2.transform.parent.gameObject.SetActive (false);
		}
	}

	void Init(){
		editMode.SetActive (true);
		for (int i = 0; i < thumbs.Count; i++)
			thumbs [i].SetSprite (null);

		timeline = new List<Frame> ();
		frame3.sprite = null;
		frame2.sprite = null;
		frame3.transform.parent.gameObject.SetActive (false);
		frame2.transform.parent.gameObject.SetActive (false);
		framesIdCount = 0;
		playMode.SetActive (false);
		SetScroll ();

		Data.Instance.state = Data.States.live;

	}

	// Use this for initialization
	void Start () {
		Events.ShowFrame += ShowFrame;
		Events.OnKeyGreen += Terminar;
		Events.OnKeyYellow+= Stop;
		Events.OnKeyRed+= Delete;
		Events.OnAnyKey += Init;
        Events.TeaserAnimEnds += TeaserAnimEnds;

        selId = -1;

	}

	public void CreateThumbs(){
		for (int i = 0; i < Data.Instance.configData.config.maxFrames; i++)
			CreateEmptyThumb (i);
	}

	void CreateEmptyThumb(int id){
		GameObject btn = Instantiate (frameBtn, btnContainer.transform);
		RectTransform btnFt = frameBtn.transform as RectTransform;
		if (id * btnFt.sizeDelta.x > Screen.width*0.5f) {
			RectTransform rt = btnContainer.transform as RectTransform;
			rt.sizeDelta = new Vector2 (rt.sizeDelta.x + btnFt.sizeDelta.x*0.55f, rt.sizeDelta.y);
		}

		FrameBtn fb = btn.GetComponent<FrameBtn> ();
		fb.Create (null, id);
		thumbs.Add (fb);
	}

	void OnDestroy(){
		Events.ShowFrame -= ShowFrame;
		Events.OnKeyP-= Terminar;
		Events.OnKeyYellow-= Stop;
		Events.OnKeyRed-= Delete;
	}
	
	// Update is called once per frame
	void Update () {
		if (Data.Instance.state == Data.States.playing) {
			if (time >= speed) {
				playMonitor.texture = timeline [cabezal].tex;
				cabezal++;
                if (cabezal >= timeline.Count) {
                    cabezal = 0;
                    if (teaserPauseCount < Data.Instance.configData.config.teaserPause) {
                        if (playingLast) {
                            if (playingTimes < Data.Instance.configData.config.lastAnimPlayTimes) {
                                playingTimes++;
                                teaserPauseCount++;
                            } else {
                                playingLast = false;
                                playingTimes = 0;
                                teaserPauseCount++;
                                timeline = Data.Instance.savedAnims.GetNextAnim().timeline;
                            }
                        } else {
                            if (playingTimes < Data.Instance.configData.config.loopPlayTimes) {
                                playingTimes++;
                                teaserPauseCount++;
                            } else {
                                playingTimes = 0;
                                teaserPauseCount++;
                                timeline = Data.Instance.savedAnims.GetNextAnim().timeline;
                            }

                        }
                    } else {
                        teaserPauseCount = 0;
                        playingTimes = 0;
                        timeline = Data.Instance.savedAnims.GetNextAnim().timeline;
                        playMode.SetActive(false);
                        teaserMode.SetActive(true);
                        Data.Instance.state = Data.States.teaser;
                    }
                } 
				time = 0;
			} else {
				time += Time.deltaTime;
			}
		}
	}

	public void Terminar(){
		if (timeline.Count >= Data.Instance.configData.config.minFrames2Save) {
			playMode.SetActive (true);
			editMode.SetActive (false);
			selId = -1;

			Data.Instance.savedAnims.Save (timeline);

			cabezal = 0;
			time = 0;
			playMonitor.texture = timeline [cabezal].tex;
			playingLast = true;
			Data.Instance.state = Data.States.playing;
		}
	}

	public void PlaySaved(){
		playMode.SetActive (true);
		editMode.SetActive (false);
		playingTimes = 0;
		timeline = Data.Instance.savedAnims.GetNextAnim ().timeline;
		Data.Instance.state = Data.States.playing;
	}

    void TeaserAnimEnds() {
        teaserMode.SetActive(false);
        playMode.SetActive(true);
        playingTimes = 0;
        Data.Instance.state = Data.States.playing;
    }

    public void Stop(){
		selId = -1;
		Data.Instance.state = Data.States.live;	
	}

	void SetScroll(){
		scrollbar.value = 1f*framesIdCount / Data.Instance.configData.config.maxFrames;
		scrollRect.horizontalNormalizedPosition = scrollbar.value;
	}

	public void ShowFrame(int id){
		selId = id;
		Data.Instance.state = Data.States.frame;
		liveMonitor.texture = timeline.Find (x => x.id == id).tex;
	}

	public void Delete(){
		if (timeline.Count < 1)
			return;
		if (selId == -1) {		
			selId = timeline [timeline.Count - 1].id;
			framesIdCount = selId;
			SetScroll ();
		}

		Frame f = timeline.Find (x => x.id == selId);
		timeline.Remove (f);

		SetLastFramesMonitor ();

		FrameBtn fb = thumbs.Find (x => x.id == selId);
		fb.SetSprite (null);

		/*FrameBtn[] btns = btnContainer.GetComponentsInChildren<FrameBtn> ();
		foreach(FrameBtn fb in thumbs){
			if (fb.id == selId)
				Destroy (fb.gameObject);
		}*/

		selId = -1;
	}

	public void DeleteAll(){		
		timeline.Clear ();
		selId = -1;
	}
}
