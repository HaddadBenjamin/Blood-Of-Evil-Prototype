var moveSpeed : float = 3; //move speed
var rotationSpeed = 3; //speed of turning
var followRange : float = 150;
var patrolRange : float = 50;
private var myCharacter : GameObject;

private var myTransform : Transform; //current transform data of this enemy
private var target : Transform; //the enemy's target
var dist : float;
private var distPoint : float;
private var point1 : GameObject;
private var point2 : GameObject;
private var point3 : GameObject;
private var boolFirstStart : boolean = true;
//attaquer le joeur
private var curDmg : int;
private var xAttackSpeed : float = 0;
private var boolAttackSpeed : boolean = true;
private var scrollEnemyCurDmg : GameObject;
//
private var pointInit : GameObject;
private var distMaxToFollow : float;
private var xDistMaxToFollow : float;
private var boolNearTarget : boolean = false;
//animation
private var boolRun : boolean = true;
private var boolAttack : boolean = false;

var enemyAttackSound : AudioClip;

function Awake()
{
    myTransform = transform; //cache transform data for easy access/preformance
}
 
function Start()
{
	//_animation = GetComponent(Animation);
	//permet de vérifier qu'il le fais qu'une seule fois
	if (boolFirstStart == true)
		Init();
}
 
function Update () 
{
	moveSpeed = gameObject.GetComponent(EnemyCharacteristic).fastMovement;
	if (boolRun)
	{
		boolAttack = false;
		animation["SkeltonRun"].speed = moveSpeed * 0.175;
		animation.Play("SkeltonRun");
	}
	else if (boolAttack)
	{
		boolRun = false;
		animation["SkeltonAttack"].speed = 1.4 / (gameObject.GetComponent(EnemyCharacteristic).attackSpeed);
		animation.Play("SkeltonAttack");
	}
    //Destroy (GameObject.Find("Cube"));
    
    
	dist = Vector3.Distance(transform.position , target.position);
	distMaxToFollow = Vector3.Distance(pointInit.transform.position , target.position);
	AttackTheCharacter();
	
	if (dist < followRange && dist > 1.4 && boolNearTarget == false)
		FollowTheCharacter();
	else if (boolNearTarget == true && distMaxToFollow > 4.5)
		ReturnToInitPoint();
		
 	else
 	{
  		distPoint = Vector3.Distance(myTransform.position , point1.transform.position);
		if (distPoint > 2.5 && dist > followRange) 
		{
  			myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
    		Quaternion.LookRotation(point1.transform.position - myTransform.position), rotationSpeed*Time.deltaTime);
			Debug.DrawLine(point1.transform.position, myTransform.position, Color.red);
    		myTransform.position += myTransform.forward * moveSpeed / 2 * Time.deltaTime;
    	}
    	
		distPoint = Vector3.Distance(myTransform.position , point2.transform.position);
		if (distPoint > 2.5 && dist > followRange) 
		{
  			myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
    		Quaternion.LookRotation(point2.transform.position - myTransform.position), rotationSpeed*Time.deltaTime);
			Debug.DrawLine(point2.transform.position, myTransform.position, Color.red);
    		myTransform.position += myTransform.forward * moveSpeed / 2 * Time.deltaTime;
    	}
    	
    	distPoint = Vector3.Distance(myTransform.position , point3.transform.position);
		if (distPoint > 2.5 && dist > followRange) 
		{
  			myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
    		Quaternion.LookRotation(point3.transform.position - myTransform.position), rotationSpeed*Time.deltaTime);
			Debug.DrawLine(point3.transform.position, myTransform.position, Color.red);
    		myTransform.position += myTransform.forward * moveSpeed / 2 * Time.deltaTime;
    	}
    }
}

function	Init()
{
	myCharacter = GameObject.Find("Barbarian");
	myTransform = transform;
	target = myCharacter.transform; //target the player
	
	point1 = gameObject.Find("Point");
	point2 = gameObject.Find("Point");
	point3 = gameObject.Find("Point");
	pointInit = gameObject.Find("Point");
	
	point1 = Instantiate(point1, Vector3(transform.position.x + patrolRange, transform.position.y + 1, transform.position.z), transform.rotation);
	point2 = Instantiate(point2, Vector3(transform.position.x, transform.position.y + 1, transform.position.z - patrolRange), transform.rotation);
	point3 = Instantiate(point3, Vector3(transform.position.x - patrolRange, transform.position.y + 1, transform.position.z + patrolRange), transform.rotation);
	pointInit = Instantiate(pointInit, transform.position, transform.rotation);
		
	point1.transform.localScale = Vector3(.0, .0, .0);
	point2.transform.localScale = Vector3(.0, .0, .0);
	point3.transform.localScale = Vector3(.0, .0, .0);
	pointInit.transform.localScale = Vector3.zero;
	
	point1.transform.position.y -= 1;
	point2.transform.position.y -= 1;
	point3.transform.position.y -= 1;
//	pointInit.transform.position.y -= 1;
	
	point1.collider.enabled = false;
	point2.collider.enabled = false;
	point3.collider.enabled = false;
	pointInit.collider.enabled = false;
	 			
	point1.name = "point1";
	point2.name = "point2";
	point3.name = "point3";
	pointInit.name = "Init Point";
	
	boolFirst = false;
}

function	AttackTheCharacter()
{
	xAttackSpeed += Time.deltaTime;
	scrollEnemyCurDmg = gameObject.Find("Enemy Dmg Text");
    if (xAttackSpeed >= gameObject.GetComponent(EnemyCharacteristic).attackSpeed)
    	boolAttackSpeed = true;
    if (dist <= 2 && boolAttackSpeed == true)
    {
    	boolRun = false;
    	boolAttack = true;
    	scrollEnemyCurDmg.guiText.material.color.a = 1;
    	scrollEnemyCurDmg.transform.position.y = 0.38;
    	curDmg = Random.Range(gameObject.GetComponent(EnemyCharacteristic).minDmg, gameObject.GetComponent(EnemyCharacteristic).maxDmg + 1);
    	curDmg = curDmg - PlayerResistances.physicalDammageReduction - (PlayerResistances.physicalResistance * 0.01 * curDmg);
    	if (curDmg > 0)
    		PlayerHealthManaXp.curHp -= curDmg;
    	xAttackSpeed = 0;
    	boolAttackSpeed = false;
    	scrollEnemyCurDmg.guiText.material.color = Color(1, 0, 0, 1);
		scrollEnemyCurDmg.transform.position.y = 0.40;
		scrollEnemyCurDmg.guiText.fontSize = 22.5;
		scrollEnemyCurDmg.guiText.text = curDmg+ "";
		audio.PlayOneShot(enemyAttackSound);
    }
    if (dist > 2)
   	{
   		boolRun = true;
   		boolAttack = false;
   	}
    scrollEnemyCurDmg.transform.position.y += .050 * Time.deltaTime;
    scrollEnemyCurDmg.guiText.material.color.a -= 0.01 * Time.deltaTime;
    if (PlayerHealthManaXp.curHp <= 0 || scrollEnemyCurDmg.transform.position.y > 0.73)
    	scrollEnemyCurDmg.guiText.material.color.a = 0;
}

function	FollowTheCharacter()
{
	distPoint = 0;
	myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
	Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed*Time.deltaTime);
	Debug.DrawLine(target.position, myTransform.position, Color.red);
	myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
	if (distMaxToFollow > followRange * 3)
		boolNearTarget = true;
}

function	ReturnToInitPoint()
{
		distMaxToFollow = Vector3.Distance(pointInit.transform.position , transform.position);
		myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
		Quaternion.LookRotation(pointInit.transform.position - myTransform.position), rotationSpeed*Time.deltaTime);
		Debug.DrawLine(pointInit.transform.position, myTransform.position, Color.red);
		myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
		if (distMaxToFollow < 5)
			boolNearTarget = false;
}
