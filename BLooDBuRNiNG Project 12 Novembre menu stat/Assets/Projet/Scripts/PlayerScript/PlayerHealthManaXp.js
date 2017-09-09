#pragma strict
import System;

var glob : Texture2D;
var healthGlob : Texture2D;
var manaGlob : Texture2D;
var skillBar : Texture2D;
var xpBarEmpty : Texture2D;
var xpBarFull : Texture2D;

private var healthText : String;
private var manaText : String;

static var curHp : long = 50000;
static var xMaxHp : long = 5000;
static var maxHp : long = xMaxHp;
static var hpPercent : float = 1.00;
var hpPercentText : float;
static var boolHpTextInPercent : boolean = false;


//var healthText : GUIText;
var deathSound : AudioClip;
private var deathPosition : Vector3;
deathPosition = Vector3(1207, 12, 250);

//mana system
static var curMp : long = 100;
static var xMaxMp : long = 100;
static var maxMp : long = xMaxMp;
static var mpPercent : float = 1.00;
var mpSound : AudioClip;
var mpPercentText : float;
static var boolShrineManaRegeneration : boolean = false;
static var boolMpTextInPercent : boolean = false; 

//xp system
static var curXp : long = 0;
static var maxXp : long = 5000;
static var totalXp : long = 0;
static var xpPercent : float = 1.00; 
static var xpText : String; 
static var boolXpTextInPercent : boolean = false; 
var xpPercentText : float;
var xpGUIStyle : GUIStyle = new GUIStyle();

var xpPText : GUIText;
var levelUpSound : AudioClip;

var fireBallIcon : Texture2D;
var cureIcon : Texture2D;

function Start()
{
	maxHp = xMaxHp * hpPercent;
	maxMp = xMaxMp * mpPercent;
	
	curMp = maxMp;
	curHp = maxHp;	

	HpRegen();
	MpRegen();
}

function	Update()
{
	if (curXp >= maxXp)
		gameObject.GetComponent(PlayerStats).LevelUpSystem();
		
	if (curHp <= 0 ) 
		Death();
		
	if (curHp > maxHp) 
 		curHp = maxHp;
 		
	if (curMp < 0 ) 
		curMp = 0;
		
	if (curMp > maxMp) 
 		curMp = maxMp;
 		
 	maxHp = xMaxHp * hpPercent;
	maxMp = xMaxMp * mpPercent;
	
	var xcurHp : float = curHp;
	var xmaxHp : float = maxHp;
	var xcurMp : float = curMp;
	var xmaxMp : float = maxMp;
	var xcurXp : float = curXp;
	var xmaxXp : float = maxXp;
	
	hpPercentText = Math.Round(100 * (xcurHp / xmaxHp) - (10000 * (xcurHp / xmaxHp)) % 1 * 0.01, 2);
	mpPercentText = Math.Round(100 * (xcurMp / xmaxMp) - (10000 * (xcurMp / xmaxMp)) % 1 * 0.01, 2);
	xpPercentText = Math.Round(100 * (xcurXp / xmaxXp) - (10000 * (xcurXp / xmaxXp)) % 1 * 0.01, 2);
}

function	HpRegen () 
{
	for(var i : int = 1; i > 0; i++) 
	{
		yield WaitForSeconds(0.8);
 		if (curHp < maxHp) 
 			curHp += (maxHp * 0.002) + (gameObject.GetComponent(PlayerResistances).xFireResistance * maxHp * 0.002) + 1;
	}
} 

function	MpRegen () 
{
	for(var i : int = 1 ; i > 0; i++) 
	{
		if (boolShrineManaRegeneration == true)
			yield WaitForSeconds(0.3);
		else if (boolShrineManaRegeneration == false)
			yield WaitForSeconds(0.8);
 		if (curMp < maxMp)
 			curMp += ((maxMp * 0.002) + (gameObject.GetComponent(PlayerResistances).xFaithResistance * maxMp * 0.0002)) + 1;
 	}
} 

function	Death()
{
	curHp = maxHp;	
	curMp = maxMp;
	transform.position = deathPosition;
	audio.PlayOneShot(deathSound);
}

function OnGUI()
{
	//manaGlob.Resize(manaGlob.width * 1.5, manaGlob.height * 1.5, manaGlob.format, true);
	//var texImporter : TextureImporter = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(manaGlob));
	//texImporter.isReadable = true;
	//manaGlob = AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texImporter));
	//print(AssetDatabase.GetAssetPath(manaGlob));
	//Debug.Log("Texture readable? " + texImporter);
	//AssetDatabase.ImportAsset(texImporter.assetPath);
	//manaGlob.Resize(256, 256);


	//GUI.DrawTexture(new Rect(Screen.width * 0.0625, Screen.height * 0.728699, glob.width * 0.443, glob.height * 0.443), glob);//Globe gauche
	//GUI.DrawTexture(new Rect(Screen.width * 0.822916, Screen.height * 0.728699, glob.width * 0.443, glob.height * 0.443), glob);//Globe droite 
//boolHpTextInPercent = true;
	if (boolXpTextInPercent)
			xpText = "" + xpPercentText + "%";
	else
		xpText = "" + curXp + " / " + maxXp;
		
	if (boolHpTextInPercent)
		healthText = "" + hpPercentText + "%";
	else
		healthText = curHp + " / " + maxHp;
		
	if (boolMpTextInPercent)
		manaText = "" + mpPercentText + "%";
	else
		manaText = curMp + " / " + maxMp;
	
		
	Graphics.DrawTexture(new Rect(Screen.width * 0.1, 
		Screen.height * 0.83273 - (healthGlob.height  * curHp / maxHp) + healthGlob.height , 
		//Screen.height * 0.703273 - (healthGlob.height  * GetComponent(PlayerCharacteristic).curHp / GetComponent(PlayerCharacteristic).maxHp) + healthGlob.height , 
		healthGlob.width, 
		healthGlob.height  * curHp / maxHp),
		healthGlob, 
		0, 0 , 0, healthGlob.height * curHp / maxHp);
		
	Graphics.DrawTexture(new Rect(Screen.width * 0.83541, 
		Screen.height * 0.83273 - (manaGlob.height  * curMp / maxMp) + manaGlob.height, 
		manaGlob.width, 
		manaGlob.height * curMp / maxMp),
		manaGlob, 0, 0 , 0, manaGlob.height * curMp / maxMp);
		
	GUI.DrawTexture(new Rect(Screen.width * 0.232,
		Screen.height * 0.97 - skillBar.height,
		skillBar.width,
		skillBar.height),
		skillBar);
	GUI.DrawTexture(new Rect(Screen.width * 0.232,
		Screen.height * 0.97 - xpBarEmpty.height,
		xpBarEmpty.width,
		xpBarEmpty.height),
		xpBarEmpty); 
	Graphics.DrawTexture(new Rect(Screen.width * 0.232 + skillBar.width * 0.067,
		Screen.height * 0.97 - xpBarFull.height,
		xpBarFull.width * curXp / maxXp * 0.867,
		xpBarFull.height),
		xpBarFull, 0, xpBarFull.width * curXp / maxXp * 0.867, 0, 0); 
	
/*	GUI.Box(new Rect(Screen.width * 0.232 + skillBar.width * 0.067,
		Screen.height * 1.034 - xpBarFull.height,
		xpBarFull.width * 0.867,
		xpBarFull.height * 0.12), "yo");
	*/  
	GUI.Label(new Rect(Screen.width * 0.1 - healthGlob.width * 0.5,
		Screen.height * 0.83273,
		healthGlob.width * 2,
		healthGlob.height), healthText,
		xpGUIStyle);
	   //print("width " + Screen.width + " height " Screen.height);
	
	GUI.Label(new Rect(Screen.width * 0.83541 - manaGlob.width * 0.5,
		Screen.height * 0.83273,
		manaGlob.width * 2,
		manaGlob.height), manaText,
		xpGUIStyle);
	
	GUI.Label(new Rect(Screen.width * 0.232 + skillBar.width * 0.067,
		Screen.height * 0.945 - (xpBarFull.height * 1.015),
		xpBarFull.width * 0.867,
		xpBarFull.height),
		"Level : " + gameObject.GetComponent(PlayerStats).level,
		xpGUIStyle);		
		   
	GUI.Label(new Rect(Screen.width * 0.232 + skillBar.width * 0.067,
		Screen.height * 0.97 - (xpBarFull.height * 1.015),
		xpBarFull.width * 0.867,
		xpBarFull.height),
		xpText,
		xpGUIStyle);		
		//xpPText.text = "Level " + gameObject.GetComponent(PlayerStats).level + " XP " + curXp + " / " + maxXp;
		
	GUI.Button(new Rect(Screen.width * 0.27, Screen.height * 0.9065, Screen.width * 0.022, Screen.height * 0.056), fireBallIcon);
	GUI.Button(new Rect(Screen.width * (0.268  + 0.023), Screen.height * 0.9065, Screen.width * 0.022, Screen.height * 0.056), cureIcon);
}