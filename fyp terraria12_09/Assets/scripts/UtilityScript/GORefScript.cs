using UnityEngine;
using System.Collections;

public class GORefScript : MonoBehaviour {
	public GameObject go1;
	public GameObject go2;
	public GameObject go3;
	public GameObject go4;
	public GameObject go5;
	public GameObject go6;
	public GameObject go7;
	public GameObject go8;
	public GameObject go9;

	public static GameObject gos1;//main menu page
	public static GameObject gos2;//singlePlayer page
	public static GameObject gos3;//createNewWorld page
	public static GameObject gos4;//singlePlayerWorldPage
	public static GameObject gos5;//worldGenTextPage
	public static GameObject gos6;//progress
	public static GameObject gos7;//loadingWorldPage
	public static GameObject gos8;
	public static GameObject gos9;

	void Awake(){
		if (go1 != null) {
			gos1 = go1;
		}
		if (go2 != null) {
			gos2 = go2;
		}
		if (go3 != null) {
			gos3 = go3;
		}
		if (go4 != null) {
			gos4 = go4;
		}
		if (go5 != null) {
			gos5 = go5;
		}
		if (go6 != null) {
			gos6 = go6;
		}
		if (go7 != null) {
			gos7 = go7;
		}
		if (go8 != null) {
			gos8 = go8;
		}
		if (go9 != null) {
			gos9 = go9;
		}

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
