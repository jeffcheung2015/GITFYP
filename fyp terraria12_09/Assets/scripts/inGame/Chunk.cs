using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Chunk : MonoBehaviour
{
    public Block[,] blocks = new Block[chunkSize, chunkSize];
    public Dictionary<int, Complex> complexs = new Dictionary<int, Complex>();//blocks that are composed of multiple blocks

    SortedDictionary<int, Vector3[]> vertDict = new SortedDictionary<int, Vector3[]>();
    SortedDictionary<int, int[]> triDict = new SortedDictionary<int, int[]>();
    SortedDictionary<int, Vector2[]> uvDict = new SortedDictionary<int, Vector2[]>();
    Dictionary<int, int> triDictSubIndex = new Dictionary<int, int>();

    SortedDictionary<int, Vector3[]> wallVertDict = new SortedDictionary<int, Vector3[]>();
    SortedDictionary<int, int[]> wallTriDict = new SortedDictionary<int, int[]>();
    SortedDictionary<int, Vector2[]> wallUVDict = new SortedDictionary<int, Vector2[]>();


    //Dictionary<int, Vector2[]> collDict = new Dictionary<int, Vector2[]> ();
    //Dictionary<int, Vector2[]> triDict = new Dictionary<int, Vector2[]> ();

    //SortedDictionary<int, Vector2[]> collSDict = new SortedDictionary<int, Vector2[]> ();
    //SortedDictionary<int, Vector2[]> triSDict = new SortedDictionary<int, Vector2[]> ();
    //blocks need 16 different tiles sprite

    //Lighting system region
    SortedDictionary<int, Color32[]> wallColorDict = new SortedDictionary<int, Color32[]>();
    SortedDictionary<int, Color32[]> colorDict = new SortedDictionary<int, Color32[]>();
    //Color32[] colorArray = new Color32[chunkSize*chunkSize];
    public byte lightWorldFactor;
    public byte[] lightAmount = new byte[chunkSize * chunkSize];
    //

    public static int chunkSize = 16;
    public static int tileSize = 16;

    public bool update = false;
    public bool rendered;

    MeshFilter filter;
    MeshCollider coll;

    public bool isDrew = false;
    public bool isLoaded = false;

    public int movingBlocks = 0;

    //public bool debug = false;

    public World world;//assigned in world.cs's CreateChunk function! and world.cs has only one instance in scene

    public int[] pos;//it is set to the chunk's correct world position on the CreateChunk() in world.cs
    MeshRenderer mr;
    Material[] ms;

    List<Vector3> chunkVertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uv = new List<Vector2>();

    List<List<Vector2>> paths = new List<List<Vector2>>();
    List<List<Vector2>> paths1 = new List<List<Vector2>>();

    HashSet<Block> movableHashSet = new HashSet<Block>();
    
    //future work remove whole mesh collider verts with new mesh => replace part of the mesh collider verts

    public bool hasChanged = false;//will only save the chunk that has changed

    //the vector3 to set vertices are already in local coord,
    //so no pos[0] and pos[1] added in the vector3 component is needed 
    PolygonCollider2D[] pc2DList;
    PolygonCollider2D pc2D;
    PolygonCollider2D pc2D1;

    int vertCount = 0;
    int wallVertCount = 0;

    void Awake()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        mr = gameObject.GetComponent<MeshRenderer>();
        pc2DList = gameObject.GetComponents<PolygonCollider2D>();
        pc2D = pc2DList[0];
        pc2D1 = pc2DList[1];
        ms = mr.materials;//assign materials array first, for multiple texture atlas in one mesh
    }

    //Although it supports out of chunk cBlockWPosX/Y, Better call this within chunk to avoid recursion
    //+- light
    //should be called before UpdateFilter()
    public void Update5x5NeigLight(int cBlockWPosX, int cBlockWPosY, int light)
    {

        UpdateBlockLight(cBlockWPosX, cBlockWPosY + 5 * tileSize, light);
        UpdateBlockLight(cBlockWPosX, cBlockWPosY - 5 * tileSize, light);

        UpdateBlockLight(cBlockWPosX - tileSize, cBlockWPosY + 4 * tileSize, light);
        UpdateBlockLight(cBlockWPosX, cBlockWPosY + 4 * tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX + tileSize, cBlockWPosY + 4 * tileSize, light);

        UpdateBlockLight(cBlockWPosX - tileSize, cBlockWPosY - 4 * tileSize, light);
        UpdateBlockLight(cBlockWPosX, cBlockWPosY - 4 * tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX + tileSize, cBlockWPosY - 4 * tileSize, light);

        UpdateBlockLight(cBlockWPosX - 2 * tileSize, cBlockWPosY + 3 * tileSize, light);
        UpdateBlockLight(cBlockWPosX - tileSize, cBlockWPosY + 3 * tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX, cBlockWPosY + 3 * tileSize, 3 * light);
        UpdateBlockLight(cBlockWPosX + tileSize, cBlockWPosY + 3 * tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX + 2 * tileSize, cBlockWPosY + 3 * tileSize, light);

        UpdateBlockLight(cBlockWPosX - 2 * tileSize, cBlockWPosY - 3 * tileSize, light);
        UpdateBlockLight(cBlockWPosX - tileSize, cBlockWPosY - 3 * tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX, cBlockWPosY - 3 * tileSize, 3 * light);
        UpdateBlockLight(cBlockWPosX + tileSize, cBlockWPosY - 3 * tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX + 2 * tileSize, cBlockWPosY - 3 * tileSize, light);

        UpdateBlockLight(cBlockWPosX - 3 * tileSize, cBlockWPosY + 2 * tileSize, light);
        UpdateBlockLight(cBlockWPosX - 2 * tileSize, cBlockWPosY + 2 * tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX - tileSize, cBlockWPosY + 2 * tileSize, 3 * light);
        UpdateBlockLight(cBlockWPosX, cBlockWPosY + 2 * tileSize, 4 * light);
        UpdateBlockLight(cBlockWPosX + tileSize, cBlockWPosY + 2 * tileSize, 3 * light);
        UpdateBlockLight(cBlockWPosX + 2 * tileSize, cBlockWPosY + 2 * tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX + 3 * tileSize, cBlockWPosY + 2 * tileSize, light);

        UpdateBlockLight(cBlockWPosX - 3 * tileSize, cBlockWPosY - 2 * tileSize, light);
        UpdateBlockLight(cBlockWPosX - 2 * tileSize, cBlockWPosY - 2 * tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX - tileSize, cBlockWPosY - 2 * tileSize, 3 * light);
        UpdateBlockLight(cBlockWPosX, cBlockWPosY - 2 * tileSize, 4 * light);
        UpdateBlockLight(cBlockWPosX + tileSize, cBlockWPosY - 2 * tileSize, 3 * light);
        UpdateBlockLight(cBlockWPosX + 2 * tileSize, cBlockWPosY - 2 * tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX + 3 * tileSize, cBlockWPosY - 2 * tileSize, light);

        UpdateBlockLight(cBlockWPosX - 4 * tileSize, cBlockWPosY + tileSize, light);
        UpdateBlockLight(cBlockWPosX - 3 * tileSize, cBlockWPosY + tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX - 2 * tileSize, cBlockWPosY + tileSize, 3 * light);
        UpdateBlockLight(cBlockWPosX - tileSize, cBlockWPosY + tileSize, 4 * light);
        UpdateBlockLight(cBlockWPosX, cBlockWPosY + tileSize, 5 * light);
        UpdateBlockLight(cBlockWPosX + tileSize, cBlockWPosY + tileSize, 4 * light);
        UpdateBlockLight(cBlockWPosX + 2 * tileSize, cBlockWPosY + tileSize, 3 * light);
        UpdateBlockLight(cBlockWPosX + 3 * tileSize, cBlockWPosY + tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX + 4 * tileSize, cBlockWPosY + tileSize, light);

        UpdateBlockLight(cBlockWPosX - 4 * tileSize, cBlockWPosY - tileSize, light);
        UpdateBlockLight(cBlockWPosX - 3 * tileSize, cBlockWPosY - tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX - 2 * tileSize, cBlockWPosY - tileSize, 3 * light);
        UpdateBlockLight(cBlockWPosX - tileSize, cBlockWPosY - tileSize, 4 * light);
        UpdateBlockLight(cBlockWPosX, cBlockWPosY - tileSize, 5 * light);
        UpdateBlockLight(cBlockWPosX + tileSize, cBlockWPosY - tileSize, 4 * light);
        UpdateBlockLight(cBlockWPosX + 2 * tileSize, cBlockWPosY - tileSize, 3 * light);
        UpdateBlockLight(cBlockWPosX + 3 * tileSize, cBlockWPosY - tileSize, 2 * light);
        UpdateBlockLight(cBlockWPosX + 4 * tileSize, cBlockWPosY - tileSize, light);

        UpdateBlockLight(cBlockWPosX - 5 * tileSize, cBlockWPosY, light);
        UpdateBlockLight(cBlockWPosX - 4 * tileSize, cBlockWPosY, 2 * light);
        UpdateBlockLight(cBlockWPosX - 3 * tileSize, cBlockWPosY, 3 * light);
        UpdateBlockLight(cBlockWPosX - 2 * tileSize, cBlockWPosY, 4 * light);
        UpdateBlockLight(cBlockWPosX - tileSize, cBlockWPosY, 5 * light);
        UpdateBlockLight(cBlockWPosX + tileSize, cBlockWPosY, 5 * light);
        UpdateBlockLight(cBlockWPosX + 2 * tileSize, cBlockWPosY, 4 * light);
        UpdateBlockLight(cBlockWPosX + 3 * tileSize, cBlockWPosY, 3 * light);
        UpdateBlockLight(cBlockWPosX + 4 * tileSize, cBlockWPosY, 2 * light);
        UpdateBlockLight(cBlockWPosX + 5 * tileSize, cBlockWPosY, light);


    }

    //2.4 max 0 min,  3 blocks neigh
    //set the light amount at startup
    //should be called after loadChunk since tileID need to be known before setting up the light
    //light comes from infinity towards screen direction
    public void StartUpSetLightAmount()
    {
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                Block b = blocks[i, j];
                //solid has 0 affect to neig block's light
                if (b.wallID == WallIndex.noWall && b.tileID == BlockIndex.air)//no wall and no block 
                {
                    Update5x5NeigLight(b.bWorldPos[0], b.bWorldPos[1], 1);     //1 means add a factor of 1 to neig              
                    
                }
            }
        }
    }
    


    Chunk c;//chunk below this chunk
    bool tem = false;

    int frame = 0;

    void Update()
    {
		frame++;

		if(frame > 40 && isDrew == true && WorldGenTestOnly.doneGenerated){//might need to check chunk below whether it is drew			
			//CheckMovableBlocks();
            
			frame = 0;
		}
    }
    //50fps performance really bad need to find better solution
    /*
	public void CheckMovableBlocks(){//may be called in coroutine
		bool update2Chunks = false;
		Chunk c = world.GetChunk (pos[0], pos[1] - chunkSize * tileSize);
        
		for (int i = 0; i < movableHashSet.Count; i++) {
			if (movableHashSet.ElementAt (i).stable == true) {//work on this later
				//continue;
			} else {//not stable
				int x = movableHashSet.ElementAt (i).bWorldPos[0];//block world pos
				int y = movableHashSet.ElementAt (i).bWorldPos[1];
				int[] bIndex = BWPosToCindex (x,y);//block chunk index
                
                if (bIndex [1] == 0) {//bottom chunk's boundary, y==0
					
					if (c != null && c.blocks[bIndex[0],chunkSize-1].tileID == BlockIndex.air) {//must check if the current chunk is the bottom most chunk in the world
                        
						c.SetBlock (x,y-tileSize, movableHashSet.ElementAt (i));
						c.AddBlock (bIndex [0], chunkSize-1);
                        c.AddPathsRemovePaths1(bIndex[0], chunkSize - 1);
						c.SetBlockImage (bIndex [0], chunkSize-1);
						c.hasChanged = true;

                        
						movableHashSet.Remove (blocks[bIndex[0],bIndex[1]]);
						SetBlock(x,y,new AirBlock());
						RemoveBlock (bIndex [0], bIndex [1]);
                        AddPaths1RemovePaths(x, y);
						hasChanged = true;

						update2Chunks = true;

					} else {//bottom most in the world
						//do nothing
						continue;
					}
				} else {//above chunk's bottom boundary
					//Debug.Log (bIndex[0]+","+bIndex[1]);
					if (blocks [bIndex [0], bIndex [1] - 1].tileID == BlockIndex.air) {
                        
						SetBlock (x, y - tileSize, movableHashSet.ElementAt (i));
						AddBlock (bIndex [0], bIndex [1]-1);
                        AddPathsRemovePaths1(bIndex[0], bIndex[1]-1);
						SetBlockImage (bIndex [0], bIndex [1]-1);

						movableHashSet.Remove (blocks[bIndex[0],bIndex[1]]);
						SetBlock (x, y, new AirBlock ());                        
						RemoveBlock (bIndex [0], bIndex [1]);
                        AddPaths1RemovePaths(bIndex[0], bIndex[1]);
                        hasChanged = true;


					} else {
						//do nothing and assign stable
					
						continue;
					}
				}


			}
		}

		UpdateFilter ();
		ClearCollTriList ();
		RefreshChunkCollTri ();

		if (update2Chunks) {
			c.UpdateFilter ();
			c.ClearCollTriList ();
			c.RefreshChunkCollTri ();
		}

	}

    */
    /// <summary>
    /// Convert from block's world position to chunk index.
    /// </summary>
    /// <param name="blockWPosx"></param>
    /// <param name="blockWPosy"></param>
    /// <returns></returns>
    public int[] BWPosToCindex(int blockWPosx, int blockWPosy)
    {//block world's pos to chunk's index(currently only consider within chunk's boundary blocks)
        int[] bIndex = new int[2];//0 is x, 1 is y
        bIndex[0] = (blockWPosx - (pos[0] - Chunk.chunkSize * Chunk.tileSize / 2 + Chunk.tileSize / 2)) / tileSize;
        bIndex[1] = (blockWPosy - (pos[1] - Chunk.chunkSize * Chunk.tileSize / 2 + Chunk.tileSize / 2)) / tileSize;

        return bIndex;
    }

    TClipper tcp = new TClipper();
    List<List<Vector2>> result;
    List<List<Vector2>> result1;


    /// <summary>
    /// Used to set chunk's mesh info.
    /// </summary>
    public void SetChunkMesh()
    {//it is called in PlayerControlScript.HandleMouse func which includes modification of the pc2D, pc2D1 and mesh vertices
        SetChunkVertUV();
        UpdateFilter();
    }
    public void UpdateFilter()
    {
        filter.mesh.Clear();
        filter.mesh.RecalculateBounds();
 
        Vector3[] vec3Arr = new Vector3[wallVertDict.Count * 4 + vertDict.Count * 4];

        List<int> wallList = new List<int>();//wall
        List<int> triList0 = new List<int>();//shape 0
        List<int> triList1 = new List<int>();//shape 1

        Vector2[] vec2Arr = new Vector2[wallUVDict.Count * 4 + uvDict.Count * 4];

        //color
        Color32[] colorArr = new Color32[wallColorDict.Count * 4 + colorDict.Count * 4];
        
        int c = 0;
        foreach (var v in wallColorDict)
        {
            for (int q = 0; q < 4; q++)
            {
                colorArr[c * 4 + q] = v.Value[q];
            }
            c++;
        }
        foreach (var v in colorDict)
        {            
            for(int q = 0; q < 4; q++)
            {
                colorArr[c * 4 + q] = v.Value[q];
            }
            c++;
        }        
        //color
        int i = 0, j = 0;
        foreach (var v in wallVertDict)
        {
            for (int q = 0; q < 4; q++)
            {
                vec3Arr[i * 4 + q] = v.Value[q];
            }
            i++;
        }
        foreach (var v in vertDict)
        {
            for (int q = 0; q < 4; q++)
            {
                vec3Arr[i * 4 + q] = v.Value[q];
            }
            i++;
        }
        foreach (var v in wallUVDict)
        {
            for (int q = 0; q < 4; q++)
            {
                vec2Arr[j * 4 + q] = v.Value[q];
            }
            j++;
        }
        foreach (var v in uvDict)
        {
            for (int q = 0; q < 4; q++)
            {
                vec2Arr[j * 4 + q] = v.Value[q];
            }
            j++;
        }

        foreach (var v in wallTriDict)//wall tri dict
        {
            wallList.Add(v.Value[0]); wallList.Add(v.Value[1]); wallList.Add(v.Value[2]);
            wallList.Add(v.Value[3]); wallList.Add(v.Value[4]); wallList.Add(v.Value[5]);
                  
        }
        
        foreach (var v in triDict)
        {
            if (triDictSubIndex[v.Key] == 0){         
                triList0.Add(v.Value[0] + wallVertCount); triList0.Add(v.Value[1] + wallVertCount); triList0.Add(v.Value[2] + wallVertCount);
                triList0.Add(v.Value[3] + wallVertCount); triList0.Add(v.Value[4] + wallVertCount); triList0.Add(v.Value[5] + wallVertCount);
            }
            else if(triDictSubIndex[v.Key] == 1){                  
                triList1.Add(v.Value[0] + wallVertCount); triList1.Add(v.Value[1] + wallVertCount); triList1.Add(v.Value[2] + wallVertCount);
                triList1.Add(v.Value[3] + wallVertCount); triList1.Add(v.Value[4] + wallVertCount); triList1.Add(v.Value[5] + wallVertCount);
            }         
        }

        filter.mesh.vertices = vec3Arr;

        //ms[0] == wall, ms[1] == shape 0, ms[2] = shape1
        if (wallList.Count > 0 && triList0.Count > 0 && triList1.Count > 0 ) //wall,shape1 and shape0
        {
            mr.sharedMaterials = new Material[] { ms[0], ms[1], ms[2] };
            filter.mesh.subMeshCount = 3;

            filter.mesh.SetTriangles(wallList, 0);
            filter.mesh.SetTriangles(triList0, 1);
            filter.mesh.SetTriangles(triList1, 2);
        }
        else if (wallList.Count > 0 && triList0.Count == 0 && triList1.Count > 0) //wall, shape1
        {
            mr.sharedMaterials = new Material[] { ms[0], ms[2] };
            filter.mesh.subMeshCount = 2;

            filter.mesh.SetTriangles(wallList, 0);
            filter.mesh.SetTriangles(triList1, 1);
        }
        else if (wallList.Count > 0 && triList0.Count > 0 && triList1.Count == 0) //wall, shape0
        {
            mr.sharedMaterials = new Material[] { ms[0], ms[1] };
            filter.mesh.subMeshCount = 2;

            filter.mesh.SetTriangles(wallList, 0);
            filter.mesh.SetTriangles(triList0, 1);
        }
        else if (wallList.Count > 0 && triList0.Count == 0 && triList1.Count == 0) //wall only
        {
            mr.sharedMaterials = new Material[] { ms[0] };
            
            filter.mesh.SetTriangles(wallList, 0);
        }
        else if (wallList.Count == 0 && triList0.Count > 0 && triList1.Count > 0) //shape1, shap0
        {
            mr.sharedMaterials = new Material[] { ms[1], ms[2] };
            filter.mesh.subMeshCount = 2;

            filter.mesh.SetTriangles(triList0, 0);
            filter.mesh.SetTriangles(triList1, 1);
        }
        else if (wallList.Count == 0 && triList0.Count == 0 && triList1.Count > 0) //shape1 only
        {
            mr.sharedMaterials = new Material[] { ms[2] };
            filter.mesh.subMeshCount = 1;
            filter.mesh.SetTriangles(triList1, 0);
        }
        else if (wallList.Count == 0 && triList0.Count > 0 && triList1.Count == 0) //shape0 only
        {
            mr.sharedMaterials = new Material[] { ms[1] };
            filter.mesh.subMeshCount = 1;
            filter.mesh.SetTriangles(triList0, 0);
        }
        else if (wallList.Count == 0 && triList0.Count == 0 && triList1.Count == 0) //none
        {
            mr.sharedMaterials = new Material[] { };
            filter.mesh.triangles = triList0.ToArray();
        }              
        
        
        filter.mesh.uv = vec2Arr;
        filter.mesh.colors32 = colorArr;
    }

   

    Color32[] CalculateColor(byte light)//input light and then determine the color -> return color32 array
    {
        Color32[] res;
        if (light >= 0 && light <= 1)
        { //darkest light
            res = new Color32[] { color4IntensityArr[6, 0], color4IntensityArr[6, 1], color4IntensityArr[6, 2], color4IntensityArr[6, 3] };
        }
        else if (light >= 2 && light <= 6)
        {
            res = new Color32[] { color4IntensityArr[5, 0], color4IntensityArr[5, 1], color4IntensityArr[5, 2], color4IntensityArr[5, 3] };
        }
        else if (light >= 7 && light <= 11)
        {
            res = new Color32[] { color4IntensityArr[4, 0], color4IntensityArr[4, 1], color4IntensityArr[4, 2], color4IntensityArr[4, 3] };
        }
        else if (light >= 12 && light <= 15)
        {
            res = new Color32[] { color4IntensityArr[3, 0], color4IntensityArr[3, 1], color4IntensityArr[3, 2], color4IntensityArr[3, 3] };
        }
        else if (light >= 16 && light <= 30)
        {
            res = new Color32[] { color4IntensityArr[2, 0], color4IntensityArr[2, 1], color4IntensityArr[2, 2], color4IntensityArr[2, 3] };
        }
        else if (light >= 31 && light <= 40)
        {
            res = new Color32[] { color4IntensityArr[1, 0], color4IntensityArr[1, 1], color4IntensityArr[1, 2], color4IntensityArr[1, 3] };
        }
        else //  brightest light
        {
            res = new Color32[] { color4IntensityArr[0, 0], color4IntensityArr[0, 1], color4IntensityArr[0, 2], color4IntensityArr[0, 3] };
        }

        return res;
    }
    //for wall, wall determines the light and hence the block's color
    public void UpdateCurrAndCallUpdateNeig4x4Color(int blockx, int blocky, bool isBlockRemove)//update neig 4x4 solid block's color
    {
        int[] bWP = blocks[blockx, blocky].bWorldPos; //current block's world position
        if (isBlockRemove) {//removing blocks
            //Curr block
            colorDict.Remove(blockx * chunkSize + chunkSize - blocky - 1); //remove the color light on this block           
            //4x4 block
            Update5x5NeigColor(bWP[0], bWP[1]);
        }
        else//adding blocks, Adding array of color32 to colorDict
        {
            //Curr block 
            Color32[] c = CalculateColor(blocks[blockx, blocky].light);
            colorDict.Add(blockx * chunkSize + chunkSize - blocky - 1, c);
            //4x4 block
            Update5x5NeigColor(bWP[0], bWP[1]);

        }
    }
    //for wall, wall determines the light and hence the block's color
    void Update5x5NeigColor(int bWPosx, int bWPosy)
    {
        UpdateColor(bWPosx, bWPosy + 5 * tileSize);
        UpdateColor(bWPosx, bWPosy - 5 * tileSize);

        UpdateColor(bWPosx - tileSize, bWPosy + 4 * tileSize);
        UpdateColor(bWPosx, bWPosy - 4 * tileSize);
        UpdateColor(bWPosx + tileSize, bWPosy + 4 * tileSize);

        UpdateColor(bWPosx - tileSize, bWPosy - 4 * tileSize);
        UpdateColor(bWPosx, bWPosy - 4 * tileSize);
        UpdateColor(bWPosx + tileSize, bWPosy - 4 * tileSize);

        UpdateColor(bWPosx - 2 * tileSize, bWPosy + 3 * tileSize);
        UpdateColor(bWPosx - tileSize, bWPosy + 3 * tileSize);
        UpdateColor(bWPosx, bWPosy + 3 * tileSize);
        UpdateColor(bWPosx + tileSize, bWPosy + 3 * tileSize);
        UpdateColor(bWPosx + 2 * tileSize, bWPosy + 3 * tileSize);

        UpdateColor(bWPosx - 2 * tileSize, bWPosy - 3 * tileSize);
        UpdateColor(bWPosx - tileSize, bWPosy - 3 * tileSize);
        UpdateColor(bWPosx, bWPosy - 3 * tileSize);
        UpdateColor(bWPosx + tileSize, bWPosy - 3 * tileSize);
        UpdateColor(bWPosx + 2 * tileSize, bWPosy - 3 * tileSize);

        UpdateColor(bWPosx - 3 * tileSize, bWPosy + 2 * tileSize);
        UpdateColor(bWPosx - 2 * tileSize, bWPosy + 2 * tileSize);
        UpdateColor(bWPosx - tileSize, bWPosy + 2 * tileSize);
        UpdateColor(bWPosx, bWPosy + 2 * tileSize);
        UpdateColor(bWPosx + tileSize, bWPosy + 2 * tileSize);
        UpdateColor(bWPosx + 2 * tileSize, bWPosy + 2 * tileSize);
        UpdateColor(bWPosx + 3 * tileSize, bWPosy + 2 * tileSize);

        UpdateColor(bWPosx - 3 * tileSize, bWPosy - 2 * tileSize);
        UpdateColor(bWPosx - 2 * tileSize, bWPosy - 2 * tileSize);
        UpdateColor(bWPosx - tileSize, bWPosy - 2 * tileSize);
        UpdateColor(bWPosx, bWPosy - 2 * tileSize);
        UpdateColor(bWPosx + tileSize, bWPosy - 2 * tileSize);
        UpdateColor(bWPosx + 2 * tileSize, bWPosy - 2 * tileSize);
        UpdateColor(bWPosx + 3 * tileSize, bWPosy - 2 * tileSize);

        UpdateColor(bWPosx - 4 * tileSize, bWPosy + tileSize);
        UpdateColor(bWPosx - 3 * tileSize, bWPosy + tileSize);
        UpdateColor(bWPosx - 2 * tileSize, bWPosy + tileSize);
        UpdateColor(bWPosx - tileSize, bWPosy + tileSize);
        UpdateColor(bWPosx, bWPosy + tileSize);
        UpdateColor(bWPosx + tileSize, bWPosy + tileSize);
        UpdateColor(bWPosx + 2 * tileSize, bWPosy + tileSize);
        UpdateColor(bWPosx + 3 * tileSize, bWPosy + tileSize);
        UpdateColor(bWPosx + 4 * tileSize, bWPosy + tileSize);

        UpdateColor(bWPosx - 4 * tileSize, bWPosy - tileSize);
        UpdateColor(bWPosx - 3 * tileSize, bWPosy - tileSize);
        UpdateColor(bWPosx - 2 * tileSize, bWPosy - tileSize);
        UpdateColor(bWPosx - tileSize, bWPosy - tileSize);
        UpdateColor(bWPosx, bWPosy - tileSize);
        UpdateColor(bWPosx + tileSize, bWPosy - tileSize);
        UpdateColor(bWPosx + 2 * tileSize, bWPosy - tileSize);
        UpdateColor(bWPosx + 3 * tileSize, bWPosy - tileSize);
        UpdateColor(bWPosx + 4 * tileSize, bWPosy - tileSize);

        UpdateColor(bWPosx - 5 * tileSize, bWPosy);
        UpdateColor(bWPosx - 4 * tileSize, bWPosy);
        UpdateColor(bWPosx - 3 * tileSize, bWPosy);
        UpdateColor(bWPosx - 2 * tileSize, bWPosy);
        UpdateColor(bWPosx - tileSize, bWPosy);
        UpdateColor(bWPosx + tileSize, bWPosy);
        UpdateColor(bWPosx + 2 * tileSize, bWPosy);
        UpdateColor(bWPosx + 3 * tileSize, bWPosy);
        UpdateColor(bWPosx + 4 * tileSize, bWPosy);
        UpdateColor(bWPosx + 5 * tileSize, bWPosy);
    }

    //Handle the out of chunk and world's boundary conditions
    //just update the color according the new light amount in current block
    void UpdateColor(int bWPosx, int bWPosy)
    {
        //Check which block it refers to first
        bool withinpY = bWPosy < pos[1] + tileSize * chunkSize / 2 ? true : false;
        bool withinpX = bWPosx < pos[0] + tileSize * chunkSize / 2 ? true : false;
        bool withinnY = bWPosy > pos[1] - tileSize * chunkSize / 2 ? true : false;
        bool withinnX = bWPosx > pos[0] - tileSize * chunkSize / 2 ? true : false;

        //If not inside the chunk's boundary
        if (!withinpX || !withinpY || !withinnX || !withinnY)
        {
            //if within world's boundary
            if (bWPosx > World.worldLBlockPos && bWPosx < World.worldRBlockPos && bWPosy > World.worldBBlockPos && bWPosy < World.worldTBlockPos)
            {
                int chunkTileSize = chunkSize * tileSize;
                //blockWorldPosx - (World.worldLMostChunkPosX - chunkSize2 / 2) / (chunkSize2) -> index of chunkx
                int cPosX = Mathf.FloorToInt((bWPosx - (World.worldLMostChunkPosX - chunkTileSize / 2)) / chunkTileSize) *
                    chunkTileSize + World.worldLMostChunkPosX;
                int cPosY = Mathf.FloorToInt((bWPosy - (World.worldBMostChunkPosY - chunkTileSize / 2)) / chunkTileSize) *
                    chunkTileSize + World.worldBMostChunkPosY;
                
                world.GetChunk(cPosX, cPosY).UpdateColor(bWPosx, bWPosy);
            }
            //else out of world's boundary do nothing
        }
        else   //if the block is not out of chunk, then these codes should be called
        {
            int blockChunkArrayx = Mathf.FloorToInt((bWPosx - pos[0] + (chunkSize * tileSize / 2)) / (tileSize));
            int blockChunkArrayy = Mathf.FloorToInt((bWPosy - pos[1] + (chunkSize * tileSize / 2)) / (tileSize));
            //Case1: within chunk (no recursion), Case2: just found that block is out of chunk's boundary and called getChunk once
            if (colorDict.ContainsKey(blockChunkArrayx * chunkSize + chunkSize - blockChunkArrayy - 1))
            {
                colorDict[blockChunkArrayx * chunkSize + chunkSize - blockChunkArrayy - 1] = CalculateColor(blocks[blockChunkArrayx, blockChunkArrayy].light);

            }
        }
    }


    public void RemoveBlock(int blockx, int blocky)
    {//remove block's vert, uv, tri, and need to check face too, 
     //allow blockx overflow chunk's size normally blockx and blocky should within chunk's boundary, however
     //there is a case when the grass dirt block is inside the boundary but the grass block is outside the boundary!!!

        bool withinpx = blockx < chunkSize ? true : false;
        bool withinnx = blockx >= 0 ? true : false;
        bool withinpy = blocky < chunkSize ? true : false;
        bool withinny = blocky >= 0 ? true : false;

        //need to handle world's boundary(not done)
        if (!withinpx && withinnx && withinpy && withinny)
        {//right 
            world.GetChunk(pos[0] + chunkSize * tileSize, pos[1]).RemoveBlock(-chunkSize + blockx, blocky);
        }
        else if (withinpx && withinnx && withinpy && !withinny)
        {//bot
            world.GetChunk(pos[0], pos[1] - chunkSize * tileSize).RemoveBlock(blockx, chunkSize + blocky);
        }
        else if (withinpx && !withinnx && withinpy && withinny)
        {//left
            world.GetChunk(pos[0] - chunkSize * tileSize, pos[1]).RemoveBlock(chunkSize + blockx, blocky);
        }
        else if (withinpx && withinnx && !withinpy && withinny)
        {//top
            world.GetChunk(pos[0], pos[1] + chunkSize * tileSize).RemoveBlock(blockx, -chunkSize + blocky);
        }
        else
        {
            vertDict.Remove(blockx * chunkSize + chunkSize - blocky - 1);
            uvDict.Remove(blockx * chunkSize + chunkSize - blocky - 1);
          
            triDict.Remove(blockx * chunkSize + chunkSize - blocky - 1);
            triDictSubIndex.Remove(blockx * chunkSize + chunkSize - blocky - 1);

            var temTri = triDict.Where(v => v.Key > blockx * chunkSize + chunkSize - blocky - 1);
            foreach (var v in temTri)
            {
                for (int i = 0; i < 6; i++)
                {
                    v.Value[i] -= 4;
                }
            }

            UpdateBlockUV(blockx + 1, blocky);//right
            UpdateBlockUV(blockx, blocky + 1);//top
            UpdateBlockUV(blockx - 1, blocky);//left
            UpdateBlockUV(blockx, blocky - 1);//bot

        }
    }
    public void SetBlockImage(int blockx, int blocky)
    {//it seems no need to think of chunk's boundary right now
        Vector3[] chunkVerArr = new Vector3[]{new Vector3(- chunkSize*tileSize/2+(blockx)*tileSize,
            - chunkSize*tileSize/2+(blocky)*tileSize,0),
            new Vector3(- chunkSize*tileSize/2+(blockx+1)*tileSize,
                - chunkSize*tileSize/2+(blocky)*tileSize,0),
            new Vector3(- chunkSize*tileSize/2+(blockx)*tileSize,
                - chunkSize*tileSize/2+(blocky+1)*tileSize,0),
            new Vector3(- chunkSize*tileSize/2+(blockx+1)*tileSize,
                - chunkSize*tileSize/2+(blocky+1)*tileSize,0)
        };

        vertDict.Add(blockx * chunkSize + chunkSize - blocky - 1, chunkVerArr);

        int vertCount = triDict.Where(v => v.Key < (blockx * chunkSize + chunkSize - blocky - 1)).Count() * 4;
        var tem = triDict.Where(v => v.Key > (blockx * chunkSize + chunkSize - blocky - 1));

        foreach (var v in tem)
        {
            for (int i = 0; i < 6; i++)
            {
                v.Value[i] += 4;
            }
        }

        int[] triArr = new int[] { vertCount, vertCount + 2, vertCount + 3, vertCount + 3, vertCount + 1, vertCount };
        triDict.Add(blockx * chunkSize + chunkSize - blocky - 1, triArr);
        //triDictsub  
        if (blocks[blockx, blocky].tileID == BlockIndex.grassDirt || blocks[blockx, blocky].tileID == BlockIndex.junDirt ||
            blocks[blockx, blocky].tileID == BlockIndex.corrDirt)
        {            
            triDictSubIndex[blockx * chunkSize + chunkSize - blocky - 1] = 1;            
        }
        else
        {
            triDictSubIndex[blockx * chunkSize + chunkSize - blocky - 1] = 0;
        }


        int face = CalBlockOrWallFace(blockx, blocky, blocks[blockx, blocky].tileID, true);

        uvDict.Add(blockx * chunkSize + chunkSize - blocky - 1, GetBlockUVList(blocks[blockx, blocky], face));
        
        UpdateBlockUV(blockx + 1, blocky);//right
        UpdateBlockUV(blockx, blocky + 1);//top
        UpdateBlockUV(blockx - 1, blocky);//left
        UpdateBlockUV(blockx, blocky - 1);//bot
    }
    
	public void AddBlock(int blockx, int blocky){//for adding movable block to movableHashset
		movableHashSet.Add(blocks[blockx, blocky]);
	}

    public void UpdateBlockUV(int blockx, int blocky)
    {//not block's world coordinate !!need to test world coordinate too
     //update neighbor block uv

        bool withinpx = blockx < chunkSize ? true : false;
        bool withinnx = blockx >= 0 ? true : false;
        bool withinpy = blocky < chunkSize ? true : false;
        bool withinny = blocky >= 0 ? true : false;

        if (!withinpx && withinnx && withinpy && withinny)
        {//exceed right boundary			
            int x = -chunkSize + blockx;
            int y = blocky;
            Chunk c = world.GetChunk(pos[0] + chunkSize * tileSize, pos[1]);
            if (c != null)
            {
                if (c.blocks[x, y].passable == false)
                {//if it is unpassable, then it is block that has 16 faces
                    int face = c.CalBlockOrWallFace(x, y, blocks[x, y].tileID, true);
                    c.uvDict[x * chunkSize + chunkSize - y - 1] = GetBlockUVList(c.blocks[x, y], face);
                    
                    c.UpdateFilter();//necessary because we need to update neigh chunk's filter not just current chunk's filter
                }
            }
        }
        else if (withinpx && withinnx && withinpy && !withinny)
        {//bot
            int x = blockx;
            int y = chunkSize + blocky;
            Chunk c = world.GetChunk(pos[0], pos[1] - chunkSize * tileSize);
            if (c != null)
            {
                if (c.blocks[x, y].passable == false)
                {
                    int face = c.CalBlockOrWallFace(x, y, blocks[x, y].tileID, true);
                    c.uvDict[x * chunkSize + chunkSize - y - 1] = GetBlockUVList(c.blocks[x, y], face);
                    
                    c.UpdateFilter();
                }
            }
        }
        else if (withinpx && !withinnx && withinpy && withinny)
        {//left
            int x = chunkSize + blockx;
            int y = blocky;
            Chunk c = world.GetChunk(pos[0] - chunkSize * tileSize, pos[1]);
            if (c != null)
            {
                if (c.blocks[x, y].passable == false)
                {
                    int face = c.CalBlockOrWallFace(x, y, blocks[x, y].tileID, true);
                    c.uvDict[x * chunkSize + chunkSize - y - 1] = GetBlockUVList(c.blocks[x, y], face);
                    
                    c.UpdateFilter();
                }
            }
        }
        else if (withinpx && withinnx && !withinpy && withinny)
        {//top
            int x = blockx;
            int y = -chunkSize + blocky;
            Chunk c = world.GetChunk(pos[0], pos[1] + chunkSize * tileSize);
            if (c != null)
            {
                if (c.blocks[x, y].passable == false)
                {
                    int face = c.CalBlockOrWallFace(x, y, blocks[x, y].tileID, true);

                    c.uvDict[x * chunkSize + chunkSize - y - 1] = GetBlockUVList(c.blocks[x, y], face);
                    
                    c.UpdateFilter();
                }
            }
        }
        else
        {
            if (blocks[blockx, blocky].passable == false)
            {
                int face = CalBlockOrWallFace(blockx, blocky, blocks[blockx, blocky].tileID, true);

                uvDict[blockx * chunkSize + chunkSize - blocky - 1] = GetBlockUVList(blocks[blockx, blocky], face);
                
            }
        }
    }
    public Vector2[] GetWallUVList(Block block, int face)
    {
        Vector2[] output;
        if(block.wallID == WallIndex.brickBWall)
        {
            output = new Vector2[] {
                BrickBackWall.leftBotUV + WallUV.uvList[face, 0], BrickBackWall.leftBotUV + WallUV.uvList[face, 1],
                BrickBackWall.leftBotUV + WallUV.uvList[face, 2], BrickBackWall.leftBotUV + WallUV.uvList[face, 3]
            };
            return output;
        }
        else if(block.wallID == WallIndex.dirtBWall)
        {
            output = new Vector2[] {
                DirtBackWall.leftBotUV + WallUV.uvList[face, 0], DirtBackWall.leftBotUV + WallUV.uvList[face, 1],
                DirtBackWall.leftBotUV + WallUV.uvList[face, 2], DirtBackWall.leftBotUV + WallUV.uvList[face, 3]
            };
            return output;
        }
        else if(block.wallID == WallIndex.stoneBrickBWall)
        {
            output = new Vector2[] {
                StoneBrickBackWall.leftBotUV + WallUV.uvList[face, 0], StoneBrickBackWall.leftBotUV + WallUV.uvList[face, 1],
                StoneBrickBackWall.leftBotUV + WallUV.uvList[face, 2], StoneBrickBackWall.leftBotUV + WallUV.uvList[face, 3]
            };
            return output;
        }
        else if(block.wallID == WallIndex.stoneBWall)
        {
            output = new Vector2[] {
                StoneBackWall.leftBotUV + WallUV.uvList[face, 0], StoneBackWall.leftBotUV + WallUV.uvList[face, 1],
                StoneBackWall.leftBotUV + WallUV.uvList[face, 2], StoneBackWall.leftBotUV + WallUV.uvList[face, 3]
            };
            return output;
        }
        else if(block.wallID == WallIndex.woodBWall)
        {
            output = new Vector2[] {
                WoodBackWall.leftBotUV + WallUV.uvList[face, 0], WoodBackWall.leftBotUV + WallUV.uvList[face, 1],
                WoodBackWall.leftBotUV + WallUV.uvList[face, 2], WoodBackWall.leftBotUV + WallUV.uvList[face, 3]
            };
            return output;
        }
        else if (block.wallID == WallIndex.unDWall)
        {
            output = new Vector2[] {
                UndestructibleBackWall.leftBotUV + WallUV.uvList[face, 0], UndestructibleBackWall.leftBotUV + WallUV.uvList[face, 1],
                UndestructibleBackWall.leftBotUV + WallUV.uvList[face, 2], UndestructibleBackWall.leftBotUV + WallUV.uvList[face, 3]
            };
            return output;
        }
        else
        {
            output = new Vector2[] {
                UndestructibleBackWall.leftBotUV + WallUV.uvList[face, 0], UndestructibleBackWall.leftBotUV + WallUV.uvList[face, 1],
                UndestructibleBackWall.leftBotUV + WallUV.uvList[face, 2], UndestructibleBackWall.leftBotUV + WallUV.uvList[face, 3]
            };
            return output;
        }



    }
    public Vector2[] GetBlockUVList(Block block, int face)
    {
        Vector2[] output;
        if (block.tileID == BlockIndex.dirt)
        {
            output = new Vector2[] {
                DirtBlock.leftBotUV + ShapeZeroUV.uvList [face, 0], DirtBlock.leftBotUV + ShapeZeroUV.uvList [face, 1],
                DirtBlock.leftBotUV + ShapeZeroUV.uvList [face, 2], DirtBlock.leftBotUV + ShapeZeroUV.uvList [face, 3]
            };
            return output;
        }
        else if (block.tileID == BlockIndex.sand)
        {
            output = new Vector2[] {
                SandBlock.leftBotUV + ShapeZeroUV.uvList [face, 0], SandBlock.leftBotUV + ShapeZeroUV.uvList [face, 1],
                SandBlock.leftBotUV + ShapeZeroUV.uvList [face, 2], SandBlock.leftBotUV + ShapeZeroUV.uvList [face, 3]
            };
            return output;
        }
        else if (block.tileID == BlockIndex.corrDirt)
        {
            output = new Vector2[] {
                CorrDirtBlock.leftBotUV + ShapeOneUV.uvList [face, 0], CorrDirtBlock.leftBotUV + ShapeOneUV.uvList [face, 1],
                CorrDirtBlock.leftBotUV + ShapeOneUV.uvList [face, 2], CorrDirtBlock.leftBotUV + ShapeOneUV.uvList [face, 3]
            };
            return output;
        }
        else if (block.tileID == BlockIndex.junDirt)
        {
            output = new Vector2[] {
                JunDirtBlock.leftBotUV + ShapeOneUV.uvList [face, 0], JunDirtBlock.leftBotUV + ShapeOneUV.uvList [face, 1],
                JunDirtBlock.leftBotUV + ShapeOneUV.uvList [face, 2], JunDirtBlock.leftBotUV + ShapeOneUV.uvList [face, 3]
            };
            return output;
        }
        else if (block.tileID == BlockIndex.stone)
        {
            output = new Vector2[] {
                StoneBlock.leftBotUV + ShapeZeroUV.uvList [face, 0], StoneBlock.leftBotUV + ShapeZeroUV.uvList [face, 1],
                StoneBlock.leftBotUV + ShapeZeroUV.uvList [face, 2], StoneBlock.leftBotUV + ShapeZeroUV.uvList [face, 3]
            };
            return output;
        }
        else if (block.tileID == BlockIndex.grassDirt)
        {
            output = new Vector2[] {
                GrassDirtBlock.leftBotUV + ShapeOneUV.uvList [face, 0], GrassDirtBlock.leftBotUV + ShapeOneUV.uvList [face, 1],
                GrassDirtBlock.leftBotUV + ShapeOneUV.uvList [face, 2], GrassDirtBlock.leftBotUV + ShapeOneUV.uvList [face, 3]
            };
            return output;
        }
        /*
        else if (block.tileID == BlockIndex.grass)
        {
            output = new Vector2[] {
                GrassBlock.leftBotUV + ShapeOneUV.uvList [face, 0], GrassBlock.leftBotUV + ShapeOneUV.uvList [face, 1],
                GrassBlock.leftBotUV + ShapeOneUV.uvList [face, 2], GrassBlock.leftBotUV + ShapeOneUV.uvList [face, 3]
            };
            return output;
        }
        
        else if (block.tileID == BlockIndex.corrGrass)
        {
            output = new Vector2[] {
                DirtBlock.leftBotUV + ShapeOneUV.uvList [face, 0], DirtBlock.leftBotUV + DirtBlock.uvList [face, 1],
                DirtBlock.leftBotUV + DirtBlock.uvList [face, 2], DirtBlock.leftBotUV +  DirtBlock.uvList [face, 3]
            };
            return output;
        }
        else if (block.tileID == BlockIndex.junGrass)
        {
            output = new Vector2[] {
                DirtBlock.leftBotUV + ShapeOneUV.uvList [face, 0], DirtBlock.leftBotUV + DirtBlock.uvList [face, 1],
                DirtBlock.leftBotUV + DirtBlock.uvList [face, 2], DirtBlock.leftBotUV +  DirtBlock.uvList [face, 3]
            };
            return output;
        }
        */
        else if (block.tileID == BlockIndex.copper)
        {
            output = new Vector2[] {
                CopperBlock.leftBotUV + ShapeZeroUV.uvList [face, 0], CopperBlock.leftBotUV + ShapeZeroUV.uvList [face, 1],
                CopperBlock.leftBotUV + ShapeZeroUV.uvList [face, 2], CopperBlock.leftBotUV + ShapeZeroUV.uvList [face, 3]
            };
            return output;
        }
        /*
        else if (block.tileID == BlockIndex.catus)
        {
            output = new Vector2[] {
                DirtBlock.leftBotUV + ShapeOneUV.uvList [face, 0], DirtBlock.leftBotUV + DirtBlock.uvList [face, 1],
                DirtBlock.leftBotUV + DirtBlock.uvList [face, 2], DirtBlock.leftBotUV +  DirtBlock.uvList [face, 3]
            };
            return output;
        }
        */
        return null;//return null for debugging purpose, if vec2arr has problem then the issue might be from this func
    }

    public void ClearMeshAndCollider()//just clear the filter.mesh , pc2D and pc2D1,Chunkvertices,uv,tri,paths and paths1 still contains data
    {
        filter.mesh.Clear();

        pc2D.pathCount = 0;
        pc2D1.pathCount = 0;
    }
    public void RevertOrSetChunkMesh()//revert back the collider and mesh if Chunkvertices,uv,triangles exist otherwise call SetChunkMesh 
    {
        if (vertDict.Count > 0)
        {
            UpdateFilter();

            pc2D.pathCount = result.Count;
            pc2D1.pathCount = result1.Count;
            for (int i = 0; i < result.Count; i++)
            {
                pc2D.SetPath(i, result[i].ToArray());
            }
            for (int i = 0; i < result1.Count; i++)
            {
                pc2D1.SetPath(i, result1[i].ToArray());
            }
        }
        else
        {
            SetChunkMesh();
        }
    }

    public void ClearCollTriList()
    {
        pc2D.pathCount = 0;
        pc2D1.pathCount = 0;
    }

    public void SetChunkVertUV()
    {
        vertCount = 0;
        wallVertCount = 0;

        int k = 0;
        int x = 0;
        int y = chunkSize - 1;

        for (int i = 0; i < chunkSize * chunkSize; i++)
        {
            if (i != 0)
            {
                if (i % (chunkSize) == 0 && i != 0)
                {
                    k += 2 * (chunkSize + 1);
                }
                else
                {
                    k += 2;
                }
            }
            if (i % (chunkSize) == 0 && i != 0)
            {
                x++;
            }

            if (blocks[x, y].wallID != WallIndex.noWall)
            {
                //wall verts
                Vector3[] wallVerArr = new Vector3[]{
                        new Vector3(- chunkSize*tileSize/2+(2*x-1)*tileSize/2, - chunkSize*tileSize/2+(2*y-1)*tileSize/2,0),
                        new Vector3(- chunkSize*tileSize/2+(2*x+3)*tileSize/2, - chunkSize*tileSize/2+(2*y-1)*tileSize/2,0),
                        new Vector3(- chunkSize*tileSize/2+(2*x-1)*tileSize/2, - chunkSize*tileSize/2+(2*y+3)*tileSize/2,0),
                        new Vector3(- chunkSize*tileSize/2+(2*x+3)*tileSize/2, - chunkSize*tileSize/2+(2*y+3)*tileSize/2,0)
                    };
                wallVertDict[x * chunkSize + chunkSize - y - 1] = wallVerArr;

                //wall uv
                int face = CalBlockOrWallFace(x, y, blocks[x, y].wallID, false);
                wallUVDict[x * chunkSize + chunkSize - y - 1] = GetWallUVList(blocks[x, y], face);

                //wall tri
                int[] tri = new int[] { wallVertCount, wallVertCount + 2, wallVertCount + 3, wallVertCount + 3, wallVertCount + 1, wallVertCount };
                wallTriDict[x * chunkSize + chunkSize - y - 1] = tri;
                wallVertCount += 4;

                //wall color
                Color32[] colorArr = CalculateColor(blocks[x, y].light);
                wallColorDict[x * chunkSize + chunkSize - y - 1] = colorArr;

            }

            if (blocks[x, y].passable == false)
            {
                Vector3[] chunkVerArr = new Vector3[]{
                    new Vector3(- chunkSize*tileSize/2+(x)*tileSize,
                        - chunkSize*tileSize/2+(y)*tileSize,0),
                    new Vector3(- chunkSize*tileSize/2+(x+1)*tileSize,
                        - chunkSize*tileSize/2+(y)*tileSize,0),
                    new Vector3(- chunkSize*tileSize/2+(x)*tileSize,
                        - chunkSize*tileSize/2+(y+1)*tileSize,0),
                    new Vector3(- chunkSize*tileSize/2+(x+1)*tileSize,
                        - chunkSize*tileSize/2+(y+1)*tileSize,0)
                };
                
                vertDict[x * chunkSize + chunkSize - y - 1] = chunkVerArr;

                //color
                Color32[] colorArr = CalculateColor(blocks[x, y].light);

                colorDict[x * chunkSize + chunkSize - y - 1] = colorArr;
                //color

                int[] triArr = new int[] { vertCount, vertCount + 2, vertCount + 3, vertCount + 3, vertCount + 1, vertCount };
                triDict[x * chunkSize + chunkSize - y - 1] = triArr;
                //triDictsub  
                if(blocks[x, y].tileID == BlockIndex.grassDirt || blocks[x, y].tileID == BlockIndex.junDirt || 
                    blocks[x, y].tileID == BlockIndex.corrDirt){
                    triDictSubIndex[x * chunkSize + chunkSize - y - 1] = 1;
                }
                else{
                    triDictSubIndex[x * chunkSize + chunkSize - y - 1] = 0;
                }

                int face = CalBlockOrWallFace(x, y, blocks[x, y].tileID, true);
                Vector2[] vec2Arr = GetBlockUVList(blocks[x, y], face);
                uvDict[x * chunkSize + chunkSize - y - 1] = vec2Arr;

                List<Vector2> colTem = new List<Vector2>();
                colTem.Add(new Vector2(-chunkSize * tileSize / 2 + (x + 1) * tileSize, -chunkSize * tileSize / 2 + (y + 1) * tileSize));
                colTem.Add(new Vector2(-chunkSize * tileSize / 2 + (x) * tileSize, -chunkSize * tileSize / 2 + (y + 1) * tileSize));
                colTem.Add(new Vector2(-chunkSize * tileSize / 2 + (x) * tileSize, -chunkSize * tileSize / 2 + (y) * tileSize));
                colTem.Add(new Vector2(-chunkSize * tileSize / 2 + (x + 1) * tileSize, -chunkSize * tileSize / 2 + (y) * tileSize));

                vertCount += 4;
                //
                paths.Add(colTem);
                paths1.Add(new List<Vector2>());
                //
            }
            else if (blocks[x, y].tileID != BlockIndex.air)
            {
                Vector3[] chunkVerArr = new Vector3[]{new Vector3(- chunkSize*tileSize/2+(x)*tileSize,
                    - chunkSize*tileSize/2+(y)*tileSize,0),
                    new Vector3(- chunkSize*tileSize/2+(x+1)*tileSize,
                        - chunkSize*tileSize/2+(y)*tileSize,0),
                    new Vector3(- chunkSize*tileSize/2+(x)*tileSize,
                        - chunkSize*tileSize/2+(y+1)*tileSize,0),
                    new Vector3(- chunkSize*tileSize/2+(x+1)*tileSize,
                        - chunkSize*tileSize/2+(y+1)*tileSize,0)
                };

                vertDict[x * chunkSize + chunkSize - y - 1] = chunkVerArr;

                int[] triArr = new int[] { vertCount, vertCount + 2, vertCount + 3, vertCount + 3, vertCount + 1, vertCount };
                triDict[x * chunkSize + chunkSize - y - 1] = triArr;
                //triDictsub  actually if it is passable and not air then it must be triDictsubindex 2 , flower not added
                if(blocks[x, y].tileID == BlockIndex.grassDirt || blocks[x, y].tileID == BlockIndex.junDirt ||
                    blocks[x, y].tileID == BlockIndex.corrDirt){
                    triDictSubIndex[x * chunkSize + chunkSize - y - 1] = 1;
                }
                else{
                    triDictSubIndex[x * chunkSize + chunkSize - y - 1] = 0;
                }

                /*
                if (blocks[x, y].tileID == BlockIndex.grass)
                {
                    int face = blocks[x, y].face;
                    Vector2[] vec2Arr = new Vector2[]{GrassBlock.uvList[face, 0],GrassBlock.uvList[face, 1],
                        GrassBlock.uvList[face, 2],GrassBlock.uvList[face, 3]};
                    uvDict.Add(x * chunkSize + chunkSize - y - 1, vec2Arr);
                    
                }
                else if (blocks[x, y].tileID == BlockIndex.corrGrass)
                {
                    int face = blocks[x, y].face;
                    Vector2[] vec2Arr = new Vector2[]{CorrGrassBlock.uvList[face, 0],CorrGrassBlock.uvList[face, 1],
                        CorrGrassBlock.uvList[face, 2],CorrGrassBlock.uvList[face, 3]};
                    uvDict.Add(x * chunkSize + chunkSize - y - 1, vec2Arr);
                    
                }
                else if (blocks[x, y].tileID == BlockIndex.junGrass)
                {
                    int face = blocks[x, y].face;
                    Vector2[] vec2Arr = new Vector2[]{JungleGrassBlock.uvList[face, 0],JungleGrassBlock.uvList[face, 1],
                        JungleGrassBlock.uvList[face, 2],JungleGrassBlock.uvList[face, 3]};
                    uvDict.Add(x * chunkSize + chunkSize - y - 1, vec2Arr);
                    
                }

                else if (blocks[x, y].tileID == BlockIndex.catus)
                {
                    int face = blocks[x, y].face;
                    Vector2[] vec2Arr = new Vector2[]{CatusBlock.uvList[face, 0],CatusBlock.uvList[face, 1],
                        CatusBlock.uvList[face, 2],CatusBlock.uvList[face, 3]};
                    uvDict.Add(x * chunkSize + chunkSize - y - 1, vec2Arr);
                }
                */

                List<Vector2> triTem = new List<Vector2>();
                triTem.Add(new Vector2(-chunkSize * tileSize / 2 + (x + 1) * tileSize, -chunkSize * tileSize / 2 + (y + 1) * tileSize));
                triTem.Add(new Vector2(-chunkSize * tileSize / 2 + (x) * tileSize, -chunkSize * tileSize / 2 + (y + 1) * tileSize));
                triTem.Add(new Vector2(-chunkSize * tileSize / 2 + (x) * tileSize, -chunkSize * tileSize / 2 + (y) * tileSize));
                triTem.Add(new Vector2(-chunkSize * tileSize / 2 + (x + 1) * tileSize, -chunkSize * tileSize / 2 + (y) * tileSize));
                vertCount += 4;

                paths.Add(new List<Vector2>());
                paths1.Add(triTem);
                //
            }
            else if (blocks[x, y].tileID == BlockIndex.air)
            {
                List<Vector2> triTem = new List<Vector2>();
                triTem.Add(new Vector2(-chunkSize * tileSize / 2 + (x + 1) * tileSize, -chunkSize * tileSize / 2 + (y + 1) * tileSize));
                triTem.Add(new Vector2(-chunkSize * tileSize / 2 + (x) * tileSize, -chunkSize * tileSize / 2 + (y + 1) * tileSize));
                triTem.Add(new Vector2(-chunkSize * tileSize / 2 + (x) * tileSize, -chunkSize * tileSize / 2 + (y) * tileSize));
                triTem.Add(new Vector2(-chunkSize * tileSize / 2 + (x + 1) * tileSize, -chunkSize * tileSize / 2 + (y) * tileSize));

                


                //
                paths.Add(new List<Vector2>());
                paths1.Add(triTem);
                //
            }

            y--;
            if ((k + 2) % (chunkSize * 2) == 0)
            {
                y = chunkSize - 1;
            }
        }

        result = new List<List<Vector2>>(tcp.UniteCollisionPolygons(paths));
        result1 = new List<List<Vector2>>(tcp.UniteCollisionPolygons(paths1));

        pc2D.pathCount = result.Count;
        pc2D1.pathCount = result1.Count;
        for (int i = 0; i < result.Count; i++)
        {
            pc2D.SetPath(i, result[i].ToArray());
        }
        for (int i = 0; i < result1.Count; i++)
        {
            pc2D1.SetPath(i, result1[i].ToArray());
        }

    }

    Color32[,] color4IntensityArr = new Color32[,] {
        //brightest to darkest
                  { new Color32(255,255,255,255),new Color32(255,255,255,255),new Color32(255,255,255,255),new Color32(255,255,255,255)},
                  { new Color32(224,224,224,255),new Color32(224,224,224,255),new Color32(224,224,224,255),new Color32(224,224,224,255) },
                  { new Color32(172,172,172,255),new Color32(172,172,172,255),new Color32(172,172,172,255),new Color32(172,172,172,255), },
                  { new Color32(132,132,132,255),new Color32(132,132,132,255),new Color32(132,132,132,255),new Color32(132,132,132,255), },
                  { new Color32(84,84,84,255), new Color32(84,84,84,255), new Color32(84,84,84,255), new Color32(84,84,84,255)},
                  { new Color32(32,32,32,255),new Color32(32,32,32,255),new Color32(32,32,32,255),new Color32(32,32,32,255) },
                  { new Color32(0,0,0,255),new Color32(0,0,0,255),new Color32(0,0,0,255),new Color32(0,0,0,255) },                            
                };


    public void RefreshChunkCollTri()
    {//refresh collider and trigger shape: it can be further improved just reducing the coll verts of 
     //removed or added block  
         

        result = new List<List<Vector2>>(tcp.UniteCollisionPolygons(paths));
        result1 = new List<List<Vector2>>(tcp.UniteCollisionPolygons(paths1));

        pc2D.pathCount = result.Count;
        pc2D1.pathCount = result1.Count;


        for (int i = 0; i < result.Count; i++)
        {
            pc2D.SetPath(i, result[i].ToArray());
        }
        for (int i = 0; i < result1.Count; i++)
        {
            pc2D1.SetPath(i, result1[i].ToArray());
        }
    }

    //for adding collider and removing trigger
    public void AddPathsRemovePaths1(int blockx, int blocky)//used to update the path and paths (collider and trigger)
    {
        int index = blockx * chunkSize + chunkSize - blocky - 1;
        List<Vector2> tem = new List<Vector2>();
        tem.Add(new Vector2(-chunkSize * tileSize / 2 + (blockx + 1) * tileSize, -chunkSize * tileSize / 2 + (blocky + 1) * tileSize));
        tem.Add(new Vector2(-chunkSize * tileSize / 2 + (blockx) * tileSize, -chunkSize * tileSize / 2 + (blocky + 1) * tileSize));
        tem.Add(new Vector2(-chunkSize * tileSize / 2 + (blockx) * tileSize, -chunkSize * tileSize / 2 + (blocky) * tileSize));
        tem.Add(new Vector2(-chunkSize * tileSize / 2 + (blockx + 1) * tileSize, -chunkSize * tileSize / 2 + (blocky) * tileSize));

        paths[index] = tem;
        paths1[index] = new List<Vector2>();
    }
    //for adding trigger and removing collider
    public void AddPaths1RemovePaths(int blockx, int blocky)//used to update the path and paths (collider and trigger)
    {
        int index = blockx * chunkSize + chunkSize - blocky - 1;
        List<Vector2> tem = new List<Vector2>();
        tem.Add(new Vector2(-chunkSize * tileSize / 2 + (blockx + 1) * tileSize, -chunkSize * tileSize / 2 + (blocky + 1) * tileSize));
        tem.Add(new Vector2(-chunkSize * tileSize / 2 + (blockx) * tileSize, -chunkSize * tileSize / 2 + (blocky + 1) * tileSize));
        tem.Add(new Vector2(-chunkSize * tileSize / 2 + (blockx) * tileSize, -chunkSize * tileSize / 2 + (blocky) * tileSize));
        tem.Add(new Vector2(-chunkSize * tileSize / 2 + (blockx + 1) * tileSize, -chunkSize * tileSize / 2 + (blocky) * tileSize));

        paths1[index] = tem;
        paths[index] = new List<Vector2>();
    }
    
    public int CalBlockOrWallFace(int x, int y, int currTileOrWallID, bool isTileID)//should be called in its own chunk since it checks it's own blocks
    {
        bool[] facearr = new bool[] { false,false,false,false};//l r t b

        //int currTileID = blocks[x, y].tileID;//this block's tileID
        //In chunk's boundary
        if (x > 0 && y > 0 && x < chunkSize - 1 && y < chunkSize - 1)
        {
            int l, r, t, b = 0;

            if (isTileID) {
                l = blocks[x - 1, y].tileID; r = blocks[x + 1, y].tileID;
                t = blocks[x, y + 1].tileID; b = blocks[x, y - 1].tileID;
            }else {
                l = blocks[x - 1, y].wallID; r = blocks[x + 1, y].wallID;
                t = blocks[x, y + 1].wallID; b = blocks[x, y - 1].wallID;
            }
            //top
            if ((r == currTileOrWallID) && (l == currTileOrWallID)
                            && (t != currTileOrWallID) && (b == currTileOrWallID))
            {
                return 0;
            }
            //right
            else if ((r != currTileOrWallID) && (l == currTileOrWallID)
                            && (t == currTileOrWallID) && (b == currTileOrWallID))
            {
                return 1;
            }
            //bot
            else if ((r == currTileOrWallID) && (l == currTileOrWallID)
                            && (t == currTileOrWallID) && (b != currTileOrWallID))
            {
                return 2;
            }
            //left
            else if ((r == currTileOrWallID) && (l != currTileOrWallID)
                            && (t == currTileOrWallID) && (b == currTileOrWallID))
            {
                return 3;
            }
            //right top
            else if ((r != currTileOrWallID) && (l == currTileOrWallID)
                            && (t != currTileOrWallID) && (b == currTileOrWallID))
            {
                return 4;
            }
            //right bot
            else if ((r != currTileOrWallID) && (l == currTileOrWallID)
                            && (t == currTileOrWallID) && (b != currTileOrWallID))
            {
                return 5;
            }
            //left bot
            else if ((r == currTileOrWallID) && (l != currTileOrWallID)
                            && (t == currTileOrWallID) && (b != currTileOrWallID))
            {
                return 6;
            }
            //left top
            else if ((r == currTileOrWallID) && (l != currTileOrWallID)
                            && (t != currTileOrWallID) && (b == currTileOrWallID))
            {
                return 7;
            }
            //left right
            else if ((r != currTileOrWallID) && (l != currTileOrWallID)
                            && (t == currTileOrWallID) && (b == currTileOrWallID))
            {
                return 8;
            }
            //top bot
            else if ((r == currTileOrWallID) && (l == currTileOrWallID)
                            && (t != currTileOrWallID) && (b != currTileOrWallID))
            {
                return 9;
            }
            //top right bot
            else if ((r != currTileOrWallID) && (l == currTileOrWallID)
                            && (t != currTileOrWallID) && (b != currTileOrWallID))
            {
                return 10;
            }
            //left bot right
            else if ((r != currTileOrWallID) && (l != currTileOrWallID)
                            && (t == currTileOrWallID) && (b != currTileOrWallID))
            {
                return 11;
            }
            //top left bot
            else if ((r == currTileOrWallID) && (l != currTileOrWallID)
                            && (t != currTileOrWallID) && (b != currTileOrWallID))
            {
                return 12;
            }
            //left top right
            else if ((r != currTileOrWallID) && (l != currTileOrWallID)
                            && (t != currTileOrWallID) && (b == currTileOrWallID))
            {
                return 13;
            }
            //outward
            else if ((r == currTileOrWallID) && (l == currTileOrWallID)
                            && (t == currTileOrWallID) && (b == currTileOrWallID))
            {
                return 14;
            }
            //inward
            else if ((r != currTileOrWallID) && (l != currTileOrWallID)
                            && (t != currTileOrWallID) && (b != currTileOrWallID))
            {
                return 15;
            }
        }
        else
        {
            if (blocks[x, y].bWorldPos[0] == World.worldLBlockPos && blocks[x, y].bWorldPos[1] == World.worldTBlockPos)//left top
            {
                facearr[0] = false; facearr[2] = false;
                facearr[1] = TestR(x, y, isTileID);
                facearr[3] = TestB(x, y, isTileID);
                
            }
            else if (blocks[x, y].bWorldPos[1] == World.worldTBlockPos && blocks[x, y].bWorldPos[0] != World.worldLBlockPos && //top
                (blocks[x, y].bWorldPos[0] != World.worldRBlockPos))
            {
                facearr[2] = false;
                facearr[0] = TestL(x, y, isTileID);
                facearr[1] = TestR(x, y, isTileID);
                facearr[3] = TestB(x, y, isTileID);
            }
            else if (blocks[x, y].bWorldPos[0] == World.worldRBlockPos && blocks[x, y].bWorldPos[1] == World.worldTBlockPos) 
            {
                facearr[1] = false; facearr[2] = false;
                facearr[0] = TestL(x, y, isTileID);
                facearr[3] = TestB(x, y, isTileID);
            }
            else if (blocks[x, y].bWorldPos[0] == World.worldRBlockPos && blocks[x, y].bWorldPos[1] != World.worldTBlockPos &&
                blocks[x, y].bWorldPos[1] != World.worldLBlockPos)
            {
                facearr[1] = false;
                facearr[0] = TestL(x, y, isTileID);
                facearr[2] = TestT(x, y, isTileID);
                facearr[3] = TestB(x, y, isTileID);
            }
            else if (blocks[x, y].bWorldPos[1] == World.worldBBlockPos && blocks[x, y].bWorldPos[0] == World.worldRBlockPos)
            {
                facearr[1] = false; facearr[3] = false;
                facearr[0] = TestL(x, y, isTileID);
                facearr[2] = TestT(x, y, isTileID);
            }
            else if (blocks[x, y].bWorldPos[1] == World.worldBBlockPos && blocks[x, y].bWorldPos[0] != World.worldRBlockPos &&
                blocks[x, y].bWorldPos[0] != World.worldLBlockPos)
            {
                facearr[3] = false;
                facearr[0] = TestL(x, y, isTileID);
                facearr[1] = TestR(x, y, isTileID);
                facearr[2] = TestT(x, y, isTileID);
            }
            else if (blocks[x, y].bWorldPos[0] == World.worldLBlockPos && blocks[x, y].bWorldPos[1] == World.worldBBlockPos)
            {
                facearr[0] = false; facearr[3] = false;
                facearr[1] = TestR(x, y, isTileID);
                facearr[2] = TestT(x, y, isTileID);
            }
            else if (blocks[x, y].bWorldPos[0] == World.worldLBlockPos && blocks[x, y].bWorldPos[1] != World.worldTBlockPos &&
                blocks[x, y].bWorldPos[1] != World.worldBBlockPos)
            {
                facearr[0] = false;
                facearr[1] = TestR(x, y, isTileID);
                facearr[2] = TestT(x, y, isTileID);//aa
                facearr[3] = TestB(x, y, isTileID);
            }
            
            else//x,y not on the edge of world boundary, then test only in chunk's boundary
            {
                facearr[0] = TestL(x, y, isTileID);
                facearr[1] = TestR(x, y, isTileID);
                facearr[2] = TestT(x, y, isTileID);
                facearr[3] = TestB(x, y, isTileID);
            }
            

            if (facearr[0] == true && facearr[1] == true && facearr[2] == false && facearr[3] == true) { return 0; }//top
            if (facearr[0] == true && facearr[1] == false && facearr[2] == true && facearr[3] == true) { return 1; }//right
            if (facearr[0] == true && facearr[1] == true && facearr[2] == true && facearr[3] == false) { return 2; }//bot
            if (facearr[0] == false && facearr[1] == true && facearr[2] == true && facearr[3] == true) { return 3; }//left
            if (facearr[0] == true && facearr[1] == false && facearr[2] == false && facearr[3] == true) { return 4; }//right top
            if (facearr[0] == true && facearr[1] == false && facearr[2] == true && facearr[3] ==false) { return 5; }//right bot
            if (facearr[0] ==false && facearr[1] == true && facearr[2] == true && facearr[3] ==false) { return 6; }//left bot
            if (facearr[0] ==false && facearr[1] == true && facearr[2] == false && facearr[3] == true) { return 7; }//left top
            if (facearr[0] ==false && facearr[1] ==false && facearr[2] == true && facearr[3] == true) { return 8; }//left right
            if (facearr[0] == true && facearr[1] == true && facearr[2] == false && facearr[3] ==false) { return 9; }//top bot
            if (facearr[0] == true && facearr[1] ==false && facearr[2] == false && facearr[3] ==false) { return 10; }//top right bot
            if (facearr[0] == false && facearr[1] ==false && facearr[2] == true && facearr[3] ==false) { return 11; }//left bot right
            if (facearr[0] == false && facearr[1] == true && facearr[2] == false && facearr[3] ==false) { return 12; }//top left bot
            if (facearr[0] == false && facearr[1] ==false && facearr[2] == false && facearr[3] == true) { return 13; }//left top right
            if (facearr[0] == true && facearr[1] == true && facearr[2] == true && facearr[3] == true) { return 14; }//outward
            if (facearr[0] == false && facearr[1] == false && facearr[2] == false && facearr[3] == false) { return 15; }//inward

        }
        return 15;//default which is impossible to reach
    }
    public bool TestL(int x, int y, bool isTileID) //Test against the tileID of left block, if left block has the same tileid as curr block, then return true
    {
        bool retVal = false;
        if (x == 0)
        {
            if (isTileID)
            {
                if (world.GetChunk(pos[0] - chunkSize * tileSize, pos[1]) != null &&
                    world.GetChunk(pos[0] - chunkSize * tileSize, pos[1]).blocks[chunkSize - 1, y].tileID == blocks[x, y].tileID)
                {
                    retVal = true;
                }
            }else
            {
                if (world.GetChunk(pos[0] - chunkSize * tileSize, pos[1]) != null &&
                    world.GetChunk(pos[0] - chunkSize * tileSize, pos[1]).blocks[chunkSize - 1, y].wallID == blocks[x, y].wallID)
                {
                    retVal = true;
                }
            }
        }
        else
        {
            if (isTileID)
            {
                if (blocks[x - 1, y].tileID == blocks[x, y].tileID)
                {
                    retVal = true;
                }
            }else
            {
                if (blocks[x - 1, y].wallID == blocks[x, y].wallID)
                {
                    retVal = true;
                }
            }
        }
        return retVal;
    }
    public bool TestR(int x, int y, bool isTileID)
    {
        bool retVal = false;
        if (x == chunkSize - 1)
        {
            if (isTileID)
            {
                if (world.GetChunk(pos[0] + chunkSize * tileSize, pos[1]) != null &&
                world.GetChunk(pos[0] + chunkSize * tileSize, pos[1]).blocks[0, y].tileID == blocks[x, y].tileID)
                {
                    retVal = true;
                }
            }
            else
            {
                if (world.GetChunk(pos[0] + chunkSize * tileSize, pos[1]) != null &&
                world.GetChunk(pos[0] + chunkSize * tileSize, pos[1]).blocks[0, y].wallID == blocks[x, y].wallID)
                {
                    retVal = true;
                }
            }
        }
        else
        {
            if (isTileID)
            {
                if (blocks[x + 1, y].tileID == blocks[x, y].tileID)
                {
                    retVal = true;
                }
            }
            else
            {
                if (blocks[x + 1, y].wallID == blocks[x, y].wallID)
                {
                    retVal = true;
                }
            }
        }
        return retVal;
    }
    public bool TestT(int x, int y, bool isTileID)
    {
        bool retVal = false;
        if (y == chunkSize - 1)
        {
            if (isTileID)
            {
                if (world.GetChunk(pos[0], pos[1] + chunkSize * tileSize) != null &&
                    world.GetChunk(pos[0], pos[1] + chunkSize * tileSize).blocks[x, 0].tileID == blocks[x, y].tileID)
                {
                    retVal = true;
                }
            }else
            {
                if (world.GetChunk(pos[0], pos[1] + chunkSize * tileSize) != null &&
                    world.GetChunk(pos[0], pos[1] + chunkSize * tileSize).blocks[x, 0].wallID == blocks[x, y].wallID)
                {
                    retVal = true;
                }
            }
        }
        else
        {
            if (isTileID)
            {
                if (blocks[x, y + 1].tileID == blocks[x, y].tileID)
                {
                    retVal = true;
                }
            }
            else
            {
                if (blocks[x, y + 1].wallID == blocks[x, y].wallID)
                {
                    retVal = true;
                }
            }
        }
        return retVal;
    }
    public bool TestB(int x, int y, bool isTileID)
    {
        bool retVal = false;
        if (y == 0)
        {
            if (isTileID)
            {
                if (world.GetChunk(pos[0], pos[1] - chunkSize * tileSize) != null &&
                    world.GetChunk(pos[0], pos[1] - chunkSize * tileSize).blocks[x, chunkSize - 1].tileID == blocks[x, y].tileID)
                {
                    retVal = true;
                }
            }else
            {
                if (world.GetChunk(pos[0], pos[1] - chunkSize * tileSize) != null &&
                    world.GetChunk(pos[0], pos[1] - chunkSize * tileSize).blocks[x, chunkSize - 1].wallID == blocks[x, y].wallID)
                {
                    retVal = true;
                }
            }
        }
        else
        {
            if (isTileID)
            {
                if (blocks[x, y - 1].tileID == blocks[x, y].tileID)
                {
                    retVal = true;
                }
            }else
            {
                if (blocks[x, y - 1].wallID == blocks[x, y].wallID)
                {
                    retVal = true;
                }
            }
        }
        return retVal;
    }
    //should be modified that get block will check whether the block get is in world's boundary or chunk's boundary
    public Block GetBlock(int blockWorldPosx, int blockWorldPosy)
    {
        bool withinpY = blockWorldPosy < pos[1] + tileSize * chunkSize / 2 ? true : false;
        bool withinpX = blockWorldPosx < pos[0] + tileSize * chunkSize / 2 ? true : false;
        bool withinnY = blockWorldPosy > pos[1] - tileSize * chunkSize / 2 ? true : false;
        bool withinnX = blockWorldPosx > pos[0] - tileSize * chunkSize / 2 ? true : false;

        if (!withinpX || !withinpY || !withinnX || !withinnY)
        {
            //if within world's boundary
            if (blockWorldPosx > World.worldLBlockPos && blockWorldPosx < World.worldRBlockPos && blockWorldPosy > World.worldBBlockPos &&
              blockWorldPosy < World.worldTBlockPos)
            {
                int chunkTileSize = chunkSize * tileSize;
                //blockWorldPosx - (World.worldLMostChunkPosX - chunkSize2 / 2) / (chunkSize2) -> index of chunkx
                int cPosX = Mathf.FloorToInt((blockWorldPosx - (World.worldLMostChunkPosX - chunkTileSize / 2)) / (chunkTileSize)) *
                    chunkTileSize + World.worldLMostChunkPosX;
                int cPosY = Mathf.FloorToInt((blockWorldPosy - (World.worldBMostChunkPosY - chunkTileSize / 2)) / (chunkTileSize)) *
                    chunkTileSize + World.worldBMostChunkPosY;

                
                //doesnt need to check whether the chunk is null.
                return world.GetChunk(cPosX, cPosY).GetBlock(blockWorldPosx, blockWorldPosy);
            }
            else //out of world's boundary
            {
                return new Block();//"valid" variable in Block's struct is false by default
            }
        }
        else
        {
            int blockChunkArrayx = Mathf.FloorToInt((blockWorldPosx - pos[0] + (chunkSize * tileSize / 2)) / (tileSize));
            int blockChunkArrayy = Mathf.FloorToInt((blockWorldPosy - pos[1] + (chunkSize * tileSize / 2)) / (tileSize));
            return blocks[blockChunkArrayx, blockChunkArrayy];
        }
    }

    

    public void SetBlock(int blockWorldPosx, int blockWorldPosy, ushort tileID, byte fluid, byte light, byte wallID)
    //Use tileID instead since SetBlock() is modifying block with a new tileID but not copying blocks
    {
        bool withinpY = blockWorldPosy < pos[1] + tileSize * chunkSize / 2 ? true : false;
        bool withinpX = blockWorldPosx < pos[0] + tileSize * chunkSize / 2 ? true : false;
        bool withinnY = blockWorldPosy > pos[1] - tileSize * chunkSize / 2 ? true : false;
        bool withinnX = blockWorldPosx > pos[0] - tileSize * chunkSize / 2 ? true : false;

        //If not inside the chunk's boundary
        if (!withinpX || !withinpY || !withinnX || !withinnY)
        {
            //if within world's boundary
            if (blockWorldPosx > World.worldLBlockPos && blockWorldPosx < World.worldRBlockPos && blockWorldPosy > World.worldBBlockPos &&
              blockWorldPosy < World.worldTBlockPos)
            {
                int chunkTileSize = chunkSize * tileSize;
                //blockWorldPosx - (World.worldLMostChunkPosX - chunkSize2 / 2) / (chunkSize2) -> index of chunkx
                int cPosX = Mathf.FloorToInt((blockWorldPosx - (World.worldLMostChunkPosX - chunkTileSize / 2)) / (chunkTileSize)) *
                    chunkTileSize + World.worldLMostChunkPosX;
                int cPosY = Mathf.FloorToInt((blockWorldPosy - (World.worldBMostChunkPosY - chunkTileSize / 2)) / (chunkTileSize)) *
                    chunkTileSize + World.worldBMostChunkPosY;
                world.GetChunk(cPosX, cPosY).SetBlock(blockWorldPosx, blockWorldPosy, tileID, fluid, light, wallID);
            }
            //else out of world's boundary do nothing
        }
        else   //if the block is not out of chunk, then these codes should be called
        {
            int blockChunkArrayx = Mathf.FloorToInt((blockWorldPosx - pos[0] + (chunkSize * tileSize / 2)) / (tileSize));
            int blockChunkArrayy = Mathf.FloorToInt((blockWorldPosy - pos[1] + (chunkSize * tileSize / 2)) / (tileSize));

            blocks[blockChunkArrayx, blockChunkArrayy].valid = true;//only set it here
            blocks[blockChunkArrayx, blockChunkArrayy].tileID = tileID;
            blocks[blockChunkArrayx, blockChunkArrayy].fluid = fluid;
            blocks[blockChunkArrayx, blockChunkArrayy].light = light;
            blocks[blockChunkArrayx, blockChunkArrayy].wallID = wallID;
            blocks[blockChunkArrayx, blockChunkArrayy].bWorldPos = new int[] { blockWorldPosx, blockWorldPosy };
            blocks[blockChunkArrayx, blockChunkArrayy].passable = blocks[blockChunkArrayx, blockChunkArrayy].IsPassable(blocks[blockChunkArrayx, blockChunkArrayy]);
            blocks[blockChunkArrayx, blockChunkArrayy].movable = blocks[blockChunkArrayx, blockChunkArrayy].IsMovable(blocks[blockChunkArrayx, blockChunkArrayy]);
        }
    }
    
    //Set tile's light amount only, +/-,  !!! light is in int but not byte is because byte is in range(0,255) that not support -.
    //This func is supposed to support decrease too, but will clamp the range of tile's light in range(0,255)
    public void UpdateBlockLight(int blockWorldPosx, int blockWorldPosy, int light)
    {
        bool withinpY = blockWorldPosy < pos[1] + tileSize * chunkSize / 2 ? true : false;
        bool withinpX = blockWorldPosx < pos[0] + tileSize * chunkSize / 2 ? true : false;
        bool withinnY = blockWorldPosy > pos[1] - tileSize * chunkSize / 2 ? true : false;
        bool withinnX = blockWorldPosx > pos[0] - tileSize * chunkSize / 2 ? true : false;

        //If not inside the chunk's boundary
        if (!withinpX || !withinpY || !withinnX || !withinnY)
        {
            //if inside world's boundary
            if (blockWorldPosx > World.worldLBlockPos && blockWorldPosx < World.worldRBlockPos && blockWorldPosy > World.worldBBlockPos &&
              blockWorldPosy < World.worldTBlockPos)
            {
                int chunkTileSize = chunkSize * tileSize;
                //blockWorldPosx - (World.worldLMostChunkPosX - chunkSize2 / 2) / (chunkSize2) -> index of chunkx
                int cPosX = Mathf.FloorToInt((blockWorldPosx - (World.worldLMostChunkPosX - chunkTileSize / 2)) / (chunkTileSize)) *
                    chunkTileSize + World.worldLMostChunkPosX;
                int cPosY = Mathf.FloorToInt((blockWorldPosy - (World.worldBMostChunkPosY - chunkTileSize / 2)) / (chunkTileSize)) *
                    chunkTileSize + World.worldBMostChunkPosY;

                world.GetChunk(cPosX, cPosY).UpdateBlockLight(blockWorldPosx, blockWorldPosy, light);
            }
            //else out of world's boundary
        }
        else   //if the block is not out of chunk, then these codes should be called
        {
            int blockChunkArrayx = Mathf.FloorToInt((blockWorldPosx - pos[0] + (chunkSize * tileSize / 2)) / (tileSize));
            int blockChunkArrayy = Mathf.FloorToInt((blockWorldPosy - pos[1] + (chunkSize * tileSize / 2)) / (tileSize));

            if ((int)(blocks[blockChunkArrayx, blockChunkArrayy].light + light) > 255) //exceed the max range
            {
                blocks[blockChunkArrayx, blockChunkArrayy].light = 255;
            }
            else if ((int)(blocks[blockChunkArrayx, blockChunkArrayy].light + light) < 0) //exceed the min range
            {
                blocks[blockChunkArrayx, blockChunkArrayy].light = 0;
            }
            else
            {
                blocks[blockChunkArrayx, blockChunkArrayy].light += (byte)light;
            }
        }
    }
    //Set fluid only [Check if this tile is (passable and not air block) or not, if (passable and not air block) then remove this tile.]
    //Set tile's fluid amount only, +/-,  !!! fluid is in int but not byte is because byte is in range(0,255) that not support -.
    //This func is supposed to support decrease too, but will clamp the range of tile's fluid in range(0,255)
    public void UpdateBlockFluid(int blockWorldPosx, int blockWorldPosy, int fluid)
    {
        bool withinpY = blockWorldPosy < pos[1] + tileSize * chunkSize / 2 ? true : false;
        bool withinpX = blockWorldPosx < pos[0] + tileSize * chunkSize / 2 ? true : false;
        bool withinnY = blockWorldPosy > pos[1] - tileSize * chunkSize / 2 ? true : false;
        bool withinnX = blockWorldPosx > pos[0] - tileSize * chunkSize / 2 ? true : false;

        //If not inside the chunk's boundary
        if (!withinpX || !withinpY || !withinnX || !withinnY)
        {
            //if inside world's boundary
            if (blockWorldPosx > World.worldLBlockPos && blockWorldPosx < World.worldRBlockPos && blockWorldPosy > World.worldBBlockPos &&
              blockWorldPosy < World.worldTBlockPos)
            {
                int chunkTileSize = chunkSize * tileSize;
                //blockWorldPosx - (World.worldLMostChunkPosX - chunkSize2 / 2) / (chunkSize2) -> index of chunkx
                int cPosX = Mathf.FloorToInt((blockWorldPosx - (World.worldLMostChunkPosX - chunkTileSize / 2)) / (chunkTileSize)) *
                    chunkTileSize + World.worldLMostChunkPosX;
                int cPosY = Mathf.FloorToInt((blockWorldPosy - (World.worldBMostChunkPosY - chunkTileSize / 2)) / (chunkTileSize)) *
                    chunkTileSize + World.worldBMostChunkPosY;
                world.GetChunk(cPosX, cPosY).UpdateBlockFluid(blockWorldPosx, blockWorldPosy, fluid);
            }
            //else out of world's boundary
        }
        else   //if the block is not out of chunk, then these codes should be called
        {
            int blockChunkArrayx = Mathf.FloorToInt((blockWorldPosx - pos[0] + (chunkSize * tileSize / 2)) / (tileSize));
            int blockChunkArrayy = Mathf.FloorToInt((blockWorldPosy - pos[1] + (chunkSize * tileSize / 2)) / (tileSize));

            if ((int)(blocks[blockChunkArrayx, blockChunkArrayy].fluid + fluid) > 255) //exceed the max range
            {
                blocks[blockChunkArrayx, blockChunkArrayy].fluid = 255;
            }
            else if ((int)(blocks[blockChunkArrayx, blockChunkArrayy].fluid + fluid) < 0) //exceed the min range
            {
                blocks[blockChunkArrayx, blockChunkArrayy].fluid = 0;
            }
            else
            {
                blocks[blockChunkArrayx, blockChunkArrayy].fluid += (byte)fluid;
            }
        }
    }

}




