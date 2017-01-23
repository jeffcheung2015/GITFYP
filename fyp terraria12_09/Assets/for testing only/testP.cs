using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;



public class testP : MonoBehaviour {
    PolygonCollider2D pC2d;
    List<IntPoint> path = new List<IntPoint>();
    // Use this for initialization
    void Start () {
        pC2d = gameObject.GetComponent<PolygonCollider2D>();
        
		vec2List.Add (new Vector2(1,5));
		vec2List.Add (new Vector2(3,5));
		vec2List.Add (new Vector2(3,3));
		vec2List.Add (new Vector2(1,3));
        //vec2List.Add(new Vector2(1, 5));
        //vec2List.Add(new Vector2(3, 5));

        vec2List.Add(new Vector2(3, 7));
        vec2List.Add(new Vector2(5, 7));
        vec2List.Add(new Vector2(5, 9));
        vec2List.Add(new Vector2(3, 9));

        vec2List.Add(new Vector2(3, 9));
        vec2List.Add(new Vector2(5, 9));
        vec2List.Add(new Vector2(5, 11));
        vec2List.Add(new Vector2(3, 11));
        //vec2List.Add(new Vector2(3, 7));
        //vec2List.Add(new Vector2(5, 7));

        vec2List.Add (new Vector2(5,5));
		vec2List.Add (new Vector2(7,5));
		vec2List.Add (new Vector2(7,3));
		vec2List.Add (new Vector2(5,3));
        //vec2List.Add(new Vector2(5, 5));
        //vec2List.Add(new Vector2(7, 5));

        TClipper tcp = new TClipper();
        vec2ListList.Add(vec2List.GetRange(0, 4));
        vec2ListList.Add(vec2List.GetRange(4, 4));
        vec2ListList.Add(new List<Vector2>());
        vec2ListList.Add(vec2List.GetRange(8, 4));
        vec2ListList.Add(vec2List.GetRange(12, 4));

        Debug.Log(vec2ListList.Count);

        float starttime = Time.realtimeSinceStartup; //time counter
        resultList = new List<List<Vector2>>(tcp.UniteCollisionPolygons(vec2ListList));
        float endtime = Time.realtimeSinceStartup;//time counter
        Debug.Log("loading time = " + (endtime - starttime));//time counter

        Debug.Log("@"+resultList.Count);
        pC2d.pathCount = resultList.Count;
        
    }
    List<Vector2> vec2List = new List<Vector2>();
    List<List<Vector2>> vec2ListList = new List<List<Vector2>>();
    List<List<Vector2>> resultList ;
    // Update is called once per frame
    void Update () {    
        
        //Vector2[] vec2arr = ;
        //}
        for (int i = 0; i < resultList.Count; i++)
        {
            pC2d.SetPath(i, resultList[i].ToArray());

        }
        //pC2d.SetPath(0, vec2List.ToArray());
    }
}
