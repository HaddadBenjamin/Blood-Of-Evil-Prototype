var textStyle : GUIStyle;
var textStyle2 : GUIStyle;

private var	informationShow : boolean = false;
private var	informationString : String;
var enemyIcon : Texture2D;

private var redString : String = "<color=red>";
private var blueString : String = "<color=blue>";
private var greenString : String = "<color=green>";
private var yellowString : String = "<color=yellow>";
private var purpleString : String = "<color=purple>";
private var pinkString : String = "<color=pink>";
private var maroonString : String = "<color=maroon>";
private var greyString : String = "<color=grey>";

private var hit : RaycastHit;

function	Start()
{
	textStyle2.normal.textColor = Color.white;
	textStyle2.richText = true;
	textStyle2.wordWrap = true;
}

function	OnGUI()
{
	//textStyle.getComponentalignment = TextAlignment.Center;
	//if (ChooseTypeOfMonster == "Normal")
			
		//if (OnMouseOver == true && false == PlayerCharacteristic.enemyInformationDisplayed)
		if (collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), hit, Mathf.Infinity) && false == PlayerCharacteristic.enemyInformationDisplayed)
		{
			var	playerLevel : int = PlayerStats.level;
		
			GUI.Box(Rect(Screen.width * 0.392, Screen.height * 0.001, Screen.width * 0.23, Screen.height * 0.12), "");
	 		if (gameObject.GetComponent(EnemyTypeOfMonster).typeOfMonster == "Champion")
				informationString += blueString;
			else if (gameObject.GetComponent(EnemyTypeOfMonster).typeOfMonster == "Gozu")
				informationString += yellowString;
			else if (gameObject.GetComponent(EnemyTypeOfMonster).typeOfMonster == "Boss")
				informationString += purpleString;
			else if (gameObject.GetComponent(EnemyTypeOfMonster).typeOfMonster == "World Boss")
				informationString += redString;
			else if (gameObject.GetComponent(EnemyTypeOfMonster).typeOfMonster == "Gobelin")
				informationString += greyString;
			else
				informationString += "<color=#FFFFFFFF>";	
	
			informationString += "<b>Skelton</b> " + " </color>(" + gameObject.GetComponent(EnemyTypeOfMonster).typeOfMonster + " level : ";
			
			if (gameObject.GetComponent(EnemyHealthManaXp).level - playerLevel >= 10)
				informationString += "<color=#FF0000FF>";
			else if (gameObject.GetComponent(EnemyHealthManaXp).level - playerLevel >= 5 && gameObject.GetComponent(EnemyHealthManaXp).level - playerLevel < 10)
				informationString += "<color=#FFa500FF>";
			else if (gameObject.GetComponent(EnemyHealthManaXp).level - playerLevel >= 0 && gameObject.GetComponent(EnemyHealthManaXp).level - playerLevel < 5)
				informationString += "<color=#FFFF00FF>";
			else if (gameObject.GetComponent(EnemyHealthManaXp).level - playerLevel >= -4 && gameObject.GetComponent(EnemyHealthManaXp).level - playerLevel < 0)
				informationString += "<color=#00FF00FF>";
			else if (gameObject.GetComponent(EnemyHealthManaXp).level - playerLevel >= -9 && gameObject.GetComponent(EnemyHealthManaXp).level - playerLevel < -4)
				informationString += "<color=#F00FFFFFF>";
			else if (gameObject.GetComponent(EnemyHealthManaXp).level - playerLevel <= -10)
				informationString += "<color=#FF69b4FF>";
			informationString += gameObject.GetComponent(EnemyHealthManaXp).level + "</color>)\n";
			
			informationString += "Life : " + "<color=#FF0000FF>" + gameObject.GetComponent(EnemyCharacteristic).curHp + "</color> / " + "<color=#FF0000FF>" + gameObject.GetComponent(EnemyCharacteristic).maxHp + "</color>\n";
			informationString += "Mana : " + "<color=#0000FFFF>" + gameObject.GetComponent(EnemyCharacteristic).curMp + "</color> / " + "<color=#0000FFFF>" + gameObject.GetComponent(EnemyCharacteristic).maxMp + "</color>\n";
				informationString += "<b>Abilities:</b> Boule de feu, Attaque multiple\n";
			informationString += "<b>Resitance:</b> Physique : " + gameObject.GetComponent(EnemyResistance).physicalResistance + ", Feu : <color=#FF0000FF> " + gameObject.GetComponent(EnemyResistance).fireResistance + " </color>, Froid : <color=#42C0FBFF>" 
																 + gameObject.GetComponent(EnemyResistance).coldResistance + " </color>, Foudre : <color=yellow>" + gameObject.GetComponent(EnemyResistance).lightingResistance + "</color>,\n";
			informationString += "                " + " Poison : <color=#00FF00FF>" + gameObject.GetComponent(EnemyResistance).poisonResistance + "</color>, Vent : <color=#c0c0c0FF>" + gameObject.GetComponent(EnemyResistance).wingResistance
							               + "</color>, Terre : <color=#964514FF>" + gameObject.GetComponent(EnemyResistance).eartResistance + "</color>, Foi : <color=#FF6600FF>" + gameObject.GetComponent(EnemyResistance).faithResistance + "</color>\n";
		
			if (informationString)
				GUI.TextArea(new Rect(Screen.width * 0.407, Screen.height * 0.005, Screen.width * 0.265, Screen.height * 0.4), informationString, textStyle2);
			informationString = "";
			
			//GUI.skin = enemyIconGUISkin;
			//GUI.DrawTexture(new Rect(Screen.width * 0.595, Screen.height * 0.005, 64, 64), enemyIcon);
			//GUI.BeginGroup(new Rect(Screen.width * 0.595, Screen.height * 0.005, 64, 64));

			GUI.Box(new Rect(Screen.width * 0.587, Screen.height * 0.005, 64, 64), enemyIcon, textStyle);
			//GUI.EndGroup();
			
			PlayerCharacteristic.enemyInformationDisplayed = true;
		}
		else
			PlayerCharacteristic.enemyInformationDisplayed = false; 
}
