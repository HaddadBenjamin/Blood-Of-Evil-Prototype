#pragma strict

function Start () {

}

function Update () {

}
////REPAIR CALCULATOR
//durabilityToRepair = maxDurability - curDurability;
//repairPrice = numberOfItemAttribute * itemLevelRequiried * durabilityToRepair;
//
////WEAPON DPS CALCULATOR
//weaponDps = (weaponMinDmg + weaponMaxDmg) * 0.5 * weaponDmgPercent / weaponAttackSpeed * (1 + ((2.85 + weaponCriticalDamage) * weaponCriticalChance));
//
////FULL STUFF DPS CALCULATOR
//totalDps = (minDmg + maxDmg) * 0.5 * dmgPercent / attackSpeedPercent * (1 + ((2.85 + weaponCriticalDamage) * weaponCriticalChance));
//
////MINION DPS CALCULATOR
//minionDps = minionDmgPercent * minionDmg;
//
////RECOLT CALCULATOR
//recoltAverage = (minRecolt + maxRecolt) * 0.5 / timeToRecolt

//GetCurHp
//this->PlayerCharacterstic.curHp

//function	InitLevel()
//{
//	if (this.level < 15)
//	{
//		this.curHp = this.curHp * (1 - level * 0.03);
//		this.maxHp = this.maxHp * (1 - level * 0.03);
//		this.xpAmount = this.xpAmount * (1 - level * 0.06);
//		this.minDmg = this.minDmg * (1 - level * 0.03);
//		this.maxDmg = this.maxDmg * (1 - level * 0.03);
//		this.attackSpeed = this.attackSpeed * (1 - level * 0.03);
//		this.fastMovement = this.fastMovement * (1 - level * 0.03);
//	}
//	
//	else if (this.level > 15)
//	{
//		this.curHp = this.curHp * (1 + level * 0.03);
//		this.maxHp = this.maxHp * (1 + level * 0.03);
//		this.xpAmount = this.xpAmount * (1 + level * 0.06);
//		this.minDmg = this.minDmg * (1 + level * 0.03);
//		this.maxDmg = this.maxDmg * (1 + level * 0.03);
//		this.attackSpeed = this.attackSpeed / (1 + level * 0.03);
//		this.fastMovement = this.fastMovement * (1 + level * 0.03);
//	}
//}
