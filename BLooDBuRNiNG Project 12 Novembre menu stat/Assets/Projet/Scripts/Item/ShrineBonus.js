#pragma strict
var shrineName : GUIText;
private var boolShrineActivate : boolean = false;
private var allShrineTime : float = 1.0;
private var shrineTime : float = 30.0;
private var xShrineTime : float = 0.00;
private var whichShrine : int = 0;
private var boolCanCount : boolean = false;
private var boolMoreThanShrineTime : boolean = false;
private var myCharacter : GameObject;
private var myCharacterColor : GameObject;
private var myCharacterShader  : Shader;
private var myCharacterInitShader : Shader;
private var colorThatChange : Vector4;


var numberOfShrine : int = 14;
var boolNumberOfShrine : boolean[] = new boolean[numberOfShrine];
var indexCurrentShrine : int = 0;
var shrineBonusTexture : Texture2D;
var boolDisplayShrine : boolean = false;
var countShrineTime : float;

var bloodTexture : Texture2D;
var celerityTexture : Texture2D;
var frenziedTexture : Texture2D;
var fortuneTexture : Texture2D;
var spiritualityTexture : Texture2D;
var yingYangTexture : Texture2D;
var rainbowTexture : Texture2D;
var gladiatorTexture : Texture2D;
var enlightnedTexture : Texture2D;
var blessedTexture : Texture2D;
var restorationTexture : Texture2D;
var skinTexture : Texture2D;
var warningTexture : Texture2D;
var transformationTexture : Texture2D;

var shrineGUIStyle : GUIStyle = new GUIStyle();
shrineGUIStyle.normal.textColor = Color.white;

var shrineSound : AudioClip;


function Start () 
{	
print("screen w " + Screen.width + " screen h " + Screen.height);
	shrineName.text = "Passive Statue";
	ShrineColor();
	shrineName.material.color.a = 0;
	myCharacter = gameObject.Find("Barbarian");
	myCharacterColor = gameObject.Find("BarbarianColor");
	myCharacterShader = myCharacterColor.renderer.material.shader = Shader.Find("Reflective/Bumped Specular");
	//myCharacterInitShader = myCharacterShader;

	for (indexCurrentShrine = 0; indexCurrentShrine < numberOfShrine; indexCurrentShrine += 1)
		boolNumberOfShrine[indexCurrentShrine] = false;
}

function Update () 
{
	if (countShrineTime > 0)
		countShrineTime -= Time.deltaTime;


	if (Vector3.Distance(transform.position, myCharacter.transform.position) < 2)
		boolShrineActivate = true;
	else
		boolShrineActivate = false;
	if (boolShrineActivate == true && boolCanCount == false)
	{
		boolCanCount = true;
		boolMoreThanShrineTime = false;
		ShrineInit();
		ShrineColor();
		ShrineSkills();
	}
	
	if (boolCanCount == true)
	{
		xShrineTime += Time.deltaTime;
		if (shrineName.transform.position.y < 0.72)
		{
			shrineName.transform.position.y += 0.06 * Time.deltaTime;
			shrineName.material.color.a -= 0.25 * Time.deltaTime;
		}

		if (xShrineTime >= shrineTime && boolMoreThanShrineTime == false) //0 > 5
		{
			boolShrineActivate = false;
			boolMoreThanShrineTime = true;
		}
		if (boolMoreThanShrineTime == true)
		{
			ShrineWithoutSkills();
			boolMoreThanShrineTime = true;
			shrineName.text = "Passive Statue";
			ShrineColor();
			ShrineSkills();
		}
		if (xShrineTime >= allShrineTime)// 0 > 10
		{
			xShrineTime = 0.0;
			boolCanCount = false;
			boolShrineActivate = true;		
		}
	}
}

function	ShrineInit()
{
	whichShrine = Random.Range(1, 12 + 1); // modifie en 11 > 13 lorsque j'aurais fini
	if (whichShrine == 0)
		shrineName.text = "Passive Statue";
	else if (whichShrine == 1)
	{
		shrineName.text = "Blood Statue";
		shrineBonusTexture = bloodTexture;
	}
	else if (whichShrine == 2)
	{
		shrineName.text = "Spirituality Statue";
		shrineBonusTexture = spiritualityTexture;	
	}
	else if (whichShrine == 3)
	{
		shrineName.text = "Ying Yang Statue";
		shrineBonusTexture = yingYangTexture;
	}
	else if (whichShrine == 4)
	{
		shrineName.text = "Rainbow Statue";
		shrineBonusTexture = rainbowTexture;
	}
	else if (whichShrine == 5)
	{
		shrineName.text = "Restoration Statue";
		shrineBonusTexture = restorationTexture;
	}
	else if (whichShrine == 6)
	{
		shrineName.text = "Skin Statue";
		shrineBonusTexture = skinTexture;
	}
	else if (whichShrine == 7)
	{
		shrineName.text = "Blessed Statue";
		shrineBonusTexture = blessedTexture;
	}
	else if (whichShrine == 8)
	{
		shrineName.text = "Fortune Statue";
		shrineBonusTexture = fortuneTexture;
	}
	else if (whichShrine == 9)
	{
		shrineName.text = "Gladiator Statue";
		shrineBonusTexture = gladiatorTexture;
	}
	else if (whichShrine == 10)
	{
		shrineName.text = "Enlightened Statue";
		shrineBonusTexture = enlightnedTexture;
	}
	else if (whichShrine == 11)
	{
		shrineName.text = "Frenzied Statue";
		shrineBonusTexture = frenziedTexture;
	}
	else if (whichShrine == 12)
	{
		shrineName.text = "Celerity Statue";
		shrineBonusTexture = celerityTexture;
	}
	//	else if (whichShrine == 13)
	//		shrineName.text = "Warning Statue";
	//	else if (whichShrine == 14)
	//		shrineName.text = "Transformation Statue";
}

function	ShrineColor()
{
	if (shrineName.text == "Passive Statue")
		colorThatChange = Vector4(0.1, 0.1, 0.1, 1);
	else 
	{	
		boolDisplayShrine = true;
		boolShrineBonusButton = false;
		countShrineTime = shrineTime;
		
		if (shrineName.text == "Blood Statue")
			colorThatChange = Vector4(1, 0, 0, 1);
		else if (shrineName.text == "Spirituality Statue")
			colorThatChange = Vector4(0, 0, 1, 1);
		else if (shrineName.text == "Ying Yang Statue")
			colorThatChange = Vector4(0.6, 0.2, 0.8, 1);
		else if (shrineName.text == "Rainbow Statue")
			colorThatChange= Vector4(0, 0, 0, 0.5);
		else if (shrineName.text == "Restoration Statue")
			colorThatChange = Vector4(0, 0, 0.5, 1);
		else if (shrineName.text == "Skin Statue")
			colorThatChange = Vector4(1, 1, 1, 1);
		else if (shrineName.text == "Warning Statue")
			colorThatChange = Vector4(0.7, 0.15, 0.15, 1);
		else if (shrineName.text == "Transformation Statue")
			colorThatChange = Vector4(1, 0.35, 0, 1);
		else if (shrineName.text == "Gladiator Statue")
			colorThatChange = Vector4(1, 0.65, 0, 1);
		else if (shrineName.text == "Enlightened Statue")
			colorThatChange = Vector4(1, 1, 0, 1);
		else if (shrineName.text == "Frenzied Statue")
			colorThatChange = Vector4(0, 1, 0, 1);
		else if (shrineName.text == "Blessed Statue")
			colorThatChange = Vector4(0.55, 0.32, 0.1, 1);
		else if (shrineName.text == "Fortune Statue")
			colorThatChange = Vector4(1, 0.8, 0, 1);
		else if (shrineName.text == "Celerity Statue")
			colorThatChange = Vector4(0, 0.8, 0, 1);
	}
	renderer.material.color = colorThatChange; 
}

function	ShrineWithoutSkills()
{
	PlayerResistances.allResistance -= allResistance;
	PlayerHealthManaXp.boolShrineManaRegeneration = false;
	PlayerDefence.defencePercent -= defencePercent;
	PlayerDamage.shrineAccuracy -= shrineAccuracy;
	PlayerDamage.dmgPercent -= dmgPercent;
	PlayerHealthManaXp.xpPercent -= xpPercent;
	PlayerDamage.attackSpeedPercent -= attackSpeedPercent;
	PlayerResistances.physicalDammageReduction -= physicaDmgReduction;
	PlayerGoldItem.itemFind -= itemFind;
	PlayerGoldItem.goldFindQuantity -= goldFindQuantity;

	allResistance = 0;
	defencePercent = 0.0;
	shrineAccuracy = 0.0;
	dmgPercent = 0.0;
	xpPercent = 0.0;
	attackSpeedPercent = 0.0;
	physicaDmgReduction = 0;
	itemFind = 0.0;
	goldFindQuantity = 0.0;
	fastMovementPercent = 0.0;
}


private var allResistance : int = 0;
private var defencePercent : float  = 0.0;
private var shrineAccuracy : float = 0.0;
private var dmgPercent : float = 0.0;
private var xpPercent : float = 0.0;
private var attackSpeedPercent : float = 0.0;
private var fastMovementPercent : float = 0.0;
private var physicaDmgReduction : int = 0;
private var itemFind : float = 0.0;
private var goldFindQuantity : float = 0.0;

function	ShrineSkills()
{
	myCharacterColor.renderer.material.SetColor("_SpecColor", colorThatChange);
	shrineName.material.color = colorThatChange;
	shrineName.transform.position.y = 0.5;
	if (shrineName.text == "Passive Statue")
		shrineName.material.color = Vector4.zero;
	else
	{
		if (shrineName.text == "Blood Statue")//V
			PlayerHealthManaXp.curHp = PlayerHealthManaXp.maxHp;
		else if (shrineName.text == "Spirituality Statue")//V
			PlayerHealthManaXp.curMp = PlayerHealthManaXp.maxMp;
		else if (shrineName.text == "Ying Yang Statue")//V
		{
			if (PlayerHealthManaXp.curHp <= PlayerHealthManaXp.maxHp * 0.334)
				PlayerHealthManaXp.curHp += PlayerHealthManaXp.maxHp * 0.666;
			else
				PlayerHealthManaXp.curHp =  PlayerHealthManaXp.maxHp;
			if (PlayerHealthManaXp.curMp <= PlayerHealthManaXp.maxMp * 0.334)
				PlayerHealthManaXp.curMp += PlayerHealthManaXp.maxMp * 0.666;
			else
				PlayerHealthManaXp.curMp = PlayerHealthManaXp.maxMp;
		}
		else if (shrineName.text == "Rainbow Statue")//V
			allResistance += 20;
		else if (shrineName.text == "Restoration Statue")//V
			PlayerHealthManaXp.boolShrineManaRegeneration = true;
		else if (shrineName.text == "Skin Statue")//V
			defencePercent += 1;
		else if (shrineName.text == "Gladiator Statue")//V
		{
			shrineAccuracy += 0.66;
			dmgPercent += 0.20;
		}
		else if (shrineName.text == "Enlightened Statue")//V
			xpPercent += 0.33;
		else if (shrineName.text == "Frenzied Statue")//V
			attackSpeedPercent += 0.33;
		else if (shrineName.text == "Blessed Statue")//V
			physicaDmgReduction += 33;
		else if (shrineName.text == "Fortune Statue")//V
		{
			itemFind = PlayerGoldItem.itemFind * 0.33;
			goldFindQuantity = PlayerGoldItem.goldFindQuantity * 0.33;
		}
		else if (shrineName.text == "Celerity Statue")
			fastMovementPercent = PlayerCharacteristic.fastMovement * 0.3;
		PlayerResistances.allResistance += allResistance;
		PlayerDefence.defencePercent += defencePercent;
		PlayerDamage.shrineAccuracy += shrineAccuracy;
		PlayerDamage.dmgPercent += dmgPercent;
		PlayerHealthManaXp.xpPercent += xpPercent;
		PlayerDamage.attackSpeedPercent += attackSpeedPercent;
		PlayerResistances.physicalDammageReduction += physicaDmgReduction;
		PlayerGoldItem.itemFind += itemFind;
		PlayerGoldItem.goldFindQuantity += goldFindQuantity;
		PlayerCharacteristic.fastMovement += fastMovementPercent;
	
		audio.PlayOneShot(shrineSound);
	}
}

private var boolShrineBonusButton : boolean = false;

function		OnGUI()
{
	if (boolDisplayShrine == true)
	{
		//GUI.Button(Rect(1 + (64 * whichShrine - 1), Screen.height * 0.07, 48, 48), shrineBonusTexture);
		//GUI.Label(Rect(1 + (64 * whichShrine - 1), Screen.height * 0.07 + 50, countShrineTime, 48), shrineName.text);
		if ((GUI.Button(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01, 48, 48), shrineBonusTexture)))
			boolShrineBonusButton = !boolShrineBonusButton;
		
		if (boolShrineBonusButton)
		{
			GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 50, 300, 48), shrineName.text + " stay : " + (countShrineTime - countShrineTime % 1) + " seconds", shrineGUIStyle);
			
			if (shrineName.text == "Blood Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "restore 100% of your health", shrineGUIStyle);
			else if (shrineName.text == "Spirituality Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "restore 100% of your mana", shrineGUIStyle);
			else if (shrineName.text == "Ying Yang Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "restore 66.6% of your health and mana", shrineGUIStyle);
			else if (shrineName.text == "Rainbow Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "gain 20% of all your resistance", shrineGUIStyle);
			else if (shrineName.text == "Celerity Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "gain 33% of fast movement", shrineGUIStyle);
			else if (shrineName.text == "Restoration Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.025 + 70, 300, 48), "gain 300% mana regeneration", shrineGUIStyle);
			else if (shrineName.text == "Skin Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "gain 100% defence", shrineGUIStyle);
			else if (shrineName.text == "Gladiator Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "gain 66.6% of accuracy and 20% of DPS", shrineGUIStyle);
			else if (shrineName.text == "Enlightened Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "gain 33% increased experience from monster kills", shrineGUIStyle);
			else if (shrineName.text == "Frenzied Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "gain 33% of attack speed", shrineGUIStyle);
			else if (shrineName.text == "Blessed Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "reduce all incoming damage by 25%", shrineGUIStyle);
			else if (shrineName.text == "Fortune Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "gain 33% increased magic and gold find rate", shrineGUIStyle);
			else if (shrineName.text == "Warning Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "invoke a boss", shrineGUIStyle);
			else if (shrineName.text == "Transformation Statue")
				GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.01 + 70, 300, 48), "Increase the kind of monster of the nearest monster", shrineGUIStyle);
		}
		//GUI.DrawTexture(Rect(0 + (64 * whichShrine - 1), Screen.height * 0.06, 48, 48), shrineBonusTexture);
		//GUI.DrawTexture(Rect(0, 0, 64, 64), celerityTexture);
		//boolDisplayShrine = false;
	}

}
