#pragma strict

var spawnSound : AudioClip;
private var xNumberSpawn : int = 0;
var numberSpawn : int = 10;
var timeToSpawn : float = 0.5;
private var xTimeToSpawn : float = 0.0;
var monsterToSpawn : GameObject;
var monsterSpawned : GameObject;
var spawnOrNot : boolean = false;
var horizontal : float = 8;
var line : int = 5;
var vertical : float = 6;
var futureRepsawn = 1;

private var numberOfKill : int = 0;
private var myPosition : Vector3;
private var yNumberSpawn : int;
private var yTimeToSpawn : float = 0.0;

function Start()
{
	myPosition = transform.position;
	yNumberSpawn = numberSpawn;
}

//function qui li le son si un float = 0.00 sinon il ne la lit pas
// si la musique est fini float = 0.00
var spawnSoundTime : float = 0.00;

function		SoundPlayIfNotPlayed(soundToPlay : AudioClip, isPlayed : float)
{
	if (isPlayed == 0.00)
	{
		audio.PlayOneShot(soundToPlay);
		isPlayed += Time.deltaTime;
	}
	
	if (isPlayed >= soundToPlay.length)
		isPlayed = 0.00;
	
	return isPlayed;
}

function Update () 
{
	if (spawnSoundTime > 0.00)
		spawnSoundTime += Time.deltaTime;
		
	xTimeToSpawn += Time.deltaTime;
	if (xTimeToSpawn >= timeToSpawn && xNumberSpawn < numberSpawn && spawnOrNot)
	{
		if (xNumberSpawn % line == 0)
		{
			transform.position.x -= horizontal * line;
			transform.position.z -= vertical;
		}
		monsterSpawned = Instantiate(monsterToSpawn, transform.position, Quaternion.identity);
		spawnSoundTime = SoundPlayIfNotPlayed(spawnSound, spawnSoundTime);
		//audio.PlayOneShot(spawnSound);
		transform.position.x += horizontal;
		monsterToSpawn.name = "skeltons";
		xTimeToSpawn = 0.0;
		xNumberSpawn += 1;
	}
	if (xNumberSpawn > yNumberSpawn)
	{
		transform.position = myPosition;
		yNumberSpawn += numberSpawn;
		vertical *= 0.5;
	}
	if (numberOfKill < PlayerScore.killOfSkelton)
	{
		if (yTimeToSpawn < 0.01)
			yTimeToSpawn += Time.deltaTime;
		else
		{
			numberSpawn++;
			numberOfKill++;
			yTimeToSpawn = 1;
		}

	}	
}