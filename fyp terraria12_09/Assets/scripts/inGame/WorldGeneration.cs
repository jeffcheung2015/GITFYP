using SimplexNoise;

using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine.Events;

public class WorldGeneration : MonoBehaviour {

	//This script is used to create the world 
	//and store the tile's data in a txt file in project folder


	//world's leftbot most block's worldPos (called only in gen World)
	public static int mostLBBlockOfLBChunkWorldPosx;
	public static int mostLBBlockOfLBChunkWorldPosy;

	//world's Lefttop most block's worldPos  (currently not called)
	public static int mostLTBlockOfLTChunkWorldPosx = -Chunk.chunkSize*Chunk.tileSize * 
		World.worldLeftChunksNo + Chunk.tileSize / 2;
	public static int mostLTBlockOfLTChunkWorldPosy = Chunk.chunkSize*Chunk.tileSize *
		(World.worldSizeInChunky-World.worldBotChunksNo) - Chunk.tileSize / 2;

    //Custom world data database
    WorldDatabase worldDB;

    List<int[]> worldPosOfChunks = new List<int[]>();


	//used later
	public static int cave = 30;
	public static int stoneHeight;
	public static int dirtHeight;
	public static int sandHeight;
	public static int copper = 40;
	public static int stone = 55;
	public static int corrHeight;
	public static int junHeight;
	public static int corrLOffsetX;
	public static int corrROffsetX;
	public static int junLOffsetX;
	public static int junROffsetX;
	public static int sandLOffsetX;
	public static int sandROffsetX;
	public static int mountainHeight;
    public static int wallHeight;

    public GameObject chunkPrefab;
	GameObject world;//it is used as a ref only, if need to access the world script's var then use newWorld
	World newWorld;

    public static int worldNameFactor; // the world's name affecting the world's shape in simplex noise

	void Awake(){
	
		world = gameObject;
		newWorld = world.GetComponent<World> ();//remember not removing the world.cs script from world gameobject
	}

	//generate the chunk using simplex noise
	public static List<int[]> GenChunk(Chunk chunk){
        //return value
        int[] tileID = new int[Chunk.chunkSize * Chunk.chunkSize];
        int[] fluid = new int[Chunk.chunkSize * Chunk.chunkSize];
        int[] wallID = new int[Chunk.chunkSize * Chunk.chunkSize];

        //chunk's leftbot most block's worldPos
        int mostLeftBotBlockWorldPosx = chunk.pos[0] - Chunk.chunkSize * Chunk.tileSize / 2 + Chunk.tileSize / 2;
		int mostLeftBotBlockWorldPosy = chunk.pos[1] - Chunk.chunkSize * Chunk.tileSize / 2 + Chunk.tileSize / 2;

        mostLBBlockOfLBChunkWorldPosx = -Chunk.chunkSize * Chunk.tileSize *
            World.worldLeftChunksNo + Chunk.tileSize / 2;
        mostLBBlockOfLBChunkWorldPosy = -Chunk.chunkSize * Chunk.tileSize *
            World.worldBotChunksNo + Chunk.tileSize / 2;

		for (int j = 0; j <= Chunk.chunkSize-1; j++) {
			for (int i = 0; i <= Chunk.chunkSize-1; i++) {

				if (chunk.blocks [i, j].tileID == BlockIndex.catus) {
					continue;
				}

				int blockWorldPosx = mostLeftBotBlockWorldPosx + i * Chunk.tileSize;
				int blockWorldPosy = mostLeftBotBlockWorldPosy + j * Chunk.tileSize;

				stoneHeight = -200;
				float factor = 0.05f;
				float caveFactor = 0.1f;
				float dirtFactor = 0.06f;
				float stoneFactor = 0.2f;
				float copperFactor = 0.01f;

                wallHeight = Mathf.FloorToInt(ModiNoise(worldNameFactor +
                    (int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * factor),
                    0, 0.0005f, 200, 2)) + Mathf.FloorToInt(ModiNoise(worldNameFactor +
                    (int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * factor),
                    0, 0.005f, 100, 2)) + Mathf.FloorToInt(ModiNoise(worldNameFactor +
                    (int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * factor),
                    0, 0.05f, 50, 2));

                stoneHeight = Mathf.FloorToInt(ModiNoise(worldNameFactor+(int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * factor),
					0, 0.05f, 200, 1));
				dirtHeight = stoneHeight;
				dirtHeight += Mathf.FloorToInt(ModiNoise(worldNameFactor+(int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * factor),
					0, 0.00005f, 850, 6));
				sandHeight = stoneHeight;
				sandHeight += Mathf.FloorToInt(ModiNoise(worldNameFactor+(int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * factor),
					0, 0.00005f, 700, 6));
				corrHeight = stoneHeight;
				corrHeight += Mathf.FloorToInt(ModiNoise(worldNameFactor+(int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * factor),
					0, 0.00005f, 600, 6));

				junHeight = stoneHeight;

				junHeight += Mathf.FloorToInt(ModiNoise(worldNameFactor+(int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * factor),
					0, 0.00005f, 400, 6));

				int caveOccurence = Mathf.FloorToInt(ModiNoise(worldNameFactor+(int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * caveFactor),
					(int)((mostLeftBotBlockWorldPosy - mostLBBlockOfLBChunkWorldPosy + j * Chunk.tileSize) * caveFactor),
					0.01f, 100, 1));

				int stoneOccurence = Mathf.FloorToInt(ModiNoise(worldNameFactor+(int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * stoneFactor),
					(int)((mostLeftBotBlockWorldPosy - mostLBBlockOfLBChunkWorldPosy + j * Chunk.tileSize) * stoneFactor),
					0.01f, 70, 1));

				int copperOccurence = Mathf.FloorToInt(ModiNoise(worldNameFactor+(int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * copperFactor),
					(int)((mostLeftBotBlockWorldPosy - mostLBBlockOfLBChunkWorldPosy + j * Chunk.tileSize) * copperFactor),
					0.05f, 70, 5));

				sandLOffsetX = Mathf.FloorToInt(ModiNoise(0, worldNameFactor+(int)((mostLeftBotBlockWorldPosy - mostLBBlockOfLBChunkWorldPosy + j * Chunk.tileSize) * factor),
					0.01f, 224, 1)) - World.worldLeftChunksNo * Chunk.chunkSize * Chunk.tileSize + (int)(World.worldSizeInChunkx * Chunk.chunkSize *0.7f* Chunk.tileSize / 4);

				sandROffsetX = Mathf.FloorToInt(ModiNoise(0, worldNameFactor+(int)((mostLeftBotBlockWorldPosy - mostLBBlockOfLBChunkWorldPosy + j * Chunk.tileSize) * factor),
					0.01f, 145, 2))  - World.worldLeftChunksNo * Chunk.chunkSize * Chunk.tileSize + (int)(World.worldSizeInChunkx * Chunk.chunkSize *1.5f* Chunk.tileSize / 4);

				corrLOffsetX = sandROffsetX;

				corrROffsetX = corrLOffsetX + Mathf.FloorToInt(ModiNoise(0, worldNameFactor+(int)((mostLeftBotBlockWorldPosy - mostLBBlockOfLBChunkWorldPosy + j * Chunk.tileSize) * factor),
					0.01f, 75, 3)) + (int)(World.worldSizeInChunkx * Chunk.chunkSize *0.8f* Chunk.tileSize / 4);

				junLOffsetX = corrROffsetX + Mathf.FloorToInt(ModiNoise(0, worldNameFactor+(int)((mostLeftBotBlockWorldPosy - mostLBBlockOfLBChunkWorldPosy + j * Chunk.tileSize) * factor),
					0.01f, Chunk.chunkSize*Chunk.tileSize*5, 2));

				junROffsetX = junLOffsetX  + Mathf.FloorToInt(ModiNoise(0, worldNameFactor+(int)((mostLeftBotBlockWorldPosy - mostLBBlockOfLBChunkWorldPosy + j * Chunk.tileSize) * factor),
					0.01f, 54, 2))  + (int)(World.worldSizeInChunkx * Chunk.chunkSize *0.8f* Chunk.tileSize / 4);

				int dirtUnderStoneHeight = Mathf.FloorToInt(ModiNoise(worldNameFactor+(int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * dirtFactor),
					worldNameFactor+(int)((mostLeftBotBlockWorldPosy - mostLBBlockOfLBChunkWorldPosy + j * Chunk.tileSize) * dirtFactor),
					0.01f, 100, 4));

				bool withinSand = (blockWorldPosx < sandROffsetX && blockWorldPosx >= sandLOffsetX) ? true : false;
				bool withinCorr = (blockWorldPosx < corrROffsetX && blockWorldPosx >= corrLOffsetX) ? true : false;
				bool withinJun = (blockWorldPosx < junROffsetX && blockWorldPosx >= junLOffsetX) ? true : false;
				bool withinDeepHole = (blockWorldPosx >= corrROffsetX && blockWorldPosx < junLOffsetX) ? true : false;

				int crayonBase = -Mathf.FloorToInt(ModiNoise(worldNameFactor+(int)((mostLeftBotBlockWorldPosx - mostLBBlockOfLBChunkWorldPosx + i * Chunk.tileSize) * factor),
					worldNameFactor+(int)((mostLeftBotBlockWorldPosy - mostLBBlockOfLBChunkWorldPosy + j * Chunk.tileSize) * factor),
					0.05f, 2275, 1));

				if (blockWorldPosy <= stoneHeight && cave < caveOccurence && (!withinSand) && (!withinCorr) && (!withinJun) &&(!withinDeepHole))
				{
					if (dirtUnderStoneHeight < 70 && copper < copperOccurence)
					{
						chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.dirt, 0, 0, WallIndex.dirtBWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.dirt;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.dirtBWall;

                    }
					else if (dirtUnderStoneHeight >= 70 && copper >= copperOccurence)
					{
						chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.copper, 0, 0, WallIndex.unDWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.copper;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.unDWall;
                    }
					else
					{
						chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.stone, 0, 0, WallIndex.stoneBWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.stone;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.stoneBWall;
                    }
				}
				else if (blockWorldPosy <= dirtHeight && cave < caveOccurence && (!withinSand) && (!withinCorr) && (!withinJun) && (!withinDeepHole))
				{
					if (blockWorldPosy >= 350 && copper < copperOccurence  && stoneOccurence < stone)
					{
						chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.grassDirt, 0, 0, WallIndex.dirtBWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.grassDirt;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.dirtBWall;
                    }
					else if(blockWorldPosy < 350 && copper < copperOccurence && stoneOccurence < stone)
					{
						chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.dirt, 0, 0, WallIndex.dirtBWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.dirt;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.dirtBWall;
                    }
					else if(stoneOccurence >= stone){
						chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.stone, 0, 0, WallIndex.stoneBWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.stone;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.stoneBWall;
                    }
					else
					{
						chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.copper, 0, 0, WallIndex.unDWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.copper;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.unDWall;
                    }
				}
				else if (blockWorldPosy <= sandHeight && cave < caveOccurence && (withinSand) && (!withinCorr) && (!withinJun) && (!withinDeepHole))
				{
					if (blockWorldPosy <= stoneHeight)
					{	if (stoneOccurence >= stone) {
							chunk.SetBlock (blockWorldPosx, blockWorldPosy, BlockIndex.stone, 0, 0, WallIndex.stoneBWall);
                            tileID[i * Chunk.chunkSize + j] = BlockIndex.stone;
                            wallID[i * Chunk.chunkSize + j] = WallIndex.stoneBWall;
                        } else {
						    chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.dirt, 0, 0, WallIndex.dirtBWall);
                            tileID[i * Chunk.chunkSize + j] = BlockIndex.dirt;
                            wallID[i * Chunk.chunkSize + j] = WallIndex.dirtBWall;
                        }
				    }
					else
					{
						chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.sand, 0, 0, WallIndex.noWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.sand;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.noWall;
                        //chunk.AddBlock (i, j);
                    }
				}

				else if (blockWorldPosy <= corrHeight && cave < caveOccurence && (!withinSand) && (withinCorr) && (!withinJun) && (!withinDeepHole))
				{
					if (stone > stoneOccurence && copper < copperOccurence) {
						chunk.SetBlock (blockWorldPosx, blockWorldPosy, BlockIndex.corrDirt, 0, 0, WallIndex.dirtBWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.corrDirt;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.dirtBWall;
                    }
                    else if (stone > stoneOccurence && copper >= copperOccurence) {
						chunk.SetBlock (blockWorldPosx, blockWorldPosy, BlockIndex.copper, 0, 0, WallIndex.unDWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.copper;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.unDWall;
                    }
					else {
						chunk.SetBlock (blockWorldPosx, blockWorldPosy, BlockIndex.stone, 0, 0, WallIndex.stoneBWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.stone;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.stoneBWall;
                    }
				}
				else if (blockWorldPosy <= junHeight && cave < caveOccurence && (!withinSand) && (!withinCorr) && (withinJun) && (!withinDeepHole))
				{
					if (stone > stoneOccurence && copper < copperOccurence) {
						chunk.SetBlock (blockWorldPosx, blockWorldPosy, BlockIndex.junDirt, 0, 0, WallIndex.dirtBWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.junDirt;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.dirtBWall;
                    }
                    else if (stone > stoneOccurence && copper >= copperOccurence) {
						chunk.SetBlock (blockWorldPosx, blockWorldPosy, BlockIndex.copper, 0, 0, WallIndex.unDWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.copper;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.unDWall;
                    }
                    else {
						chunk.SetBlock (blockWorldPosx, blockWorldPosy, BlockIndex.stone, 0, 0, WallIndex.stoneBWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.stone;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.stoneBWall;

                    }
                }
				else if(blockWorldPosx >= corrROffsetX && blockWorldPosx < junLOffsetX){
					if (blockWorldPosy < crayonBase) {
						chunk.SetBlock (blockWorldPosx, blockWorldPosy, BlockIndex.corrDirt, 0, 0, WallIndex.dirtBWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.corrDirt;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.dirtBWall;
                    }
                    else {
                        if (blockWorldPosy > wallHeight)
                        {
                            chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.air, 0, 0, WallIndex.noWall);
                            tileID[i * Chunk.chunkSize + j] = BlockIndex.air;
                            wallID[i * Chunk.chunkSize + j] = WallIndex.noWall;
                        }
                        else
                        {
                            chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.air, 0, 0, WallIndex.noWall);
                            tileID[i * Chunk.chunkSize + j] = BlockIndex.air;
                            wallID[i * Chunk.chunkSize + j] = WallIndex.unDWall;
                        }                
                    }
                }
				else
				{       //currently dont know whether it is necessary	
                    if (blockWorldPosy > wallHeight)
                    {
                        chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.air, 0, 0, WallIndex.noWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.air;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.noWall;
                    }
                    else
                    {
                        chunk.SetBlock(blockWorldPosx, blockWorldPosy, BlockIndex.air, 0, 0, WallIndex.noWall);
                        tileID[i * Chunk.chunkSize + j] = BlockIndex.air;
                        wallID[i * Chunk.chunkSize + j] = WallIndex.unDWall;
                    }
                }

                if (j - 1 != -1)
				{
                    /*
					if (chunk.blocks[i, j - 1].GetType().ToString() == "GrassDirtBlock" &&
						chunk.blocks[i, j].GetType().ToString() == "AirBlock" && UnityEngine.Random.Range(0, 2) == 1)
					{
						int face = UnityEngine.Random.Range(0, 8);
						chunk.SetBlock(blockWorldPosx, blockWorldPosy, new GrassBlock(face));

					}
					else if (chunk.blocks[i, j - 1].GetType().ToString() == "CorrDirtBlock" &&
						chunk.blocks[i, j].GetType().ToString() == "AirBlock" && UnityEngine.Random.Range(0, 2) == 1)
					{
						int face = UnityEngine.Random.Range(0, 8);
						chunk.SetBlock(blockWorldPosx, blockWorldPosy, new CorrGrassBlock(face));

					}
					else if (chunk.blocks[i, j - 1].GetType().ToString() == "JunDirtBlock" &&
						chunk.blocks[i, j].GetType().ToString() == "AirBlock" && UnityEngine.Random.Range(0, 2) == 1)
					{
						int face = UnityEngine.Random.Range(0, 8);
						chunk.SetBlock(blockWorldPosx, blockWorldPosy, new JungleGrassBlock(face));

					}
                    
					else if (chunk.blocks[i, j - 1].GetType().ToString() == "SandBlock" &&
						chunk.blocks[i, j].GetType().ToString() == "AirBlock" && UnityEngine.Random.Range(0, 5) == 1)
					{
						int ranNo = UnityEngine.Random.Range (0, 6);

						int[] zer = { blockWorldPosx, blockWorldPosy };
						int[] one = new int[]{ blockWorldPosx, blockWorldPosy+Chunk.tileSize };
						int[] two = new int[]{ blockWorldPosx, blockWorldPosy+2*Chunk.tileSize };
						int[] thr = new int[]{ blockWorldPosx+Chunk.tileSize, blockWorldPosy };
						int[] fou = new int[]{ blockWorldPosx+Chunk.tileSize, blockWorldPosy+Chunk.tileSize };
						int[] fiv = new int[]{ blockWorldPosx+Chunk.tileSize, blockWorldPosy+2*Chunk.tileSize };


						if (ranNo == 0) {							
							chunk.SetBlock (blockWorldPosx, blockWorldPosy, new CatusBlock (0, 0));
							chunk.SetBlock (blockWorldPosx, blockWorldPosy + Chunk.tileSize, new CatusBlock (1, 0));
							chunk.SetBlock (blockWorldPosx, blockWorldPosy + 2 * Chunk.tileSize, new CatusBlock (2, 0));
							chunk.SetBlock (blockWorldPosx + Chunk.tileSize, blockWorldPosy, new CatusBlock (4,0));
							chunk.SetBlock (blockWorldPosx + Chunk.tileSize, blockWorldPosy + Chunk.tileSize, new CatusBlock (5, 0));
							chunk.SetBlock (blockWorldPosx + Chunk.tileSize, blockWorldPosy + 2 * Chunk.tileSize, new CatusBlock (6, 0));

						} else if (ranNo == 1) {
							chunk.SetBlock (blockWorldPosx, blockWorldPosy, new CatusBlock (1,1));
							chunk.SetBlock (blockWorldPosx, blockWorldPosy+Chunk.tileSize, new CatusBlock (2, 1));
						}else if (ranNo == 2) {
							chunk.SetBlock (blockWorldPosx, blockWorldPosy, new CatusBlock (5, 2));
							chunk.SetBlock (blockWorldPosx, blockWorldPosy+Chunk.tileSize, new CatusBlock (6, 2));
						}else if (ranNo == 3) {
							chunk.SetBlock (blockWorldPosx, blockWorldPosy, new CatusBlock (1, 3));
							chunk.SetBlock (blockWorldPosx, blockWorldPosy+Chunk.tileSize, new CatusBlock (3, 3));
							chunk.SetBlock (blockWorldPosx, blockWorldPosy+2*Chunk.tileSize, new CatusBlock (2, 3));
						}else if (ranNo == 4) {
							chunk.SetBlock (blockWorldPosx, blockWorldPosy, new CatusBlock (2, 4));
						}else if (ranNo == 5) {
							chunk.SetBlock (blockWorldPosx, blockWorldPosy, new CatusBlock (2, 5));
							chunk.SetBlock (blockWorldPosx, blockWorldPosy+Chunk.tileSize, new CatusBlock (1, 5));
							chunk.SetBlock (blockWorldPosx, blockWorldPosy+2*Chunk.tileSize, new CatusBlock (2, 5));
						}

					}
                    */
                }

                //may be later set water block or lava block will use this in if clause
                fluid[i * Chunk.chunkSize + j] = 0;                 

            }
		}
        List<int[]> retList = new List<int[]>();
        retList.Add(tileID);
        retList.Add(wallID);
        retList.Add(fluid);
        
        return retList;
	}
	//call genChunk() [Draw the chunk's blocks]
	public IEnumerator GenWorld(){       

        //DataBase initialization
        worldDB = new WorldDatabase(newWorld.worldName);//initial database with worldName (added as table name)
        worldDB.SQLiteInit();
        worldDB.CreateTable();
        // 
       

		worldNameFactor = newWorld.worldName.GetHashCode ();
		if (worldNameFactor < 0) {
			worldNameFactor = -worldNameFactor;
		}
		if (worldNameFactor > 1000) {
			worldNameFactor = worldNameFactor % 1000;
		}
		UnityEngine.Random.InitState (worldNameFactor);
		worldNameFactor = UnityEngine.Random.Range(0,1000);
        
		for(int i = 0; i < World.worldSizeInChunkx * World.worldSizeInChunky;i++){
            Chunk c = newWorld.chunks[worldPosOfChunks[i]];
            List<int[]> tileInfo = GenChunk(c); //tileInfo[0] = tileid, tileInfo[1] = wallID, tileInfo[2] = fluid
            worldDB.InsertChunk(c.pos[0], c.pos[1], tileInfo[0], tileInfo[1], tileInfo[2]);   
			yield return null;
		}
        //Database close connection
        worldDB.CloseConnection();


		//The following lines must be called only after all the coroutines called
		//get the progress UI Text from the scene
		GameObject progressTextGO = GORefScript.gos6;
		if (progressTextGO != null) {
			progressTextGO.GetComponent<TextScript> ().EndGeneratingWorld ();
		}
		//load the world button
		GameObject sPWGO = GORefScript.gos4;
		if (sPWGO != null) {
			sPWGO.GetComponent<LoadWorldButtonScript> ().LoadButton ();
		}
		//delete all the temp world gameobj
		GameObject[] tempWGO = GameObject.FindGameObjectsWithTag ("TempWorld");
		if (tempWGO != null) {
			foreach (GameObject go in tempWGO) {
				Destroy (go);
			}
		}
	}

	void CreateWorld(){
		int originChunkWorldPosx = (int)(gameObject.transform.position.x + 
		                                 Chunk.tileSize * Chunk.chunkSize/2 -
		                                 Chunk.tileSize * Chunk.chunkSize * World.worldLeftChunksNo);
		int originChunkWorldPosy = (int)(gameObject.transform.position.y + 
		                                 Chunk.tileSize * Chunk.chunkSize/2 -
		                                 Chunk.tileSize * Chunk.chunkSize * World.worldBotChunksNo);
		
		for(int i = 0; i < World.worldSizeInChunkx; i ++){
            for (int j = 0; j < World.worldSizeInChunky; j++) {

                newWorld.CreateChunk(chunkPrefab, originChunkWorldPosx + i * Chunk.chunkSize * Chunk.tileSize,
                                      originChunkWorldPosy + j * Chunk.chunkSize * Chunk.tileSize);
                worldPosOfChunks.Add(new int[] { originChunkWorldPosx + i * Chunk.chunkSize * Chunk.tileSize,
                                                  originChunkWorldPosy + j * Chunk.chunkSize * Chunk.tileSize});
			}
		}
	}




	void Start () {
		CreateWorld ();
		StartCoroutine ("GenWorld");        

    }


    //all parameters should larger than 0
    public static float ModiNoise(float x, float y, float frequency, int maxAmplitude, int loop){
        float result = 0;
        int temAmp = maxAmplitude;
        float temFreq = frequency;

        for(int i = 0; i < loop; i++)
        {
            result += Mathf.FloorToInt((Noise.Generate(x * temFreq, y * temFreq) + 1f) * (temAmp / 2f));
            temFreq *= 2;
            temAmp /= 2; 
        }
		return result;
	}

}
