//Our inventory
var inventory : Array;
//This will be drawn when a slot is empty
public var emptyTex : Texture;
//the size of the inventory in x and y dimension
public var inventorySizeX = 8;
public var inventorySizeY = 5;
//The pixel size (height and width) of an inventory slot
var iconWidthHeight = 20;
//Space between slots (in x and y)
var spacing = 4;
//set the position of the inventory
public var offSet = Vector2(100, 100);
// TEST VARIABLES
// Assign these to test adding Items with mouse clicks (see Update())
public var testTex : Texture;
public var testTex2 : Texture;
static public var hideInventory : boolean = false;
//Our Representation of an InventoryItem
class InventoryItem
{
	//GameObject this item refers to
	var worldObject : GameObject;
	//What the item will look like in the inventory
	var texRepresentation : Texture;
}

// Create the Inventory
function Awake()
{
	inventory = new Array(inventorySizeX);
	for (var i = 0; i < inventory.length; i++)
	{
		inventory[i] = new Array(inventorySizeY);
	}
}

function OnGUI()
{
	var texToUse : Texture;
	var currentInventoryItem : InventoryItem;
	if(hideInventory == false) 
	{
		//Go through each row
		for (var i = 0; i < inventory.length; i++)
		{
			// and each column
			for (var k = 0; k < inventory[i].length; k++)
			{
				texToUse = emptyTex;
				currentInventoryItem = inventory[i][k];
				//if there is an item in the i-th row and the k-th column, draw it
				if(inventory[i][k] != null)
				{
					texToUse = currentInventoryItem.texRepresentation;
				}
				GUI.DrawTexture(new Rect(offSet.x + k * (iconWidthHeight + spacing), offSet.y + i * (iconWidthHeight + spacing), iconWidthHeight, iconWidthHeight), texToUse);
			}
		}
	}
}

function AddItem(item : InventoryItem)
{
	//Go through each row
	for (var i = 0; i < inventory.length; i++)
	{
		// and each column
		for (var k = 0; k < inventory[i].length; k++)
		{
			//If the position is empty, add the new item and exit the function
			if(inventory[i][k] == null)
			{
			inventory[i][k] = item;
			return;
		}
	}
}  

//If we got this far, the inventory is full, do somethign appropriate here 
}

function AddItem(worldObject : GameObject, texRep : Texture)
{
	var newItem = new InventoryItem();
	newItem.worldObject = worldObject;
	newItem.texRepresentation = texRep;
	AddItem(newItem);    
}

function Update()
{	
	if(Input.GetKeyDown(KeyCode.I))
	{
		hideInventory = true;
	}        
	if(Input.GetKeyDown(KeyCode.I))
	{
		hideInventory = false;          
	}
	if(Input.GetKeyDown(KeyCode.K))
	{
		AddItem(gameObject, testTex);
	}
	if(Input.GetKeyDown(KeyCode.L))
	{
		AddItem(gameObject, testTex2);       
	}      
}

function buyShopItem1 () 
{
	AddItem(gameObject, testTex);
}

function buyShopItem2()
{
	AddItem(gameObject, testTex2);       
}