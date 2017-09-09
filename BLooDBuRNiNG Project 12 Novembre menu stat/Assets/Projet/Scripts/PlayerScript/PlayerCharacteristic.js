static var fastMovement : int = 100;
static var fastMovementPercent : float = 1.00; 

static var numberOfMinions : int = 5;
static var minionHp : float = 1.00;
static var minionDmg : float = 1.00;

//a chaque variable rajouter on doit devoir les rajouter dans playersave..
//display enemy information
static var enemyInformationDisplayed : boolean = false;

function	Start () 
{ 
	QualitySettings.currentLevel = QualityLevel.Simple;
	transform.rotation.z = 200;
}

function	Update()
{

}