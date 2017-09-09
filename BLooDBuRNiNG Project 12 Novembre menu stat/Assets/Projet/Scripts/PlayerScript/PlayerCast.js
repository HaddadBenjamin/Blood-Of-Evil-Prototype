#pragma strict
//skill system
static var fastCastPercent : float = 1.00;
static var fastCast : float = 0.7;
static var yFastCast : float = fastCast;
static var xFastCast : float = 0.0;
static var minHl : int = 1082;
static var maxHl : int = 1218;
static var curHl : int;
static var boolFastCast : boolean = true;
static var fireBall : GameObject;


var fireBallObject : GameObject;

function Start () 
{
	fireBall = gameObject.Find("Boule de feu");
	fastCast = yFastCast / fastCastPercent;
}

function Update () 
{
	xFastCast += Time.deltaTime;
	fastCast = yFastCast / fastCastPercent;
	
	if (xFastCast >= fastCast)
		boolFastCast = true;
		
	if (minHl > maxHl)
 		minHl = maxHl;
}