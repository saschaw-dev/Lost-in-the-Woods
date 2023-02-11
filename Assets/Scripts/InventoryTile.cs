using System;
using System.Collections.Generic;

/// <summary>
/// This class represents the data structure of a tile of the inventory. An inventory consists of multiple
/// tiles or slots (how much depends on if it is a chest inventory or the player inventory), 
/// each can hold an amount of '0' to '10' pieces of a certain item.
/// </summary>
public class InventoryTile
{
    /// <summary>
    /// Constructor for instantiating an empty tile.
    /// </summary>
    /// <param name="tilePos">The certain position of a tile in the inventory. Starts with '0'.</param>
    public InventoryTile(int tilePos)
    {
        this.tilePos = tilePos;
        this.stackSize = 0;
    }

    /// <summary>
    /// Constructor for a tile which contains pieces of items.
    /// </summary>
    /// <param name="ip">The type of item <code>InventoryItem</code> the tile will hold</param>
    /// <param name="tilePos">The certain position of a tile in the inventory. Starts with '0'.</param>
    public InventoryTile(InventoryItem ip, int tilePos)
    {
        this.tilePos = tilePos;
        this.stackSize = 0;
        this.inventoryItem = ip;
        this.addOneItem(ip);
        this.isEmptyTile = false;
    }

    // An inventory tile equals another inventory tile if they have the same item number and item position
    public override bool Equals(Object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return (this.tilePos == ((InventoryTile) obj).getTilePos()) && 
                (this.getInventoryItem().itemNumber == ((InventoryTile)obj).getInventoryItem().itemNumber);
        }
    }

    public override int GetHashCode()
    {
        var hashCode = -2017781355;
        hashCode = hashCode * -1521134295 + EqualityComparer<InventoryItem>.Default.GetHashCode(inventoryItem);
        hashCode = hashCode * -1521134295 + tilePos.GetHashCode();
        return hashCode;
    }

    /// <summary>
    /// The inventory item this tile is holding.
    /// </summary>
    private InventoryItem inventoryItem;

    /// <summary>
    /// The certain position of a tile in the inventory. Starts with '0'.
    /// </summary>
    private int tilePos;

    /// <summary>
    /// The number of pieces of a certain item, which this tile holds.
    /// </summary>
    private int stackSize;

    /// <summary>
    /// The maximum stack size a tile is capable to hold
    /// </summary>
    private const int MAX_STACK_SIZE = 10;

    /// <summary>
    /// Flag indicating whether this tile is empty or not.
    /// True means that this tile is empty. False means that there are pieces of an item in it.
    /// </summary>
    private bool isEmptyTile = true;

    /// <summary>
    /// Getter for <code>this.inventoryItem</code>
    /// </summary>
    /// <returns>The inventory item this tile is holding</returns>
    public InventoryItem getInventoryItem()
    {
        return this.inventoryItem;
    }

    /// <summary>
    /// Setter for <code>this.inventoryItem</code>
    /// </summary>
    /// <param name="inventoryItem">The inventory item this tile will hold</param>
    public void setInventoryItem(InventoryItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;
    }

    /// <summary>
    /// Getter for the <code>this.stackSize</code>
    /// </summary>
    /// <returns>The number of items that the tile is holding</returns>
    public int getNumberOfItems()
    {
        return this.stackSize;
    }

    /// <summary>
    /// Adds an item to this tile.
    /// </summary>
    /// <param name="ip">The type of item to be added</param>
    /// <returns>A boolean which indicates whether the add process was successful or not.
    /// True indicating that adding was successfull and false indicating that it was not.</returns>
    public bool addOneItem(InventoryItem ip)
    {
        if (this.isEmptyTile)
        {
            this.inventoryItem = ip;
            this.isEmptyTile = false;
        }
        if (ip.itemNumber != this.inventoryItem.itemNumber)
        {
            // Wrong item type
            throw new ArgumentException();
        }
        if (this.stackSize >= MAX_STACK_SIZE)
        {
            return false;
        }
        this.stackSize++;
        return true;
    }

    /// <summary>
    /// Reduce item number of the stack by one item.
    /// </summary>
    /// <param name="ip">The type of item to be reduced by one unit</param>
    /// <returns>A boolean which indicates whether the remove process was successful or not.
    /// True indicating that removing was successfull and false indicating that it was not.</returns>
    public bool removeOneItem(InventoryItem ip)
    {
        if (ip.itemNumber != this.inventoryItem.itemNumber)
        {
            // Wrong item type
            throw new ArgumentException();
        }
        if (this.isEmptyTile || this.getNumberOfItems() <= 0)
        {
            return false;
        }
        this.stackSize--;
        if (this.stackSize == 0)
        {
            this.isEmptyTile = true;
            this.inventoryItem = null;
        }
        return true;
    }

    /// <summary>
    /// Empties the item stack of this inventory tile.
    /// </summary>
    public void emptyStack()
    {
        this.stackSize = 0;
        this.isEmptyTile = true;
        this.setInventoryItem(null);
    }

    /// <summary>
    /// Getter for <code>this.isEmptyTile</code>
    /// </summary>
    /// <returns>The isEmptyTile boolean</returns>
    public bool getIsEmptyTile()
    {
        return this.isEmptyTile;
    }

    /// <summary>
    /// Checks if this tile is full or not.
    /// </summary>
    /// <returns>True if it is full, false if it is not</returns>
    public bool isInventorySlotFull()
    {
        return this.stackSize >= MAX_STACK_SIZE;
    }

    /// <summary>
    /// Getter for the tile position in the inventory.
    /// </summary>
    /// <returns>An integer of the position in the inventory</returns>
    public int getTilePos()
    {
        return this.tilePos;
    }

    /// <summary>
    /// Setter for the tile position of the inventory.
    /// </summary>
    /// <param name="tilePos">An integer of the position in the inventory</param>
    public void setTilePos(int tilePos)
    {
        this.tilePos = tilePos;
    }
}