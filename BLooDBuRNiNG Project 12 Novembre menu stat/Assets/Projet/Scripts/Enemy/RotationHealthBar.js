private var camera1 : GameObject;
private var camera2 : GameObject;

function Start () 
{
	camera1 = GameObject.Find("Camera MMORPG 1");
	camera2 = GameObject.Find("Camera Hack & Slash 2");
	boolChangeCamera = false;
}

function Update() 
{
	if (MultipleCamera.startCamera % 2 == 1)
		transform.rotation = camera1.transform.rotation;
	else if (MultipleCamera.startCamera % 2 == 0)
		transform.rotation = camera2.transform.rotation;	
}
