using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogScript : MonoBehaviour {

	public GameObject chunkGO;
	MeshFilter mf;
	MeshRenderer mr;
	Chunk chunk;

	void Awake(){
		chunk = chunkGO.GetComponent<Chunk> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	bool fogVertIsSet = false;
	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();
	List<Vector2> uv = new List<Vector2>();

	void Update () {
		if (chunk.isLoaded == true && fogVertIsSet == true) {
			fogVertIsSet = true;

			for(int i = 0; i < Chunk.chunkSize; i++){
				for(int j = 0; j < Chunk.chunkSize; j++){
					
				}
			}

		}
	}



}
