using UnityEngine;
using System.Collections;
using System.Data;
using Mono.Data.SqliteClient;
using System.IO;
using SQLiter;//For importing LoomManager
using System.Text;//For importing stringbuilder
using System.Collections.Generic;

//Remind:
//ExecuteScalar : For Single Value
//ExecuteReader : Row reading in forward mode
//ExecuteNonQuery : For Inserting/Deleting/Updating the rows into table

//Dont use INDEX since it is reserved
//SpaceBar between keyword and variable in SQL query is IMPORTANT!!!!
//StringBuilder works well with huge no of string concatenation

//Inside string query, remember to include the whole index in close brackets ()
//E.g. "WHERE CHUNKINDEX = " + (chunkPosX * WorldSizeX + chunkPosY) != 
//"WHERE CHUNKINDEX = " + chunkPosX * WorldSizeX + chunkPosY

//Custom Database for storing world's chunk's metadata


public class WorldDatabase : MonoBehaviour
{
    private static string _sqlDBLocation = "";
    //Database name
    private string SQL_DB_NAME = "";

    //Database Table name
    private string SQL_TABLE_NAME = "World";

    //affect SQL_TABLE_NAME, initialized in constructor
    public string _worldName = string.Empty;

    //Predefined column key (SQL_TABLE_NAME)
    private const string COL_CHUNKX = "CHUNKX"; //[PRIMARY KEY], chunkposx
    private const string COL_CHUNKY = "CHUNKY"; //[PRIMARY KEY], chunkposy
    private const string COL_INDEXNO = "INDEXNO"; //x * chunksize + y
    private const string COL_TILEID = "BLOCKID";
    private const string COL_FLUID = "FLUID";
    private const string COL_WALLID = "WALLID";

    //DB object(important)
    private IDbConnection _connection = null;
    private IDbCommand _command = null;
    private IDataReader _reader = null;
    //For sql string query, but only used when we have to loop through all the columns
    private StringBuilder sb;

    bool _createNewTable = false;


    public WorldDatabase(string worldName)
    {
        _worldName = worldName;       
        SQL_DB_NAME = "./WorldSaves/" + worldName + "/WorldTileSQL";        
    }

    void Start()
    {
        //Example code
        /*
        WorldDatabase wd = new WorldDatabase("abc");
        wd.SQLiteInit();
        wd.CreateTable();
        wd.InsertChunk(2, 3, new int[256], new int[256]);
        wd.InsertChunk(12, 44, new int[256], new int[256]);
        wd.InsertChunk(22, 3, new int[256], new int[256]);
        wd.InsertChunk(32, 3, new int[256], new int[256]);
        wd.UpdateChunk(2, 3, new int[] { 2, 3, 4 }, new int[] { 22, 33, 44 }, new int[] { 222, 333, 444 });
        List<List<int[]>> info = wd.LoadRangeOfXChunk(0, 23);
        
        for (int i = 0; i < info.Count; i++)
        {
            Debug.Log(i+":"+info[i][0][0]+","+info[i][0][1]);
        }
        List<int[]> info1 = wd.LoadChunk(2, 3);
        for(int i = 0; i < 256; i++)
        {
            Debug.Log(info[0][1][i]+"<"+info1[1][i]);
        }
        //List<int[]> info1 = wd.LoadChunk(2, 3);
        //Debug.Log(info1[0][0]+","+info1[0][1]);
        
        //wd.GetAllChunkRows();
        wd.DropTable("abc");
        wd._connection.Close();
        */
    }

    /// <summary>
    /// Called in other functions
    /// Must be called after this class's worldName variable is set (CONSTUCTOR)
    /// Because the table name need the worldName
    /// </summary>
    public void SQLiteInit()
    {
        sb = new StringBuilder();
        SQL_TABLE_NAME += _worldName;

        //create directiory in the WorldSaves/_worldName/
        System.IO.Directory.CreateDirectory("./WorldSaves/" + _worldName);

        _sqlDBLocation = "URI=file:" + SQL_DB_NAME + ".db";
        //"SQLiter - Opening SQLite Connection"
        _connection = new SqliteConnection(_sqlDBLocation);
        _command = _connection.CreateCommand();
        //once opened then don't close unless application closed
        _connection.Open();

        // WAL = write ahead logging, very huge speed increase
        _command.CommandText = "PRAGMA journal_mode = WAL;";
        _command.ExecuteNonQuery();

        // journal mode
        _command.CommandText = "PRAGMA journal_mode";
        _reader = _command.ExecuteReader();//executereader should be followed by a close
        _reader.Close();

        // more speed increases by turning off the synchronous option
        _command.CommandText = "PRAGMA synchronous = OFF";
        _command.ExecuteNonQuery();

        // and some more
        _command.CommandText = "PRAGMA synchronous";
        _reader = _command.ExecuteReader();
        _reader.Close();
    }
    /// <summary>
    /// Create World table 
    /// </summary>
    public void CreateTable() //Create world table if not exists (will check it's existence)
    {
        // Here we check if the table you want to use exists or not.  If it doesn't exist we create it.
        _command.CommandText = "SELECT name FROM sqlite_master WHERE name='" + SQL_TABLE_NAME + "'";
        _reader = _command.ExecuteReader(); //read the above command
        if (!_reader.Read())//SQLiter - Could not find SQLite table
        {
            _createNewTable = true;
        }
        _reader.Close();

        // create new table if it wasn't found
        if (_createNewTable)
        {
            //SQLiter - Creating new SQLite table
            //Insurance policy, drop table (double check only, normally it wouldn't happen)
            _command.CommandText = "DROP TABLE IF EXISTS '" + SQL_TABLE_NAME + "'";
            _command.ExecuteNonQuery();

            sb.Length = 0;
            sb.Append("CREATE TABLE IF NOT EXISTS '" + SQL_TABLE_NAME + "' (" +
                COL_CHUNKX + " INTEGER, " + COL_CHUNKY + " INTEGER, ");

            for (int i = 0; i < Chunk.chunkSize; i++)
            {
                for (int j = 0; j < Chunk.chunkSize; j++)
                {
                    if (i == Chunk.chunkSize - 1 && j == Chunk.chunkSize - 1)
                    {//skip through the last block
                     //last block
                        sb.Append(COL_INDEXNO + (i * Chunk.chunkSize + j) + " INTEGER, " + COL_TILEID
                            + (i * Chunk.chunkSize + j) + " INTEGER, " + COL_FLUID
                            + (i * Chunk.chunkSize + j) + " INTEGER, " + COL_WALLID
                            + (i * Chunk.chunkSize + j) + " INTEGER, PRIMARY KEY("
                            + COL_CHUNKX + "," + COL_CHUNKY+"))");
                        continue;
                    }
                    sb.Append(COL_INDEXNO + (i * Chunk.chunkSize + j) + " INTEGER, "
                        + COL_TILEID + (i * Chunk.chunkSize + j) + " INTEGER, "
                        + COL_WALLID + (i * Chunk.chunkSize + j) + " INTEGER, "
                        + COL_FLUID + (i * Chunk.chunkSize + j) + " INTEGER, ");
                }
            }
            
            _command.CommandText = sb.ToString();
            _command.ExecuteNonQuery();
        }
    }
    /// <summary>
    /// Insert Chunk in the World table
    /// Should only be called when there are no chunk inside the table!!
    /// It wouldn't check whether there is a same chunk inside the table.
    /// tileid and fluid have the same length
    /// </summary>
    public void InsertChunk(int chunkPosX, int chunkPosY, int[] tileID, int[] wallID, int[] fluid)
    {
        sb.Length = 0;//must reset to 0 since it is global variable
        sb.Append("INSERT OR REPLACE INTO '" + SQL_TABLE_NAME + "' (" + COL_CHUNKX + "," + COL_CHUNKY + ",");

        for (int i = 0; i < Chunk.chunkSize; i++)
        {
            for (int j = 0; j < Chunk.chunkSize; j++)
            {
                if (i == Chunk.chunkSize - 1 && j == Chunk.chunkSize - 1)
                {//skip through the last block
                    sb.Append(COL_INDEXNO + (i * Chunk.chunkSize + j) + ","
                        + COL_TILEID + (i * Chunk.chunkSize + j) + ","
                        + COL_WALLID + (i * Chunk.chunkSize + j) + ","
                        + COL_FLUID + (i * Chunk.chunkSize + j)
                        + ") VALUES ( "
                        + chunkPosX + "," + chunkPosY + ",");
                    continue;
                }
                sb.Append(COL_INDEXNO + (i * Chunk.chunkSize + j) + ", "
                    + COL_TILEID + (i * Chunk.chunkSize + j) + ", "
                    + COL_WALLID + (i * Chunk.chunkSize + j) + ", "
                    + COL_FLUID + (i * Chunk.chunkSize + j) + ", ");
            }
        }
        for (int i = 0; i < Chunk.chunkSize; i++)
        {
            for (int j = 0; j < Chunk.chunkSize; j++)
            {
                if (i == Chunk.chunkSize - 1 && j == Chunk.chunkSize - 1)
                {//skip through the last block
                    sb.Append((i * Chunk.chunkSize + j) + ","
                    + tileID[i * Chunk.chunkSize + j] + ","
                    + wallID[i * Chunk.chunkSize + j] + ","
                    + fluid[i * Chunk.chunkSize + j] + ");");
                    continue;
                }
                sb.Append((i * Chunk.chunkSize + j) + ","
                + tileID[i * Chunk.chunkSize + j] + ","
                + wallID[i * Chunk.chunkSize + j] + ","
                + fluid[i * Chunk.chunkSize + j] + ",");
            }
        }
        _command.CommandText = sb.ToString();
        _command.ExecuteNonQuery();
    }
    /// <summary>
    /// Update chunk's blocks with index in index[] array
    /// index.length == tile.length == wall.length == fluid.length 
    /// </summary>
    public void UpdateChunk(int chunkPosX, int chunkPosY, int[] index, int[] tileID, int[] wallID, int[] fluid)
    {
        //if chunk's row exists in the table, it updates partial or full content of chunk 
        sb.Length = 0;
        sb.Append("UPDATE " + SQL_TABLE_NAME + " SET ");
        for (int i = 0; i < index.Length; i++)
        {
            if (i == index.Length - 1)
            {
                sb.Append(COL_TILEID + index[i] + " = " + tileID[i] + ","
                + COL_WALLID + index[i] + " = " + wallID[i] + ","
                + COL_FLUID + index[i] + " = " + fluid[i]);
                continue;
            }
            sb.Append(COL_TILEID + index[i] + " = " + tileID[i] + ","
                + COL_WALLID + index[i] + " = " + wallID[i] + ","
                + COL_FLUID + index[i] + " = " + fluid[i] + ",");
        }
        sb.Append(" WHERE " + COL_CHUNKX + " = " + chunkPosX + " AND " + COL_CHUNKY + " = " + chunkPosY);
        _command.CommandText = sb.ToString();
        _command.ExecuteNonQuery();

    }
    /// <summary>
    /// Return one whole chunk's block's info
    /// </summary>
    /// <returns>
    /// TYPE = null or byte[]
    /// Return byte array with [0]index, [1]tileid , [2]wallID and [3]fluid</returns>
    public List<int[]> LoadChunk(int chunkPosX, int chunkPosY)
    {       
        _command.CommandText = "SELECT * FROM " + SQL_TABLE_NAME + " WHERE " 
            + COL_CHUNKX + " = " + chunkPosX + " AND " + COL_CHUNKY + " = " + chunkPosY;
        _reader = _command.ExecuteReader(); //reader right now it is in zero row

        if (_reader.Read()) //read first, if row no is zero, then it will return false
        {
            int[] index = new int[2];
            int[] tileID = new int[Chunk.chunkSize * Chunk.chunkSize];
            int[] wallID = new int[Chunk.chunkSize * Chunk.chunkSize];
            int[] fluid = new int[Chunk.chunkSize * Chunk.chunkSize];

            List<int[]> tileInfo = new List<int[]>();

            index[0] = _reader.GetInt16(0);
            index[1] = _reader.GetInt16(1);
            
            //chunkx, chunky, indexOfBlock, tileid, wallid, fluid            
            for (int i = 3, j = 0; j < Chunk.chunkSize * Chunk.chunkSize; i += 4, j++)
            {
                tileID[j] = _reader.GetInt16(i);//tileID
                wallID[j] = _reader.GetInt16(i + 1);//wallID
                fluid[j] = _reader.GetInt16(i + 2);//fluid
            }

            tileInfo.Add(index);
            tileInfo.Add(tileID);
            tileInfo.Add(wallID);
            tileInfo.Add(fluid);

            _reader.Close();
            return tileInfo;
        }
        else
        {
            _reader.Close();
            return null;
        }
    }




    /// <summary>
    /// Load chunks that are within square range.
    /// </summary>
    /// <param name="lChunkPosX"> It is the left threshold range of chunk</param>
    /// <param name="bChunkPosY"> It is the bot threshold range of chunk</param>
    /// <param name="rChunkPosX"> It is the right threshold range of chunk</param> 
    /// <param name="tChunkPosY"> It is the top threshold range of chunk</param>
    /// <returns>
    /// A List of List of [0]index, [1]tileid ,[2]wallID and [3]fluid  
    /// </returns>
    public List<List<int[]>> LoadSquareRangeOfChunk(int lChunkPosX, int bChunkPosY, int rChunkPosX, int tChunkPosY)
    {
        _command.CommandText = "SELECT * FROM " + SQL_TABLE_NAME + " WHERE " + COL_CHUNKX + " >= "
            + lChunkPosX + " AND " + COL_CHUNKX + " <= " + rChunkPosX + " AND "
            + COL_CHUNKY + " >= "
            + bChunkPosY + " AND " + COL_CHUNKY + " <= " + tChunkPosY;
        _reader = _command.ExecuteReader(); //reader right now it is in zero row
        // Must call _reader.Read() first!!
        // Reason: there are no direct ways to know the number of rows return from the query
        if (_reader.Read()) //read first, if row no is zero, then it will return false
        {
            //Return List
            List<List<int[]>> tileInfoList = new List<List<int[]>>();
            //First row read
            int[] index = new int[2];
            index[0] = _reader.GetInt16(0);
            index[1] = _reader.GetInt16(1);

            int[] tileID = new int[Chunk.chunkSize * Chunk.chunkSize];
            int[] wallID = new int[Chunk.chunkSize * Chunk.chunkSize];
            int[] fluid = new int[Chunk.chunkSize * Chunk.chunkSize];            

            List<int[]> tileInfo = new List<int[]>();
            
            for (int i = 3, j = 0; j < Chunk.chunkSize * Chunk.chunkSize; i += 4, j++)
            {
                tileID[j] = _reader.GetInt16(i);//tileid 
                wallID[j] = _reader.GetInt16(i + 1);//wallID 
                fluid[j] = _reader.GetInt16(i + 2);//fluid 
            }
            tileInfo.Add(index);
            tileInfo.Add(tileID);
            tileInfo.Add(wallID);
            tileInfo.Add(fluid);

            tileInfoList.Add(tileInfo);

            //Second and more rows read
            while (_reader.Read())
            {
                int[] index1 = new int[2];
                index1[0] = _reader.GetInt16(0);
                index1[1] = _reader.GetInt16(1);

                int[] tileID1 = new int[Chunk.chunkSize * Chunk.chunkSize];
                int[] wallID1 = new int[Chunk.chunkSize * Chunk.chunkSize];
                int[] fluid1 = new int[Chunk.chunkSize * Chunk.chunkSize];

                List<int[]> tileInfo1 = new List<int[]>();

                for (int i = 3, j = 0; j < Chunk.chunkSize * Chunk.chunkSize; i += 4, j++)
                {
                    tileID1[j] = _reader.GetInt16(i);//tileID
                    wallID1[j] = _reader.GetInt16(i + 1);//wallID 
                    fluid1[j] = _reader.GetInt16(i + 2);//fluid 
                }
                
                tileInfo1.Add(index1);
                tileInfo1.Add(tileID1);
                tileInfo1.Add(wallID1);
                tileInfo1.Add(fluid1);

                tileInfoList.Add(tileInfo1);
            }
            
            _reader.Close();
            return tileInfoList;
        }
        else
        {
            _reader.Close();
            return null;
        }
    }
    

    /// <summary>
    /// Remove specific chunk in table World
    /// </summary>
    /// <param name="chunkPosX"></param>
    /// <param name="chunkPosY"></param>
    public void RemoveSpecificChunk(int chunkPosX, int chunkPosY)
    {
        _command.CommandText = "DELETE from " + SQL_TABLE_NAME
            + " WHERE CHUNKINDEX = " + (chunkPosX * World.worldSizeInChunky + chunkPosY);
        _command.ExecuteNonQuery();
    }


    //Supplementary functions for manually control of database
    public void DropTable(string tableName)
    {
        _command.CommandText = "DROP table World" + tableName;
        _command.ExecuteNonQuery();
    }
    public void RemoveAllChunkRows() //Manually call this if we need to remove all chunks in table
    {
        _command.CommandText = "DELETE FROM " + SQL_TABLE_NAME;
        _command.ExecuteNonQuery();
    }
    public void GetAllChunkRows() //For debug only
    {
        _command.CommandText = "SELECT * FROM " + SQL_TABLE_NAME;
        _reader = _command.ExecuteReader();
        while (_reader.Read())
        {
            int x = _reader.GetInt16(0);
            int y = _reader.GetInt16(1);
            Debug.Log("chunk" + x + "," + y);
            
        }
        _reader.Close();
    }
    public void CloseConnection()
    {
        _connection.Close();
    }
}