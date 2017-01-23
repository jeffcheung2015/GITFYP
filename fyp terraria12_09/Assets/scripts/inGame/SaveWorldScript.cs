using UnityEngine;
using System.Collections;
using System.IO;
using System;


public static class SaveWorldScript
{
    public static string saveFolderName = "WorldSaves";

	//the location of the file
	public static string SaveLocation(string worldName)
	{
		string saveLocation = saveFolderName + "/" + worldName + "/WorldData.json";
		
		if (!Directory.Exists (saveLocation)) {
			Directory.CreateDirectory (saveLocation);
		} 
		
		return saveLocation;
	}
	//the name of file
	public static string ChunkXYIndex(int[] chunkWorldLocation) //Chunk x, y index
	{
        int x = (int)((chunkWorldLocation[0] + World.worldLeftChunksNo * Chunk.chunkSize * Chunk.tileSize) / (Chunk.chunkSize * Chunk.tileSize));
        int y = (int)((chunkWorldLocation[1] + World.worldBotChunksNo * Chunk.chunkSize * Chunk.tileSize) / (Chunk.chunkSize * Chunk.tileSize));

        string fileName = "(" + x.ToString("D6") + "," + y.ToString("D6") + ")";
		return fileName;
	}
	public static void SaveWorldInfo(string worldName, int X, int Y, int lX, int bY)
    {
        string saveFile = SaveLocation(worldName);
        String worldInfoFile = saveFile + "worldInfo.bin";
        WorldInfoScript wiScript = new WorldInfoScript();
        wiScript.sizeX = X;
        wiScript.sizeY = Y;
        wiScript.lSizeX = lX;
        wiScript.bSizeY = bY;

        Stream stream = new FileStream(worldInfoFile, FileMode.Create, FileAccess.Write, FileShare.None);
        stream.Close();
        

    }




}