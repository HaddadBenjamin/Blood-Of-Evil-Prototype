#pragma strict

function Start () {
timer = 0.0;
}
private static var textOn : boolean = true;
static var message : String;
private var timer : float;

var timeToDisplay : float = 6.0;
private var dist : float = 0.0;
var distToDisplay : float = 10.0;
var targetToShow : Transform;

function Update ()
{
//	dist = Vector3.Distance(transform.position , targetToShow.position);
//	if (dist <= distToDisplay)
//	{
//		textOn = true;
//	}
//	TextHints.message = "yoyo";
	if (textOn)
		{
			guiText.enabled = true;
			guiText.text = "Welcome in the plain of hell";
			timer += Time.deltaTime;
		}
	
	if (timer >= timeToDisplay)
	{
		textOn = false;
		guiText.enabled = false;
		timer = 0.0;
	}
}