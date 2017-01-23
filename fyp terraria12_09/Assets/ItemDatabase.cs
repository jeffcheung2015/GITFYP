using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;

public class ItemDatabase : MonoBehaviour {
    List<Item> database = new List<Item>();

    void Start()
    {
        ConstructItemDatabase();

        //Debug.Log(database[0].Title);
    }

    void ConstructItemDatabase()
    {

    }
	
    public Item fetchItemByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].ID == id)
                return database[i];
        }

        return null;
    }
}

public class Item
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    //public int Damage { get; set; }
    //public float Knockback { get; set; }

    public Item(int id, string title, string type/*, int damage, float knockback*/)
    {
        this.ID = id;
        this.Title = title;
        this.Type = type;
        //this.Damage = damage;
        //this.Knockback = knockback;
    }


    // Empty Item
    public Item()
    {
        this.ID = -1; 
    }

}
