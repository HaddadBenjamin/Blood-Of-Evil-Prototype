#pragma strict
//score system
var scoreText : GUIText;
static var score : int = 0;
static var killOfSkelton : int = 0;

function Start () 
{

}

function	Update()
{
	if (score > 0 && Quest1.quest1 == false)
		scoreText.text = "Os de Squelette : " +  score;
}