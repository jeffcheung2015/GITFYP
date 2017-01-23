using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class testcombine : MonoBehaviour {

    List<Vector3> vertList = new List<Vector3>();
    List<Vector2> uvList = new List<Vector2>();
    List<int> tri = new List<int>();


    MeshRenderer[] mr;
    MeshFilter[] mf;


    // Use this for initialization
    void Start () {
        mr = GetComponentsInChildren<MeshRenderer>();
        mf = GetComponentsInChildren<MeshFilter>();
        
        vertList.Add(new Vector3(2, 2, 1));
        vertList.Add(new Vector3(12, 2, 1));
        vertList.Add(new Vector3(2, 12, 1));
        vertList.Add(new Vector3(12, 12, 1));

        uvList.Add(new Vector2(0, 0));
        uvList.Add(new Vector2(0.5f, 0));
        uvList.Add(new Vector2(0, 0.5f));
        uvList.Add(new Vector2(0.5f, 0.5f));

        tri.Add(0);
        tri.Add(2);
        tri.Add(3);
        tri.Add(3);
        tri.Add(1);
        tri.Add(0);


        mf[1].mesh.Clear();
        mf[1].mesh.RecalculateBounds();

        mf[1].mesh.vertices = vertList.ToArray();
        mf[1].mesh.uv = uvList.ToArray();
        mf[1].mesh.triangles = tri.ToArray();



    }
	
	// Update is called once per frame
	void Update () {
        
	}
}
