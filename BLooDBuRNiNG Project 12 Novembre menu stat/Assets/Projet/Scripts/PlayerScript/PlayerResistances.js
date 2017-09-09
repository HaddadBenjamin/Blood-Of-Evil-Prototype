#pragma strict
//resistances élémentaire
//faut que je fasse les calculs des lights resistance et rahotuer xlight resistance
static var lightingResistance : float = 0;
static var coldResistance : float = 0;
static var fireResistance : float = 0;
static var earthResistance : float = 0;
static var poisonResistance : float = 0;
static var wingResistance : float = 0;
static var faithResistance : float = 0;
static var physicalResistance : float = 0;

static var xColdResistance : float = coldResistance;
static var xFireResistance : float = fireResistance;
static var xEarthResistance : float = earthResistance;
static var xPoisonResistance : float = poisonResistance;
static var xWingResistance : float = wingResistance;
static var xFaithResistance : float = faithResistance;

static var physicalDammageReduction : float = physicalResistance;
static var allResistance : int = 0;

static var slowEffectReduction : float = 0.00;
static var stunEffectReduction : float = 0.00;
static var poisonTimeEffectReduction : float = 0.00;
static var dodgePercent : float = 0.00;

function Start () {

}

function Update () 
{
	ForetIf3AllRes();
	ResistanceCalcul1();
	ResistanceCalcul2();
}

function	ForetIf3AllRes()
{
	if (allResistance != 0)
	{
		xColdResistance += allResistance;
		xFireResistance += allResistance;
		xEarthResistance += allResistance;
		xPoisonResistance += allResistance;
		xWingResistance += allResistance;
		xFaithResistance += allResistance;
		allResistance = 0;
	}
}

function	ResistanceCalcul1()
{
	if (xColdResistance <= 150)
		slowEffectReduction = xColdResistance * 0.5;
	else
		slowEffectReduction = 100;
		
	if (xEarthResistance <= 150)
		stunEffectReduction = xEarthResistance * 0.5;
	else
		stunEffectReduction = 100;
		
	if (xPoisonResistance <= 75)
		poisonTimeEffectReduction = xPoisonResistance;
	else
		poisonTimeEffectReduction = 75;
	
	if (xWingResistance <= 333)
		dodgePercent = xWingResistance * 0.1;
	else
		dodgePercent = 33.33;
}

function	ResistanceCalcul2()
{
	if (xColdResistance <= 75)
		coldResistance = xColdResistance;
	else
		coldResistance = 75;
		
	if (xFireResistance <= 75)
		fireResistance = xFireResistance;
	else
		fireResistance = 75;
		
	if (xEarthResistance <= 75)
		earthResistance = xEarthResistance;
	else
		earthResistance = 75;
		
	if (xPoisonResistance <= 75)
		poisonResistance = xPoisonResistance;
	else
		poisonResistance = 75;
		
	if (xWingResistance <= 75)
		wingResistance= xWingResistance;
	else
		wingResistance = 75;
				
	if (xFaithResistance <= 75)
		faithResistance = xFaithResistance;
	else
		faithResistance = 75;
				
	if (physicalDammageReduction <= 50)
		physicalResistance = physicalDammageReduction;
	else
		physicalResistance = 75;
}