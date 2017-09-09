#pragma strict
static var accuracy : long = 50;
static var accuracyPercent : float = 1.00;
static var xAccuracy : long = accuracy;
static var shrineAccuracy = 1.00;

static var curDmg : long;
static var minDmg : long = 14;//14
static var xMinDmg : long = minDmg;
static var maxDmg : long = 27;//27
static var xMaxDmg : long = maxDmg;
static var dmgPercent : float = 1.00;
static var attackSpeed : float = 0.2; //0.5
static var yAttackSpeed : float = attackSpeed; //vitesse d'attaque = yattackspeed / attackspeedpercent il faut tjr augmenter yattackspeed lorsqu'on fait nos calculs d'attack
static var attackSpeedPercent : float = 1.00;
static var xAttackSpeed : float = 0.0; // permet juste à tester une variavle dans une boucle
static var boolAttackSpeed : boolean = true;
static var criticalChance : float = 40.00;

//dommage élémentaire
static var minFrozenDmg : long = 0;
static var maxFrozenDmg : long = 0;
static var tmpMinFrozenDmg : long = minFrozenDmg;
static var tmpMaxFrozenDmg : long = maxFrozenDmg;
static var frozenDmgPercent : float = 1.00;

static var minFireDmg : long = 0;
static var maxFireDmg : long = 0;
static var tmpMinFireDmg : long = minFireDmg;
static var tmpMaxFireDmg : long = maxFireDmg;
static var fireDmgPercent : float = 1.00;

static var minEarthDmg : long = 0;
static var maxEarthDmg : long = 0;
static var tmpMinEarthDmg : long = minEarthDmg;
static var tmpMaxEarthDmg : long = maxEarthDmg;
static var earthDmgPercent : float = 1.00;

static var minPoisonDmg : long = 0;
static var maxPoisonDmg : long = 0;
static var tmpMinPoisonDmg : long = minPoisonDmg;
static var tmpMaxPoisonDmg : long = maxPoisonDmg;
static var poisonDmgPercent : float = 1.00;

static var minWingDmg : long = 0;
static var maxWingDmg : long = 0;
static var tmpMinWingDmg : long = minWingDmg;
static var tmpMaxWingDmg : long = maxWingDmg;
static var wingDmgPercent : float = 1.00;

static var minFaithDmg : long = 0;
static var maxFaithDmg : long = 0;
static var tmpMinFaithDmg : long = minFaithDmg;
static var tmpMaxFaithDmg : long = maxFaithDmg;
static var faithDmgPercent : float = 1.00;

//scrolling des dommage du playe
static var numberOfPhysicalDmgText : long = 20;
static var tmpIScrollPhysicalDmg : long = 0;
static var iScrollPhysicalDmg : long = 0;
static var scrolPhysicalDmgSpeed : float = 1;
static var scrollPhysicalDmg : GameObject[];

function Start () 
{
	scrollPhysicalDmg = new GameObject[numberOfPhysicalDmgText];
	scrollPhysicalDmg[0] = gameObject.Find("Player Dmg Text");
	for (tmpIScrollPhysicalDmg = 0; tmpIScrollPhysicalDmg < numberOfPhysicalDmgText; tmpIScrollPhysicalDmg += 1)
	{
		scrollPhysicalDmg[tmpIScrollPhysicalDmg] = Instantiate(scrollPhysicalDmg[0], scrollPhysicalDmg[0].transform.position, scrollPhysicalDmg[0].transform.rotation);
		scrollPhysicalDmg[tmpIScrollPhysicalDmg].name = "Player Physical Damage Text";
		//scrollPhysicalDmg[tmpIScrollPhysicalDmg].tranform.parent = GameObject.Find("Barbarian").transform;
	}
	
	maxDmg *= 1.05;
	minDmg *= 1.05;
	
	attackSpeed = yAttackSpeed / attackSpeedPercent;
	
	minDmg = xMinDmg * dmgPercent;
	maxDmg = xMaxDmg * dmgPercent;
	
	accuracy = xAccuracy * accuracyPercent * shrineAccuracy;
}

function Update () 
{
//	if (scrollPhysicalDmg && scrollPhysicalDmg[tmpIScrollPhysicalDmg])
		for (tmpIScrollPhysicalDmg = 0; tmpIScrollPhysicalDmg < numberOfPhysicalDmgText; tmpIScrollPhysicalDmg += 1)
    		scrollPhysicalDmg[tmpIScrollPhysicalDmg].transform.position.y += scrolPhysicalDmgSpeed * Time.deltaTime;

	xAttackSpeed += Time.deltaTime;
	attackSpeed = yAttackSpeed / attackSpeedPercent;
	
	minDmg = xMinDmg * dmgPercent;
	maxDmg = xMaxDmg * dmgPercent;
	accuracy = xAccuracy * accuracyPercent * shrineAccuracy;
	
	if (xAttackSpeed >= attackSpeed)
		boolAttackSpeed = true;
	
	 if (criticalChance > 50)
 		criticalChance = 50;
}