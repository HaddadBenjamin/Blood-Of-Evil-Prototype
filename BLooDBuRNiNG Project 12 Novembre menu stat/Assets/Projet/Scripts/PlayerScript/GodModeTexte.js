#pragma strict

private var boolCanCalculEverything : boolean = false;
var myColor : Color;

function Start () 
{
	guiText.material.color = myColor;
}

function Update () 
{
	if (Input.GetKey(KeyCode.G) && PlayerDementiaMode.boolCanGodMode == true && PlayerDementiaMode.boolGodMode == false)
	{
		transform.position.y = 0.38;
		guiText.text = "GOD MODE !!!";
		guiText.fontSize = 35;
		boolCanCalculEverything = true;
	}
	if (boolCanCalculEverything == true)
	{
		transform.position.y += 0.09 * Time.deltaTime;
		if (transform.position.y > 0.73)
		{
			transform.position.y -= 0.09 * Time.deltaTime;
			transform.position.y = 0.38;
			guiText.text = "";
			boolCanCalculEverything = false;
		}
	}
}

