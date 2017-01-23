using UnityEngine;
using System.Collections;

public class ScrollScript : MonoBehaviour {
    public float scrollSpeed;
    public bool scrollToRight;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        int rightOrLeft = scrollToRight ? -1 : 1;
        GetComponent<Renderer>().material.mainTextureOffset =
            new Vector2(Time.time * scrollSpeed * rightOrLeft, 0f);
	}
}
