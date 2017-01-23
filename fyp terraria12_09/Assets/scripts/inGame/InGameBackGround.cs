using UnityEngine;
using System.Collections;

public class InGameBackGround : MonoBehaviour {
	//public GameObject playerGO;
	public Vector3 playerOpos;
	public float speedFactor;

	void Start () {
		//Debug.Log (playerGO.transform.position);
		playerOpos = transform.parent.parent.GetComponent<Rigidbody2D> ().position;
	}
	

	void Update () {
		Vector3 pos = transform.parent.parent.GetComponent<Rigidbody2D> ().position;
		GetComponent<MeshRenderer> ().material.mainTextureOffset =
			new Vector2 (speedFactor*(pos.x - playerOpos.x) / 1500, 0f);
	}
}
