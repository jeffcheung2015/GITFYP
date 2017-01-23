using UnityEngine;
using System.Collections;

public class MovingTransformScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	public float speed;
	// Update is called once per frame
	void FixedUpdate () {
        transform.Translate(new Vector3(speed, 0),Space.World);

	}
	void Update(){
		if (transform.localPosition.x >= 1100) {			
			transform.localPosition = new Vector3 (-1100,transform.localPosition.y,transform.localPosition.z);
		}
	}
}
