using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Testshader : MonoBehaviour {
    List<Vector3> vertList = new List<Vector3>();
    List<int> triList = new List<int>();
    List<Vector2> uvList = new List<Vector2>();
    List<Color32> colorList = new List<Color32>();

    MeshFilter mf;
    MeshRenderer mr;

    Material m;

    void Awake()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                vertList.Add(new Vector3(i * 16, j * 16, 0));
                vertList.Add(new Vector3(16 * (i + 2), j * 16, 0));
                vertList.Add(new Vector3(i * 16, 16 * (j + 2), 0));
                vertList.Add(new Vector3(16 * (i + 2), 16 * (j + 2), 0));
                /*
                vertList.Add(new Vector3(i * 32, j * 32, 0));
                vertList.Add(new Vector3(32 * (i + 1), j * 32, 0));
                vertList.Add(new Vector3(i * 32, 32 * (j + 1), 0));
                vertList.Add(new Vector3(32 * (i + 1), 32 * (j + 1), 0));
                */
                //vertList.Add(new Vector3((i) * 16 + 8, (j) * 16 + 8, 0));

                if ((i + j) % 5 == 0)
                {
                    colorList.Add(new Color32(0, 0, 0, 255));
                    colorList.Add(new Color32(0, 0, 0, 255));
                    colorList.Add(new Color32(0, 0, 0, 255));
                    colorList.Add(new Color32(0, 0, 0, 255));
                }
                else if ((i + j) % 5 == 1)
                {
                    colorList.Add(new Color32(51, 51, 51, 255));
                    colorList.Add(new Color32(51, 51, 51, 255));
                    colorList.Add(new Color32(51, 51, 51, 255));
                    colorList.Add(new Color32(51, 51, 51, 255));
                }
                else if ((i + j) % 5 == 2)
                {
                    colorList.Add(new Color32(102, 102, 102, 255));
                    colorList.Add(new Color32(102, 102, 102, 255));
                    colorList.Add(new Color32(102, 102, 102, 255));
                    colorList.Add(new Color32(102, 102, 102, 255));
                }
                else if ((i + j) % 5 == 3)
                {
                    colorList.Add(new Color32(153, 153, 153, 255));
                    colorList.Add(new Color32(153, 153, 153, 255));
                    colorList.Add(new Color32(153, 153, 153, 255));
                    colorList.Add(new Color32(153, 153, 153, 255));
                }
                else if ((i + j) % 5 == 4)
                {
                    colorList.Add(new Color32(204, 204, 204, 255));
                    colorList.Add(new Color32(204, 204, 204, 255));
                    colorList.Add(new Color32(204, 204, 204, 255));
                    colorList.Add(new Color32(204, 204, 204, 255));
                }
                else
                {
                    colorList.Add(new Color32(255, 255, 255, 255));
                    colorList.Add(new Color32(255, 255, 255, 255));
                    colorList.Add(new Color32(255, 255, 255, 255));
                    colorList.Add(new Color32(255, 255, 255, 255));
                }
                //colorList.Add(new Color(0, 0, 0, 1));
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

                /*
                triList.Add(v);
                triList.Add(v + 2);
                triList.Add(v + 4);
                triList.Add(v + 4);
                triList.Add(v + 2);
                triList.Add(v + 3);

                triList.Add(v + 4);
                triList.Add(v + 3);
                triList.Add(v + 1);
                triList.Add(v + 4);
                triList.Add(v + 1);
                triList.Add(v);
                 */
                uvList.Add(WallUV.uvList[14, 0] + StoneBackWall.leftBotUV);
                uvList.Add(WallUV.uvList[14, 1] + StoneBackWall.leftBotUV);
                uvList.Add(WallUV.uvList[14, 2] + StoneBackWall.leftBotUV);
                uvList.Add(WallUV.uvList[14, 3] + StoneBackWall.leftBotUV);

                    
                v += 4;
            }
        }

        mf.mesh.Clear();
        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();
        mf.mesh.vertices = vertList.ToArray();
        mf.mesh.triangles = triList.ToArray();
        
        mf.mesh.uv = uvList.ToArray();
        //mf.mesh.colors32 = colorList.ToArray();
        //mr.material.SetFloat("abc", 1f);
    }

    void Start () {
	    
	}

    void Update () {
	
	}
}
