using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour {
    ItemDatabase database;

    public GameObject mainInventory;
    public GameObject partInventory;

    public GameObject firstTenItemsPanel;
    public GameObject[] firstTenItemSlots = new GameObject[10];

    public List<Item> items = new List<Item>();

    public bool isInventoryOpened = false;


    void Start()
    {
        mainInventory.SetActive(false);
        partInventory.SetActive(true);

        database = GetComponent<ItemDatabase>();

        // Add empty items to the 10 item slots
        for (int i = 0; i < 10; i++)
            items.Add(new Item());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isInventoryOpened = !isInventoryOpened;
            mainInventory.SetActive(isInventoryOpened);
            partInventory.SetActive(!isInventoryOpened);
        }
    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.fetchItemByID(id);
        for (int i = 0; i < items.Count; i++)
        {

        }
    }
}
