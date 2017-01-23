using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextScript : MonoBehaviour {

	public GameObject gObjActivate1;//pass by reference , only assign it in game window if necessary
	public GameObject gObjActivate2;//pass by reference , only assign it in game window if necessary
	public GameObject gObjActivate3;//pass by reference , only assign it in game window if necessary
	public GameObject gObjActivate4;//pass by reference , only assign it in game window if necessary
	
	public GameObject gObjDeactivate1;//pass by reference , only assign it in game window if necessary
	public GameObject gObjDeactivate2;//pass by reference , only assign it in game window if necessary
	public GameObject gObjDeactivate3;//pass by reference , only assign it in game window if necessary
	public GameObject gObjDeactivate4;//pass by reference , only assign it in game window if necessary

	Text uitext;
	// Use this for initialization
	void Start () {
		uitext = GetComponent<Text> ();

	}

	public void EndLoadingWorld(){

	}

	public void EndGeneratingWorld(){
		
		if (gObjActivate1 != null) {
			gObjActivate1.SetActive (true);
		}
		if (gObjActivate2 != null) {
			gObjActivate2.SetActive (true);
		}
		if (gObjActivate3 != null) {
			gObjActivate3.SetActive (true);
		}
		if (gObjActivate4 != null) {
			gObjActivate4.SetActive (true);
		}
		if (gObjDeactivate1 != null) {
			gObjDeactivate1.SetActive (false);			
		}
		if (gObjDeactivate2 != null) {
			gObjDeactivate2.SetActive (false);
		}
		if (gObjDeactivate3 != null) {
			gObjDeactivate3.SetActive (false);
		}
		if (gObjDeactivate4 != null) {
			gObjDeactivate4.SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
