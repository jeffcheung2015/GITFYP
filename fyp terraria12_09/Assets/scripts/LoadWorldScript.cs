using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

//This one is for loading the world in game
public class LoadWorldScript : MonoBehaviour{

    //world Database
    WorldDatabase worldDB;

	public static bool LoadTheWorld = false;//trigger this from the world button and
    //will be set false just after the call of LoadWorld and before the first yield null
    public bool worldLoaded = false;//will be set true in DrawWorldLoadScene() and true forever

	public static string worldName;

	public GameObject playerPrefab;
    Rigidbody2D prb;

    GameObject camObj;
    GameObject player;
    
    int frame = 0;
    int frame1 = 0;
    int frame2 = 0;
    public bool worldCreated = false;

    World world;
    bool loadWorldAtRunTimeIsCalled = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);        
    }
    void Start()
    {
        world = GetComponent<World>();        
    }

    bool savingWorld = false;//Ensure two or more saveworld() wouldn't be called at the same time.

    void Update()
    {
        if (LoadTheWorld)
        {
            //instantiate the player
            player = Instantiate(playerPrefab, new Vector3((float)0, (float)1200, 0),
                Quaternion.Euler(Vector3.zero)) as GameObject;
            prb = player.GetComponent<Rigidbody2D>();
            prb.isKinematic = true;

            camObj = player.GetComponentInChildren<Camera>().gameObject;

            camObj.SetActive(false);//set the camera of the player inactive first then activate it later after the world is generated

            DontDestroyOnLoad(player);
            StartCoroutine("LoadWorld");
        }
        if (!savingWorld) { frame++; }
        
        frame1++;
        if (frame > 500 && !savingWorld)
        {
            frame = 0;
            savingWorld = true;
            StartCoroutine("SaveWorld");
        }
        if(frame > 1000)
        {
            //load only 20 * 20 chunks arround.
            //drop those chunks that are far away from player.
            frame2 = 0;
        }
        if (frame1 > 60 && worldLoaded == true)
        {
            frame1 = 0;
            DrawWorldAtRunTime();
        }
        if (worldLoaded == true && loadWorldAtRunTimeIsCalled == false)
        {
            loadWorldAtRunTimeIsCalled = true;
            StartCoroutine("LoadWorldAtRunTime");
        }
    }

    int enumCount = 0;
    public IEnumerator SaveWorld() //called every 500 frame
    {
        if (worldCreated == true)//check whether the world is created
        {            
            foreach (var c in world.chunks)
            {
                if (c.Value.hasChanged == true)
                {
                    //Convert chunk's block to savable forms.E.g.: Block's info (tileid, fluid).
                    List<int[]> resList = new List<int[]>();
                    int[] tileID = new int[Chunk.chunkSize * Chunk.chunkSize];
                    int[] wallID = new int[Chunk.chunkSize * Chunk.chunkSize];
                    int[] fluid = new int[Chunk.chunkSize * Chunk.chunkSize];

                    for (int i = 0; i < Chunk.chunkSize; i++)
                    {
                        for (int j = 0; j < Chunk.chunkSize; j++)
                        {
                            tileID[i * Chunk.chunkSize + j] = c.Value.blocks[i, j].tileID;
                            wallID[i * Chunk.chunkSize + j] = c.Value.blocks[i, j].wallID;
                            fluid[i * Chunk.chunkSize + j] = c.Value.blocks[i, j].fluid;
                        }
                    }
                    worldDB.UpdateChunk(c.Value.pos[0], c.Value.pos[1], Enumerable.Range(0, 256).ToArray(), tileID, wallID, fluid);

                    Debug.Log("chunk saved"+c.Value.pos[0]+","+c.Value.pos[1]);

                    enumCount++;
                    if (enumCount >= 2)//Control the save speed.
                    {
                        enumCount = 0;
                        yield return null;
                    }
                }
            }
            savingWorld = false;
        }
    }
    

    public void DrawWorldAtRunTime()
    {//drawing the world according the player's position
        foreach (var c in world.chunks)
        {
            if(c.Value.isDrew == true && c.Value.pos[0] >= player.transform.position.x - 5 * Chunk.chunkSize * Chunk.tileSize
                && c.Value.pos[0] <= player.transform.position.x + 5 * Chunk.chunkSize * Chunk.tileSize)
            {
                continue;
            }
            else if (c.Value.isDrew == true && (c.Value.pos[0] < player.transform.position.x - 5 * Chunk.chunkSize * Chunk.tileSize
                || c.Value.pos[0] > player.transform.position.x + 5 * Chunk.chunkSize * Chunk.tileSize))
            {
                c.Value.isDrew = false;
                c.Value.ClearMeshAndCollider();
            }
            else if (c.Value.isDrew == false && c.Value.pos[0] >= player.transform.position.x - 5 * Chunk.chunkSize * Chunk.tileSize
                && c.Value.pos[0] <= player.transform.position.x + 5 * Chunk.chunkSize * Chunk.tileSize)
            {
                c.Value.isDrew = true;
                c.Value.RevertOrSetChunkMesh();
            }
            else if (c.Value.isDrew == false && (c.Value.pos[0] < player.transform.position.x - 5 * Chunk.chunkSize * Chunk.tileSize
                || c.Value.pos[0] > player.transform.position.x + 5 * Chunk.chunkSize * Chunk.tileSize))
            {
                continue;
            }
        }
    }
    bool loadAllWorld = false;
    IEnumerator LoadWorldAtRunTime()
    {        
        if (loadAllWorld == false)
        {
            

            yield return null;
            loadAllWorld = true;
        }
        Debug.Log("End loading World!!!!");
    }
    
    public GameObject chunkPrefab;//need to be set in editor

    /// <summary>
    /// Create game obj for chunks ,Object pooling(Not implemented)
    /// </summary>
	void CreateWorld(){//is called inside LoadWorld()
		int originChunkWorldPosx = (int)(0 + Chunk.tileSize * Chunk.chunkSize/2 -
		                                 Chunk.tileSize * Chunk.chunkSize * World.worldLeftChunksNo);
		int originChunkWorldPosy = (int)(0 + Chunk.tileSize * Chunk.chunkSize/2 -
		                                 Chunk.tileSize * Chunk.chunkSize * World.worldBotChunksNo);              

		for(int i = 0; i < World.worldSizeInChunkx; i ++){
			for(int j = 0; j < World.worldSizeInChunky; j ++){
				
				world.CreateChunk (chunkPrefab,originChunkWorldPosx + i * Chunk.chunkSize * Chunk.tileSize,
				                      originChunkWorldPosy + j * Chunk.chunkSize * Chunk.tileSize);							
			}
		}
        player.GetComponent<PlayerControlScript>().world = world;
        worldCreated = true;
	}

	/// <summary>
    /// Draw left and right 5 chunks arround the player first.
    /// </summary>
	IEnumerator DrawWorldLoadScene(){  //also handle the fog
        //Must be called after loadWorld since the setchunkmesh call in draw world will make use of neig chunk
        int loopCount = 0;//Used to adjust the drawing speed by controlling the no of foreach loop.
        foreach (var c in world.chunks)
        {

            if (c.Value.pos[0] <= Chunk.chunkSize * Chunk.tileSize * 5 &&
                c.Value.pos[0] >= -Chunk.chunkSize * Chunk.tileSize * 5)
            {
                c.Value.StartUpSetLightAmount();
                
            }
        }

            foreach (var c in world.chunks)
        {

            if (c.Value.pos[0] <= Chunk.chunkSize * Chunk.tileSize * 5 &&
                c.Value.pos[0] >= -Chunk.chunkSize * Chunk.tileSize * 5)
            {                
                c.Value.SetChunkMesh();
                c.Value.isDrew = true;             
            }
            else {continue;}
            loopCount++;
            if (loopCount >= 2)//Change this no. to adjust the drawing speed, but will scarifice the performance.
            {
                loopCount = 0;
                yield return null;
            }        
        }
		camObj.SetActive (true);//After drawing world then set the player's camera active
        prb.isKinematic = false;//Setting the player's rigidbody2d's kinematic false so it is in free fall
        worldLoaded = true;
		SceneManager.LoadScene ("insideGame");//Load the scene
	}

    /// <summary>
    /// Load the chunks from database and store it in chunk gameobject.
    /// </summary>
    public void LoadWorld(){
        LoadTheWorld = false;//need to be placed before the yield return,otherwise multiple coroutine called

        world.worldName = worldName;//assign the world script's worldName
        //Database ,table initialization
        worldDB = new WorldDatabase(worldName);
        worldDB.SQLiteInit();
        worldDB.CreateTable();
        //

        CreateWorld();
        //First load 20 * 20 chunks arround player.
        List<List<int[]>> listChunksInfo = worldDB.LoadSquareRangeOfChunk(
            (int)(player.transform.position.x - 10 * Chunk.chunkSize * Chunk.tileSize),
            (int)(player.transform.position.y - 10 * Chunk.chunkSize * Chunk.tileSize),
            (int)(player.transform.position.x + 10 * Chunk.chunkSize * Chunk.tileSize),
            (int)(player.transform.position.y + 10 * Chunk.chunkSize * Chunk.tileSize));

        for (int i = 0; i < listChunksInfo.Count; i++) {
            LoadChunk(listChunksInfo[i][0][0],listChunksInfo[i][0][1], listChunksInfo[i][1], listChunksInfo[i][2], listChunksInfo[i][3]);
            world.chunks[new int[] { listChunksInfo[i][0][0], listChunksInfo[i][0][1] }].isLoaded = true;
        }

       


        StartCoroutine("DrawWorldLoadScene");
	}


    /// <summary>
    /// //Load specific chunk, Explicitly convert tileid to ushort and fluid to byte
    /// </summary>
    /// <param name="chunkPosX"></param>
    /// <param name="chunkPosY"></param>
    /// <param name="tileid"></param>
    /// <param name="fluid"></param>
    void LoadChunk(int chunkPosX, int chunkPosY, int[] tileID, int[] wallID, int[] fluid)
	{                
        for (int i = 0; i < Chunk.chunkSize; i++)
        {
            for (int j = 0; j < Chunk.chunkSize; j++)
            {
                Chunk c = world.chunks[new int[] { chunkPosX, chunkPosY }];
                c.blocks[i, j].valid = true; //only set it here
                c.blocks[i, j].tileID = (ushort)tileID[i * Chunk.chunkSize + j];
                c.blocks[i, j].fluid = (byte)fluid[i * Chunk.chunkSize + j];
                c.blocks[i, j].wallID = (byte)wallID[i * Chunk.chunkSize + j];
                c.blocks[i, j].light = 0;
              
                
            }
        }
    }
    
	

}
