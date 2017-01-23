using UnityEngine;
using System.Collections;

public class fogofwarmanager : MonoBehaviour {
    MeshFilter mf;
	// Use this for initialization
	void Start () {
        mf = GetComponent<MeshFilter>();
        Mesh m = mf.sharedMesh;
        Vector3[] v = mf.sharedMesh.vertices;
        int[] t = mf.sharedMesh.triangles;
        mf.mesh.Clear();
        mf.mesh.RecalculateNormals();
        mf.mesh.RecalculateBounds();
        Color[] c = new Color[4];
        c[0] = new Color(0, 0, 0, 0f);
        c[1] = new Color(0, 0, 0, 0f);
        c[2] = new Color(1, 0, 0, 0f);
        c[3] = new Color(0, 0, 0f, 1f);
        
        
        mf.mesh.vertices = v;
        Debug.Log(mf.mesh.vertices.Length);
        mf.mesh.triangles = t;
        mf.mesh.colors = c;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
