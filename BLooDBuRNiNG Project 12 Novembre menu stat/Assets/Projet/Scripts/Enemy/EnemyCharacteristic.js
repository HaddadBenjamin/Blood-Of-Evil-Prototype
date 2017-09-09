import UnityEngine.GUISkin;

var curHp : long = 100;
var maxHp : long = 100;
private var xCurHp : int;
private var yCurHp : long = 100;
private var yMaxHp : long = 100;
var healthPercent : float = 1.00;
private var healthBar : GameObject;
private var boolActiveHealthBar : boolean = true;

var curMp : long = 100;
var maxMp : long = 100;


var minDmg : long = 172;
var maxDmg : long = 242;
private var yMinDmg : long = 45;
private var yMaxDmg : long = 58;
var dmgPercent : float = 1.00;

var attackSpeed : float = 0.46;
var yAttackSpeed : float = 0.66;
var attackSpeedPercent : float = 1.00;

var fastMovement : float = 3.00;
private var yFastMovement : float = 5;
var fastMovementPercent = 1.00;

var itemRarityPercent = 1.00;
var itemQuantityPercent = 1.00;
var yItemRarityPercent : float = 1.00;		//le type de rareté des objets en fonction du type de monstre
var yItemQuantityPercent : float = 1.00;	//la quantity d'objet en fonction du type de monstre
var questItemPercent : float = 1.00;

private var playerCurDmg : long;
private var	xPlayerDammage : long;

static var playerName : String;
static var playerItemFind : float;
static var playerItemRarity : float;
static var playerGoldFindAmount : float = 1;
static var playerGoldFindQuantity : float;
static var playerGoldCircleGameObject : GameObject;
static var playerItemCircleGameObject : GameObject;

var playerAttackSound : AudioClip;

function	Start ()
{
	//enemyIcon = EditorUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath("Assets/Projet/Icon/Monster/skeleton", Texture2D)) as Texture2D;

	playerGoldCircleGameObject = GameObject.Find("Gold Circle Detection");
	playerItemCircleGameObject = GameObject.Find("Item Circle Detection");

	//scrollPhysicalDmg = new GameObject[PlayerDamage.numberOfPhysicalDmgText];
	//scrollPhysicalDmg[0] = gameObject.Find("Dmg Text");
	
	//scrollPhysicalDmg[0] = Instantiate(scrollPhysicalDmg[0], scrollPhysicalDmg[0].transform.position, scrollPhysicalDmg[0].transform.rotation);
	gameObject.GetComponent(EnemyHealthManaXp).level = Random.Range(1, 10 + 1);
	Init();
	InitPercent();
	gameObject.GetComponent(EnemyTypeOfMonster).InitWhichTypeOfMonster();
	if (gameObject.GetComponent(EnemyTypeOfMonster).typeOfMonster != "Normal");
		gameObject.GetComponent(EnemyTypeOfMonster).ChooseTypeOfMonster();

	//level = 10;//Random.Range(1, 100);
	gameObject.GetComponent(EnemyHealthManaXp).InitWithLevel();
	
	this.curHp = yCurHp * healthPercent;
	this.maxHp = yMaxHp * healthPercent;
	this.minDmg = yMinDmg * dmgPercent;
	this.maxDmg = yMaxDmg * dmgPercent;
	this.attackSpeed = yAttackSpeed / attackSpeedPercent;
	this.fastMovement = yFastMovement * fastMovementPercent;
	this.itemRarityPercent = yItemRarityPercent;
	this.itemQuantityPercent = yItemQuantityPercent;
}

function	Init()
{
	//findTextCurDmg = gameObject
		//scrollPhysicalDmg.name = "Player physical Damage";
	//scrollPhysicalDmg[0] = gameObject.Find("Dmg Text");
//	scrollPhysicalDmg[0].guiText.material.color = Color(1, 0, 0, 1);
	alpha = 1;
	//test
	HOWManyHpPercent();
	healthBar = GameObject.Find("EnemyHealthBar");
	healthBar = Instantiate(healthBar, Vector3(transform.position.x, transform.position.y + 2, transform.position.z), transform.rotation);
	healthBar.collider.enabled = false;
	healthBar.name = "EnemyHealthBar";
	healthBar.transform.parent = gameObject.transform;
}

function	InitPercent()
{
	transform.localScale = Vector3(1 * 2, 1 * 2, 1 * 2);
	healthPercent = 1.00;
	dmgPercent = 1.00;
	attackPercent = 1.00;
	xpPercent = 1.00;
	fastMovementPercent = 1.00;	
	itemRarityPercent = 1.00;
	itemQuantityPercent = 1.00;
}

function Update () 
{
	xCurHp = curHp;
	if(this.curHp <= 0)
		Death();
	AttackTheEnemy();
    //healthbar
	if (xCurHp != curHp)
		HealthBarLenght();
	ActiveHealthBar();
}

function HOWManyHpPercent()
{
	hpPercent = curHp / maxHp;
}

function	AttackTheEnemy()
{
	if(Input.GetKey(KeyCode.R) && PlayerDamage.boolAttackSpeed == true && gameObject.GetComponent(FollowMe).dist < 2) 
 	{
		playerItemFind = PlayerGoldItem.itemFind;
		playerItemRarity = PlayerGoldItem.itemRarity;
		playerGoldFindAmount = PlayerGoldItem.goldFindAmount;
		playerGoldFindQuantity = PlayerGoldItem.goldFindQuantity;
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].transform.parent = gameObject.transform;
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].transform.position = gameObject.transform.position;
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].transform.rotation = gameObject.transform.rotation;
			
 		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).renderer.material.color.a = 1;
 		playerCurDmg = Random.Range(PlayerDamage.minDmg, PlayerDamage.maxDmg + 1);
 		playerCurDmg = playerCurDmg * (1 + (0.03 * PlayerStats.strength));
 		xPlayerDammage = playerCurDmg;
 		playerCurDmg = CriticalStrike(playerCurDmg, PlayerDamage.criticalChance);
	 	xPlayerDammage = playerCurDmg / xPlayerDammage;
	 	LeadMyFont();
		
	 	//rajouter les variables goldQuantity amount et itemQuantity/Rarity de mon player
	 	this.curHp -= playerCurDmg;
 		//if (this.curHp <= 0)
 		//	PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].transform.position.y -= PlayerCharacteristic.scrolPhysicalDmgSpeed * Time.deltaTime;
		//PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].transform.position.y = 0.38;
 		//print(PlayerCharacteristic.minDmg * (1 + (0.01 * PlayerCharacteristic.strength)) + "-" + PlayerCharacteristic.maxDmg * (1 + (0.01 * PlayerCharacteristic.strength)) + " CurDmg : " + playerCurDmg);
	    PlayerDamage.xAttackSpeed = 0;
	    PlayerDamage.boolAttackSpeed = false;
	    
	    audio.PlayOneShot(playerAttackSound, 0.09);
 	}
 	
//    PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].guiText.material.color.a -= 0.25 * Time.deltaTime;
	
	for (PlayerDamage.tmpIScrollPhysicalDmg = 0; PlayerDamage.tmpIScrollPhysicalDmg < PlayerDamage.numberOfPhysicalDmgText; PlayerDamage.tmpIScrollPhysicalDmg += 1)
    	if 		(PlayerDamage && PlayerDamage.scrollPhysicalDmg && PlayerDamage.scrollPhysicalDmg[PlayerDamage.tmpIScrollPhysicalDmg] && 
    			PlayerDamage.scrollPhysicalDmg[PlayerDamage.tmpIScrollPhysicalDmg].transform && gameObject &&
    		PlayerDamage.scrollPhysicalDmg[PlayerDamage.tmpIScrollPhysicalDmg].transform.position.y > gameObject.transform.position.y + 5)
    	{
    		PlayerDamage.scrollPhysicalDmg[PlayerDamage.tmpIScrollPhysicalDmg].transform.position.y -= PlayerDamage.scrolPhysicalDmgSpeed * Time.deltaTime;
    		PlayerDamage.scrollPhysicalDmg[PlayerDamage.tmpIScrollPhysicalDmg].GetComponent(TextMesh).renderer.material.color.a = 0;
			PlayerDamage.scrollPhysicalDmg[PlayerDamage.tmpIScrollPhysicalDmg].transform.position.y = 0.38;	 
    	}
    
     PlayerDamage.iScrollPhysicalDmg += 1;
    if (PlayerDamage.iScrollPhysicalDmg == PlayerDamage.numberOfPhysicalDmgText)
    	 PlayerDamage.iScrollPhysicalDmg = 0;
}

function	CriticalStrike(playerDammage : int, criticalChance : float)
{
	var		booCritical : boolean = false;
		
	if (Random.Range(0, 100) <= criticalChance)//3.85 =  1.5-2.35 = 1.925 1.925 * 2 = 3.85
		boolCritical = true;
	if (boolCritical == true)
		playerDammage = playerDammage * (Random.Range(1.50, 2.35));
	boolCritical = false;
	if (Random.Range(0, 100) <= criticalChance)
		boolCritical = true;
	if (boolCritical == true)
		playerDammage = playerDammage * (Random.Range(1.50, 2.35));
	return (playerDammage);	
}


function	LeadMyFont()
{
	if (xPlayerDammage == 1)
	{
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).renderer.material.color = Color(1, 1, 1, 1);
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).fontSize = 24.5;
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).text = playerCurDmg+ "";
	}
	else if (xPlayerDammage >= 2 && xPlayerDammage < 2.5)
	{
		//PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).renderer.material.color = Color(1, 0, 0, 1);
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).fontSize = 30;
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).text = playerCurDmg+ "  \t   <color=red>Brutality</color>";
		//print("Brutality\n");
		//audio.PlayOneShot(brutalitySound);
	}
	else if (xPlayerDammage >= 2.5 && xPlayerDammage < 3.0)
	{
		//PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).renderer.material.color = Color(1, 0, 0, 1);
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).fontSize = 33;
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).text = playerCurDmg+ "  \t   <color=red>Bestiality</color>";
		//print("Bestiality\n");
		//audio.PlayOneShot(bestialitySound);
	}
	else if (xPlayerDammage >= 3 && xPlayerDammage < 3.5)
	{
		//PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).renderer.material.color = Color(1, 0, 0, 1);
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).fontSize = 36;
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).text = playerCurDmg+ "  \t <color=red>Ferrocity</color>";
		//print("Ferrocity\n");
		//audio.PlayOneShot(ferrocitySound);
	}
	else if (xPlayerDammage >= 3.5 && xPlayerDammage < 4.0)
	{
		//PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).renderer.material.color = Color(1, 0, 0, 1);
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).fontSize = 39;
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).text = playerCurDmg + "  \t   <color=red>Atrocity</color>";
		//print("Atrocity\n");
		//audio.PlayOneShot(atrocitySound);
	}
	else if (xPlayerDammage >= 4 && xPlayerDammage < 4.5)
	{
		//PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).renderer.material.color = Color(1, 0, 0, 1);
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).fontSize = 42;
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).text = playerCurDmg+ "  \t   <color=red>Menstruality</color>";
		//print("Menstruality\n");
		//audio.PlayOneShot(menstrualitySound);
	}
	else
	{
		//PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).renderer.material.color = Color(1, 0, 0, 1);
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).fontSize = 45;
		PlayerDamage.scrollPhysicalDmg[PlayerDamage.iScrollPhysicalDmg].GetComponent(TextMesh).text = playerCurDmg+ "  \t   <color=red>Fatility</color>";
		//print("Fatality\n");
		//audio.PlayOneShot(fatalitySound);
	}			
}

function	Death()
{
	var levelDifferenceWithPlayer : int = gameObject.GetComponent(EnemyHealthManaXp).level - PlayerStats.level;
	
	if (levelDifferenceWithPlayer <= 9 && levelDifferenceWithPlayer >= -9)
	{
		var xpGain : int = ((gameObject.GetComponent(EnemyHealthManaXp).xpAmount * gameObject.GetComponent(EnemyHealthManaXp).xpPercent * PlayerHealthManaXp.xpPercent) * 1 + (levelDifferenceWithPlayer * 0.1));
		PlayerHealthManaXp.curXp += xpGain;
		PlayerHealthManaXp.totalXp += xpGain;
	}
	
	InstanciateGoldAndItem.playerItemQuantity = playerItemFind;
	InstanciateGoldAndItem.playerItemRarity = playerItemRarity;
	InstanciateGoldAndItem.playerGoldFindAmount = playerGoldFindAmount;
	InstanciateGoldAndItem.tmpPlayerGoldFindAmount = playerGoldFindAmount;
	InstanciateGoldAndItem.playerGoldFindQuantity = playerGoldFindQuantity;
	InstanciateGoldAndItem.playerGoldCircleGameObject = playerGoldCircleGameObject;
	InstanciateGoldAndItem.playerItemCircleGameObject = playerItemCircleGameObject;
		
	InstanciateGoldAndItem.pos = transform.position;
	InstanciateGoldAndItem.rot = transform.rotation.eulerAngles;
	InstanciateGoldAndItem.typeOfMonster = gameObject.GetComponent(EnemyTypeOfMonster).typeOfMonster;
	InstanciateGoldAndItem.level = gameObject.GetComponent(EnemyHealthManaXp).level;
	InstanciateGoldAndItem.canDropGoldAndItem = true;
	
	Quest1.questItemPercent = gameObject.GetComponent(EnemyHealthManaXp).xpPercent;//questItemPercent;
	Quest1.pos = transform.position;
	Quest1.rot = transform.rotation;
	Quest1.boolActiveInstantiateSkeltonBones = true;
	PlayerScore.killOfSkelton++;
	
	for (PlayerDamage.tmpIScrollPhysicalDmg = 0; PlayerDamage.tmpIScrollPhysicalDmg < PlayerDamage.numberOfPhysicalDmgText; PlayerDamage.tmpIScrollPhysicalDmg += 1)
	{
		if (PlayerDamage.scrollPhysicalDmg[PlayerDamage.tmpIScrollPhysicalDmg].transform.parent == gameObject.transform)
		{
			//PlayerDamage.scrollPhysicalDmg[PlayerDamage.tmpIScrollPhysicalDmg].GetComponent(TextMesh).renderer.material.color.a = 0;
			PlayerDamage.scrollPhysicalDmg[PlayerDamage.tmpIScrollPhysicalDmg].transform.parent = null;
			//PlayerDamage.scrollPhysicalDmg[PlayerDamage.tmpIScrollPhysicalDmg].transform.rotation = gameObject.trasnform.rotation;
		}
			
		//PlayerDamage.scrollPhysicalDmg[PlayerDamage.tmpIScrollPhysicalDmg].guiText.material.color.a = 0;
		//Destroy(PlayerDamage.scrollPhysicalDmg[PlayerDamage.tmpIScrollPhysicalDmg]);
	}
	Destroy(healthBar);
	Destroy(this.gameObject); 
}

function	HealthBarLenght()
{
	healthBar.transform.localScale.x = .58;
	healthBar.transform.localScale.x = healthBar.transform.localScale.x * curHp / maxHp;
	xCurHp = curHp;
}

function	ActiveHealthBar()
{
	if (MultipleCamera.moduloTab % 2 == 0)
		this.healthBar.renderer.enabled = false;
	else if (MultipleCamera.moduloTab % 2 == 1)
		this.healthBar.renderer.enabled = true;
}

function	Power(number : float, puissance : int)
{
	var iPuissance : int = 0;
	for (i = 0; i < puissance; i++)
		number *= number;
	return
	 (number);
}

function	ElemDmgPower(number : float, puissance : int)
{
	var iPuissance : int = 0;
	for (i = 0; i < puissance; i++)
		number *= 1.015;
	return
	 (number);
}
