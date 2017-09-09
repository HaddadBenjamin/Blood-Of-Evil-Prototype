#pragma strict

static var playerName : String = "BLooDBuRNiNG";
static var className : String = "BARBARIAN";
static var levelDifficulty : String = "NORMAL";
static var level : int = 1;
static var strength : int = 5; 		// 2 min 3 max 3% dommage												V
static var power : int = 1; 		// 2 min 3 max Elementary damage in random and and 3% elementary damage	X
static var dexterity : int = 3; 	// 15 Accuracy and 3% de accuracy										V
static var endurance : int = 4; 	// 20 Health and 3% de health											V 
static var thickness : int = 3; 	// 3% Defence and 1 reduction physical damage							V
static var spirit : int = 1; 		// 1 to all resistance and 3% to energy									V	
static var faith : int = 1; 		// 15 Energy and 3% healing												V
static var constitution : int = 0;	// 1/10 invocation additional and 3% health and damage of invocation	V
static var chance : int = 2; 		// 0.5% of critical strike and 3 item find								V
static var allStats : int = 0;
var levelUpSound : AudioClip;

var showGUI : boolean = false;
var myGUISkin : GUISkin;


function	Update()
{
	ForetIf2AllStats();
}

function	ForetIf2AllStats()
{
	if (allStats > 0)
	{
		strength += allStats;
		power += allStats;
		dexterity += allStats;
		endurance += allStats;
		thickness += allStats;
		spirit += allStats;
		faith += allStats;
		constitution += allStats;
		chance +=allStats;
		allStats = 0;
	}
}

function	LevelUpSystem ()
{
	level++;
	gameObject.GetComponent(PlayerHealthManaXp).maxHp = 0;
	if (level % 2)
		LevelUpMod2();
	if (level % 3)
		LevelUpMod3();
	LevelUpAutoCalcul4();
	LevelUpSounds();
}  

function	LevelUpMod2()
{
	dexterity++;
	gameObject.GetComponent(PlayerDamage).xAccuracy += 15;
	gameObject.GetComponent(PlayerDamage).accuracyPercent += 0.03;
	
	thickness++;
	gameObject.GetComponent(PlayerDefence).physicalDammageReduction++;
	gameObject.GetComponent(PlayerDefence).defencePercent += 0.03;

	if (level % 4)
		constitution++;
	gameObject.GetComponent(PlayerCharacteristic).minionDmg += 0.03;
	gameObject.GetComponent(PlayerCharacteristic).minionHp += 0.03;
	if (constitution % 10 == 0)
		gameObject.GetComponent(PlayerCharacteristic).numberOfMinions++;
}

function	LevelUpMod3()
{
	endurance += 2;
	gameObject.GetComponent(PlayerHealthManaXp).xMaxHp += 40;
	gameObject.GetComponent(PlayerHealthManaXp).hpPercent += 0.06;
	power++;
	
	spirit++;
	gameObject.GetComponent(PlayerHealthManaXp).mpPercent += 0.03;
	gameObject.GetComponent(PlayerResistances).xColdResistance += 0.35;
	gameObject.GetComponent(PlayerResistances).xFireResistance += 0.35;
	gameObject.GetComponent(PlayerResistances).xEarthResistance += 0.35;
	gameObject.GetComponent(PlayerResistances).xPoisonResistance += 0.35;
	gameObject.GetComponent(PlayerResistances).xWingResistance += 0.35;
	gameObject.GetComponent(PlayerResistances).xFaithResistance += 0.35;
	gameObject.GetComponent(PlayerDefence).defence += 5;
	
	faith++;
	gameObject.GetComponent(PlayerCast).minHl += 2;
	gameObject.GetComponent(PlayerCast).maxHl += 3;
	gameObject.GetComponent(PlayerHealthManaXp).maxMp += 15;	
	
	chance++;
	gameObject.GetComponent(PlayerDamage).criticalChance += 0.5;
	gameObject.GetComponent(PlayerGoldItem).itemFind += 0.03;
}

function	LevelUpAutoCalcul4()
{
	gameObject.GetComponent(PlayerHealthManaXp).curXp -= gameObject.GetComponent(PlayerHealthManaXp).maxXp;
	gameObject.GetComponent(PlayerHealthManaXp).maxXp = (gameObject.GetComponent(PlayerHealthManaXp).maxXp * 1.15) + 19;
	
	gameObject.GetComponent(PlayerHealthManaXp).xMaxHp += 7;
	gameObject.GetComponent(PlayerHealthManaXp).xMaxMp += 4;
	
	strength++;
	gameObject.GetComponent(PlayerDamage).xMinDmg += 2;
	gameObject.GetComponent(PlayerDamage).xMaxDmg += 3;
	
	gameObject.GetComponent(PlayerDamage).attackSpeed = gameObject.GetComponent(PlayerDamage).yAttackSpeed / gameObject.GetComponent(PlayerDamage).attackSpeedPercent;
	gameObject.GetComponent(PlayerCast).fastCast = gameObject.GetComponent(PlayerCast).yFastCast / gameObject.GetComponent(PlayerCast).fastCastPercent;
	gameObject.GetComponent(PlayerDefence).defence = gameObject.GetComponent(PlayerDefence).xDefence * gameObject.GetComponent(PlayerDefence).defencePercent;
	gameObject.GetComponent(PlayerDamage).accuracy = gameObject.GetComponent(PlayerDamage).xAccuracy * gameObject.GetComponent(PlayerDamage).accuracyPercent * gameObject.GetComponent(PlayerDamage).shrineAccuracy;
	gameObject.GetComponent(PlayerHealthManaXp).maxHp = gameObject.GetComponent(PlayerHealthManaXp).xMaxHp * gameObject.GetComponent(PlayerHealthManaXp).hpPercent;
	gameObject.GetComponent(PlayerHealthManaXp).maxMp = gameObject.GetComponent(PlayerHealthManaXp).xMaxMp * gameObject.GetComponent(PlayerHealthManaXp).mpPercent;
	gameObject.GetComponent(PlayerDamage).minDmg = gameObject.GetComponent(PlayerDamage).xMinDmg * gameObject.GetComponent(PlayerDamage).dmgPercent;
	gameObject.GetComponent(PlayerDamage).maxDmg = gameObject.GetComponent(PlayerDamage).xMaxDmg * gameObject.GetComponent(PlayerDamage).dmgPercent;
	
	gameObject.GetComponent(PlayerHealthManaXp).curMp = gameObject.GetComponent(PlayerHealthManaXp).maxMp;
	gameObject.GetComponent(PlayerHealthManaXp).curHp = gameObject.GetComponent(PlayerHealthManaXp).maxHp;
}

function	LevelUpSounds()
{
//	switch (level)
//	{
//		case level >= 1 && level < 10: audio.PlayOneShot(Cool man); break;
//		case level >= 10 && level < 20: audio.PlayOneShot(Good job bro); break;
//		case level >= 20 && level < 30: audio.PlayOneShot(Oh Yeah!); break;
//		case level >= 30 && level < 40: audio.PlayOneShot(Wooooot!); break;
//		case level >= 40 && level < 50: audio.PlayOneShot(Woooow!); break;
//		case level >= 50 && level < 60: audio.PlayOneShot(Incredible!); break;
//		case level >= 60 && level < 70: audio.PlayOneShot(What the fuck?); break;
//		case level >= 70 && level < 80: audio.PlayOneShot(You are a monster!!); break;
//		case level >= 80 && level < 90: audio.PlayOneShot(Fuck them all!!); break;
//		case level >= 90 && level < 100: audio.PlayOneShot(Oh my god!!!!); break;
//		case level >= 100: audio.PlayOneShot(You did it, congratulation!!!!!!!); break;
//	}
	audio.PlayOneShot(levelUpSound); 
}



private var  contour_pos_x : float = Screen.width 	* 0.02;
private var  contour_pos_y : float = Screen.height 	* 0.03;
private var  contour_width : float = Screen.width 	* 0.25;
private var  contour_height : float = Screen.height	* 0.80;

private var  name_pos_x : float = contour_pos_x + Screen.width 	* 0.045;
private var  name_pos_y : float = contour_pos_y + Screen.height * 0.027;
private var  name_width : float = Screen.width 					* 0.080;
private var  name_height : float = Screen.height				* 0.035;

private var  class_pos_x : float = name_pos_x + name_width + Screen.width * 0.045 - Screen.width * 0.005;

private var  total_xp_pos_x : float = contour_pos_x + Screen.width 	* 0.045;
private var  total_xp_pos_y : float = name_pos_y + name_height + Screen.height * 0.005;

private var  difficulty_pos_x : float = name_pos_x + name_width + Screen.width 	* 0.045 - Screen.width * 0.005;

private var  level_height : float = name_height + Screen.height * 0.013;
private var  level_width : float = Screen.width * 0.07;
private var  level_pos_y : float = total_xp_pos_y + Screen.height * 0.07;
private var  cur_xp_pos_x : float = name_pos_x + Screen.width * 0.0825;
private var  max_xp_pos_x : float = cur_xp_pos_x + Screen.width * 0.0825;

private var  life_pos_y : float = level_pos_y + level_height + Screen.height * 0.005;

private var  stats_pos_x : float = name_pos_x + name_width * 0.2;
private var  stats_pos_y : float = life_pos_y + name_height + Screen.height * 0.013;

private var  characteristic_pos_x : float = stats_pos_x + Screen.width * 0.08;

private var  strength_width			=		Screen.width * 0.03;
private var  strength_pos_y : float =		stats_pos_y + name_height + Screen.height * 0.005;
private var  strength_pos_x : float =		name_pos_x + Screen.width * 0.02;
private var  power_pos_y : float =			strength_pos_y + name_height + Screen.height * 0.005;
private var  dexterity_pos_y : float =	 	power_pos_y + name_height + Screen.height * 0.005;
private var  endurance_pos_y : float =	 	dexterity_pos_y + name_height + Screen.height * 0.005;
private var  constitution_pos_y : float =	endurance_pos_y + name_height + Screen.height * 0.005;
private var  spirit_pos_y : float =	 		constitution_pos_y + name_height + Screen.height * 0.005;
private var  thickness_pos_y : float =	 	spirit_pos_y + name_height + Screen.height * 0.005;
private var  faith_pos_y : float =		 	thickness_pos_y + name_height + Screen.height * 0.005;
private var  chance_pos_y : float =	 		faith_pos_y + name_height + Screen.height * 0.005;

function		OnGUI()
{
	GUI.skin =  myGUISkin;
	//My_Own_Stat();
	myStats(); 
	
} 

function 		DoMyWindowStat(windowID : int)
{ 
	contour_pos_x = Screen.width 	* 0.02;
	contour_width = Screen.width 	* 0.25;
	contour_pos_y = Screen.height 	* 0.03;
	contour_height = Screen.height	* 0.80;
	
	AddSpikes(windowStat.width);
	FancyTop(windowStat.width);
	//WaxSeal(windowStat.width, windowStat.height);
	//DeathBadge(windowStat.width * 0.1, windowStat.height * 0.85);
	//MyDeathBadge(windowStat.width, windowStat.height); 
	GUI.skin.customStyles[28].fontSize = ResizeFunctions.ResizeFont() + 9; // Variables
	GUI.skin.customStyles[31].fontSize = ResizeFunctions.ResizeFont() + 9; // Values
	GUI.skin.label.fontSize = ResizeFunctions.ResizeFont() + 6; 
	Stats_Left_Side();
	Stats_Right_Side();
	Stats_Value_Left_Side();
	Stats_Value_Right_Side();
	GUI.DragWindow (Rect (0,0,10000,10000));
}

function		Stats_Value_Right_Side()
{
 	GUILayout.BeginArea(Rect(contour_pos_x + contour_width * 0.60, contour_pos_y + 69, contour_width - 100, contour_height), "");
 	GUILayout.BeginVertical();
 	GUILayout.Space(10);
 	
	GUILayout.Label ("", "Labelempty");
 	GUILayout.Box ("<color=#FF6600FF>" + className + "</color>", "ValueStats");
 	GUILayout.Box ("<color=#00FF00FF>" + levelDifficulty + "</color>", "ValueStats");
 	GUILayout.Box ("", "Dividerempty");
 	GUILayout.Box ("<color=yellow>" + (gameObject.GetComponent(PlayerHealthManaXp).maxXp - gameObject.GetComponent(PlayerHealthManaXp).curXp) + "</color>", "ValueStats");
 	GUILayout.Box ("<color=#0000FFFF>" + gameObject.GetComponent(PlayerHealthManaXp).maxMp + "</color>", "ValueStats");
 	GUILayout.Label ("", "Dividerempty");
 	GUILayout.Box (" ", "ValueStats");
 	GUILayout.Box (" ", "ValueStats");
 	//GUILayout.Box ("<color=red>" + "DPS : " + "</color>", "ValueStats");
 	//GUILayout.Box ("<color=black>" gameObject.GetComponent(PlayerResistances).defence + "</color>, "ValueStats");
 	GUILayout.Box ("<color=yellow>" + (1 / gameObject.GetComponent(PlayerDamage).attackSpeed) + "</color>", "ValueStats");
 	GUILayout.Box ("<color=blue>" + (1 / gameObject.GetComponent(PlayerCast).fastCast) + "</color>", "ValueStats");
 	GUILayout.Box ("<color=green>" + gameObject.GetComponent(PlayerCharacteristic).fastMovement + "</color>", "ValueStats");
 	GUILayout.Box ("<color=purple>" + (gameObject.GetComponent(PlayerGoldItem).itemFind * 100) + "%</color>", "ValueStats");
	GUILayout.Box ("<color=purple>" + (gameObject.GetComponent(PlayerGoldItem).itemRarity * 100) + "%</color>", "ValueStats");
	GUILayout.Box ("<color=yellow>" + (gameObject.GetComponent(PlayerGoldItem).goldFindQuantity * 100) + "%</color>", "ValueStats"); 	
	GUILayout.Box ("<color=yellow>" + (gameObject.GetComponent(PlayerGoldItem).goldFindAmount * 100) + "%</color>", "ValueStats");
	GUILayout.Label ("", "Labelempty");
	GUILayout.Box ("<color=yellow>" + gameObject.GetComponent(PlayerResistances).lightingResistance + "</color>", "ValueStats"); 
	GUILayout.Box ("<color=#964514FF>" + gameObject.GetComponent(PlayerResistances).earthResistance + "</color>", "ValueStats");
	GUILayout.Box ("<color=grey>" + gameObject.GetComponent(PlayerResistances).wingResistance + "</color>", "ValueStats");
	GUILayout.Box ("<color=#FF6600FF>" + gameObject.GetComponent(PlayerResistances).faithResistance + "</color>", "ValueStats");
	GUILayout.Button ("<color=#FF0000FF><b>DETAILS</b></color>", "details");
	// if (GUILayout.Button("Details"))
	//boolmenudetails = true;
 	GUILayout.EndVertical();
 	GUILayout.EndArea();	
}
function		Stats_Left_Side()
{//remplacer 69 et 100 en fonction des gui screen puis modifier les tailels de toute mes polices en fonction de la taille de mon ecran
	GUILayout.BeginArea(Rect(contour_pos_x + 10, contour_pos_y + 69, contour_width - 100, contour_height), ""); 	
	GUILayout.BeginVertical();
	GUILayout.Space(10);
 	
 	GUILayout.Label("<b>" + playerName + " 's  STATS</b>");
 	GUILayout.Label ("Level : ", "VariableStats");
  	GUILayout.Label ("Total XP : ", "VariableStats");
 	GUILayout.Label("", "Divider"); 
 	
 	GUILayout.Label ("Current XP : ", "VariableStats");
 	GUILayout.Label ("Life : ", "VariableStats");
 	GUILayout.Label("", "Divider");
 	
 	GUILayout.Label ("Strength : ", "VariableStats");
 	GUILayout.Label ("Power : ", "VariableStats");
 	GUILayout.Label ("Dexterity : ", "VariableStats");
 	GUILayout.Label ("Endurance : ", "VariableStats");
 	GUILayout.Label ("Constitution : ", "VariableStats");
 	GUILayout.Label ("Spirit : ", "VariableStats");
 	GUILayout.Label ("Thickness : ", "VariableStats");
 	GUILayout.Label ("Faith : ", "VariableStats");
 	GUILayout.Label ("Chance : ", "VariableStats");
 	//GUILayout.Label("", "Divider");
 	
 	GUILayout.Label("<b>RESITANCES</b>");
 	GUILayout.Label ("Physical : ", "VariableStats"); 
 	GUILayout.Label ("<color=#FF0000FF>Fire</color> : ", "VariableStats");
 	GUILayout.Label ("<color=#0000FFFF>Cold</color> : ", "VariableStats");
 	GUILayout.Label ("<color=#00FF00FF>Poison</color> : ", "VariableStats");
 	
 	GUILayout.EndVertical();
 	GUILayout.EndArea();
}

function		Stats_Value_Left_Side()
{//remplacer 69 et 100 en fonction des gui screen puis modifier les tailels de toute mes polices en fonction de la taille de mon ecran
	GUILayout.BeginArea(Rect(contour_pos_x + contour_width * 0.23, contour_pos_y + 69, contour_width - 100, contour_height), ""); 	
	GUILayout.BeginVertical();
	GUILayout.Space(10);
 	
 	GUILayout.Label ("", "Labelempty");
 	GUILayout.Label ("<color=yellow>" + level + "</color>", "ValueStats");
  	GUILayout.Label ("<color=yellow>" + gameObject.GetComponent(PlayerHealthManaXp).totalXp + "</color>", "ValueStats");
 	GUILayout.Label("", "Divider");
 	
 	GUILayout.Label ("<color=yellow>" + gameObject.GetComponent(PlayerHealthManaXp).curXp + "</color>", "ValueStats");
 	GUILayout.Label ("<color=#FF0000FF>" + gameObject.GetComponent(PlayerHealthManaXp).maxHp + "</color>", "ValueStats");
 	GUILayout.Label("", "Divider");
 	
 	GUILayout.Label ("<color=red>" + strength + "</color>", "ValueStats");
 	GUILayout.Label ("<color=#0000FFFF>" + power + "</color>", "ValueStats");
 	GUILayout.Label ("<color=yellow>" + dexterity + "</color>", "ValueStats");
 	GUILayout.Label ("<color=red>" + endurance + "</color>", "ValueStats");
 	GUILayout.Label ("<color=green>" + constitution + "</color>", "ValueStats");
 	GUILayout.Label ("<color=#0000FFFF>" + spirit + "</color>", "ValueStats");
 	GUILayout.Label ("<color=green>" + thickness + "</color>", "ValueStats");//autre couleur
 	GUILayout.Label ("<color=grey>" + faith + "</color>", "ValueStats");
 	GUILayout.Label ("<color=yellow>" + chance + "</color>", "ValueStats");
 	//GUILayout.Label("", "Divider");
 	
 	GUILayout.Label ("", "Labelempty");
 	GUILayout.Label ("" + gameObject.GetComponent(PlayerResistances).physicalResistance, "ValueStats"); 
 	GUILayout.Label ("<color=#FF0000FF>" + gameObject.GetComponent(PlayerResistances).fireResistance + "</color>", "ValueStats");
 	GUILayout.Label ("<color=#0000FFFF>" + gameObject.GetComponent(PlayerResistances).coldResistance + "</color>", "ValueStats");
 	GUILayout.Label ("<color=#00FF00FF>" + gameObject.GetComponent(PlayerResistances).poisonResistance + "</color>", "ValueStats");
 	
 	GUILayout.EndVertical();
 	GUILayout.EndArea();
}

function		Stats_Right_Side()
{
 	GUILayout.BeginArea(Rect(contour_pos_x + contour_width * 0.325, contour_pos_y + 69, contour_width - 100, contour_height), "");
 	GUILayout.BeginVertical();
 	GUILayout.Space(10);
 	
 	GUILayout.Label ("", "Labelempty");
 	GUILayout.Label ("Class : ", "VariableStats");
 	GUILayout.Label ("Difficulty : ", "VariableStats");
 	GUILayout.Label ("", "Dividerempty");
 	GUILayout.Label ("XP Requiered : ", "VariableStats");
 	GUILayout.Label ("Mana : ", "VariableStats");
 	GUILayout.Label ("", "Dividerempty");
 	GUILayout.Label ("DPS : ", "VariableStats");
 	GUILayout.Label ("Defence : ", "VariableStats");
 	GUILayout.Label ("Attack / Second : ", "VariableStats");
 	GUILayout.Label ("Cast / Second : ", "VariableStats");
 	GUILayout.Label ("Fast Movement : ", "VariableStats");
 	GUILayout.Label ("Item Quantity : ", "VariableStats");
	GUILayout.Label ("Item Rarity : ", "VariableStats");
	GUILayout.Label ("Gold Quantity : ", "VariableStats"); 	
	GUILayout.Label ("Gold Amount : ", "VariableStats");
	GUILayout.Label ("", "Labelempty");
	GUILayout.Label ("<color=yellow>Lighting</color> : ", "VariableStats"); 
	GUILayout.Label ("<color=#964514FF>Earth</color> : ", "VariableStats");
	GUILayout.Label ("<color=grey>Wing</color> : ", "VariableStats");
	GUILayout.Label ("<color=#FF6600FF>Faith</color> : ", "VariableStats");
	
 	GUILayout.EndVertical();
 	GUILayout.EndArea();	
}

//Necroamln,cer skin 

/*
Necromancer GUI Demo Script
Author: Jason Wentzel
jc_wentzel@ironboundstudios.com

In this script you'll find some handy little functions for some of the 
Custom elements in the skin, these should help you create your own;

AddSpikes (not perfect but works well enough if youre careful with your window widths)
FancyTop (just an example of using the elements to do a centered header graphic)
WaxSeal (adds the waxseal and ribbon to the right of the window)
DeathBadge (adds the iconFrame, skull, and ribbon elements properly aligned)

*/

private var leafOffset;
private var frameOffset;
private var skullOffset;

private var RibbonOffsetX;
private var FrameOffsetX;
private var SkullOffsetX;
private var RibbonOffsetY;
private var FrameOffsetY;
private var SkullOffsetY;

private var WSwaxOffsetX;
private var WSwaxOffsetY;
private var WSribbonOffsetX;
private var WSribbonOffsetY;
	
private var spikeCount;

// This script will only work with the Necromancer skin
var mySkin : GUISkin;

//if you're using the spikes you'll need to find sizes that work well with them these are a few...
private var windowStat =Rect (contour_pos_x, contour_pos_y, contour_width, contour_height); 
//private var 10;

private var scrollPosition : Vector2;
private var HroizSliderValue = 0.5;
private var VertSliderValue = 0.5;
private var ToggleBTN = false;

//skin info
private var NecroText ="This started as a question... How flexible is the built in GUI in unity? The answer... pretty damn flexible! At first I wasnt so sure; it seemed no one ever used it to make a non OS style GUI at least not a publicly available one. So I decided I couldnt be sure until I tried to develop a full GUI, Long story short Necromancer was the result and is now available to the general public, free for comercial and non-comercial use. I only ask that if you add something Share it.   Credits to Kevin King for the fonts.";


function AddSpikes(winX : int)
{ 
	var spiky : int =  Mathf.Floor(winX - 152)/22;
	//spikeCount = Mathf.Floor(winX - 152)/22;
	GUILayout.BeginHorizontal();
	GUILayout.Label ("", "SpikeLeft");//-------------------------------- custom
	for (var i = 0; i < spiky; i++)
        {
			GUILayout.Label ("", "SpikeMid");//-------------------------------- custom
        }
	GUILayout.Label ("", "SpikeRight");//-------------------------------- custom
	GUILayout.EndHorizontal();
}

function FancyTop(topX : int)
{
	leafOffset = (topX/2)-64;
	frameOffset = (topX/2)-27;
	skullOffset = (topX/2)-20;
	GUI.Label(new Rect(leafOffset, 18, 0, 0), "", "GoldLeaf");//-------------------------------- custom	
	GUI.Label(new Rect(frameOffset, 3, 0, 0), "", "IconFrame");//-------------------------------- custom	
	GUI.Label(new Rect(skullOffset, 12, 0, 0), "", "Skull");//-------------------------------- custom	
}

function WaxSeal(x : int, y : int)
{
	WSwaxOffsetX = x - 120;
	WSwaxOffsetY = y - 115;
	WSribbonOffsetX = x - 114;
	WSribbonOffsetY = y - 83;
	
	GUI.Label(new Rect(WSribbonOffsetX, WSribbonOffsetY, 0, 0), "", "RibbonBlue");//-------------------------------- custom	
	GUI.Label(new Rect(WSwaxOffsetX, WSwaxOffsetY, 0, 0), "", "WaxSeal");//-------------------------------- custom	
}

function MyDeathBadge(x : int, y : int)
{  
	x += 2;
	y += 12;
	
	RibbonOffsetX = x - 115;
	FrameOffsetX = x - 115;
	SkullOffsetX = x - 108;
	RibbonOffsetY = y- 95;
	FrameOffsetY = y - 120;
	SkullOffsetY = y - 110;
	
	GUI.Label(new Rect(RibbonOffsetX, RibbonOffsetY, 0, 0), "", "RibbonRed");//-------------------------------- custom	
	GUI.Label(new Rect(FrameOffsetX, FrameOffsetY, 0, 0), "", "IconFrame");//-------------------------------- custom	
	GUI.Label(new Rect(SkullOffsetX, SkullOffsetY, 0, 0), "", "Skull");//-------------------------------- custom	
}

function DeathBadge(x : int, y : int)
{
	RibbonOffsetX = x;
	FrameOffsetX = x+3;
	SkullOffsetX = x+10;
	RibbonOffsetY = y+22;
	FrameOffsetY = y;
	SkullOffsetY = y+9;
	
	GUI.Label(new Rect(RibbonOffsetX, RibbonOffsetY, 0, 0), "", "RibbonRed");//-------------------------------- custom	
	GUI.Label(new Rect(FrameOffsetX, FrameOffsetY, 0, 0), "", "IconFrame");//-------------------------------- custom	
	GUI.Label(new Rect(SkullOffsetX, SkullOffsetY, 0, 0), "", "Skull");//-------------------------------- custom	
}


function	myStats()
{
	//if (showGUI)
		windowStat =Rect (contour_pos_x, contour_pos_y, contour_width, contour_height);
		windowStat = GUI.Window(0, windowStat, DoMyWindowStat, "");
	//	10 = ResizeFunctions.ResizeHeight() - 5;
		GUI.BeginGroup (Rect (0,0,100,100));
	
		//windowStat = GUI.Window(0, windowStat, DoMyWindowStat, ""); 
		// End the group we started above. This is very important to remember!
		GUI.EndGroup (); 
		//	windowStat.x = windowStat.width *  0.5;
		 //windowStat.y = windowStat.height *  0.5;
		// windowStat = GUI.Window(0, windowStat, DoMyWindowStat, ""); 
}
