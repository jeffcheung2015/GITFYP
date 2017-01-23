using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;


public class LoadWorldButtonScript : MonoBehaviour {
	public GameObject worldButPrefab;

	string saveFolderName = "WorldSaves";

    public WorldInfoScript worldInfo;

	List<string> dirName = new List<string>();
	//load the world button from World Save Button
	public void LoadButton(){
		//the string[] get from this one is like in WorldSaves\abc, WorldSaves\abcd but we need abc and abcd
		string[] entries = Directory.GetDirectories (saveFolderName);
		
		int k = 0;
		//this one split the entries and only get the last one token
		foreach (string ss in entries) {
            if (ss != "worldInfo.bin")
            {
                string[] s = ss.Split('\\');
                if (!dirName.Contains(s[1]))
                {
                    dirName.Add(s[1]);
                }
                else
                {
                    k++;
                }
            }
            else
            {

            }
		}
		int j = 0;
		foreach (string x in dirName) {
			if(j>=k){
				GameObject bGObj = Instantiate (worldButPrefab) as GameObject;//world button game object
				bGObj.transform.SetParent (gameObject.transform, false);
				bGObj.name = x;
					
				RectTransform rt = bGObj.GetComponent<RectTransform> ();
			
				Text buttonText = bGObj.GetComponentInChildren<Text> ();
				buttonText.text = x;
			
				rt.anchorMax = new Vector2(0.5f,0.5f);
				rt.anchorMin = new Vector2(0.5f,0.5f);
				//set the anchor pos of the new world button
				rt.anchoredPosition = new Vector2 (0, 25f-j*30f);  //1024*768

			}
			j++;
		}
	}



	// Use this for initialization
	void Start () {
		//called here first and then called whenever the world is generated to update the list of the world
		LoadButton ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
