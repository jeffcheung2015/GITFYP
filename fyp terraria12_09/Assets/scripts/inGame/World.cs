using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class World :MonoBehaviour {
	public Dictionary<int[], Chunk> chunks = new Dictionary<int[], Chunk>(new IntArrayComparer());

	public static int worldLeftChunksNo = 5;
	public static int worldBotChunksNo = 5;

    public static int worldSizeInChunkx = 10;//initial world size
    public static int worldSizeInChunky = 10;

    public static int worldLMostChunkPosX = (int)(-worldLeftChunksNo * Chunk.chunkSize * Chunk.tileSize + Chunk.chunkSize * Chunk.tileSize / 2);
    public static int worldBMostChunkPosY = (int)(-worldBotChunksNo * Chunk.chunkSize * Chunk.tileSize + Chunk.chunkSize * Chunk.tileSize / 2);

    public static int worldLBlockPos = (int)(-World.worldLeftChunksNo * Chunk.chunkSize * Chunk.tileSize + Chunk.tileSize / 2);
	public static int worldBBlockPos = (int)(-World.worldBotChunksNo * Chunk.chunkSize * Chunk.tileSize + Chunk.tileSize / 2);
	public static int worldTBlockPos = (int)((World.worldSizeInChunky - World.worldBotChunksNo) * Chunk.chunkSize * Chunk.tileSize - Chunk.tileSize / 2);
	public static int worldRBlockPos = (int)((World.worldSizeInChunkx - World.worldLeftChunksNo) * Chunk.chunkSize * Chunk.tileSize - Chunk.tileSize / 2);

	public static bool isGenerated = false;

    public string worldName;
    /// <summary>
    /// Create chunk game object from chunkPrefab and set it's worldPos with chunkWorldPosx,y
    /// </summary>
    /// <param name="chunkPrefab"></param>
    /// <param name="chunkWorldPosx"></param>
    /// <param name="chunkWorldPosy"></param>
	public void CreateChunk(GameObject chunkPrefab,int chunkWorldPosX, int chunkWorldPosY){
		GameObject newChunkGO = Instantiate (chunkPrefab,
		                                   new Vector3 ((float)chunkWorldPosX, (float)chunkWorldPosY, 0),
		                                   Quaternion.Euler (Vector3.zero),gameObject.transform) as GameObject;

		//newChunkGO.transform.SetParent (gameObject.transform);

		newChunkGO.GetComponent<Chunk> ().world = gameObject.GetComponent<World>();//assign world script to the chunk's gameobject

		Chunk newChunkScript = newChunkGO.GetComponent<Chunk>();//need to access this new chunk's field in Chunk's script
        newChunkScript.pos = new int[] { chunkWorldPosX, chunkWorldPosY };


		newChunkGO.name = "chunk("+newChunkScript.pos[0]+","+newChunkScript.pos[1]+")";
			
		chunks.Add (newChunkScript.pos, newChunkScript);
	}
	public void DestroyChunk(int chunkWorldPosX, int chunkWorldPosY){
		Chunk chunk = chunks[new int[] { chunkWorldPosX, chunkWorldPosY}];
		Object.Destroy(chunk.gameObject);
		chunks.Remove(new int[] { chunkWorldPosX, chunkWorldPosY });
	}
	public Chunk GetChunk(int chunkWorldPosX, int chunkWorldPosY){
		if (chunks.ContainsKey (new int[] { chunkWorldPosX, chunkWorldPosY })) {
            int[] chunkWorldPos = new int[] { chunkWorldPosX, chunkWorldPosY };

			return chunks [chunkWorldPos];
		} else {
			return null;//return null if it exceeds world boundary(mostly for checking so return null)
		}
	}

    public void RemoveChunk(int chunkWorldPosX, int chunkWorldPosY)
    {
        chunks.Remove(new int[] { chunkWorldPosX, chunkWorldPosY });
    }
    public Block GetBlock(int bWorldPosX, int bWorldPosY)
    {
        //if within world's boundary
        if (bWorldPosX > World.worldLBlockPos && bWorldPosX < World.worldRBlockPos && bWorldPosY > World.worldBBlockPos &&
          bWorldPosY < World.worldTBlockPos)
        {
            int chunkTileSize = Chunk.chunkSize * Chunk.tileSize;
            //blockWorldPosx - (World.worldLMostChunkPosX - chunkSize2 / 2) / (chunkSize2) -> index of chunkx
            int cPosX = Mathf.FloorToInt((bWorldPosX - (World.worldLMostChunkPosX - chunkTileSize / 2)) / (chunkTileSize)) *
                chunkTileSize + World.worldLMostChunkPosX;
            int cPosY = Mathf.FloorToInt((bWorldPosY - (World.worldBMostChunkPosY - chunkTileSize / 2)) / (chunkTileSize)) *
                chunkTileSize + World.worldBMostChunkPosY;


            //doesnt need to check whether the chunk is null.
            return this.GetChunk(cPosX, cPosY).GetBlock(bWorldPosX, bWorldPosY);
        }
        else //out of world's boundary
        {
            return new Block();//"valid" variable in Block's struct is false by default
        }


    }
}





/// <summary>
/// A defined class for int array comparing in Dictionary in World.cs, because use array as a key would cause a identifying problem.
/// </summary>
class IntArrayComparer : IEqualityComparer<int[]>
{
    public bool Equals(int[] a, int[] b)
    {
        return a.SequenceEqual(b);
    }

    public int GetHashCode(int[] a)
    {
        return a.Aggregate(0, (acc, i) => unchecked(acc * 457 + i * 389));
    }
}
