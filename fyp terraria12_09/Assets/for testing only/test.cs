using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimplexNoise;

public class test : MonoBehaviour {

	MeshRenderer mr;
	MeshFilter mf;
	public List<Vector3> vertices = new List<Vector3>();
	public List<int> triangles = new List<int>();
	public List<Vector2> uv = new List<Vector2>(); 


	// Use this for initialization
	void Start () {
		//mr = GetComponent<MeshRenderer> ();
		mf = GetComponent<MeshFilter> ();
		vertices.Add (new Vector3(0,0,0));
		vertices.Add (new Vector3(0,16f,0));
		vertices.Add (new Vector3(0,16f,0));
		vertices.Add (new Vector3(0,32f,0));
		vertices.Add (new Vector3(16f,0,0));
		vertices.Add (new Vector3(16f,16f,0));
		vertices.Add (new Vector3(16f,16f,0));
		vertices.Add (new Vector3(16f,32f,0));
		
		uv.Add (new Vector2 (2f/180f,2f/162f));
		uv.Add (new Vector2 (2f/180f,18f/162f));
		uv.Add (new Vector2 (2f/180f,20f/162f));
		uv.Add (new Vector2 (2f/180f,36f/162f));
		uv.Add (new Vector2 (18f/180f,2f/162f));
		uv.Add (new Vector2 (18f/180f,18f/162f));
		uv.Add (new Vector2 (18f/180f,20f/162f));
		uv.Add (new Vector2 (18f/180f,36f/162f));

		
		triangles.Add (0);
		triangles.Add (1);
		triangles.Add (5);
		triangles.Add (4);
		triangles.Add (0);
		triangles.Add (5);
		triangles.Add (2);
		triangles.Add (3);		
		triangles.Add (7);
		triangles.Add (6);
		triangles.Add (2);
		triangles.Add (7);
		
		mf.mesh.Clear ();
		mf.mesh.vertices = vertices.ToArray();
		mf.mesh.uv = uv.ToArray();
		mf.mesh.triangles = triangles.ToArray();

		int chunkSize = 4;
		int k = 0;
		int x = 0;
		int y = 0;

		for (int i = 0; i < chunkSize*chunkSize; i++) {
			if(i != 0){
				if(i%(chunkSize)==0 && i!=0){
					k += 2*(chunkSize+1);
				}else{
					k += 2;
				}
			}
			if(i%(chunkSize)==0 && i!=0){
				x++;
			}

			Debug.Log ("x="+x+" ,y="+y+",i:"+i+", k="+k);
			//Debug.Log ("k+2:"+(k+2));
			y++;
			if((k+2)%(chunkSize*2)==0){
				y=0;
			}
			//yield return null;
		}
	}

	float modiNoise(int x, int y, float frequency, int maxAmplitude){
		return Mathf.FloorToInt ((Noise.Generate (x * frequency, y * frequency)+1f) *(maxAmplitude/2f));
	}
	
	// Update is called once per frame
	void Awake () {
//		Debug.Log(modiNoise ((int)0, (int)0, 0.3f, 70));

		/*for (int i = 0; i < 32; i++) {
			for (int j = 0; j < 32; i++) {
				vertices.Add (new Vector3 (i * 32, j * 32, 0f));
			}
		}
		for(int i = 0; i < 32; i++){
			for(int j = 0; j < 32; i++){

			}
		}*/
					
	}
}
