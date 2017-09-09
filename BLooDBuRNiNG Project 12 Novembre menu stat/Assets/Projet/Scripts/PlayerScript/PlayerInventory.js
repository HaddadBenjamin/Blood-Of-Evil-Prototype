//import System.Collections.Generic;
//
//var	items : Item[]; // representation de tout mes objets
import System.Collections.Generic;

var	items : Item[]; // representation de tout mes objets
var	equipment : Item[]; // mon équipement equipment[0] = helm, 1 mask 2 amulet1 3 amulet2 etc...

var	mainInventory : List.<Item> = new List.<Item>(); // créer un inventaire on défini le nombre d'emplacement dans le menu inspector ou en fesant mainIventory.Count = nb
var	bag1 : List.<Item> = new List.<Item>();
var	bag2: List.<Item> = new List.<Item>();
var	bag3 : List.<Item> = new List.<Item>();
var	bag4 : List.<Item> = new List.<Item>();
var	bag5 : List.<Item> = new List.<Item>();
var	bag6 : List.<Item> = new List.<Item>();

static var boolShowInventory : boolean = false;
function Start () 
{
	mainInventory.Add(items[0]);	//méthode de system.collections.generic permet d'ajouter a la suite de mon array un objet. qui est l'objet[0] de inspector
	//mainInventory.Add(items[0]);
	mainInventory.Add(items[1]);
	mainInventory.Add(items[1]);
	//mainInventory.Add(items[2]);
	//equipment[0] = null;
}

function	OnGUI() 
{
	if (boolShowInventory)
	{
		for (var x = 0; x < mainInventory.Count; x++)
		{
			if (GUI.Button(Rect(Screen.width * 0.5, Screen.height * 0.5 + (x * 20), 100, 20), GUIContent(
			mainInventory[x].name,
			"Type: " + mainInventory[x].itemType + "\n" +
			//"Description: " + mainInventory[x].description + "\n" +
			"Durability: " + mainInventory[x].durability + "\n" +
			"Rarity: " + mainInventory[x].rarity + "\n" +
			"Level requiered: " + mainInventory[x].levelRequiered)))
			{
				switch (mainInventory[x].itemType)
				{
					case mainInventory[x].itemType.helmet: stuffMyPlayer(0, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.mask: stuffMyPlayer(1, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.amulet1: stuffMyPlayer(2, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.amulet2: stuffMyPlayer(3, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.amulet3: stuffMyPlayer(4, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.amulet4: stuffMyPlayer(5, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.armor: stuffMyPlayer(6, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.glove: stuffMyPlayer(7, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.bracer: stuffMyPlayer(8, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.belt: stuffMyPlayer(9, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.orb1: stuffMyPlayer(10, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.orb2: stuffMyPlayer(11, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.ring1: stuffMyPlayer(12, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.ring2: stuffMyPlayer(13, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.ring3: stuffMyPlayer(14, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.ring4: stuffMyPlayer(15, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.pant: stuffMyPlayer(16, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.weapon: stuffMyPlayer(17, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.shinProtection: stuffMyPlayer(18, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.shield: stuffMyPlayer(19, x, mainInventory, equipment); break;
					case mainInventory[x].itemType.shoes: stuffMyPlayer(20, x, mainInventory, equipment); break;
				}	
			}
			//GUI.Box(Rect(Screen.width / 2 + 150, Screen.height / 2 + (20 * x), 200, 300), "");
			GUI.Label(Rect(Screen.width / 2 + 150, Screen.height / 2 + (20 * x), 300, 300), GUI.tooltip);
			var p : int = 0;	
			switch (mainInventory[x].itemType)
			{
				case mainInventory[x].itemType.helmet: p = 0; break;
				case mainInventory[x].itemType.mask: p = 1; break;
				case mainInventory[x].itemType.amulet1: p = 2; break;
				case mainInventory[x].itemType.amulet2: p = 3; break;
				case mainInventory[x].itemType.amulet3: p = 4; break;
				case mainInventory[x].itemType.amulet4: p = 5; break;
				case mainInventory[x].itemType.armor: p = 6; break;
				case mainInventory[x].itemType.glove: p = 7; break;
				case mainInventory[x].itemType.bracer: p = 8; break;
				case mainInventory[x].itemType.belt: p = 9; break;			//le rajouter ailleur  il n'y était pas
				case mainInventory[x].itemType.orb1: p = 10; break;
				case mainInventory[x].itemType.orb2: p = 11; break;
				case mainInventory[x].itemType.ring1: p = 12; break;
				case mainInventory[x].itemType.ring2: p = 13; break;
				case mainInventory[x].itemType.ring3: p = 14; break;
				case mainInventory[x].itemType.ring4: p = 15; break;
				case mainInventory[x].itemType.pant: p = 16; break;
				case mainInventory[x].itemType.weapon: p = 17; break;
				case mainInventory[x].itemType.shinProtection: p = 18; break;
				case mainInventory[x].itemType.shield: p = 19; break;
				case mainInventory[x].itemType.shoes: p = 20; break;
			}
			var tooltip2 = 	
			//mainInventory[p].name +
			"Type: " + mainInventory[p].itemType + "\n" +
			//"Description: " + mainInventory[p].description + "\n" +
			"Durability: " + mainInventory[p].durability + "\n" +
			"Rarity: " + mainInventory[p].rarity + "\n" +
			"Level requiered: " + mainInventory[p].levelRequiered;
			if (GUI.tooltip != "")
			{
				GUI.Box(Rect(Screen.width / 2 + 350, Screen.height / 2 + (20 * x), 200, 300), "");
				GUI.Label(new Rect(Screen.width / 2 + 350, Screen.height / 2 + (20 * x), 300, 300), tooltip2);
			}
		}
		for (var y = 0; y < equipment.length; y++)
		{
			if (equipment[y] != null)
			{
				if (GUI.Button(Rect(Screen.width / 2 - 150, Screen.height / 2 + (20 * y), 100, 20), "" + equipment[y].itemType))
				{
					if (equipment[y] != null)
					{
						mainInventory.Add(equipment[y]);
						equipment[y] = null;
						//power -= equipment[0].power;
						//debug.log(power);
					}
				}
			}
		}
	}
}

function	stuffMyPlayer(whichItem : int, x : int, mainInventory : List.<Item>, equipment : Item[])
{
	if (equipment[whichItem] != null)
		mainInventory.Add (equipment[whichItem]);
	equipment[whichItem] = mainInventory[x];
	mainInventory.RemoveAt (x);
	//power += equipment[0].power;
	//debug.log(power);
}
//	helm	0
// 	mask	1
//	amulet1	2
//	amulet2	3
//	amulet3	4
//	amulet4	5
//	armor	6
//	glove	7
//	bracer	8
//	belt	9
//	orb1	10
//	orb2	11
//	ring1	12
//	ring2	13
//	ring3	14
//	ring4	15
//	pant	16
//	weapon	17
//	shinProtection	18
//	shield			19
//	shoes			20