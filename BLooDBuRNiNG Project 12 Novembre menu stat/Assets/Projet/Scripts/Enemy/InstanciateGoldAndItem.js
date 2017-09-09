#pragma strict
//vérifier les valeurs de toute mes variablse casiment fait
//récupéré le type de monstre, gold quantity amount item quantity et rareté et le niveau du monstres
//faire le script pour le rammasage de l'or
static var canDropGoldAndItem : boolean = false;
static var typeOfMonster : String;
static var pos : Vector3;
static var rot : Vector3;
static var level : int;

static var playerGoldFindQuantity : float = 1; //marquer dans le script d'attaque du joueur que si après avoir attaque l'enemi il le tu alors playerGoldFindQuantity de skelton = playerGoldFindQuantity du joueur puis playerGoldFindQuantity de InstnaciateGooldanditem vaut goldquantity du skelette
static var playerGoldFindAmount : float = 1;
static var playerItemQuantity : float = 1; // la meme chose pour les objets
static var playerItemRarity : float = 1;

static var tmpPlayerGoldFindAmount : float = 1;
static var tmpPlayerGoldFindQuantity : float = 1;

static var playerGoldCircleGameObject : GameObject;
static var playerItemCircleGameObject : GameObject;

var lowGold : GameObject;
var mediumGold : GameObject;
var highGold : GameObject;
var goldText : GameObject;

private var numberLowGold : float = 0;		 //100%	=	1/1   Il 				1-50	faudrait les trier par ordre de rareté
private var numberMediumGold : float = 0.2;		 //20%	=	1/5					100-250
private var numberHighGold : float = 0.04;		 //4% 	=	1/25 				500-1000

private var initGoldQuantity : float = playerGoldFindQuantity; //permettent juste a faire la réinitilisation
private var initGoldAmount : float = playerGoldFindAmount;
private var initItemQuantity : float = playerItemQuantity; 
private var initItemRarity : float = playerItemRarity;

var goldFont : Font;
var goldMaterialFont : Material;


//determiner la rareté
function Update () 
{
	tmpPlayerGoldFindAmount = playerGoldFindAmount;
	tmpPlayerGoldFindQuantity = playerGoldFindQuantity;
	
	if (canDropGoldAndItem == true)
	{
		//DropMyItems est juste pour l'or car on utilise la variable *= playerGoldFindQuantity
		//dansInit quantity rarity est-ceq ue le fois 2 est necessaier?
		DropMyItems(numberLowGold, lowGold, pos, rot, "Pièce d'or", 60.00);
		DropMyItems(numberMediumGold, mediumGold, pos, rot, "Amas de pièces", 60.00);
		DropMyItems(numberHighGold, highGold, pos, rot, "Coffre de pièces", 60.00);
		//pareil pour le mf
		ReinitializeMyGoldVariables(); 
		canDropGoldAndItem = false;
	}
	
}

function	DropMyItems(chanceToDrop : float, itemToDrop : GameObject, pos : Vector3, rot : Vector3, itemName : String, timeToDestroyItem : float)
{
	var numberOfItemDrop : float = 0.00;
	var	numberOfGoldAmount : int = 0;
	
	playerGoldFindAmount = tmpPlayerGoldFindAmount;
	playerGoldFindQuantity = tmpPlayerGoldFindQuantity;
	
	InitQuantityRarityWithType();	//fonctionnel
	InitGoldAmountyWithLevel();    //fonctionnel
		
	playerGoldFindAmount *= InitGoldAmountWithGoldName(itemName);
	
	
	numberOfItemDrop = InitQuantityRarityWithRandom(numberOfItemDrop, chanceToDrop);
	numberOfGoldAmount = playerGoldFindAmount - (playerGoldFindAmount % 1);
	DropQuantityAmountGold(itemToDrop, numberOfItemDrop, numberOfGoldAmount, pos, rot, itemName, timeToDestroyItem);
}

function	InitQuantityRarityWithType()
{  
	if (typeOfMonster == "Champion")
	{
		playerGoldFindQuantity *= 2; 
		playerItemQuantity *= 2;
		playerGoldFindAmount *= 1.5;
		playerItemRarity *= 1.5;
	}
	else if (typeOfMonster == "Gozu")
	{
		playerGoldFindQuantity *= 3; 
		playerItemQuantity *= 3;
		playerGoldFindAmount *= 2;
		playerItemRarity *= 2;
	}
	else if (typeOfMonster == "Boss")
	{
		playerGoldFindQuantity *= 5; 
		playerItemQuantity *= 5;
		playerGoldFindAmount *= 4;
		playerItemRarity *= 4;
	}
	else if (typeOfMonster == "World Boss")
	{
		playerGoldFindQuantity *= 12;
		playerItemQuantity *= 12;
		playerGoldFindAmount *= 8;
		playerItemRarity *= 8;	
	}
	
	//print("gold amount : " + playerGoldFindAmount);
}

function	InitGoldAmountyWithLevel()
{
	//une fonction avec un while pourrait faire ce que je fais plus bas avec une temporaire qui recupere le level et a chaque iteration on enleve - 10 level a ce tmp
	
	if (level < 10)
		playerGoldFindAmount = playerGoldFindAmount * (1 + (0.05 * level % 10));
	
	if (level > 10)
		playerGoldFindAmount = playerGoldFindAmount * (1 + (0.02 * (level - 10)));
	if (level > 20)
		playerGoldFindAmount = playerGoldFindAmount * (1 + (0.02 * (level - 20)));
	if (level > 30)
		playerGoldFindAmount = playerGoldFindAmount * (1 + (0.02 * (level - 30)));  
	if (level > 40)
		playerGoldFindAmount = playerGoldFindAmount * (1 + (0.02 * (level - 40)));  
	if (level > 50)
		playerGoldFindAmount = playerGoldFindAmount * (1 + (0.02 * (level - 50)));  
	if (level > 60)
		playerGoldFindAmount = playerGoldFindAmount * (1 + (0.02 * (level - 60)));  
	if (level > 70)
		playerGoldFindAmount = playerGoldFindAmount * (1 + (0.02 * (level - 70)));  
	if (level > 80)
		playerGoldFindAmount = playerGoldFindAmount * (1 + (0.02 * (level - 80)));  
	if (level > 90)
		playerGoldFindAmount = playerGoldFindAmount * (1 + (0.02 * (level - 90)));
	
	//print("level " + level + " : " + playerGoldFindAmount);  
}

function	InitQuantityRarityWithRandom(numberOfItemDrop : float, chanceToDrop : float)
{
	var xNumberOfItemDrop : float = 0.00;
	xNumberOfItemDrop = Random.Range(0.00, chanceToDrop);	//defini le pourcentage de chance qu'un objet tombe //pour le random soit juste je dois le modifier par 2 : exemple random 0 et 1 donnera 0.5 donc 50% de chance de tombber moi il me faut 100% si j'ai 1	
	numberOfItemDrop = xNumberOfItemDrop * playerGoldFindQuantity;	//VERIFIER
		
	var randomBetweenOneToZeroForMoreThanOne : float = Random.Range(0.00, 1.00);
	if 	   (numberOfItemDrop > 1 && randomBetweenOneToZeroForMoreThanOne <= xNumberOfItemDrop % 1) //VERIFIE  		// sa fera juste si il nous a 1.2 donc 0.2 de 1 + 20% de chance qu'on est 2 sinon on reste  a 1
		numberOfItemDrop = numberOfItemDrop + 1;
	else
	{		
		var randomBetweenOneToZeroForLessThanOne : float = Random.Range(0.00, 1.00);
		if (numberOfItemDrop < 1 && randomBetweenOneToZeroForLessThanOne <= numberOfItemDrop) //VERIFIE
			numberOfItemDrop = numberOfItemDrop + 1;
	}
	
	numberOfItemDrop = numberOfItemDrop - (numberOfItemDrop % 1);		//enleve la virgule si j'ai 5.51 j'aurai 6 et si j'aurais 5.49 j'aurais 5 (y) VERIFIE
	return (numberOfItemDrop);
}

function	InitGoldAmountWithGoldName(goldName : String)
{
	if (goldName == "Pièce d'or") return Random.Range(1, 50 + 1);
	if (goldName == "Amas de pièces") return Random.Range(150, 250 + 1);
	if (goldName == "Coffre de pièces") return Random.Range(500, 1000 + 1);

}

function	DropQuantityAmountGold(goldObject : GameObject, goldDropQuantity : int, goldDropAmount : int, pos : Vector3, rot : Vector3, goldName : String, timeToDestroyItem : float)
{
	var numberOfGoldDrop : int = 0; 
	var randomPosX : float = 0;
	var randomPosZ : float = 0; 
	var myItem : GameObject;
	 
	for (numberOfGoldDrop = 0; numberOfGoldDrop < goldDropQuantity; numberOfGoldDrop++)
	{ 
		goldDropAmount = Random.Range(goldDropAmount * 0.8, goldDropAmount * 1.2);
		
		randomPosX = Random.Range(pos.x - 0.7, pos.x + 0.7); // récupérer la position dans ce script
		randomPosZ = Random.Range(pos.z - 0.7, pos.z + 0.7);
		myItem = Instantiate(goldObject, Vector3(randomPosX, pos.y + 1, randomPosZ), Quaternion.identity);
		myItem.transform.rotation.eulerAngles.y = rot.y;
	
		myItem.GetComponent(PickGold).goldAmount = goldDropAmount;
		myItem.GetComponent(PickGold).playerGoldCircleGameObject = playerGoldCircleGameObject;
		myItem.GetComponent(PickGold).timeToDestroy = timeToDestroyItem;
		//timeToDestroyItem += myItem.GetComponent(PickGold).timeToScroll + ;
		//myItem.GetComponent(PickItem).playerItemCircleGameObject = playerItemCircleGameObject;
		//qmeliorer le random de l'or des montants via internet
		
		CreateGoldText(myItem, goldDropAmount, goldName, timeToDestroyItem);
		
		Destroy(myItem, timeToDestroyItem);
	}
}
 
function	CreateGoldText(myItem : GameObject, goldDropAmount : int, goldName : String, timeToDestroyItem : float)
{
		var		textMeshGold : GameObject = GameObject.Find("Gold Text");
		
		
		textMeshGold = Instantiate(textMeshGold, Vector3(myItem.transform.position.x, myItem.transform.position.y + 0.4, myItem.transform.position.z), Quaternion.identity);
		textMeshGold.GetComponent(TextMesh).text = "<color=yellow>" + goldName + "</color>\n" + goldDropAmount;
		//textMeshGold.GetComponent(TextMesh).text = goldDropAmount + " Gold";
		textMeshGold.GetComponent(TextMesh).characterSize = 0.05;
		textMeshGold.GetComponent(TextMesh).fontSize = 20;
		textMeshGold.transform.parent = myItem.transform;
		
		Destroy(textMeshGold, timeToDestroyItem + myItem.GetComponent(PickGold).timeToScroll);
} 
 
function	ReinitializeMyGoldVariables()
{
	playerGoldFindAmount = initGoldAmount;
	playerGoldFindQuantity = initGoldQuantity;
	playerItemRarity = initItemRarity;
	playerItemQuantity = initItemQuantity;
}
