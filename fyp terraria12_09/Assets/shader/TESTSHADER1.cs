using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TESTSHADER1 : MonoBehaviour {
    List<Vector3> vertList = new List<Vector3>();
    List<int> triList = new List<int>();
    List<Vector2> uvList = new List<Vector2>();
    List<Color> colors = new List<Color>();

    MeshFilter mf;
    MeshRenderer mr;
    // Use this for initialization
    
    void Start () {
        mf = gameObject.GetComponent<MeshFilter>();
        mr = gameObject.GetComponent<MeshRenderer>();

        for(int i = 0; i <= 6; i++)
        {
            for (int j = 0; j <= 6; j++)
            {
                vertList.Add(new Vector3(i/2, j/2, 1));
                //uvList.Add(Vector2.one);
                colors.Add(new Color(0.9f, 0, 0));
            }
        }
        int v = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {                
                if((v+1)%7 == 0)
                {
                    v++;
                }
                
                triList.Add(v);
                triList.Add(v + 1);
                triList.Add(v + 6 + 2);
                triList.Add(v + 6 + 1);
                triList.Add(v);
                triList.Add(v + 6 + 2);
                v += 1;
            }
        }
        Debug.Log(triList.Count + "<"+ vertList.Count);
        

        mf.mesh.Clear();
        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();
        mf.mesh.vertices = vertList.ToArray();
        mf.mesh.triangles = triList.ToArray();
        //mf.mesh.uv = uvList.ToArray();
        mf.mesh.colors = colors.ToArray();
        //mf.mesh.SetColors(colors);

        /*
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                vertList.Add(new Vector3(i * 16, j * 16, 0));
                vertList.Add(new Vector3((i + 1) * 16, j * 16, 0));
                vertList.Add(new Vector3(i * 16, (j + 1) * 16, 0));
                vertList.Add(new Vector3((i + 1) * 16, (j + 1) * 16, 0));
            }
        }
        int v = 0;
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                triList.Add(v);
                triList.Add(v + 2);
                triList.Add(v + 3);
                triList.Add(v + 3);
                triList.Add(v + 1);
                triList.Add(v);
                uvList.Add(new Vector2(0, 2 * 0.39f / 396));
                uvList.Add(new Vector2(16 * 0.14f / 288, 2 * 0.39f / 396));
                uvList.Add(new Vector2(0, 18 * 0.39f / 396));
                uvList.Add(new Vector2(16 * 0.14f / 288, 18 * 0.39f / 396));
                v += 4;
            }
        }

        mf.mesh.Clear();
        mf.mesh.RecalculateBounds();
        mf.mesh.vertices = vertList.ToArray();
        mf.mesh.triangles = triList.ToArray();
        mf.mesh.uv = uvList.ToArray();
        */
    }

   
    // Update is called once per frame
    void Update () {
	
	}
}
