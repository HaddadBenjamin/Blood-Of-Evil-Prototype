var typeOfMonster : String = "Normal";
private var whichTypeOfMonster : int;
var bossSound : AudioClip;

function	InitWhichTypeOfMonster()
{
	whichTypeOfMonster = Random.Range(1, 1273 + 1);
	if (whichTypeOfMonster > 0 && whichTypeOfMonster <= 1000)
		typeOfMonster = "Normal";
	else if (whichTypeOfMonster > 1000 && whichTypeOfMonster <= 1200)
		typeOfMonster = "Champion";
	else if (whichTypeOfMonster > 1200 && whichTypeOfMonster <= 1260)
		typeOfMonster = "Gozu";
	else if (whichTypeOfMonster > 1260 && whichTypeOfMonster <= 1270)
		typeOfMonster = "Boss";
	else if (whichTypeOfMonster > 1270 && whichTypeOfMonster <= 1272)
		typeOfMonster = "World Boss";
	else if (whichTypeOfMonster == 1273)
		typeOfMonster = "Gobelin";
}

function	ChooseTypeOfMonster()
{	
	if (typeOfMonster == "Champion")
	{
		transform.localScale = Vector3(1.5 * 2, 1.5 * 2, 1.5 * 2);
		gameObject.GetComponent(EnemyCharacteristic).healthPercent += 1;
		gameObject.GetComponent(EnemyCharacteristic).dmgPercent += 1;
		gameObject.GetComponent(EnemyHealthManaXp).xpPercent += 2; 
		gameObject.GetComponent(EnemyCharacteristic).questItemPercent += 2;
		
		gameObject.GetComponent(EnemyCharacteristic).itemRarityPercent += 2;
	}
	else if (typeOfMonster == "Gozu")
	{
		transform.localScale = Vector3(2 * 2, 2 * 2, 2 * 2);
		gameObject.GetComponent(EnemyCharacteristic).healthPercent += 4;
		gameObject.GetComponent(EnemyCharacteristic).dmgPercent += 0.5;
		gameObject.GetComponent(EnemyHealthManaXp).xpPercent += 4;
		gameObject.GetComponent(EnemyCharacteristic).attackSpeedPercent *= 0.5;
		gameObject.GetComponent(EnemyCharacteristic).questItemPercent += 3.5;
		
		gameObject.GetComponent(EnemyCharacteristic).fastMovementPercent += 0.4;		
		gameObject.GetComponent(EnemyCharacteristic).itemRarityPercent += 4;
	}	
	else if (typeOfMonster == "Boss")
	{
		transform.localScale = Vector3(3.5 * 2, 3.5 * 2, 3.5 * 2);
		gameObject.GetComponent(EnemyCharacteristic).healthPercent += 29;
		gameObject.GetComponent(EnemyCharacteristic).dmgPercent += 4;
		gameObject.GetComponent(EnemyHealthManaXp).xpPercent += 14; 
		gameObject.GetComponent(EnemyCharacteristic).questItemPercent += 10;
		
		gameObject.GetComponent(EnemyCharacteristic).fastMovementPercent += 0.6;	
		gameObject.GetComponent(EnemyCharacteristic).itemRarityPercent += 9;
		gameObject.GetComponent(EnemyCharacteristic).itemQuantityPercent += 1;
		audio.PlayOneShot(bossSound);	
	}
	else if (typeOfMonster == "World Boss")
	{
		transform.localScale = Vector3(5 * 2, 5 * 2, 5 * 2);
		gameObject.GetComponent(EnemyCharacteristic).healthPercent += 164;
		gameObject.GetComponent(EnemyCharacteristic).dmgPercent += 12;
		gameObject.GetComponent(EnemyHealthManaXp).xpPercent += 80;
		gameObject.GetComponent(EnemyCharacteristic).questItemPercent += 25;
		
		gameObject.GetComponent(EnemyCharacteristic).fastMovementPercent += 1;
		gameObject.GetComponent(EnemyCharacteristic).itemRarityPercent += 14;
		gameObject.GetComponent(EnemyCharacteristic).itemQuantityPercent += 4;
		audio.PlayOneShot(bossSound);				
	}
	else if (typeOfMonster == "Gobelin")
	{
		transform.localScale = Vector3(1, 1, 1);
		gameObject.GetComponent(EnemyHealthManaXp).xpPercent += 10;
		gameObject.GetComponent(EnemyCharacteristic).dmgPercent += 1;
		gameObject.GetComponent(EnemyCharacteristic).healthPercent += 50;
		gameObject.GetComponent(EnemyCharacteristic).attackSpeedPercent *= 0.2; 
		gameObject.GetComponent(EnemyCharacteristic).questItemPercent += 30;
		
		gameObject.GetComponent(EnemyCharacteristic).fastMovementPercent += 2;
		gameObject.GetComponent(EnemyCharacteristic).itemQuantityPercent += 10;
		audio.PlayOneShot(bossSound);	
	}
}
