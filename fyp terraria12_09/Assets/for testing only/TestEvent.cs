using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestEvent : MonoBehaviour {
	MeshFilter mf;
	MeshRenderer mr;

	// Use this for initialization

	List<Vector3> vertList = new List<Vector3>();
    List<int> triList = new List<int>();
    List<int> triList1 = new List<int>();
    List<int> triList2 = new List<int>();

    List<Vector2> uvList = new List<Vector2>();
    List<Vector2> uvList1 = new List<Vector2>();
    bool isdrew = false;

    public Texture2D[] t;

    Texture2D packT;
    Texture2D packT1;


    Rect[] uvRect; Rect[] uvRect1;

    //public void Start() {
    /*
    packT = new Texture2D(512,512);//no matter how large it is, it will grow according to the final size
    packT1 = new Texture2D(512, 512);//no matter how large it is, it will grow according to the final size

    Texture2D[] tc = new Texture2D[30];
    Texture2D[] tc1 = new Texture2D[30];

    int cnt = 0;
    for(int i = 0; i < tc.Length; i++)
    {
        cnt++;
        tc[i] = t[i];
    }
    for(int i = 0; i < t.Length - cnt; i++)
    {
        tc1[i] = t[cnt];
        cnt++;
    }

    uvRect = packT.PackTextures(tc, 0);
    uvRect1 = packT1.PackTextures(tc1, 0);

    for (int i = 0; i < uvRect.Length; i++)
    {
        Debug.Log(uvRect[i]);
    }
    for (int i = 0; i < uvRect1.Length; i++)
    {
        Debug.Log(uvRect1[i]);
    }

    byte[] bytes = packT.EncodeToPNG();
    Debug.Log(packT.width+","+ packT.height);
    string path = System.IO.Path.Combine("./", "aa.jpg");
    System.IO.File.WriteAllBytes(path, bytes);
    Debug.Log(path);

    mr.material.mainTexture = packT;
    */
    public MeshFilter[] ms;

    //}
    List<Vector2> uvList2 = new List<Vector2>();
    public void Awake() {
        mf = gameObject.GetComponent<MeshFilter>();
        mr = gameObject.GetComponent<MeshRenderer>();

        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {

                vertList.Add(new Vector3(i * 16, j * 16, 0));
                vertList.Add(new Vector3((i + 1) * 16, j * 16, 0));
                vertList.Add(new Vector3(i * 16, (j + 1) * 16, 0));
                vertList.Add(new Vector3((i + 1) * 16, (j + 1) * 16, 0));

            }
        }
        int v = 0;
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {


                uvList2.Add(new Vector2(0, 0));
                uvList2.Add(new Vector2(1, 0));
                uvList2.Add(new Vector2(0, 1));
                uvList2.Add(new Vector2(1, 1));

                triList2.Add(v);
                triList2.Add(v + 2);
                triList2.Add(v + 3);
                triList2.Add(v + 3);
                triList2.Add(v + 1);
                triList2.Add(v);

                if (i % 2 == 0 && j % 2 == 0)
                {
                    triList.Add(v);
                    triList.Add(v + 2);
                    triList.Add(v + 3);
                    triList.Add(v + 3);
                    triList.Add(v + 1);
                    triList.Add(v);

                    uvList.Add(new Vector2(0, 0));
                    uvList.Add(new Vector2(1, 0));
                    uvList.Add(new Vector2(0, 1));
                    uvList.Add(new Vector2(1, 1));
                }
                else
                {
                    triList1.Add(v);
                    triList1.Add(v + 2);
                    triList1.Add(v + 3);
                    triList1.Add(v + 3);
                    triList1.Add(v + 1);
                    triList1.Add(v);

                    uvList1.Add(new Vector2(0, 0));
                    uvList1.Add(new Vector2(1, 0));
                    uvList1.Add(new Vector2(0, 1));
                    uvList1.Add(new Vector2(1, 1));
                }

                v += 4;
            }
        }

        mf.mesh.Clear();

        //mf.mesh.vertices = vertList.ToArray();
        //mf.mesh.triangles = triList2.ToArray();
        //mf.mesh.uv = uvList2.ToArray();


        mf.mesh.subMeshCount = 2;//automatically use different materials

        mf.mesh.SetVertices(vertList);//common vertices

        mf.mesh.SetTriangles(triList, 0);//but tri need to be separated into two 
        mf.mesh.SetTriangles(triList1, 1);

        mf.mesh.uv = uvList2.ToArray();    //uv just use the normal one is enough



    }
    void Start()
    {
        //float t = Time.realtimeSinceStartup;
        //CallStart(t);
        

    }
    List<CombineInstance> ci = new List<CombineInstance>();
    List<CombineInstance> ci1 = new List<CombineInstance>();

    void CallStart(float t)
    {
        if (gameObject.tag == "1")
        {
            for(int i = 0; i < ms.Length; i++)//divide those submesh 
            {
                
                CombineInstance c = new CombineInstance();                                                
                c.mesh = new Mesh();
                CombineInstance c1 = new CombineInstance();
                c1.mesh = new Mesh();
                                
                c.mesh.vertices = ms[i].sharedMesh.vertices;
                c1.mesh.vertices = ms[i].sharedMesh.vertices;

                c.mesh.triangles = ms[i].sharedMesh.GetTriangles(0);
                c1.mesh.triangles = ms[i].sharedMesh.GetTriangles(1);
                
                c.mesh.uv = ms[i].sharedMesh.uv;
                c.transform = ms[i].transform.localToWorldMatrix;

                c1.mesh.uv = ms[i].sharedMesh.uv;
                c1.transform = ms[i].transform.localToWorldMatrix;

                ci.Add(c);

                ci1.Add(c1);

            }

            //Mesh m = new Mesh();
            //m.CombineMeshes(ci.ToArray());

            //Mesh m1 = new Mesh();
            //m1.CombineMeshes(ci1.ToArray());
            Debug.Log(Time.realtimeSinceStartup - t);
            CombineInstance[] m2 = new CombineInstance[2];
            m2[0].mesh = new Mesh();
            m2[0].mesh.CombineMeshes(ci.ToArray());
            m2[0].transform = transform.localToWorldMatrix;
            //m2[1].mesh = m1;
            m2[1].mesh = new Mesh();
            m2[1].mesh.CombineMeshes(ci1.ToArray());
            m2[1].transform = transform.localToWorldMatrix;
            
            mf.mesh.CombineMeshes(m2, false);
            Debug.Log(Time.realtimeSinceStartup - t);

        }
    }

}
