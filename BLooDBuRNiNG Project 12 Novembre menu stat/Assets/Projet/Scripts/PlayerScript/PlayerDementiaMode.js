#pragma strict
static var boolCanGodMode : boolean = true;
static var boolGodMode : boolean = false;
static var boolRemoveStats : boolean = true;
static var xRemainGodTime : float = 0.0;
static var remainGodTime : float = 60.0;
static var godTime : float = 10.0;
static var dementiaAllStats : int = 0;
static var dementiaFastCastPercent : float = 0.0;
static var dementiaAttackSpeedPercent : float = 0.0;
static var dementiaFastMovementPercent : float = 0.0;
static var dementiaItemFind : float = 0.0;
static var dementiaXpPercent : float = 0.0;


//GUI
var godTexture : Texture2D;
private var boolDisplayGodText : boolean = false;
private var timeStayGodTime : float = 0;

var godGUIStyle : GUIStyle = new GUIStyle();
godGUIStyle.normal.textColor = Color.white;

var dementiaSound : AudioClip;

function Start () {

}

function Update () 
{
	DementiaModeAlgo();
	
	if (godTime > remainGodTime)
		godTime = remainGodTime;
}


function	DementiaModeAlgo()
{
	if (boolCanGodMode == false)
	{
		if (boolCanGodMode == false)
			xRemainGodTime += Time.deltaTime;
		if (xRemainGodTime >= godTime)
		{
			boolGodMode = false;
			transform.localScale = Vector3.one;
			if (boolRemoveStats == true)
			{
				gameObject.Find("WindZone").GetComponent(SmoothFollow).enabled = false;
				gameObject.Find("WindZone").transform.position = Vector3(0,0,0);
		//		GameObject.Find("WindZone").active = false;
				gameObject.GetComponent(PlayerStats).allStats -= dementiaAllStats;
				gameObject.GetComponent(PlayerCast).fastCastPercent -= dementiaFastCastPercent;
				gameObject.GetComponent(PlayerDamage).attackSpeedPercent -= dementiaAttackSpeedPercent;
				gameObject.GetComponent(PlayerCharacteristic).fastMovementPercent -= dementiaFastMovementPercent;
				gameObject.GetComponent(PlayerGoldItem).itemFind -= dementiaItemFind;
				gameObject.GetComponent(PlayerHealthManaXp).xpPercent -= dementiaXpPercent;
				boolRemoveStats = false;
				
				dementiaAllStats 				= 0;
				dementiaFastCastPercent			= 0;
				dementiaAttackSpeedPercent		= 0;
				dementiaFastMovementPercent		= 0;
				dementiaItemFind				= 0;
				dementiaXpPercent				= 0;
			}
		}
		if (xRemainGodTime >= remainGodTime)
		{
			boolCanGodMode = true;
			xRemainGodTime = 0.0;
		}
	}
}

function	DementiaModeBonus()
{
	var level : int = GetComponent(PlayerStats).level;
	
	boolCanGodMode = false;
	boolGodMode = true;
	boolRemoveStats = true;
	transform.localScale = Vector3(1.5, 1.5, 1.5);
	
	if (level < 25 && level > 0)	{
		dementiaAllStats += 5;
		dementiaFastCastPercent += 0.5;
		dementiaAttackSpeedPercent += 0.5;
		dementiaFastMovementPercent += 0.5;
		dementiaItemFind += 0.2;
		dementiaXpPercent += 0.25;	}
	else if (level < 30 && level >= 25)	{
		dementiaAllStats += 7;
		dementiaFastCastPercent += 0.55;
		dementiaAttackSpeedPercent += 0.55;
		dementiaFastMovementPercent += 0.55;
		dementiaItemFind += 0.24;
		dementiaXpPercent += 0.27;	}
	else if (level < 35 && level >= 30)	{
		dementiaAllStats += 9;
		dementiaFastCastPercent += 0.60;
		dementiaAttackSpeedPercent += 0.60;
		dementiaFastMovementPercent += 0.60;
		dementiaItemFind += 0.28;
		dementiaXpPercent += 0.29;	}
	else if (level < 40 && level >= 35)	{
		dementiaAllStats += 11;
		dementiaFastCastPercent += 0.65;
		dementiaAttackSpeedPercent += 0.65;
		dementiaFastMovementPercent += 0.65;
		dementiaItemFind += 0.32;
		dementiaXpPercent += 0.31;	}
	else if (level < 45 && level >= 40)	{
		dementiaAllStats += 13;
		dementiaFastCastPercent += 0.70;
		dementiaAttackSpeedPercent += 0.70;
		dementiaFastMovementPercent += 0.70;
		dementiaItemFind += 0.36;
		dementiaXpPercent += 0.33;	}
	else if (level < 50 && level >= 45)	{
		dementiaAllStats += 15;
		dementiaFastCastPercent += 0.75;
		dementiaAttackSpeedPercent += 0.75;
		dementiaFastMovementPercent += 0.75;
		dementiaItemFind += 0.40;
		dementiaXpPercent += 0.35;	}
	else if (level < 55 && level >= 50)	{
		dementiaAllStats += 18;
		dementiaFastCastPercent += 0.82;
		dementiaAttackSpeedPercent += 0.82;
		dementiaFastMovementPercent += 0.82;
		dementiaItemFind += 0.45;
		dementiaXpPercent += 0.39;	}
	else if (level < 60 && level >= 55)	{
		dementiaAllStats += 21;
		dementiaFastCastPercent += 0.89;
		dementiaAttackSpeedPercent += 0.89;
		dementiaFastMovementPercent += 0.89;
		dementiaItemFind += 0.50;
		dementiaXpPercent += 0.43;	}
	else if (level < 65 && level >= 60)	{
		dementiaAllStats += 24;
		dementiaFastCastPercent += 0.96;
		dementiaAttackSpeedPercent += 0.96;
		dementiaFastMovementPercent += 0.96;
		dementiaItemFind += 0.55;
		dementiaXpPercent += 0.47;	}
	else if (level < 70 && level >= 65)	{
		dementiaAllStats += 27;
		dementiaFastCastPercent += 1.03;
		dementiaAttackSpeedPercent += 1.03;
		dementiaFastMovementPercent += 1.03;
		dementiaItemFind += 0.60;
		dementiaXpPercent += 0.51;	}
	else if (level < 75 && level >= 70)	{
		dementiaAllStats += 30;
		dementiaFastCastPercent += 1.10;
		dementiaAttackSpeedPercent += 1.10;
		dementiaFastMovementPercent += 1.10;
		dementiaItemFind += 0.65;
		dementiaXpPercent += 0.55;	}
	else if (level < 80 && level >= 75)	{
		dementiaAllStats += 33;
		dementiaFastCastPercent += 1.13;
		dementiaAttackSpeedPercent += 1.13;
		dementiaFastMovementPercent += 1.13;
		dementiaItemFind += 0.70;
		dementiaXpPercent += 0.59;	}
	else if (level < 85 && level >= 80)	{
		dementiaAllStats += 36;
		dementiaFastCastPercent += 1.20;
		dementiaAttackSpeedPercent += 1.20;
		dementiaFastMovementPercent += 1.20;
		dementiaItemFind += 0.75;
		dementiaXpPercent += 0.63;	}
	else if (level < 90 && level >= 85)	{
		dementiaAllStats += 39;
		dementiaFastCastPercent += 1.27;
		dementiaAttackSpeedPercent += 1.27;
		dementiaFastMovementPercent += 1.27;
		dementiaItemFind += 0.80;
		dementiaXpPercent += 0.67;	}
	else if (level < 95 && level >= 90)	{
		dementiaAllStats += 42;
		dementiaFastCastPercent += 1.35;
		dementiaAttackSpeedPercent += 1.35;
		dementiaFastMovementPercent += 1.35;
		dementiaItemFind += 0.85;
		dementiaXpPercent += 0.71;	}
	else if (level < 100 && level >= 95)	{
		dementiaAllStats += 45;
		dementiaFastCastPercent += 1.42;
		dementiaAttackSpeedPercent += 1.42;
		dementiaFastMovementPercent += 1.42;
		dementiaItemFind += 0.90;
		dementiaXpPercent += 0.75;	}
	else if (level >= 100)	{
		dementiaAllStats += 50;
		dementiaFastCastPercent += 1.50;
		dementiaAttackSpeedPercent += 1.50;
		dementiaFastMovementPercent += 1.50;
		dementiaItemFind += 1.00;
		dementiaXpPercent += 0.80;	}
	GetComponent(PlayerStats).allStats += dementiaAllStats;
	gameObject.GetComponent(PlayerCast).fastCastPercent += dementiaFastCastPercent;
	GetComponent(PlayerDamage).attackSpeedPercent += dementiaAttackSpeedPercent;
	GetComponent(PlayerCharacteristic).fastMovementPercent += dementiaFastMovementPercent;
	GetComponent(PlayerGoldItem).itemFind += dementiaItemFind;
	GetComponent(PlayerHealthManaXp).xpPercent += dementiaXpPercent;
	
	audio.PlayOneShot(dementiaSound);
}

function	OnGUI()
{
	if (true == boolGodMode)
	{
		if ((GUI.Button(Rect(Screen.width * 0.005 + (0), Screen.height * 0.12, 48, 48), godTexture)))
			boolDisplayGodText = !boolDisplayGodText;
			
		if (boolDisplayGodText)
		{
			timeStayGodTime = xRemainGodTime - xRemainGodTime % 1;
			GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.12 + 50, 300, 48), "God mode :" + " stay : " + (godTime - timeStayGodTime) + " seconds", godGUIStyle);
			GUI.Label(Rect(Screen.width * 0.005 + (0), Screen.height * 0.12 + 70, 300, 500), "Gain: all stats : "+ dementiaAllStats +
			"\n   -fast cast percent : " + dementiaFastCastPercent + 
			"\n   -attack speed percent : " + dementiaAttackSpeedPercent +
			"\n   -fast movement percent : " + dementiaFastMovementPercent + 
			"\n   -item find percent : " + dementiaItemFind +
			"\n   -xp percent : " + dementiaXpPercent,
			godGUIStyle);
		}
	}
	
	 if (godTime > remainGodTime)
		godTime = remainGodTime;
}