
function Start() 
{
	PlayerPrefs.DeleteAll();
	//OnLoad();
	//autoSaveEnable();
}

function autoSaveEnable()
{
	for(var x = 1; x>0; x++)
	{
		yield WaitForSeconds(5);
		Debug.Log("save me");
		//OnSave();
	}
}
// a function created to save a game

function OnSave()
{
	PlayerPrefs.SetInt("fastMovement", PlayerCharacteristic.fastMovement);//1
	PlayerPrefs.SetFloat("fastMovementPercent", PlayerCharacteristic.fastMovementPercent);//2
	
	PlayerPrefs.SetInt("curHp", PlayerCharacteristic.curHp);//1
	PlayerPrefs.SetInt("maxHp", PlayerCharacteristic.maxHp);//2
	PlayerPrefs.SetInt("xMaxHp", PlayerCharacteristic.xMaxHp);//3
	PlayerPrefs.SetFloat("hpPercent", PlayerCharacteristic.hpPercent);//4
	
	PlayerPrefs.SetInt("curMp", PlayerCharacteristic.curMp);//1
	PlayerPrefs.SetInt("maxMp", PlayerCharacteristic.maxMp);//2
	PlayerPrefs.SetInt("xMaxMp", PlayerCharacteristic.xMaxMp);//3
	PlayerPrefs.SetFloat("mpPercent", PlayerCharacteristic.mpPercent);//4
		
	PlayerPrefs.SetInt("curXp", PlayerCharacteristic.curXp);//1
	PlayerPrefs.SetInt("maxXp", PlayerCharacteristic.maxXp);//2
	PlayerPrefs.SetInt("level", PlayerCharacteristic.level);//3
	PlayerPrefs.SetFloat("xpPercent", PlayerCharacteristic.xpPercent);//4

	PlayerPrefs.SetInt("curDmg", PlayerCharacteristic.curDmg);//1
	PlayerPrefs.SetInt("minDmg", PlayerCharacteristic.minDmg);//2
	PlayerPrefs.SetInt("xMinDmg", PlayerCharacteristic.xMinDmg);//3
	PlayerPrefs.SetInt("maxDmg", PlayerCharacteristic.maxDmg);//4
	PlayerPrefs.SetInt("xMaxDmg", PlayerCharacteristic.xMaxDmg);//5
	
	PlayerPrefs.SetFloat("dmgPercent", PlayerCharacteristic.dmgPercent);//1
	PlayerPrefs.SetFloat("attackSpeed", PlayerCharacteristic.attackSpeed);//2
	PlayerPrefs.SetFloat("yAttackSpeed", PlayerCharacteristic.yAttackSpeed);//3
	PlayerPrefs.SetFloat("attackSpeedPercent", PlayerCharacteristic.attackSpeedPercent);//4
	PlayerPrefs.SetFloat("xAttackSpeed", PlayerCharacteristic.xAttackSpeed);//5
	PlayerPrefs.SetFloat("criticalChance", PlayerCharacteristic.criticalChance);//6
	
	PlayerPrefs.SetFloat("fastCastPercent", PlayerCharacteristic.fastCastPercent);//1
	PlayerPrefs.SetFloat("fastCast", PlayerCharacteristic.fastCast);//2
	PlayerPrefs.SetFloat("yFastCast", PlayerCharacteristic.yFastCast);//3
	PlayerPrefs.SetFloat("xFastCast", PlayerCharacteristic.xFastCast);//4
	
	PlayerPrefs.SetInt("minHl", PlayerCharacteristic.minHl);//1
	PlayerPrefs.SetInt("maxHl", PlayerCharacteristic.maxHl);//2
	PlayerPrefs.SetInt("curHl", PlayerCharacteristic.curHl);//3
	
	PlayerPrefs.SetString("playerName", PlayerCharacteristic.playerName);//1
	PlayerPrefs.SetInt("strength", PlayerCharacteristic.strength);//2
	PlayerPrefs.SetInt("power", PlayerCharacteristic.power);//3
	PlayerPrefs.SetInt("dexterity", PlayerCharacteristic.dexterity);//4
	PlayerPrefs.SetInt("endurance", PlayerCharacteristic.endurance);//5
	PlayerPrefs.SetInt("thickness", PlayerCharacteristic.thickness);//6
	PlayerPrefs.SetInt("spirit", PlayerCharacteristic.spirit);//7
	PlayerPrefs.SetInt("faith", PlayerCharacteristic.faith);//8
	PlayerPrefs.SetInt("constitution", PlayerCharacteristic.constitution);//9
	PlayerPrefs.SetInt("chance", PlayerCharacteristic.chance);//10
	PlayerPrefs.SetInt("allStats", PlayerCharacteristic.allStats);//11

	PlayerPrefs.SetInt("accuracy", PlayerCharacteristic.accuracy);//1
	PlayerPrefs.SetInt("xAccuracy", PlayerCharacteristic.xAccuracy);//2
	PlayerPrefs.SetInt("defence", PlayerCharacteristic.defence);//3
	PlayerPrefs.SetInt("xDefence", PlayerCharacteristic.xDefence);//4
	PlayerPrefs.SetInt("physicalDammageReduction", PlayerCharacteristic.physicalDammageReduction);//5
	PlayerPrefs.SetInt("numberOfMinions", PlayerCharacteristic.numberOfMinions);//6
	
	PlayerPrefs.SetFloat("accuracyPercent", PlayerCharacteristic.accuracyPercent);//1
	PlayerPrefs.SetFloat("defencePercent", PlayerCharacteristic.defencePercent);//2
	PlayerPrefs.SetFloat("itemFind", PlayerCharacteristic.itemFind);//3
	PlayerPrefs.SetFloat("itemRarity", PlayerCharacteristic.itemRarity);//4
	PlayerPrefs.SetFloat("goldFindQuantity", PlayerCharacteristic.goldFindQuantity);//5
	PlayerPrefs.SetFloat("goldFindAmount", PlayerCharacteristic.goldFindAmount);//6
	PlayerPrefs.SetFloat("minionHp", PlayerCharacteristic.minionHp);//7
	PlayerPrefs.SetFloat("minionDmg", PlayerCharacteristic.minionDmg);//8
	PlayerPrefs.SetFloat("slowEffectReduction", PlayerCharacteristic.slowEffectReduction);//9
	PlayerPrefs.SetFloat("stunEffectReduction", PlayerCharacteristic.stunEffectReduction);//10
	PlayerPrefs.SetFloat("poisonTimeEffectReduction", PlayerCharacteristic.poisonTimeEffectReduction);//11
	PlayerPrefs.SetFloat("dodgePercent", PlayerCharacteristic.dodgePercent);//12
	
	PlayerPrefs.SetFloat("frozenResistance", PlayerCharacteristic.frozenResistance);//1
	PlayerPrefs.SetFloat("fireResistance", PlayerCharacteristic.fireResistance);//2
	PlayerPrefs.SetFloat("earthResistance", PlayerCharacteristic.earthResistance);//3
	PlayerPrefs.SetFloat("poisonResistance", PlayerCharacteristic.poisonResistance);//4
	PlayerPrefs.SetFloat("wingResistance", PlayerCharacteristic.wingResistance);//5
	PlayerPrefs.SetFloat("faithResistance", PlayerCharacteristic.faithResistance);//6
	PlayerPrefs.SetFloat("physicalResistance", PlayerCharacteristic.physicalResistance);//7
	PlayerPrefs.SetFloat("xFrozenResistance", PlayerCharacteristic.xFrozenResistance);//8
	PlayerPrefs.SetFloat("xFireResistance", PlayerCharacteristic.xFireResistance);//9
	PlayerPrefs.SetFloat("xEarthResistance", PlayerCharacteristic.xEarthResistance);//10
	PlayerPrefs.SetFloat("xPoisonResistance", PlayerCharacteristic.xPoisonResistance);//11
	PlayerPrefs.SetFloat("xWingResistance", PlayerCharacteristic.xWingResistance);//12
	PlayerPrefs.SetFloat("xFaithResistance", PlayerCharacteristic.xFaithResistance);//13
	PlayerPrefs.SetFloat("physicaDmgReduction", PlayerCharacteristic.physicaDmgReduction);//14
	PlayerPrefs.SetInt("allResistance", PlayerCharacteristic.allResistance);//15

	PlayerPrefs.SetInt("minFrozenDmg", PlayerCharacteristic.minFrozenDmg);//1
	PlayerPrefs.SetInt("maxFrozenDmg", PlayerCharacteristic.maxFrozenDmg);//2
	PlayerPrefs.SetInt("minFireDmg", PlayerCharacteristic.minFireDmg);//3
	PlayerPrefs.SetInt("maxFireDmg", PlayerCharacteristic.maxFireDmg);//4
	PlayerPrefs.SetInt("minEarthDmg", PlayerCharacteristic.minEarthDmg);//5
	PlayerPrefs.SetInt("maxEarthDmg", PlayerCharacteristic.maxEarthDmg);//6
	PlayerPrefs.SetInt("minPoisonDmg", PlayerCharacteristic.minPoisonDmg);//7
	PlayerPrefs.SetInt("maxPoisonDmg", PlayerCharacteristic.maxPoisonDmg);//8
	PlayerPrefs.SetInt("minWingDmg", PlayerCharacteristic.minWingDmg);//9
	PlayerPrefs.SetInt("maxWingDmg", PlayerCharacteristic.maxWingDmg);//10
	PlayerPrefs.SetInt("minFaithDmg", PlayerCharacteristic.minFaithDmg);//11
	PlayerPrefs.SetInt("maxFaithDmg", PlayerCharacteristic.maxFaithDmg);//12
	PlayerPrefs.SetInt("minPhysicalDmg", PlayerCharacteristic.minPhysicalDmg);//13
	PlayerPrefs.SetInt("maxPhysicalDmg", PlayerCharacteristic.maxPhysicalDmg);//14

	PlayerPrefs.SetInt("dementiaAllStats", PlayerCharacteristic.dementiaAllStats);//1
	PlayerPrefs.SetFloat("dementiaFastCastPercent", PlayerCharacteristic.dementiaFastCastPercent);//2
	PlayerPrefs.SetFloat("dementiaAttackSpeedPercent", PlayerCharacteristic.dementiaAttackSpeedPercent);//3
	PlayerPrefs.SetFloat("dementiaFastMovementPercent", PlayerCharacteristic.dementiaFastMovementPercent);//4
	PlayerPrefs.SetFloat("dementiaItemFind", PlayerCharacteristic.dementiaItemFind);//5
	PlayerPrefs.SetFloat("dementiaXpPercent", PlayerCharacteristic.dementiaXpPercent);//6
	
	PlayerPrefs.SetFloat("shrineAccuracy", PlayerCharacteristic.shrineAccuracy);//1
	PlayerPrefs.SetInt("score", PlayerCharacteristic.score);//2
	PlayerPrefs.SetInt("killOfSkelton", PlayerCharacteristic.killOfSkelton);//3
	PlayerPrefs.SetInt("indexMinimap", PlayerCharacteristic.indexMinimap);//4
}

// a function created to load a game
function OnLoad()
{
	Debug.Log("loaded");
	PlayerCharacteristic.fastMovement = PlayerPrefs.GetInt("fastMovement");//1
	PlayerCharacteristic.fastMovementPercent = PlayerPrefs.GetFloat("fastMovementPercent");//2
	
	PlayerCharacteristic.curHp = PlayerPrefs.GetInt("curHp");//1
	PlayerCharacteristic.maxHp = PlayerPrefs.GetInt("maxHp");//2
	PlayerCharacteristic.xMaxHp = PlayerPrefs.GetInt("xMaxHp");//3
	PlayerCharacteristic.hpPercent = PlayerPrefs.GetFloat("hpPercent");//4
	
	PlayerCharacteristic.curMp = PlayerPrefs.GetInt("curMp");//1
	PlayerCharacteristic.maxMp = PlayerPrefs.GetInt("maxMp");//2
	PlayerCharacteristic.xMaxMp = PlayerPrefs.GetInt("xMaxMp");//3
	PlayerCharacteristic.mpPercent = PlayerPrefs.GetFloat("mpPercent");//4
	
	PlayerCharacteristic.curXp = PlayerPrefs.GetInt("curXp");//1
	PlayerCharacteristic.maxXp = PlayerPrefs.GetInt("maxXp");//2
	PlayerCharacteristic.level = PlayerPrefs.GetInt("level");//3
	PlayerCharacteristic.xpPercent = PlayerPrefs.GetFloat("xpPercent");//4

	PlayerCharacteristic.curDmg = PlayerPrefs.GetInt("curDmg");//1
	PlayerCharacteristic.minDmg = PlayerPrefs.GetInt("minDmg");//2
	PlayerCharacteristic.xMinDmg = PlayerPrefs.GetInt("xMinDmg");//3
	PlayerCharacteristic.maxDmg = PlayerPrefs.GetInt("maxDmg");//4
	PlayerCharacteristic.xMaxDmg = PlayerPrefs.GetInt("xMaxDmg");//5
	
	PlayerCharacteristic.dmgPercent = PlayerPrefs.GetFloat("dmgPercent");//1
	PlayerCharacteristic.attackSpeed = PlayerPrefs.GetFloat("attackSpeed");//2
	PlayerCharacteristic.yAttackSpeed = PlayerPrefs.GetFloat("yAttackSpeed");//3
	PlayerCharacteristic.attackSpeedPercent = PlayerPrefs.GetFloat("attackSpeedPercentt");//4
	PlayerCharacteristic.xAttackSpeed = PlayerPrefs.GetFloat("xAttackSpeed");//5
	PlayerCharacteristic.criticalChance = PlayerPrefs.GetFloat("criticalChance");//6
	
	PlayerCharacteristic.fastCastPercent = PlayerPrefs.GetFloat("fastCastPercent");//1
	PlayerCharacteristic.fastCast = PlayerPrefs.GetFloat("fastCast");//2
	PlayerCharacteristic.yFastCast = PlayerPrefs.GetFloat("yFastCast");//3
	PlayerCharacteristic.xFastCast = PlayerPrefs.GetFloat("xFastCast");//4
	
	PlayerCharacteristic.minHl = PlayerPrefs.GetInt("minHl");//1
	PlayerCharacteristic.maxHl = PlayerPrefs.GetInt("maxHl");//2
	PlayerCharacteristic.curHl = PlayerPrefs.GetInt("curHl");//3
	
	PlayerCharacteristic.playerName = PlayerPrefs.GetString("playerName");//1
	PlayerCharacteristic.strength = PlayerPrefs.GetInt("strength");//2
	PlayerCharacteristic.power = PlayerPrefs.GetInt("power");//3
	PlayerCharacteristic.dexterity = PlayerPrefs.GetInt("dexterity");//4
	PlayerCharacteristic.endurance = PlayerPrefs.GetInt("endurance");//5
	PlayerCharacteristic.thickness = PlayerPrefs.GetInt("thickness");//6
	PlayerCharacteristic.spirit = PlayerPrefs.GetInt("spirit");//7
	PlayerCharacteristic.faith = PlayerPrefs.GetInt("faith");//8
	PlayerCharacteristic.constitution = PlayerPrefs.GetInt("constitution");//9
	PlayerCharacteristic.chance = PlayerPrefs.GetInt("chance");//10
	PlayerCharacteristic.allStats = PlayerPrefs.GetInt("allStats");//11

	PlayerCharacteristic.accuracy = PlayerPrefs.GetInt("accuracy");//1
	PlayerCharacteristic.xAccuracy = PlayerPrefs.GetInt("xAccuracy");//2
	PlayerCharacteristic.defence = PlayerPrefs.GetInt("defence");//3
	PlayerCharacteristic.xDefence = PlayerPrefs.GetInt("xDefence");//4
	PlayerCharacteristic.physicalDammageReduction = PlayerPrefs.GetInt("physicalDammageReduction");//5
	PlayerCharacteristic.numberOfMinions = PlayerPrefs.GetInt("numberOfMinions");//6
	
	PlayerCharacteristic.accuracyPercent = PlayerPrefs.GetFloat("accuracyPercent");//1
	PlayerCharacteristic.defencePercent = PlayerPrefs.GetFloat("defencePercent");//2
	PlayerCharacteristic.itemFind = PlayerPrefs.GetFloat("itemFind");//3
	PlayerCharacteristic.itemRarity = PlayerPrefs.GetFloat("itemRarity");//4
	PlayerCharacteristic.goldFindQuantity = PlayerPrefs.GetFloat("goldFindQuantity");//5
	PlayerCharacteristic.goldFindAmount = PlayerPrefs.GetFloat("goldFindAmount");//6
	PlayerCharacteristic.minionHp = PlayerPrefs.GetFloat("minionHp");//7
	PlayerCharacteristic.minionDmg = PlayerPrefs.GetFloat("minionDmg");//8
	PlayerCharacteristic.slowEffectReduction = PlayerPrefs.GetFloat("slowEffectReduction");//9
	PlayerCharacteristic.stunEffectReduction = PlayerPrefs.GetFloat("stunEffectReduction");//10
	PlayerCharacteristic.poisonTimeEffectReduction = PlayerPrefs.GetFloat("poisonTimeEffectReduction");//11
	PlayerCharacteristic.dodgePercent = PlayerPrefs.GetFloat("dodgePercent");//12
	
	PlayerCharacteristic.frozenResistance = PlayerPrefs.GetFloat("frozenResistance");//1
	PlayerCharacteristic.fireResistance = PlayerPrefs.GetFloat("fireResistance");//2
	PlayerCharacteristic.earthResistance = PlayerPrefs.GetFloat("earthResistance");//3
	PlayerCharacteristic.poisonResistance = PlayerPrefs.GetFloat("poisonResistance");//4
	PlayerCharacteristic.wingResistance = PlayerPrefs.GetFloat("wingResistance");//5
	PlayerCharacteristic.faithResistance = PlayerPrefs.GetFloat("faithResistance");//6
	PlayerCharacteristic.physicalResistance = PlayerPrefs.GetFloat("physicalResistance");//7
	PlayerCharacteristic.xFrozenResistance = PlayerPrefs.GetFloat("xFrozenResistance");//8
	PlayerCharacteristic.xFireResistance = PlayerPrefs.GetFloat("xFireResistance");//9
	PlayerCharacteristic.xEarthResistance = PlayerPrefs.GetFloat("xEarthResistance");//10
	PlayerCharacteristic.xPoisonResistance = PlayerPrefs.GetFloat("xPoisonResistance");//11
	PlayerCharacteristic.xWingResistance = PlayerPrefs.GetFloat("xWingResistance");//12
	PlayerCharacteristic.xFaithResistance = PlayerPrefs.GetFloat("xFaithResistance");//13
	PlayerCharacteristic.physicaDmgReduction = PlayerPrefs.GetFloat("physicaDmgReduction");//14
	PlayerCharacteristic.allResistance = PlayerPrefs.GetInt("allResistance");//15
	
	PlayerCharacteristic.minFrozenDmg = PlayerPrefs.GetInt("minFrozenDmg");//1
	PlayerCharacteristic.maxFrozenDmg = PlayerPrefs.GetInt("maxFrozenDmg");//2
	PlayerCharacteristic.minFireDmg = PlayerPrefs.GetInt("minFireDmg");//3
	PlayerCharacteristic.maxFireDmg = PlayerPrefs.GetInt("maxFireDmg");//4
	PlayerCharacteristic.minEarthDmg = PlayerPrefs.GetInt("minEarthDmg");//5
	PlayerCharacteristic.maxEarthDmg = PlayerPrefs.GetInt("maxEarthDmg");//6
	PlayerCharacteristic.minPoisonDmg = PlayerPrefs.GetInt("minPoisonDmg");//7
	PlayerCharacteristic.maxPoisonDmg = PlayerPrefs.GetInt("maxPoisonDmg");//8
	PlayerCharacteristic.minWingDmg = PlayerPrefs.GetInt("minWingDmg");//9
	PlayerCharacteristic.maxWingDmg = PlayerPrefs.GetInt("maxWingDmg");//10
	PlayerCharacteristic.minFaithDmg = PlayerPrefs.GetInt("minFaithDmg");//11
	PlayerCharacteristic.maxFaithDmg = PlayerPrefs.GetInt("maxFaithDmg");//12
	PlayerCharacteristic.minPhysicalDmg = PlayerPrefs.GetInt("minPhysicalDmg");//13
	PlayerCharacteristic.maxPhysicalDmg = PlayerPrefs.GetInt("maxPhysicalDmg");//14
	
	PlayerCharacteristic.dementiaAllStats = PlayerPrefs.GetInt("dementiaAllStats");//1
	PlayerCharacteristic.dementiaFastCastPercent = PlayerPrefs.GetFloat("dementiaFastCastPercent");//2
	PlayerCharacteristic.dementiaAttackSpeedPercent = PlayerPrefs.GetFloat("dementiaAttackSpeedPercent");//3
	PlayerCharacteristic.dementiaFastMovementPercent = PlayerPrefs.GetFloat("dementiaFastMovementPercent");//4
	PlayerCharacteristic.dementiaItemFind = PlayerPrefs.GetFloat("dementiaItemFind");//5
	PlayerCharacteristic.dementiaXpPercent = PlayerPrefs.GetFloat("dementiaXpPercent");//6
	
	PlayerCharacteristic.physicaDmgReduction = PlayerPrefs.GetFloat("shrineAccuracy");//1
	PlayerCharacteristic.score = PlayerPrefs.GetInt("score");//2
	PlayerCharacteristic.killOfSkelton = PlayerPrefs.GetInt("killOfSkelton");//3
	PlayerCharacteristic.maxPhysicalDmg = PlayerPrefs.GetInt("maxPhysicalDmg");//4
	
}
