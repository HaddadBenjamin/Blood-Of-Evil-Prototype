#pragma strict

static var boolHealing : boolean = false;
private var boolStart : boolean = true;
private var fastCastHealing : float = 0.7;
private var xFastCastHealing : float = 0.0;
private var curHl : int;

function Start () 
{
	guiText.material.color = Color(0, 1, 0, 1);
	guiText.fontSize = 28;
}

function Update () 
{
	HealingUpdate();
}

function	HealingCharacter ()
{
	//audio.PlayOneShot(healingSound);
	boolStart = false;
	boolHealing = false;
	PlayerCast.boolFastCast = false;
	xFastCastHealing = 0;
	
	if (Input.GetKey(KeyCode.H) && PlayerHealthManaXp.curMp > 35)
	{
		gameObject.GetComponent(PlayerCast).xFastCast = 0.0;
		transform.position.y = 0.38;
		curHl = Random.Range(PlayerCast.minHl, PlayerCast.maxHl);
		curHl = curHl * (1 + (0.03 * PlayerStats.faith));
		PlayerHealthManaXp.curHp += curHl;
		PlayerHealthManaXp.curMp -= 35;
		guiText.text = "+ " + curHl;
	}
}

function	HealingUpdate()
{
	if (boolStart == false)
	{
		
		if (xFastCastHealing < fastCastHealing)
			xFastCastHealing += Time.deltaTime;
		transform.position.y += 0.09 * Time.deltaTime;
		if (transform.position.y > 0.73)
		{
			transform.position.y -= 0.09 * Time.deltaTime;
			transform.position.y = 0.38;
			guiText.text = "";    
		}
	}
	if (xFastCastHealing >= fastCastHealing)
		boolHealing = true;
	if (boolHealing == true && Input.GetKey(KeyCode.H))
		HealingCharacter();
}