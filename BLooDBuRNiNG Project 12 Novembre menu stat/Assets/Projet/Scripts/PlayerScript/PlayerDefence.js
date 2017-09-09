#pragma strict
static var defence : int = 0;
static var defencePercent : float = 1.00;
static var xDefence : int = defence;
static var physicalDammageReduction = 0;

function Start () 
{
	defence = xDefence * defencePercent;
}
 
function	Update () 
{	
	AutoRecalcul();
}


function	AutoRecalcul()
{
	StatsCalcul();
}

function	StatsCalcul()
{	
	defence = xDefence * defencePercent;
}