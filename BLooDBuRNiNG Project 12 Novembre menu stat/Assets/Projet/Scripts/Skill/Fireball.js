#pragma strict

//private var myTransform : Vector3;
//static var dist : float;
var fireBallSound : AudioClip;
var player : GameObject;
var dist : float;
var myTransform : Transform;

function Start () 
{
	transform.position.y += 1.6;
	gameObject.name = "Boule de feu";
//	myTransform = transform.position;
	PlayerHealthManaXp.curMp -= 22;
	PlayerCast.boolFastCast = false;
	PlayerCast.xFastCast = 0.0;
	transform.position += transform.forward * 45 / PlayerCast.fastCast * Time.deltaTime;
	
	myTransform = transform;
	player = gameObject.Find("Barbarian");
	dist = Vector3.Distance(transform.position, player.transform.position);
	if (dist < 10)
		audio.PlayOneShot(fireBallSound);
}

function Update () 
{
//	dist = Vector3.Distance(myTransform, transform.position);
//	if (dist > 5)
//		Destroy(gameObject);
	transform.position += transform.forward * 10 / PlayerCast.fastCast * Time.deltaTime;
}
