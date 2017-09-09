public class	Item
{
	var icon : Texture2D;
	var equipped : boolean;
	
	var name : String;			//toute les attributs
	var description : String;
	var durability : int;
	var price : int;
	var levelRequiered : int;
	//var type : Type;
	var rarity : Rarity;
	var itemType : ItemType;
	
	function	ApplyStats()	//toutes les méthodes
	{
		if (equipped)
		{
			Debug.Log("Add Stats");
		}
		else
		{
			Debug.Log("Remove Stats");
		}
	}
}

enum	Rarity
{
	common, // don't gives any attributes
	simple,	 // gives 1 random attribute,
	normal,	 // gives 2 random attributes 
	quality,	// gives 3 random attributes
	magic,	 // gives 4 random attributes
	precious,	// gives 5 random attributes
	powerfull,	// gives 6 random attributes
	ancient,	// gives 7 random attributes
	holy,	 // gives 8 random attributes
	epic,	 // gives 9 random attributes
	angel,	 // gives 10 random attributes
	demonic,	// gives 11 random attributes
	archangel,	// gives 12 random attributes
	evil,	 // gives 13 random attributes
	god,	 // gives 14 random attributes 
	ultimateGod,// gives 15 random attributes
	
	setItem,	// several item of the same family gives news attributes
	legendary	// defined item attributes
}

enum	ItemType
{
	helmet,
	mask,
	amulet1,
	amulet2,
	amulet3,
	amulet4,
	armor,
	glove,
	bracer,
	belt,
	orb1,
	orb2,
	ring1,
	ring2,
	ring3,
	ring4,
	pant,
	weapon,
	shinProtection,
	shield,
	shoes
}